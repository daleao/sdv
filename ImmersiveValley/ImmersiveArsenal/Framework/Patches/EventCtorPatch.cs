namespace DaLion.Stardew.Arsenal.Framework.Patches;

#region using directives

using HarmonyLib;
using LinqFasterer;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class EventCtorPatch : Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal EventCtorPatch()
    {
        Target = RequireConstructor<Event>(typeof(string), typeof(int), typeof(Farmer));
    }

    #region harmony patches

    /// <summary>Immersively adjust Marlon's intro event.</summary>
    [HarmonyPrefix]
    private static void EventCtorPrefix(ref string eventString, int eventID)
    {
        if (!ModEntry.Config.WoodyReplacesRusty || eventID != 100162) return;

        if (ModEntry.ModHelper.ModRegistry.IsLoaded("FlashShifter.StardewValleyExpandedCP"))
        {
            eventString = ModEntry.i18n.Get(
                Game1.player.Items.AnyF(item => item is MeleeWeapon weapon && !weapon.isScythe())
                    ? "events.100162.nosword.sve"
                    : "events.100162.sword.sve");
        }
        else
        {
            eventString = ModEntry.i18n.Get(
                Game1.player.Items.AnyF(item => item is MeleeWeapon weapon && !weapon.isScythe())
                    ? "events.100162.nosword"
                    : "events.100162.sword");
        }
    }

    #endregion harmony patches
}