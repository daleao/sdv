namespace DaLion.Stardew.Arsenal.Framework.Patches;

#region using directives

using System.Linq;
using HarmonyLib;
using JetBrains.Annotations;
using StardewValley;
using StardewValley.Tools;

using Common.Harmony;

#endregion using directives

[UsedImplicitly]
internal sealed class EventCtorPatch : BasePatch
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
                Game1.player.Items.Any(item => item is MeleeWeapon weapon && !weapon.isScythe())
                    ? "events.100162.nosword.sve"
                    : "events.100162.sword.sve");
        }
        else
        {
            eventString = ModEntry.i18n.Get(
                Game1.player.Items.Any(item => item is MeleeWeapon weapon && !weapon.isScythe())
                    ? "events.100162.nosword"
                    : "events.100162.sword");
        }
    }

    #endregion harmony patches
}