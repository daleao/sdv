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
using TheLion.Stardew.Common.Harmony;

namespace TheLion.Stardew.Professions.Framework.Patches.Common;

[UsedImplicitly]
internal class LevelUpMenuGetImmediateProfessionPerkPatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal LevelUpMenuGetImmediateProfessionPerkPatch()
    {
        Original = RequireMethod<LevelUpMenu>(nameof(LevelUpMenu.getImmediateProfessionPerk));
    }

    #region harmony patches

    /// <summary>Patch to add modded immediate profession perks.</summary>
    [HarmonyPostfix]
    private static void LevelUpMenuGetImmediateProfessionPerkPostfix(int whichProfession)
    {
        if (!Utility.Professions.IndexByName.TryGetReverseValue(whichProfession, out var professionName)) return;

        // add immediate perks
        if (professionName == "Aquarist")
            foreach (var b in Game1.getFarm().buildings.Where(b =>
                         (b.owner.Value == Game1.player.UniqueMultiplayerID || !Context.IsMultiplayer) &&
                         b is FishPond && !b.isUnderConstruction()))
            {
                var pond = (FishPond)b;
                pond.UpdateMaximumOccupancy();
            }

        // subscribe events
        ModEntry.Subscriber.SubscribeEventsForProfession(professionName);
        if (professionName == "Conservationist") // request the main player
            ModEntry.ModHelper.Multiplayer.SendMessage("Conservationist", "RequestEventSubscription",
                new[] {ModEntry.Manifest.UniqueID}, new[] {Game1.MasterPlayer.UniqueMultiplayerID});

        if (whichProfession is >= 26 and < 30 &&
            ModEntry.State.Value.SuperModeIndex < 0) // is level 10 combat profession and Super Mode is not yet registered
            // register Super Mode
            ModEntry.State.Value.SuperModeIndex = whichProfession;
    }

    /// <summary>Patch to move bonus health from Defender to Brute.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> LevelUpMenuGetImmediateProfessionPerkTranspiler(
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
                .SetOperand(Utility.Professions.IndexOf("Brute"));
        }
        catch (Exception ex)
        {
            ModEntry.Log($"Failed while moving vanilla Defender health bonus to Brute.\nHelper returned {ex}",
                LogLevel.Error);
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}