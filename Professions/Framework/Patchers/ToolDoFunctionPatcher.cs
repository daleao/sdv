namespace DaLion.Professions.Framework.Patchers;

#region using directives

using System.Reflection;
using System.Reflection.Emit;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class ToolDoFunctionPatcher : HarmonyPatcher
{
    private static string _target = string.Empty;

    /// <summary>Initializes a new instance of the <see cref="ToolDoFunctionPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal ToolDoFunctionPatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
    }

    /// <inheritdoc />
    protected override bool ApplyImpl(Harmony harmony)
    {
        var isChargeableLoaded = ModHelper.ModRegistry.IsLoaded("DaLion.Chargeable");

        this.Target = this.RequireMethod<Axe>(nameof(Axe.DoFunction));
        _target = "axe";
        var appliedToAxe = isChargeableLoaded || base.ApplyImpl(harmony);

        this.Target = this.RequireMethod<WateringCan>(nameof(WateringCan.DoFunction));
        _target = "can";
        var appliedToCan = base.ApplyImpl(harmony);

        this.Target = this.RequireMethod<Hoe>(nameof(Hoe.DoFunction));
        _target = "hoe";
        var appliedToHoe = base.ApplyImpl(harmony);

        this.Target = this.RequireMethod<Pickaxe>(nameof(Pickaxe.DoFunction));
        _target = "pick";
        var appliedToPick = isChargeableLoaded || base.ApplyImpl(harmony);

        return appliedToAxe && appliedToCan && appliedToHoe && appliedToPick;
    }

    #region harmony patches

    [HarmonyTranspiler]
    [UsedImplicitly]
    private static IEnumerable<CodeInstruction>? ToolDoFunctionTranspiler(
        IEnumerable<CodeInstruction> instructions, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        try
        {
            helper
                .PatternMatch(
                    [new CodeInstruction(OpCodes.Callvirt, typeof(Farmer).RequirePropertySetter(nameof(Farmer.Stamina)))])
                .PatternMatch(
                    [
                        new CodeInstruction(OpCodes.Conv_R4),
                        new CodeInstruction(OpCodes.Ldc_R4, 0.1f),
                        new CodeInstruction(OpCodes.Mul),
                    ],
                    ILHelper.SearchOption.Previous)
                .Move(-1)
                .ReplaceWith(
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(ToolDoFunctionPatcher).RequireMethod(
                            _target switch {
                                "axe" => nameof(GetForagingLevelCapped),
                                "can" => nameof(GetFarmingLevelCapped),
                                "hoe" => nameof(GetFarmingLevelCapped),
                                "pick" => nameof(GetMiningLevelCapped),
                            }
                        )
                    )
                );
        }
        catch (Exception ex)
        {
            Log.E($"Failed fixing stamina cost.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    private static int GetFarmingLevelCapped(Farmer farmer)
    {
        return Math.Max(Math.Min(farmer.farmingLevel.Value, 10) + farmer.buffs.FarmingLevel, 0);
    }

    private static int GetForagingLevelCapped(Farmer farmer)
    {
        return Math.Max(Math.Min(farmer.foragingLevel.Value, 10) + farmer.buffs.FarmingLevel, 0);
    }

    private static int GetMiningLevelCapped(Farmer farmer)
    {
        return Math.Max(Math.Min(farmer.miningLevel.Value, 10) + farmer.buffs.FarmingLevel, 0);
    }
}
