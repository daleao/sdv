namespace DaLion.Overhaul.Modules.Professions.Patchers.Common;

#region using directives

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
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

        // remove immediate perks
        profession
            .When(Profession.Aquarist).Then(() =>
            {
                var buildings = Game1.getFarm().buildings;
                for (var i = 0; i < buildings.Count; i++)
                {
                    var building = buildings[i];
                    if (building is not FishPond pond ||
                        !(pond.IsOwnedBy(Game1.player) || ProfessionsModule.Config.LaxOwnershipRequirements) ||
                        pond.isUnderConstruction() || pond.maxOccupants.Value <= 10)
                    {
                        continue;
                    }

                    pond.maxOccupants.Set(10);
                    pond.currentOccupants.Value = Math.Min(pond.currentOccupants.Value, pond.maxOccupants.Value);
                }
            })
            .When(Profession.Prospector).Then(() => EventManager.Disable<ProspectorRenderedHudEvent>())
            .When(Profession.Scavenger).Then(() => EventManager.Disable<ScavengerRenderedHudEvent>());

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
