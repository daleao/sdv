namespace DaLion.Ligo.Modules.Arsenal.Patchers.Crafting;

#region using directives

using System.Linq;
using DaLion.Ligo.Modules.Arsenal.Extensions;
using DaLion.Shared.Extensions;
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
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:Element should begin with upper-case letter", Justification = "Preference for internal functions.")]
    private static void ChestPerformOpenChestPostfix(Chest __instance)
    {
        if (!ModEntry.Config.Arsenal.DwarvishCrafting || !Globals.DwarvishBlueprintIndex.HasValue)
        {
            return;
        }

        if (__instance.items.FirstOrDefault(i => i is MeleeWeapon w && w.CanBeCrafted()) is not MeleeWeapon weapon)
        {
            return;
        }

        var player = Game1.player;
        var found = player.Read(DataFields.BlueprintsFound).ParseList<int>();
        if (found.Count == 6 || !player.canUnderstandDwarves)
        {
            var material = weapon.Name.StartsWith("Dwarven")
                ? Globals.DwarvenScrapIndex!.Value
                : Constants.DragonToothIndex;
            getEquivalentMaterial(__instance, material);
            return;
        }

        var blueprint = weapon.ParentSheetIndex;
        if (found.Contains(blueprint))
        {
            if (weapon.Name.StartsWith("Dwarven"))
            {
                if (!found.Contains(Constants.DwarfSwordIndex))
                {
                    blueprint = Constants.DwarfSwordIndex;
                }
                else if (!found.Contains(Constants.DwarfHammerIndex))
                {
                    blueprint = Constants.DwarfHammerIndex;
                }
                else if (!found.Contains(Constants.DwarfDaggerIndex))
                {
                    blueprint = Constants.DwarfDaggerIndex;
                }
                else
                {
                    getEquivalentMaterial(__instance, Globals.DwarvenScrapIndex!.Value);
                    return;
                }
            }
            else
            {
                if (!found.Contains(Constants.DragontoothCutlassIndex))
                {
                    blueprint = Constants.DragontoothCutlassIndex;
                }
                else if (!found.Contains(Constants.DragontoothClubIndex))
                {
                    blueprint = Constants.DragontoothClubIndex;
                }
                else if (!found.Contains(Constants.DragontoothShivIndex))
                {
                    blueprint = Constants.DragontoothShivIndex;
                }
                else
                {
                    getEquivalentMaterial(__instance, Constants.DragonToothIndex);
                    return;
                }
            }
        }

        player.Append(DataFields.BlueprintsFound, blueprint.ToString());
        if (player.Read(DataFields.BlueprintsFound).ParseList<int>().Count == 6)
        {
            player.completeQuest(Constants.ForgeNextQuestId);
        }

        if (!player.hasOrWillReceiveMail("dwarvishBlueprintFound"))
        {
            Game1.player.mailReceived.Add("dwarvishBlueprintFound");
        }

        player.holdUpItemThenMessage(new SObject(Globals.DwarvishBlueprintIndex.Value, 1));
        if (Context.IsMultiplayer)
        {
            Broadcaster.SendPublicChat(ModEntry.i18n.Get("blueprint.found.global", new { who = player.Name }));
        }

        void getEquivalentMaterial(Chest chest, int material)
        {
            chest.items.Remove(weapon);
            chest.items.Add(new SObject(material, 1));
        }
    }

    #endregion harmony patches
}
