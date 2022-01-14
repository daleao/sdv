namespace DaLion.Stardew.Professions.Framework.Patches.Common;

#region using directives

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Menus;

using Stardew.Common.Extensions;
using Stardew.Common.Harmony;
using SuperMode;

using Professions = Utility.Professions;

#endregion using directives

[UsedImplicitly]
internal class LevelUpMenuRemoveImmediateProfessionPerkPatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal LevelUpMenuRemoveImmediateProfessionPerkPatch()
    {
        Original = RequireMethod<LevelUpMenu>(nameof(LevelUpMenu.removeImmediateProfessionPerk));
    }

    #region harmony patches

    /// <summary>Patch to remove modded immediate profession perks.</summary>
    [HarmonyPostfix]
    private static void LevelUpMenuRemoveImmediateProfessionPerkPostfix(int whichProfession)
    {
        if (!Professions.IndexByName.TryGetReverseValue(whichProfession, out var professionName)) return;

        // remove immediate perks
        if (professionName == "Aquarist")
            foreach (var b in Game1.getFarm().buildings.Where(b =>
                         (b.owner.Value == Game1.player.UniqueMultiplayerID || !Context.IsMultiplayer) &&
                         b is FishPond && !b.isUnderConstruction() && b.maxOccupants.Value > 10))
            {
                b.maxOccupants.Set(10);
                b.currentOccupants.Value = Math.Min(b.currentOccupants.Value, b.maxOccupants.Value);
            }

        // clean unnecessary mod data
        if (!professionName.IsAnyOf("Scavenger", "Prospector"))
            ModData.CleanUpRogueData();

        // unsubscribe unnecessary events
        ModEntry.EventManager.DisableAllForProfession(professionName);

        // unregister Super Mode
        if (ModEntry.State.Value.SuperMode?.Index != (SuperModeIndex)whichProfession) return;

        if (Game1.player.professions.Any(p => p is >= 26 and < 30))
        {
            var firstIndex = (SuperModeIndex) Game1.player.professions.First(p => p is >= 26 and < 30);
            ModData.Write(DataField.SuperModeIndex, firstIndex.ToString());
            ModEntry.State.Value.SuperMode = new(firstIndex);
        }
        else
        {
            ModData.Write(DataField.SuperModeIndex, null);
            ModEntry.State.Value.SuperMode = null;
        }
    }

    /// <summary>Patch to move bonus health from Defender to Brute.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> LevelUpMenuRemoveImmediateProfessionPerkTranspiler(
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
                .SetOperand(Professions.IndexOf("Brute"));
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