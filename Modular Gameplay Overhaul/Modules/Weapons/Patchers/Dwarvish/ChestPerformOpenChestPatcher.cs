namespace DaLion.Overhaul.Modules.Weapons.Patchers.Dwarvish;

#region using directives

using System.Diagnostics.CodeAnalysis;
using System.Linq;
using DaLion.Overhaul.Modules.Weapons.Extensions;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Collections;
using DaLion.Shared.Extensions.SMAPI;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using DaLion.Shared.Networking;
using HarmonyLib;
using StardewValley;
using StardewValley.Objects;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class ChestPerformOpenChestPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="ChestPerformOpenChestPatcher"/> class.</summary>
    internal ChestPerformOpenChestPatcher()
    {
        this.Target = this.RequireMethod<Chest>(nameof(Chest.performOpenChest));
    }

    #region harmony patches

    /// <summary>Inject blueprint chest rewards.</summary>
    [HarmonyPostfix]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:Element should begin with upper-case letter", Justification = "Preference for internal functions.")]
    private static void ChestPerformOpenChestPostfix(Chest __instance)
    {
        if (!WeaponsModule.Config.DwarvishLegacy || !Globals.DwarvishBlueprintIndex.HasValue ||
            !Globals.DwarvenScrapIndex.HasValue)
        {
            return;
        }

        if (__instance.items.FirstOrDefault(i => i is MeleeWeapon w && w.IsLegacyWeapon()) is not MeleeWeapon weapon)
        {
            return;
        }

        __instance.items.Remove(weapon);

        var player = Game1.player;
        var found = player.Read(DataKeys.BlueprintsFound).ParseList<int>();
        var volcanoBlueprints = new[]
        {
            ItemIDs.DwarfSword, ItemIDs.DwarfDagger, ItemIDs.DwarfHammer,
            ItemIDs.DragontoothCutlass, ItemIDs.DragontoothShiv, ItemIDs.DragontoothClub,
        };

        if (found.ContainsAll(volcanoBlueprints) || !player.canUnderstandDwarves)
        {
            var material = weapon.Name.StartsWith("Dwarven")
                ? Globals.DwarvenScrapIndex.Value
                : ItemIDs.DragonTooth;
            __instance.items.Add(new SObject(material, 1));
            return;
        }

        var blueprint = weapon.InitialParentTileIndex;
        if (found.Contains(blueprint))
        {
            if (weapon.Name.StartsWith("Dwarven"))
            {
                if (!found.Contains(ItemIDs.DwarfSword))
                {
                    blueprint = ItemIDs.DwarfSword;
                }
                else if (!found.Contains(ItemIDs.DwarfHammer))
                {
                    blueprint = ItemIDs.DwarfHammer;
                }
                else if (!found.Contains(ItemIDs.DwarfDagger))
                {
                    blueprint = ItemIDs.DwarfDagger;
                }
                else
                {
                    __instance.items.Add(new SObject(Globals.DwarvenScrapIndex!.Value, 1));
                    return;
                }
            }
            else
            {
                if (!found.Contains(ItemIDs.DragontoothCutlass))
                {
                    blueprint = ItemIDs.DragontoothCutlass;
                }
                else if (!found.Contains(ItemIDs.DragontoothClub))
                {
                    blueprint = ItemIDs.DragontoothClub;
                }
                else if (!found.Contains(ItemIDs.DragontoothShiv))
                {
                    blueprint = ItemIDs.DragontoothShiv;
                }
                else
                {
                    __instance.items.Add(new SObject(ItemIDs.DragonTooth, 1));
                    return;
                }
            }
        }

        player.Append(DataKeys.BlueprintsFound, blueprint.ToString());
        var count = player.Read(DataKeys.BlueprintsFound).ParseList<int>().Count;
        switch (count)
        {
            case 8:
                player.completeQuest((int)Quest.ForgeNext);
                break;
            case 1:
                ModHelper.GameContent.InvalidateCacheAndLocalized("Data/Events/Blacksmith");
                break;
        }

        player.holdUpItemThenMessage(new SObject(Globals.DwarvishBlueprintIndex.Value, 1));
        if (Context.IsMultiplayer)
        {
            Broadcaster.SendPublicChat(I18n.Get("blueprint.found.global", new { who = player.Name }));
        }
    }

    #endregion harmony patches
}
