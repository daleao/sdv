namespace DaLion.Ligo.Modules.Professions.Patchers.Common;

#region using directives

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Ligo.Modules.Professions.Events.Display;
using DaLion.Ligo.Modules.Professions.Ultimates;
using DaLion.Ligo.Modules.Professions.VirtualProperties;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Collections;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Buildings;
using StardewValley.Menus;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class LevelUpMenuGetImmediateProfessionPerkPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="LevelUpMenuGetImmediateProfessionPerkPatcher"/> class.</summary>
    internal LevelUpMenuGetImmediateProfessionPerkPatcher()
    {
        this.Target = this.RequireMethod<LevelUpMenu>(nameof(LevelUpMenu.getImmediateProfessionPerk));
    }

    #region harmony patches

    /// <summary>Patch to add modded immediate profession perks.</summary>
    [HarmonyPostfix]
    private static void LevelUpMenuGetImmediateProfessionPerkPostfix(int whichProfession)
    {
        if (whichProfession.IsIn(Profession.GetRange(true)))
        {
            ModHelper.GameContent.InvalidateCache("LooseSprites/Cursors");
        }

        if (!Profession.TryFromValue(whichProfession, out var profession) ||
            whichProfession == Farmer.luckSkill)
        {
            return;
        }

        // add immediate perks
        profession
            .When(Profession.Aquarist).Then(() =>
            {
                Game1.getFarm().buildings
                    .OfType<FishPond>()
                    .Where(p => (p.owner.Value == Game1.player.UniqueMultiplayerID || !Context.IsMultiplayer ||
                                 ProfessionsModule.Config.LaxOwnershipRequirements) && !p.isUnderConstruction())
                    .ForEach(p => p.UpdateMaximumOccupancy());
            })
            .When(Profession.Rascal).Then(() =>
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
            })
            .When(Profession.Prospector).Then(() =>
            {
                EventManager.Enable<ProspectorHuntRenderedHudEvent>();
            })
            .When(Profession.Scavenger).Then(() =>
            {
                EventManager.Enable<ScavengerHuntRenderedHudEvent>();
            });

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
        var helper = new ILHelper(original, instructions);

        // From: case <defender_id>:
        // To: case <brute_id>:
        try
        {
            helper
                .Match(new[] { new CodeInstruction(OpCodes.Ldc_I4_S, Farmer.defender) })
                .SetOperand(Profession.Brute.Value);
        }
        catch (Exception ex)
        {
            Log.E($"Failed moving vanilla Defender health bonus to Brute.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}
