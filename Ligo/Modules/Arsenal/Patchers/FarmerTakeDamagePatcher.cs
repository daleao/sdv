namespace DaLion.Ligo.Modules.Arsenal.Patchers;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Ligo.Modules.Arsenal.Extensions;
using DaLion.Shared.Extensions.Reflection;
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

    /// <summary>Grant i-frames during stabby sword lunge.</summary>
    [HarmonyPrefix]
    private static bool FarmerTakeDamagePrefix(Farmer __instance)
    {
        return __instance.CurrentTool is not MeleeWeapon { type.Value: MeleeWeapon.stabbingSword, isOnSpecial: true };
    }

    /// <summary>Overhaul for farmer defense.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? FarmerTakeDamageTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);
        var floatResilience = generator.DeclareLocal(typeof(float));

        // From: bullshit resilience mitigation
        // To: if (ArsenalModule.Config.OverhauledDefense)
        //          GetOverhauledResilience(this)
        //     else { do vanilla logic }
        try
        {
            var useVanillaResilience = generator.DefineLabel();
            var resumeExecution = generator.DefineLabel();
            helper
                .Match(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(OpCodes.Ldfld, typeof(Farmer).RequireField(nameof(Farmer.resilience))),
                        new CodeInstruction(OpCodes.Stloc_3),
                    })
                .AddLabels(useVanillaResilience)
                .Insert(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Call, typeof(ModEntry).RequirePropertyGetter(nameof(Config))),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(ModConfig).RequirePropertyGetter(nameof(ModConfig.Arsenal))),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(ArsenalConfig).RequirePropertyGetter(nameof(ArsenalConfig.OverhauledDefense))),
                        new CodeInstruction(OpCodes.Brfalse_S, useVanillaResilience),
                        new CodeInstruction(OpCodes.Ldarg_0), new CodeInstruction(
                            OpCodes.Call,
                            typeof(FarmerExtensions).RequireMethod(nameof(FarmerExtensions.GetOverhauledResilience))),
                        new CodeInstruction(OpCodes.Stloc_S, floatResilience),
                        new CodeInstruction(OpCodes.Br_S, resumeExecution),
                    })
                .Match(
                    new[]
                    {
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(Farmer).RequireMethod(nameof(Farmer.isWearingRing))),
                    })
                .Move(-2)
                .AddLabels(resumeExecution);
        }
        catch (Exception ex)
        {
            Log.E($"Failed adding overhauled farmer defense (part 1).\nHelper returned {ex}");
            return null;
        }

        // From: damage = Math.Max(1, damage - effectiveResilience);
        // To: damage = (int)(damage * effectiveResilience);
        try
        {
            var useLinearScaling = generator.DefineLabel();
            var resumeExecution = generator.DefineLabel();
            helper
                .Match(
                    new[]
                    {
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(Rumble).RequireMethod(
                                nameof(Rumble.rumble),
                                new[] { typeof(float), typeof(float) })),
                        new CodeInstruction(OpCodes.Ldc_I4_1),
                    })
                .Move()
                .AddLabels(useLinearScaling)
                .Insert(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Call, typeof(ModEntry).RequirePropertyGetter(nameof(Config))),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(ModConfig).RequirePropertyGetter(nameof(ModConfig.Arsenal))),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(ArsenalConfig).RequirePropertyGetter(nameof(ArsenalConfig.OverhauledDefense))),
                        new CodeInstruction(OpCodes.Brfalse_S, useLinearScaling), new CodeInstruction(OpCodes.Ldarg_1),
                        new CodeInstruction(OpCodes.Conv_R4), new CodeInstruction(OpCodes.Ldloc_S, floatResilience),
                        new CodeInstruction(OpCodes.Mul), new CodeInstruction(OpCodes.Conv_I4),
                        new CodeInstruction(OpCodes.Starg_S, (byte)1),
                        new CodeInstruction(OpCodes.Br_S, resumeExecution),
                    })
                .Match(
                    new[] { new CodeInstruction(OpCodes.Ldarg_0) })
                .AddLabels(resumeExecution);
        }
        catch (Exception ex)
        {
            Log.E($"Failed adding overhauled farmer defense (part 2).\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}
