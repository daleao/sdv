namespace DaLion.Ligo.Modules.Arsenal.Common.Patches;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class FarmerTakeDamagePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="FarmerTakeDamagePatcher"/> class.</summary>
    internal FarmerTakeDamagePatcher()
    {
        this.Target = this.RequireMethod<Farmer>(nameof(Farmer.takeDamage));
    }

    #region harmony patches

    /// <summary>Overhaul for farmer defense.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? FarmerTakeDamageTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new IlHelper(original, instructions);

        var floatResilience = generator.DeclareLocal(typeof(float));

        // From: bullshit resilience mitigation
        // To: if (ModEntry.Config.Arsenal.OverhauledDefense ? GetOverhauledResilience(this)
        //           else { do vanilla logic }
        try
        {
            var useVanillaResilience = generator.DefineLabel();
            var resumeExecution = generator.DefineLabel();
            helper
                .FindFirst(
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldfld, typeof(Farmer).RequireField(nameof(Farmer.resilience))),
                    new CodeInstruction(OpCodes.Stloc_3))
                .AddLabels(useVanillaResilience)
                .InsertInstructions(
                    new CodeInstruction(OpCodes.Call, typeof(ModEntry).RequirePropertyGetter(nameof(ModEntry.Config))),
                    new CodeInstruction(
                        OpCodes.Callvirt,
                        typeof(ModConfig).RequirePropertyGetter(nameof(ModConfig.Arsenal))),
                    new CodeInstruction(
                        OpCodes.Callvirt,
                        typeof(Config).RequirePropertyGetter(nameof(Config.OverhauledDefense))),
                    new CodeInstruction(OpCodes.Brfalse_S, useVanillaResilience),
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(FarmerTakeDamagePatcher).RequireMethod(nameof(GetOverhauledResilience))),
                    new CodeInstruction(OpCodes.Stloc_S, floatResilience),
                    new CodeInstruction(OpCodes.Br_S, resumeExecution))
                .FindNext(
                    new CodeInstruction(OpCodes.Call, typeof(Farmer).RequireMethod(nameof(Farmer.isWearingRing))))
                .Retreat(2)
                .AddLabels(resumeExecution);
        }
        catch (Exception ex)
        {
            Log.E($"Failed while adding overhauled farmer defense (part 1).\nHelper returned {ex}");
            return null;
        }

        // From: damage = Math.Max(1, damage - effectiveResilience);
        // To: damage = (int)(damage * effectiveResilience);
        try
        {
            var useLinearScaling = generator.DefineLabel();
            var resumeExecution = generator.DefineLabel();
            helper
                .FindNext(
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(Rumble).RequireMethod(nameof(Rumble.rumble), new[] { typeof(float), typeof(float) })),
                    new CodeInstruction(OpCodes.Ldc_I4_1))
                .Advance()
                .AddLabels(useLinearScaling)
                .InsertInstructions(
                    new CodeInstruction(OpCodes.Call, typeof(ModEntry).RequirePropertyGetter(nameof(ModEntry.Config))),
                    new CodeInstruction(
                        OpCodes.Callvirt,
                        typeof(ModConfig).RequirePropertyGetter(nameof(ModConfig.Arsenal))),
                    new CodeInstruction(
                        OpCodes.Callvirt,
                        typeof(Config).RequirePropertyGetter(nameof(Config.OverhauledDefense))),
                    new CodeInstruction(OpCodes.Brfalse_S, useLinearScaling),
                    new CodeInstruction(OpCodes.Ldarg_1),
                    new CodeInstruction(OpCodes.Conv_R4),
                    new CodeInstruction(OpCodes.Ldloc_S, floatResilience),
                    new CodeInstruction(OpCodes.Mul),
                    new CodeInstruction(OpCodes.Conv_I4),
                    new CodeInstruction(OpCodes.Starg_S, (byte)1),
                    new CodeInstruction(OpCodes.Br_S, resumeExecution))
                .FindNext(
                    new CodeInstruction(OpCodes.Ldarg_0))
                .AddLabels(resumeExecution);
        }
        catch (Exception ex)
        {
            Log.E($"Failed while adding overhauled farmer defense (part 2).\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region injected subroutines

    private static float GetOverhauledResilience(Farmer farmer)
    {
        float resilience = farmer.resilience;
        if (farmer.CurrentTool is { } tool and (MeleeWeapon or Slingshot))
        {
            resilience += tool.Read<float>(DataFields.ResonantDefense);
            if (tool is MeleeWeapon weapon)
            {
                resilience += weapon.addedDefense.Value;
            }
        }

        return 10f / (10f + resilience);
    }

    #endregion injected subroutines
}
