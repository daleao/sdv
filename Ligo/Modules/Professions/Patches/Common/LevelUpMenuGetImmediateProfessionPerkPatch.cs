namespace DaLion.Ligo.Modules.Professions.Patches.Common;

#region using directives

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Ligo.Modules.Professions.Events.GameLoop;
using DaLion.Ligo.Modules.Professions.Extensions;
using DaLion.Ligo.Modules.Professions.Ultimates;
using DaLion.Ligo.Modules.Professions.VirtualProperties;
using DaLion.Shared.Extensions.Collections;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Buildings;
using StardewValley.Menus;
using StardewValley.Tools;
using HarmonyPatch = DaLion.Shared.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class LevelUpMenuGetImmediateProfessionPerkPatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="LevelUpMenuGetImmediateProfessionPerkPatch"/> class.</summary>
    internal LevelUpMenuGetImmediateProfessionPerkPatch()
    {
        this.Target = this.RequireMethod<LevelUpMenu>(nameof(LevelUpMenu.getImmediateProfessionPerk));
    }

    #region harmony patches

    /// <summary>Patch to add modded immediate profession perks.</summary>
    [HarmonyPostfix]
    private static void LevelUpMenuGetImmediateProfessionPerkPostfix(int whichProfession)
    {
        if (!Profession.TryFromValue(whichProfession, out var profession) ||
            whichProfession == Farmer.luckSkill)
        {
            return;
        }

        // add immediate perks
        if (profession == Profession.Aquarist)
        {
            Game1.getFarm().buildings
                .OfType<FishPond>()
                .Where(p => (p.owner.Value == Game1.player.UniqueMultiplayerID || !Context.IsMultiplayer ||
                             ModEntry.Config.Professions.LaxOwnershipRequirements) && !p.isUnderConstruction())
                .ForEach(p => p.UpdateMaximumOccupancy());
        }
        else if (profession == Profession.Rascal)
        {
            Utility.iterateAllItems(item =>
            {
                if (item is not Slingshot slingshot || !slingshot.getLastFarmerToUse().IsLocalPlayer)
                {
                    return;
                }

                slingshot.numAttachmentSlots.Value = 2;
                slingshot.attachments.SetCount(2);
            });
        }

        // subscribe events
        ModEntry.Events.EnableForProfession(profession);
        if (!Context.IsMainPlayer)
        {
            // request the main player
            if (profession == Profession.Aquarist)
            {
                ModEntry.Broadcaster.Message("Conservationism", "RequestEvent", Game1.MasterPlayer.UniqueMultiplayerID);
            }
            else if (profession == Profession.Conservationist)
            {
                ModEntry.Broadcaster.Message("Conservationism", "RequestEvent", Game1.MasterPlayer.UniqueMultiplayerID);
            }
        }
        else if (profession == Profession.Conservationist)
        {
            ModEntry.Events.Enable<ConservationismDayEndingEvent>();
        }

        if (whichProfession is < 26 or >= 30 || Game1.player.Get_Ultimate() is not null)
        {
            return;
        }

        // register Ultimate
        Game1.player.Set_Ultimate(Ultimate.FromValue(whichProfession));
    }

    /// <summary>Patch to move bonus health from Defender to Brute.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? LevelUpMenuGetImmediateProfessionPerkTranspiler(
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
