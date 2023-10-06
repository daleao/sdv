namespace DaLion.Overhaul.Modules.Tweex.Patchers;

#region using directives

using DaLion.Shared.Constants;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Menus;
using StardewValley.Objects;

#endregion using directives

[UsedImplicitly]
internal sealed class ForgeMenuCraftItemPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="ForgeMenuCraftItemPatcher"/> class.</summary>
    internal ForgeMenuCraftItemPatcher()
    {
        this.Target = this.RequireMethod<ForgeMenu>(nameof(ForgeMenu.CraftItem));
    }

    #region harmony patches

    /// <summary>Allow forging Glowstone Ring.</summary>
    [HarmonyPrefix]
    private static bool ForgeMenuCraftItemPrefix(ref Item? __result, Item? left_item, Item? right_item)
    {
        if (left_item is not Ring || right_item is not Ring || !TweexModule.Config.ImmersiveGlowstoneProgression)
        {
            return true; // run original logic
        }

        switch (left_item.ParentSheetIndex)
        {
            case ObjectIds.SmallGlowRing or ObjectIds.SmallMagnetRing
                when right_item.ParentSheetIndex == left_item.ParentSheetIndex:
            {
                var resultIndex = left_item.ParentSheetIndex switch
                {
                    ObjectIds.SmallGlowRing => ObjectIds.GlowRing,
                    ObjectIds.SmallMagnetRing => ObjectIds.MagnetRing,
                    _ => -1,
                };

                __result = new Ring(resultIndex);
                break;
            }

            case ObjectIds.GlowRing when
                right_item.ParentSheetIndex == ObjectIds.MagnetRing:
            case ObjectIds.MagnetRing when
                right_item.ParentSheetIndex == ObjectIds.GlowRing:
                __result = new Ring(ObjectIds.GlowstoneRing);
                break;
        }

        return false; // run original logic
    }

    #endregion harmony patches
}
