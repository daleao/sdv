namespace DaLion.Professions.Framework.Patchers.Integration;

#region using directives

using DaLion.Core.Framework.Extensions;
using DaLion.Professions;
using DaLion.Shared.Attributes;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley.GameData.Machines;

#endregion using directives

[UsedImplicitly]
[ModRequirement("DaLion.Core")]
internal sealed class Core_AttemptPushToHopperPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="Core_AttemptPushToHopperPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal Core_AttemptPushToHopperPatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = "DaLion.Core.Framework.Patchers.ObjectCheckForActionOnMachinePatcher".ToType().RequireMethod("AttemptPushToHopper");
    }

    #region harmony patches

    /// <summary>Implements Machinist's leftover material recovery on automated machine.</summary>
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void CoreAttemptPushToHopperPostfix(bool __result, SObject machine, MachineData machineData, SObject objectThatWasHeld, Farmer who)
    {
        if (!machine.IsArtisanMachine() || !who.HasProfession(Profession.Artisan, true) || !__result)
        {
            return;
        }

        var repeatedCycles = Data.ReadAs<int>(machine, DataKeys.RepeatedInputCycles);
        if (repeatedCycles <= 10 || (repeatedCycles - 10) % 5 != 0)
        {
            return;
        }

        // we know there must be an adjacent hopper because the main method returned true
        var tileBelow = new Vector2(machine.TileLocation.X, machine.TileLocation.Y + 1f);
        if (machine.Location?.Objects.TryGetValue(tileBelow, out var objBelow) != true ||
            !objBelow.TryGetHopper(out var hopper))
        {
            var tileAbove = new Vector2(machine.TileLocation.X, machine.TileLocation.Y - 1f);
            if (machine.Location?.Objects.TryGetValue(tileAbove, out var objAbove) != true ||
                !objAbove.TryGetHopper(out hopper))
            {
                return; // this should never happen
            }
        }

        var extraOutput = objectThatWasHeld.getOne();
        extraOutput.Quality = SObject.lowQuality;
        hopper.addItem(extraOutput);
    }

    #endregion harmony patches
}
