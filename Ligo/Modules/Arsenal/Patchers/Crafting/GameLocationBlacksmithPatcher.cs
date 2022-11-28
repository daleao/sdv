namespace DaLion.Ligo.Modules.Arsenal.Patchers;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Netcode;
using Shared.Extensions.Stardew;
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
                .FindFirst(
                    new CodeInstruction(OpCodes.Ldfld, typeof(Farmer).RequireField(nameof(Farmer.toolBeingUpgraded))),
                    new CodeInstruction(
                        OpCodes.Callvirt,
                        typeof(NetFieldBase<Tool, NetRef<Tool>>).RequirePropertyGetter("Value")),
                    new CodeInstruction(OpCodes.Brtrue))
                .Advance(2)
                .SetOpCode(OpCodes.Brtrue_S)
                .Advance()
                .RemoveInstructionsUntil(
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(GameLocation).RequireMethod(
                            nameof(GameLocation.createQuestionDialogue),
                            new[] { typeof(string), typeof(Response[]), typeof(string) })))
                .InsertInstructions(
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(GameLocationBlacksmithPatcher).RequireMethod(nameof(CreateBlacksmithQuestionDialogue))));
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

        if (HasUpgradeableToolsInInventory(Game1.player))
        {
            responses.Add(new Response("Upgrade",
                Game1.content.LoadString("Strings\\Locations:Blacksmith_Clint_Upgrade")));
        }

        if (HasGeodeInInventory(Game1.player))
        {
            responses.Add(new Response(
                "Process",
                Game1.content.LoadString("Strings\\Locations:Blacksmith_Clint_Geodes")));
        }

        if (HasUnlockedForgeBlueprints(Game1.player))
        {
            responses.Add(new Response("Forge", ModEntry.i18n.Get("blacksmith.forge.option")));
        }

        responses.Add(new Response("Leave", Game1.content.LoadString("Strings\\Locations:Blacksmith_Clint_Leave")));
        location.createQuestionDialogue("", responses.ToArray(), "Blacksmith");
    }

    private static bool HasUpgradeableToolsInInventory(Farmer farmer)
    {
        return farmer.getToolFromName("Axe") is Axe { UpgradeLevel: < 4 } ||
               farmer.getToolFromName("Pickaxe") is Pickaxe { UpgradeLevel: < 4 } ||
               farmer.getToolFromName("Hoe") is Hoe { UpgradeLevel: < 4 } ||
               farmer.getToolFromName("Watering Can") is WateringCan { UpgradeLevel: < 4 };
    }

    private static bool HasGeodeInInventory(Farmer farmer)
    {
        return farmer.hasItemInInventory(535, 1) || farmer.hasItemInInventory(536, 1) ||
               farmer.hasItemInInventory(537, 1) || farmer.hasItemInInventory(749, 1) ||
               farmer.hasItemInInventory(275, 1) || farmer.hasItemInInventory(791, 1);
    }

    private static bool HasUnlockedForgeBlueprints(Farmer farmer)
    {
        return true;
        return farmer.Read<bool>(DataFields.GotDwarfSwordBlueprint) || farmer.Read<bool>(DataFields.GotDragontoothCutlassBlueprint) ||
               farmer.Read<bool>(DataFields.GotDwarfHammerBlueprint) || farmer.Read<bool>(DataFields.GotDragontoothClubBlueprint) ||
               farmer.Read<bool>(DataFields.GotDwarfDaggerBlueprint) || farmer.Read<bool>(DataFields.GotDragontoothShivBlueprint);
    }

    #endregion injected subroutines
}
