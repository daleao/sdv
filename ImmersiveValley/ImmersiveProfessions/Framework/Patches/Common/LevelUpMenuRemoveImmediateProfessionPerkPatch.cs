namespace DaLion.Stardew.Professions.Framework.Patches.Common;

#region using directives

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Common.Harmony;
using DaLion.Stardew.Professions.Framework.Ultimates;
using DaLion.Stardew.Professions.Framework.VirtualProperties;
using HarmonyLib;
using StardewValley.Menus;
using StardewValley.Tools;
using HarmonyPatch = DaLion.Common.Harmony.HarmonyPatch;
using Utility = StardewValley.Utility;

#endregion using directives

[UsedImplicitly]
internal sealed class LevelUpMenuRemoveImmediateProfessionPerkPatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="LevelUpMenuRemoveImmediateProfessionPerkPatch"/> class.</summary>
    internal LevelUpMenuRemoveImmediateProfessionPerkPatch()
    {
        this.Target = this.RequireMethod<LevelUpMenu>(nameof(LevelUpMenu.removeImmediateProfessionPerk));
    }

    #region harmony patches

    /// <summary>Patch to remove modded immediate profession perks.</summary>
    [HarmonyPostfix]
    private static void LevelUpMenuRemoveImmediateProfessionPerkPostfix(int whichProfession)
    {
        if (!Profession.TryFromValue(whichProfession, out var profession) ||
            whichProfession == Farmer.luckSkill)
        {
            return;
        }

        // remove immediate perks
        if (profession == Profession.Aquarist)
        {
            foreach (var pond in Game1.getFarm().buildings.Where(p =>
                         (p.owner.Value == Game1.player.UniqueMultiplayerID || !Context.IsMultiplayer) &&
                         !p.isUnderConstruction() && p.maxOccupants.Value > 10))
            {
                pond.maxOccupants.Set(10);
                pond.currentOccupants.Value = Math.Min(pond.currentOccupants.Value, pond.maxOccupants.Value);
            }
        }
        else if (profession == Profession.Rascal)
        {
            Utility.iterateAllItems(item =>
            {
                if (item is not Slingshot { numAttachmentSlots.Value: 2 } slingshot ||
                    !slingshot.getLastFarmerToUse().IsLocalPlayer)
                {
                    return;
                }

                slingshot.attachments[1] = null;
                slingshot.numAttachmentSlots.Value = 1;
                slingshot.attachments.SetCount(1);
            });
        }

        // disable unnecessary events
        ModEntry.Events.DisableForProfession(profession);

        // unregister Ultimate
        if (Game1.player.Get_Ultimate()?.Value != whichProfession)
        {
            return;
        }

        if (Game1.player.professions.Any(p => p is >= 26 and < 30))
        {
            var firstIndex = Game1.player.professions.First(p => p is >= 26 and < 30);
            Game1.player.Set_Ultimate(Ultimate.FromValue(firstIndex));
        }
        else
        {
            Game1.player.Set_Ultimate(null);
        }
    }

    /// <summary>Patch to move bonus health from Defender to Brute.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? LevelUpMenuRemoveImmediateProfessionPerkTranspiler(
        IEnumerable<CodeInstruction> instructions, MethodBase original)
    {
        var helper = new IlHelper(original, instructions);

        // From: case <defender_id>:
        // To: case <brute_id>:
        try
        {
            helper
                .FindFirst(new CodeInstruction(OpCodes.Ldc_I4_S, Farmer.defender))
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
