namespace DaLion.Arsenal.Framework.Patchers.Dwarven;

#region using directives

using System.Diagnostics.CodeAnalysis;
using System.Linq;
using DaLion.Arsenal.Framework.Extensions;
using DaLion.Arsenal.Framework.Integrations;
using DaLion.Shared.Constants;
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
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal ChestPerformOpenChestPatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireMethod<Chest>(nameof(Chest.performOpenChest));
    }

    #region harmony patches

    /// <summary>Inject blueprint chest rewards.</summary>
    [HarmonyPostfix]
    private static void ChestPerformOpenChestPostfix(Chest __instance)
    {
        if (Config.DwarvenLegacy)
        {
            return;
        }

        if (__instance.Items.FirstOrDefault(i => i is MeleeWeapon w && w.IsLegacyWeapon()) is not MeleeWeapon weapon)
        {
            return;
        }

        __instance.Items.Remove(weapon);

        var player = Game1.player;
        var found = Data.Read(player, DataKeys.BlueprintsFound).ParseList<int>();
        string[] allBlueprints =
        [
            QualifiedWeaponIds.DwarfSword, QualifiedWeaponIds.DwarfDagger, QualifiedWeaponIds.DwarfHammer,
            QualifiedWeaponIds.DragontoothCutlass, QualifiedWeaponIds.DragontoothShiv, QualifiedWeaponIds.DragontoothClub,
        ];

        if (found.ContainsAll(allBlueprints) || !player.canUnderstandDwarves)
        {
            var material = weapon.Name.StartsWith("Dwarven")
                ? DwarvenMetalId
                : QualifiedObjectIds.DragonTooth;
            __instance.Items.Add(ItemRegistry.Create<SObject>(material));
            return;
        }

        var blueprint = weapon.InitialParentTileIndex;
        if (found.Contains(blueprint))
        {
            if (weapon.Name.StartsWith("Dwarven"))
            {
                if (!found.Contains(WeaponIds.DwarfSword))
                {
                    blueprint = WeaponIds.DwarfSword;
                }
                else if (!found.Contains(WeaponIds.DwarfHammer))
                {
                    blueprint = WeaponIds.DwarfHammer;
                }
                else if (!found.Contains(WeaponIds.DwarfDagger))
                {
                    blueprint = WeaponIds.DwarfDagger;
                }
                else
                {
                    __instance.items.Add(new SObject(JsonAssetsIntegration.DwarvenScrapIndex.Value, 1));
                    return;
                }
            }
            else
            {
                if (!found.Contains(WeaponIds.DragontoothCutlass))
                {
                    blueprint = WeaponIds.DragontoothCutlass;
                }
                else if (!found.Contains(WeaponIds.DragontoothClub))
                {
                    blueprint = WeaponIds.DragontoothClub;
                }
                else if (!found.Contains(WeaponIds.DragontoothShiv))
                {
                    blueprint = WeaponIds.DragontoothShiv;
                }
                else
                {
                    __instance.items.Add(new SObject(ObjectIds.DragonTooth, 1));
                    return;
                }
            }
        }

        player.Append(DataKeys.BlueprintsFound, blueprint.ToString());
        var count = player.Read(DataKeys.BlueprintsFound).ParseList<int>().Count;
        if (count == 1)
        {
            ModHelper.GameContent.InvalidateCacheAndLocalized("Data/Events/Blacksmith");
        }

        player.holdUpItemThenMessage(new SObject(JsonAssetsIntegration.DwarvishBlueprintIndex.Value, 1));
        if (Context.IsMultiplayer && Game1.player.mailReceived.Contains("clintForge"))
        {
            Broadcaster.SendPublicChat(I18n.Blueprint_Found_Global(player.Name));
        }
    }

    #endregion harmony patches
}
