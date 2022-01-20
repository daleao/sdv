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

using Stardew.Common.Harmony;
using Extensions;
using SuperMode;

#endregion using directives

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
        if (!Enum.IsDefined(typeof(Profession), whichProfession)) return;

        var professionName = whichProfession.ToProfessionName();

        // add immediate perks
        if (professionName == "Aquarist")
            foreach (var b in Game1.getFarm().buildings.Where(b =>
                         (b.owner.Value == Game1.player.UniqueMultiplayerID || !Context.IsMultiplayer) &&
                         b is FishPond && !b.isUnderConstruction()))
            {
                var pond = (FishPond) b;
                pond.UpdateMaximumOccupancy();
            }

        // subscribe events
        EventManager.EnableAllForProfession(professionName);
        if (professionName == "Conservationist" && !Context.IsMainPlayer) // request the main player
            ModEntry.ModHelper.Multiplayer.SendMessage("Conservationist", "RequestEventEnable",
                new[] {ModEntry.Manifest.UniqueID}, new[] {Game1.MasterPlayer.UniqueMultiplayerID});

        if (whichProfession is < 26 or >= 30 || ModEntry.State.Value.SuperMode is not null) return;
        
        // register Super Mode
        var newIndex = (SuperModeIndex) whichProfession;
        ModEntry.State.Value.SuperMode = new(newIndex);
        ModData.Write(DataField.SuperModeIndex, newIndex.ToString());
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
                .SetOperand((int) Profession.Brute);
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