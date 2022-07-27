namespace DaLion.Stardew.Professions.Framework.Patches.Common;

#region using directives

using DaLion.Common;
using DaLion.Common.Harmony;
using DaLion.Common.ModData;
using Events.GameLoop;
using HarmonyLib;
using JetBrains.Annotations;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Ultimates;

#endregion using directives

[UsedImplicitly]
internal sealed class LevelUpMenuGetImmediateProfessionPerkPatch : DaLion.Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal LevelUpMenuGetImmediateProfessionPerkPatch()
    {
        Target = RequireMethod<LevelUpMenu>(nameof(LevelUpMenu.getImmediateProfessionPerk));
    }

    #region harmony patches

    /// <summary>Patch to add modded immediate profession perks.</summary>
    [HarmonyPostfix]
    private static void LevelUpMenuGetImmediateProfessionPerkPostfix(int whichProfession)
    {
        if (!Profession.TryFromValue(whichProfession, out var profession) ||
            whichProfession == Farmer.luckSkill) return;

        // add immediate perks
        if (profession == Profession.Aquarist)
            foreach (var pond in Game1.getFarm().buildings.OfType<FishPond>().Where(p =>
                         (p.owner.Value == Game1.player.UniqueMultiplayerID || !Context.IsMultiplayer) &&
                         !p.isUnderConstruction()))
                pond.UpdateMaximumOccupancy();

        // subscribe events
        ModEntry.EventManager.HookForProfession(profession);
        if (!Context.IsMainPlayer)
        {
            // request the main player
            if (profession == Profession.Aquarist)
                ModEntry.Broadcaster.Message("Conservationism", "RequestEvent", Game1.MasterPlayer.UniqueMultiplayerID);
            else if (profession == Profession.Conservationist)
                ModEntry.Broadcaster.Message("Conservationism", "RequestEvent", Game1.MasterPlayer.UniqueMultiplayerID);
        }
        else if (profession == Profession.Conservationist)
        {
            ModEntry.EventManager.Hook<ConservationismDayEndingEvent>();
        }

        if (whichProfession is < 26 or >= 30 || ModEntry.Player.RegisteredUltimate is not null) return;

        // register Ultimate
        var newIndex = (UltimateIndex)whichProfession;
        ModEntry.Player.RegisteredUltimate =
#pragma warning disable CS8509
            ModEntry.Player.RegisteredUltimate = newIndex switch
#pragma warning restore CS8509
            {
                UltimateIndex.BruteFrenzy => new Frenzy(),
                UltimateIndex.PoacherAmbush => new Ambush(),
                UltimateIndex.PiperConcerto => new Concerto(),
                UltimateIndex.DesperadoBlossom => new DeathBlossom()
            };
        ModDataIO.Write(Game1.player, "UltimateIndex", newIndex.ToString());
    }

    /// <summary>Patch to move bonus health from Defender to Brute.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? LevelUpMenuGetImmediateProfessionPerkTranspiler(
        IEnumerable<CodeInstruction> instructions, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        /// From: case <defender_id>:
        /// To: case <brute_id>:

        try
        {
            helper
                .FindFirst(
                    new CodeInstruction(OpCodes.Ldc_I4_S, Farmer.defender)
                )
                .SetOperand(Profession.Brute.Value);
        }
        catch (Exception ex)
        {
            Log.E($"Failed while moving vanilla Defender health bonus to Brute.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}