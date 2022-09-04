namespace DaLion.Stardew.Professions.Framework.Patches.Common;

#region using directives

using DaLion.Common;
using DaLion.Common.Harmony;
using HarmonyLib;
using LinqFasterer;
using StardewValley.Menus;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Ultimates;
using VirtualProperties;

#endregion using directives

[UsedImplicitly]
internal sealed class LevelUpMenuRemoveImmediateProfessionPerkPatch : DaLion.Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal LevelUpMenuRemoveImmediateProfessionPerkPatch()
    {
        Target = RequireMethod<LevelUpMenu>(nameof(LevelUpMenu.removeImmediateProfessionPerk));
    }

    #region harmony patches

    /// <summary>Patch to remove modded immediate profession perks.</summary>
    [HarmonyPostfix]
    private static void LevelUpMenuRemoveImmediateProfessionPerkPostfix(int whichProfession)
    {
        if (!Profession.TryFromValue(whichProfession, out var profession) ||
            whichProfession == Farmer.luckSkill) return;

        // remove immediate perks
        if (profession == Profession.Aquarist)
            foreach (var pond in Game1.getFarm().buildings.WhereF(p =>
                         (p.owner.Value == Game1.player.UniqueMultiplayerID || !Context.IsMultiplayer) &&
                         !p.isUnderConstruction() && p.maxOccupants.Value > 10))
            {
                pond.maxOccupants.Set(10);
                pond.currentOccupants.Value = Math.Min(pond.currentOccupants.Value, pond.maxOccupants.Value);
            }
        else if (profession == Profession.Rascal)
            StardewValley.Utility.iterateAllItems(item =>
            {
                if (item is not Slingshot { numAttachmentSlots.Value: 2 } slingshot ||
                    !slingshot.getLastFarmerToUse().IsLocalPlayer) return;

                slingshot.attachments[1] = null;
                slingshot.numAttachmentSlots.Value = 1;
                slingshot.attachments.SetCount(1);
            });

        // disable unnecessary events
        ModEntry.Events.DisableForProfession(profession);

        // unregister Ultimate
        if (Game1.player.get_Ultimate()?.Index != (UltimateIndex)whichProfession) return;

        if (Game1.player.professions.AnyF(p => p is >= 26 and < 30))
        {
            var firstIndex = (UltimateIndex)Game1.player.professions.FirstF(p => p is >= 26 and < 30);
            Game1.player.set_Ultimate(Ultimate.FromIndex(firstIndex));
        }
        else
        {
            Game1.player.set_Ultimate(null);
        }
    }

    /// <summary>Patch to move bonus health from Defender to Brute.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? LevelUpMenuRemoveImmediateProfessionPerkTranspiler(
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