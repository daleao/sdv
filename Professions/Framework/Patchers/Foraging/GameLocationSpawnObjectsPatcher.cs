namespace DaLion.Professions.Framework.Patchers.Foraging;

#region using directives

using System.Reflection;
using System.Reflection.Emit;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.GameData.Locations;

#endregion using directives

[UsedImplicitly]
internal sealed class GameLocationSpawnObjectsPatcher : HarmonyPatcher
{
    private static int? _highestStreak = null;

    /// <summary>Initializes a new instance of the <see cref="GameLocationSpawnObjectsPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal GameLocationSpawnObjectsPatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireMethod<GameLocation>(nameof(GameLocation.spawnObjects));
    }

    #region harmony patches

    /// <summary>Patch to buff max forages for Scavenger.</summary>
    [HarmonyTranspiler]
    [UsedImplicitly]
    private static IEnumerable<CodeInstruction>? GameLocationSpawnObjectsTranspiler(
        IEnumerable<CodeInstruction> instructions, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        try
        {
            helper
                .ForEach(
                    [
                        new CodeInstruction(
                            OpCodes.Ldfld,
                            typeof(LocationData).RequireField(nameof(LocationData.MaxSpawnedForageAtOnce))),
                    ],
                    _ => helper
                        .Move()
                        .Insert([
                            new CodeInstruction(
                                OpCodes.Call,
                                typeof(GameLocationSpawnObjectsPatcher).RequireMethod(
                                    nameof(GetScavengerForageSpawnBonus))),
                            new CodeInstruction(OpCodes.Add),
                        ]));

            helper
                .PatternMatch(
                    [
                        new CodeInstruction(OpCodes.Ldc_I4_S, 11)
                    ],
                    ILHelper.SearchOption.First)
                .ReplaceWith(
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(GameLocationSpawnObjectsPatcher).RequireMethod(
                            nameof(GetScavengerSpawnAttemptBonus))));
        }
        catch (Exception ex)
        {
            Log.E($"Failed adding Prestiged Scavenger bonus forage.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    [HarmonyFinalizer]
    private static void GameLocationSpawnObjectsFinalizer()
    {
        _highestStreak = null;
    }

    #endregion harmony patches

    #region injections

    private static int GetScavengerForageSpawnBonus()
    {
        _highestStreak ??= Game1.game1.DoesAnyPlayerHaveProfession(Profession.Scavenger, out var scavengers, prestiged: true)
            ? scavengers.Aggregate(
                0,
                (current, scavenger) => Math.Max(current, Data.ReadAs<int>(scavenger, DataKeys.ScavengerHuntStreak)))
            : 0;
        return _highestStreak.Value / 2;
    }

    private static int GetScavengerSpawnAttemptBonus()
    {
        _highestStreak ??= Game1.game1.DoesAnyPlayerHaveProfession(Profession.Scavenger, out var scavengers, prestiged: true)
            ? scavengers.Aggregate(
                0,
                (current, scavenger) => Math.Max(current, Data.ReadAs<int>(scavenger, DataKeys.ScavengerHuntStreak)))
            : 0;
        return _highestStreak.Value + 11;
    }

    #endregion injections
}
