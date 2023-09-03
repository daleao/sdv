namespace DaLion.Overhaul.Modules.Professions.Patchers;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Overhaul.Modules.Professions;
using DaLion.Overhaul.Modules.Professions.Events.Display.RenderedHud;
using DaLion.Overhaul.Modules.Professions.Ultimates;
using DaLion.Overhaul.Modules.Professions.VirtualProperties;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.SMAPI;
using DaLion.Shared.Extensions.Stardew;
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
            ModHelper.GameContent.InvalidateCacheAndLocalized("LooseSprites/Cursors");
        }

        if (!Profession.TryFromValue(whichProfession, out var profession))
        {
            return;
        }

        var player = Game1.player;

        // add immediate perks
        profession
            .When(Profession.Aquarist).Then(() =>
            {
                var buildings = Game1.getFarm().buildings;
                for (var i = 0; i < buildings.Count; i++)
                {
                    var building = buildings[i];
                    if (building is FishPond pond &&
                        (pond.IsOwnedBy(player) || ProfessionsModule.Config.LaxOwnershipRequirements) &&
                        !pond.isUnderConstruction())
                    {
                        pond.UpdateMaximumOccupancy();
                    }
                }
            })
            .When(Profession.Prospector).Then(() => EventManager.Enable<ProspectorRenderedHudEvent>())
            .When(Profession.Scavenger).Then(() => EventManager.Enable<ScavengerRenderedHudEvent>())
            .When(Profession.Rascal).Then(() =>
            {
                if (player.CurrentTool is not Slingshot slingshot ||
                    (slingshot.numAttachmentSlots.Value != 1 && slingshot.attachments.Length != 1))
                {
                    return;
                }

                slingshot.numAttachmentSlots.Value = 2;
                slingshot.attachments.SetCount(2);
            });

        if (whichProfession is < 26 or >= 30 || player.Get_Ultimate() is not null)
        {
            return;
        }

        // register Ultimate
        player.Set_Ultimate(Ultimate.FromValue(whichProfession));
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
