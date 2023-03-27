namespace DaLion.Overhaul.Modules.Weapons.Patchers.Dwarvish;

#region using directives

using DaLion.Shared.Harmony;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
internal sealed class BluePrintCtorPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="BluePrintCtorPatcher"/> class.</summary>
    internal BluePrintCtorPatcher()
    {
        this.Target = this.RequireConstructor<BluePrint>(typeof(string));
    }

    #region harmony patches

    /// <summary>Remove Dragon Tooth from Obelisk blueprint.</summary>
    [HarmonyPostfix]
    private static void BluePrintCtorPostfix(BluePrint __instance)
    {
        if (!WeaponsModule.Config.DwarvishLegacy || __instance.name != "Island Obelisk" ||
            !__instance.itemsRequired.Remove(ItemIDs.DragonTooth))
        {
            return;
        }

        __instance.itemsRequired[ItemIDs.Pineapple] = 10;
        __instance.itemsRequired[ItemIDs.Mango] = 10;
        __instance.itemsRequired[ItemIDs.RadioactiveBar] = 5;
        __instance.itemsRequired.Remove(SObject.iridiumBar);
    }

    #endregion harmony patches
}
