namespace DaLion.Stardew.Professions.Framework.Patches.Combat;

#region using directives

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Tools;

using Stardew.Common.Extensions;
using Stardew.Common.Harmony;
using Extensions;
using Ultimate;

#endregion using directives

[UsedImplicitly]
internal class MeleeWeaponSetFarmerAnimatingPatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal MeleeWeaponSetFarmerAnimatingPatch()
    {
        Original = RequireMethod<MeleeWeapon>(nameof(MeleeWeapon.setFarmerAnimating));
    }

    #region harmony patches

    /// <summary>Patch to increase prestiged Brute attack speed with rage.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> MeleeWeaponSetFarmerAnimatingTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        /// Injected: if (who.professions.Contains(100 + <brute_id>) swipeSpeed *= 1f - ModEntry.PlayerState.BruteRageCounter * 0.005f;
        /// After: if (who.IsLocalPlayer)

        var skipRageBonus = generator.DefineLabel();
        try
        {
            helper
                .FindFirst(
                    new CodeInstruction(OpCodes.Callvirt, typeof(Farmer).PropertyGetter(nameof(Farmer.IsLocalPlayer)))
                )
                .AdvanceUntil(
                    new CodeInstruction(OpCodes.Ldarg_0)
                )
                .AddLabels(skipRageBonus)
                .Insert(
                    new CodeInstruction(OpCodes.Ldarg_1) // arg 1 = Farmer who
                )
                .InsertProfessionCheck((int) Profession.Brute + 100, forLocalPlayer: false)
                .Insert(
                    new CodeInstruction(OpCodes.Brfalse_S, skipRageBonus),
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldfld, typeof(MeleeWeapon).Field("swipeSpeed")),
                    new CodeInstruction(OpCodes.Ldc_R4, 1f),
                    new CodeInstruction(OpCodes.Call,
                        typeof(ModEntry).PropertyGetter(nameof(ModEntry.PlayerState))),
                    new CodeInstruction(OpCodes.Callvirt,
                        typeof(PerScreen<PlayerState>).PropertyGetter(nameof(PerScreen<PlayerState>
                            .Value))),
                    new CodeInstruction(OpCodes.Callvirt,
                        typeof(PlayerState).PropertyGetter(nameof(PlayerState.BruteRageCounter))),
                    new CodeInstruction(OpCodes.Conv_R4),
                    new CodeInstruction(OpCodes.Ldc_R4, Frenzy.PCT_INCREMENT_PER_RAGE_F / 2f),
                    new CodeInstruction(OpCodes.Mul),
                    new CodeInstruction(OpCodes.Sub),
                    new CodeInstruction(OpCodes.Mul),
                    new CodeInstruction(OpCodes.Stfld, typeof(MeleeWeapon).Field("swipeSpeed"))
                );
        }
        catch (Exception ex)
        {
            Log.E($"Failed adding attack speed to prestiged Brute.\nHelper returned {ex}");
            transpilationFailed = true;
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}