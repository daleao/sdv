namespace DaLion.Overhaul.Modules.Arsenal.Patchers.Crafting;

#region using directives

using System.Collections.Generic;
using System.Linq;
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

        if (ArsenalModule.Config.DwarvishCrafting && Game1.player.mailReceived.Contains("clintForge"))
        {
            responses.Add(new Response("Forge", I18n.Get("blacksmith.forge.option")));
        }

        responses.Add(new Response("Leave", Game1.content.LoadString("Strings\\Locations:Blacksmith_Clint_Leave")));
        location.createQuestionDialogue(string.Empty, responses.ToArray(), "Blacksmith");
    }

    private static bool HasUpgradeableToolInInventory(Farmer farmer)
    {
        return farmer.getToolFromName("Axe") is Axe { UpgradeLevel: < 4 } ||
               farmer.getToolFromName("Pickaxe") is Pickaxe { UpgradeLevel: < 4 } ||
               farmer.getToolFromName("Hoe") is Hoe { UpgradeLevel: < 4 } ||
               farmer.getToolFromName("Watering Can") is WateringCan { UpgradeLevel: < 4 } ||
               farmer.trashCanLevel < 4 ||
               (LoveOfCookingIntegration.Instance?.IsLoaded == true && Reflector
                   .GetStaticMethodDelegate<Func<bool>>("LoveOfCooking.Objects.CookingTool".ToType(), "CanBeUpgraded")
                   .Invoke()) ||
               (ModHelper.ModRegistry.IsLoaded("drbirbdev.PanningUpgrades") &&
                farmer.getToolFromName("UpgradeablePan") is Pan { UpgradeLevel: < 4 }) ||
               (ModHelper.ModRegistry.IsLoaded("drbirbdev.RanchingToolUpgrades") &&
                (farmer.getToolFromName("UpgradeablePail") is MilkPail { UpgradeLevel: < 4 } ||
                 farmer.getToolFromName("UpgradeableShears") is Shears { UpgradeLevel: < 4 }));
    }

    private static bool HasGeodeInInventory(Farmer farmer)
    {
        return farmer.hasItemInInventory(535, 1) || farmer.hasItemInInventory(536, 1) ||
               farmer.hasItemInInventory(537, 1) || farmer.hasItemInInventory(749, 1) ||
               farmer.hasItemInInventory(275, 1) || farmer.hasItemInInventory(791, 1) ||
               farmer.Items.Any(i => i?.HasContextTag("geode_item") == true);
    }

    #endregion injected subroutines
}
