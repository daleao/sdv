namespace DaLion.Professions.Framework.Patchers.Mining;

#region using directives

using System.Reflection;
using System.Reflection.Emit;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Locations;

#endregion using directives

[UsedImplicitly]
internal sealed class Game1UpdatePause : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="Game1UpdatePause"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal Game1UpdatePause(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireMethod<Game1>(nameof(Game1.updatePause));
    }

    #region harmony patches

    /// <summary>Patch to implement Prestiged Spelunker checkpoint.</summary>
    [HarmonyTranspiler]
    [UsedImplicitly]
    private static IEnumerable<CodeInstruction>? Game1UpdatePauseTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        try
        {
            var skipReviveLocations = generator.DefineLabel();
            helper
                .PatternMatch([new CodeInstruction(OpCodes.Stloc_2)])
                .Move()
                .Insert(
                [
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(Game1UpdatePause).RequireMethod(nameof(DoSpelunkerRevival))),
                    new CodeInstruction(OpCodes.Dup),
                    new CodeInstruction(OpCodes.Stloc_2),
                    new CodeInstruction(OpCodes.Brtrue, skipReviveLocations),
                ])
                .PatternMatch([new CodeInstruction(OpCodes.Ldloc_2)])
                .AddLabels(skipReviveLocations);
        }
        catch (Exception ex)
        {
            Log.E($"Failed injecting Prestiged Spelunker revive.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region injected

    private static bool DoSpelunkerRevival()
    {
        if (Game1.currentLocation is not MineShaft shaft || !Game1.player.HasProfession(Profession.Spelunker, true) ||
            State.HasSpelunkerUsedCheckpointToday || State.SpelunkerCheckpoint is null ||
            State.SpelunkerCheckpointTile is null)
        {
            return false;
        }

        var request = new LocationRequest(shaft.Name, false, State.SpelunkerCheckpoint);
        request.OnWarp += () =>
        {
            Game1.killScreen = false;
            Game1.player.health = 10;
            Game1.pauseThenMessage(1500, Game1.parseText(I18n.Spelunker_Revival()));
            State.UsingSpelunkerCheckpoint = true;
            State.HasSpelunkerUsedCheckpointToday = true;
        };

        Game1.warpFarmer(request, 0, 0, 0);
        return true;
    }

    #endregion injected
}
