namespace DaLion.Overhaul.Modules.Combat.Patchers.Quests.Dwarven;

#region using directives

using DaLion.Shared.Constants;
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
        if (!CombatModule.Config.DwarvenLegacy || __instance.name != "Island Obelisk" ||
            !__instance.itemsRequired.Remove(ObjectIds.DragonTooth))
        {
            return;
        }

        __instance.itemsRequired[ObjectIds.Pineapple] = 10;
        __instance.itemsRequired[ObjectIds.Mango] = 10;
        __instance.itemsRequired[ObjectIds.RadioactiveBar] = 5;
        __instance.itemsRequired.Remove(ObjectIds.IridiumBar);
    }

    #endregion harmony patches
}
