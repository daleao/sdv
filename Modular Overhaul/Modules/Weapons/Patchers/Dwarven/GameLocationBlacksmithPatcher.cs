namespace DaLion.Overhaul.Modules.Weapons.Patchers.Dwarven;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Overhaul.Modules.Professions.Integrations;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Netcode;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class GameLocationBlacksmithPatcher : HarmonyPatcher
{
    private static bool? _isPanningUpgradesLoaded;
    private static bool? _isRanchingToolUpgradesLoaded;

    /// <summary>Initializes a new instance of the <see cref="GameLocationBlacksmithPatcher"/> class.</summary>
    internal GameLocationBlacksmithPatcher()
    {
        this.Target = this.RequireMethod<GameLocation>(nameof(GameLocation.blacksmith));
    }

    #region harmony patches

    /// <summary>Inject forging.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? GameLocationBlacksmithTranspiler(IEnumerable<CodeInstruction> instructions, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        // Replaced: all response creation until createQuestionDialogue
        // With: CreateBlacksmithQuestionDialogue(this);
        try
        {
            helper
                .Match(
                    new[]
                    {
                        new CodeInstruction(
                            OpCodes.Ldfld,
                            typeof(Farmer).RequireField(nameof(Farmer.toolBeingUpgraded))),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(NetFieldBase<Tool, NetRef<Tool>>).RequirePropertyGetter(
                                nameof(NetFieldBase<Tool, NetRef<Tool>>.Value))),
                        new CodeInstruction(OpCodes.Brtrue),
                    })
                .Move(2)
                .SetOpCode(OpCodes.Brtrue_S)
                .Move()
                .Count(
                    new[]
                    {
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(GameLocation).RequireMethod(
                                nameof(GameLocation.createQuestionDialogue),
                                new[] { typeof(string), typeof(Response[]), typeof(string) })),
                    },
                    out var count)
                .Remove(count)
                .Insert(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(GameLocationBlacksmithPatcher).RequireMethod(
                                nameof(CreateBlacksmithQuestionDialogue))),
                    });
        }
        catch (Exception ex)
        {
            Log.E($"Failed adding blacksmith forge option.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region injected subroutines

    private static void CreateBlacksmithQuestionDialogue(GameLocation location)
    {
        var responses = new List<Response>();
        responses.Add(new Response("Shop", Game1.content.LoadString("Strings\\Locations:Blacksmith_Clint_Shop")));

        if (HasUpgradeableToolInInventory(Game1.player))
        {
            responses.Add(new Response(
                "Upgrade",
                Game1.content.LoadString("Strings\\Locations:Blacksmith_Clint_Upgrade")));
        }

        if (HasGeodeInInventory(Game1.player))
        {
            responses.Add(new Response(
                "Process",
                Game1.content.LoadString("Strings\\Locations:Blacksmith_Clint_Geodes")));
        }

        if (WeaponsModule.Config.DwarvenLegacy && Game1.player.mailReceived.Contains("clintForge"))
        {
            responses.Add(new Response("Forge", I18n.Get("blacksmith.forge.option")));
        }

        responses.Add(new Response("Leave", Game1.content.LoadString("Strings\\Locations:Blacksmith_Clint_Leave")));
        location.createQuestionDialogue(string.Empty, responses.ToArray(), "Blacksmith");
    }

    private static bool HasUpgradeableToolInInventory(Farmer farmer)
    {
        for (var i = 0; i < farmer.Items.Count; i++)
        {
            var item = farmer.Items[i];
            if (item is not Tool { UpgradeLevel: < 4 } tool)
            {
                continue;
            }

            switch (tool)
            {
                case Axe or Pickaxe or Hoe or WateringCan:
                    return true;

                case Pan:
                    _isPanningUpgradesLoaded ??= ModHelper.ModRegistry.IsLoaded("drbirbdev.PanningUpgrades");
                    if (_isPanningUpgradesLoaded.Value)
                    {
                        return true;
                    }

                    continue;

                case MilkPail or Shears:
                    _isRanchingToolUpgradesLoaded ??= ModHelper.ModRegistry.IsLoaded("drbirbdev.RanchingToolUpgrades");
                    if (_isRanchingToolUpgradesLoaded.Value)
                    {
                        return true;
                    }

                    break;
            }
        }

        return (LoveOfCookingIntegration.Instance?.IsLoaded == true && Reflector
            .GetStaticMethodDelegate<Func<bool>>("LoveOfCooking.Objects.CookingTool".ToType(), "CanBeUpgraded")
            .Invoke()) || farmer.trashCanLevel < 4;
    }

    private static bool HasGeodeInInventory(Farmer farmer)
    {
        for (var i = 0; i < farmer.Items.Count; i++)
        {
            var item = farmer.Items[i];
            if (item is null)
            {
                continue;
            }

            if (item.ParentSheetIndex is ItemIDs.Geode or ItemIDs.FrozenGeode or ItemIDs.MagmaGeode or ItemIDs.OmniGeode
                    or ItemIDs.ArtifactTrove or ItemIDs.GoldenCoconut || item.HasContextTag("geode_item"))
            {
                return true;
            }
        }

        return false;
    }

    #endregion injected subroutines
}
