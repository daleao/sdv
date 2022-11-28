namespace DaLion.Ligo.Modules.Arsenal.Patchers;

#region using directives

using System.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley;
using StardewValley.Menus;

#endregion using directives

[UsedImplicitly]
internal sealed class ShopMenuSetUpShopOwnerPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="ShopMenuSetUpShopOwnerPatcher"/> class.</summary>
    internal ShopMenuSetUpShopOwnerPatcher()
    {
        this.Target = this.RequireMethod<ShopMenu>(nameof(ShopMenu.setUpShopOwner));
    }

    #region harmony patches

    /// <summary>Set up Clint's forge shop.</summary>
    [HarmonyPrefix]
    private static bool ShopMenuSetUpShopOwnerPrefix(ShopMenu __instance, string? who)
    {
        try
        {
            if (who != "ClintForge")
            {
                return true; // run original logic
            }

            __instance.portraitPerson = Game1.getCharacterFromName("Clint");
            __instance.potraitPersonDialogue =
                Game1.parseText(
                    ModEntry.i18n.Get("blacksmith.forge.explanation"),
                    Game1.dialogueFont,
                    304);
            return false; // don't run original logic
        }
        catch (Exception ex)
        {
            Log.E($"Failed in {MethodBase.GetCurrentMethod()?.Name}:\n{ex}");
            return true; // default to original logic
        }
    }

    #endregion harmony patches
}
