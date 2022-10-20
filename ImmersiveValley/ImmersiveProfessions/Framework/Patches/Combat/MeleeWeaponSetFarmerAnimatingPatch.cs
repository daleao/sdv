namespace DaLion.Stardew.Professions.Framework.Patches.Combat;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Common.Extensions.Reflection;
using DaLion.Common.Harmony;
using DaLion.Stardew.Professions.Extensions;
using DaLion.Stardew.Professions.Framework.Ultimates;
using HarmonyLib;
using StardewValley.Tools;
using HarmonyPatch = DaLion.Common.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class MeleeWeaponSetFarmerAnimatingPatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="MeleeWeaponSetFarmerAnimatingPatch"/> class.</summary>
    internal MeleeWeaponSetFarmerAnimatingPatch()
    {
        this.Target = this.RequireMethod<MeleeWeapon>(nameof(MeleeWeapon.setFarmerAnimating));
    }

    #region harmony patches

    /// <summary>Patch to increase prestiged Brute attack speed with rage.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? MeleeWeaponSetFarmerAnimatingTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new IlHelper(original, instructions);

        // Injected: if (who.professions.Contains(100 + <brute_id>) swipeSpeed *= 1f - ModEntry.PlayerState.BruteRageCounter * 0.005f;
        // After: if (who.IsLocalPlayer)
        try
        {
            var skipRageBonus = generator.DefineLabel();
            helper
                .FindFirst(
                    new CodeInstruction(
                        OpCodes.Callvirt,
                        typeof(Farmer).RequirePropertyGetter(nameof(Farmer.IsLocalPlayer))))
                .AdvanceUntil(new CodeInstruction(OpCodes.Ldarg_0))
                .AddLabels(skipRageBonus)
                .InsertInstructions(new CodeInstruction(OpCodes.Ldarg_1)) // arg 1 = Farmer who
                .InsertProfessionCheck(Profession.Brute.Value + 100, forLocalPlayer: false)
                .InsertInstructions(
                    new CodeInstruction(OpCodes.Brfalse_S, skipRageBonus),
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldfld, typeof(MeleeWeapon).RequireField("swipeSpeed")),
                    new CodeInstruction(OpCodes.Ldc_R4, 1f),
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(ModEntry).RequirePropertyGetter(nameof(ModEntry.State))),
                    new CodeInstruction(
                        OpCodes.Callvirt,
                        typeof(ModState).RequirePropertyGetter(nameof(ModState.BruteRageCounter))),
                    new CodeInstruction(OpCodes.Conv_R4),
                    new CodeInstruction(OpCodes.Ldc_R4, Frenzy.PercentIncrementPerRage / 2f),
                    new CodeInstruction(OpCodes.Mul),
                    new CodeInstruction(OpCodes.Sub),
                    new CodeInstruction(OpCodes.Mul),
                    new CodeInstruction(OpCodes.Stfld, typeof(MeleeWeapon).RequireField("swipeSpeed")));
        }
        catch (Exception ex)
        {
            Log.E($"Failed adding attack speed to prestiged Brute.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}
