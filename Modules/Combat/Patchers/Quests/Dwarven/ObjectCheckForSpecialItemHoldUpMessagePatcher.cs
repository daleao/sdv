namespace DaLion.Overhaul.Modules.Combat.Patchers.Quests.Dwarven;

#region using directives

using DaLion.Overhaul.Modules.Combat.Enums;
using DaLion.Overhaul.Modules.Combat.Integrations;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class ObjectCheckForSpecialItemHoldUpMessagePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="ObjectCheckForSpecialItemHoldUpMessagePatcher"/> class.</summary>
    internal ObjectCheckForSpecialItemHoldUpMessagePatcher()
    {
        this.Target = this.RequireMethod<SObject>(nameof(SObject.checkForSpecialItemHoldUpMeessage));
    }

    #region harmony patches

    /// <summary>Add Dwarvish Blueprint obtain message.</summary>
    [HarmonyPostfix]
    private static void ObjectCheckForSpecialItemHoldUpPrefix(SObject? __instance, ref string? __result)
    {
        if (__instance is null || !JsonAssetsIntegration.DwarvishBlueprintIndex.HasValue ||
            __instance.ParentSheetIndex != JsonAssetsIntegration.DwarvishBlueprintIndex.Value)
        {
            return;
        }

        var found = Game1.player.Read(DataKeys.BlueprintsFound).ParseList<int>();
        if (found.Count == 1)
        {
            var type = ((WeaponType)new MeleeWeapon(found[0]).type.Value).ToStringFast();
            if (type.Contains("Sword"))
            {
                type = type.SplitCamelCase()[1];
            }

            __result = I18n.Blueprint_Found_First(type);
        }
        else
        {
            __result = I18n.Blueprint_Found_Local();
        }
    }

    #endregion harmony patches
}
