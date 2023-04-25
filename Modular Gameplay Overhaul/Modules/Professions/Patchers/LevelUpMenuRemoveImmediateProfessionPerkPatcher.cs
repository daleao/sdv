namespace DaLion.Overhaul.Modules.Professions.Patchers;

#region using directives

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Overhaul.Modules.Professions;
using DaLion.Overhaul.Modules.Professions.Events.Display;
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
internal sealed class LevelUpMenuRemoveImmediateProfessionPerkPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="LevelUpMenuRemoveImmediateProfessionPerkPatcher"/> class.</summary>
    internal LevelUpMenuRemoveImmediateProfessionPerkPatcher()
    {
        this.Target = this.RequireMethod<LevelUpMenu>(nameof(LevelUpMenu.removeImmediateProfessionPerk));
    }

    #region harmony patches

    /// <summary>Patch to remove modded immediate profession perks.</summary>
    [HarmonyPostfix]
    private static void LevelUpMenuRemoveImmediateProfessionPerkPostfix(int whichProfession)
    {
        if (whichProfession.IsIn(Profession.GetRange(true)))
        {
            ModHelper.GameContent.InvalidateCacheAndLocalized("LooseSprites/Cursors");
        }

        if (!Profession.TryFromValue(whichProfession, out var profession) ||
            whichProfession == Farmer.luckSkill)
        {
            return;
        }

        var player = Game1.player;

        // remove immediate perks
        profession
            .When(Profession.Aquarist).Then(() =>
            {
                var buildings = Game1.getFarm().buildings;
                for (var i = 0; i < buildings.Count; i++)
                {
                    var building = buildings[i];
                    if (building is not FishPond pond ||
                        !(pond.IsOwnedBy(player) || ProfessionsModule.Config.LaxOwnershipRequirements) ||
                        pond.isUnderConstruction() || pond.maxOccupants.Value <= 10)
                    {
                        continue;
                    }

                    pond.maxOccupants.Set(10);
                    pond.currentOccupants.Value = Math.Min(pond.currentOccupants.Value, pond.maxOccupants.Value);
                }
            })
            .When(Profession.Prospector).Then(() => EventManager.Disable<ProspectorRenderedHudEvent>())
            .When(Profession.Scavenger).Then(() => EventManager.Disable<ScavengerRenderedHudEvent>())
            .When(Profession.Rascal).Then(() =>
            {
                if (player.CurrentTool is not Slingshot slingshot ||
                    (slingshot.numAttachmentSlots.Value <= 1 && slingshot.attachments.Length <= 1))
                {
                    return;
                }

                var replacement = new Slingshot(slingshot.InitialParentTileIndex);
                if (slingshot.attachments[0] is { } ammo1)
                {
                    replacement.attachments[0] = (SObject)ammo1.getOne();
                    replacement.attachments[0].Stack = ammo1.Stack;
                }

                if (slingshot.attachments.Length > 1 && slingshot.attachments[1] is { } ammo2)
                {
                    var drop = (SObject)ammo2.getOne();
                    drop.Stack = ammo2.Stack;
                    if (!player.addItemToInventoryBool(drop))
                    {
                        Game1.createItemDebris(drop, player.getStandingPosition(), -1, player.currentLocation);
                    }
                }

                player.Items[player.CurrentToolIndex] = replacement;
            });

        // unregister Ultimate
        if (player.Get_Ultimate()?.Value != whichProfession)
        {
            return;
        }

        if (player.professions.Any(p => p is >= 26 and < 30))
        {
            var firstIndex = player.professions.First(p => p is >= 26 and < 30);
            player.Set_Ultimate(Ultimate.FromValue(firstIndex));
        }
        else
        {
            player.Set_Ultimate(null);
        }
    }

    /// <summary>Patch to move bonus health from Defender to Brute.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? LevelUpMenuRemoveImmediateProfessionPerkTranspiler(
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
