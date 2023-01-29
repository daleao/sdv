namespace DaLion.Overhaul.Modules.Professions.Patchers.Combat;

#region using directives

using System.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class SlingshotGetHoverBoxTextPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="SlingshotGetHoverBoxTextPatcher"/> class.</summary>
    internal SlingshotGetHoverBoxTextPatcher()
    {
        this.Target = this.RequireMethod<Slingshot>(nameof(Slingshot.getHoverBoxText));
    }

    #region harmony patches

    /// <summary>Adjust tooltip for equipping secondary ammo.</summary>
    [HarmonyPrefix]
    private static bool SlingshotGetHoverBoxTextPrefix(Slingshot __instance, ref string? __result, Item? hoveredItem)
    {
        try
        {
            switch (hoveredItem)
            {
                case SObject obj when __instance.canThisBeAttached(obj):
                    __result = Game1.content.LoadString(
                        "Strings\\StringsFromCSFiles:Slingshot.cs.14256",
                        __instance.DisplayName,
                        hoveredItem.DisplayName);
                    break;
                case null when __instance.attachments.Count > 0:
                    if (__instance.attachments[0] is not null)
                    {
                        __result = Game1.content.LoadString(
                            "Strings\\StringsFromCSFiles:Slingshot.cs.14258",
                            __instance.attachments[0].DisplayName);
                    }
                    else if (__instance.attachments.Length > 1 && __instance.attachments[1] is not null)
                    {
                        __result = Game1.content.LoadString(
                            "Strings\\StringsFromCSFiles:Slingshot.cs.14258",
                            __instance.attachments[1].DisplayName);
                    }

                    break;
                default:
                    __result = null;
                    break;
            }

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
