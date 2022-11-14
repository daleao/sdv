namespace DaLion.Ligo.Modules.Professions.Patches.Common;

#region using directives

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Ligo.Modules.Professions.Extensions;
using DaLion.Ligo.Modules.Professions.Ultimates;
using DaLion.Ligo.Modules.Professions.VirtualProperties;
using DaLion.Shared.Extensions.Collections;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Menus;
using StardewValley.Tools;
using HarmonyPatch = DaLion.Shared.Harmony.HarmonyPatch;

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
            Game1.getFarm().buildings
                .Where(p => (p.owner.Value == Game1.player.UniqueMultiplayerID || !Context.IsMultiplayer) && !p.isUnderConstruction() && p.maxOccupants.Value > 10)
                .ForEach(p =>
                {
                    p.maxOccupants.Set(10);
                    p.currentOccupants.Value = Math.Min(p.currentOccupants.Value, p.maxOccupants.Value);
                });
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
