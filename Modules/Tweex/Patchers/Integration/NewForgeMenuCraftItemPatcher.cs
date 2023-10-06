namespace DaLion.Overhaul.Modules.Tweex.Patchers.Integration;

#region using directives

using DaLion.Shared.Attributes;
using DaLion.Shared.Constants;
using DaLion.Shared.Harmony;
using HarmonyLib;
using SpaceCore.Interface;
using StardewValley.Objects;

#endregion using directives

[UsedImplicitly]
[ModRequirement("spacechase0.SpaceCore")]
internal sealed class NewForgeMenuCraftItemPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="NewForgeMenuCraftItemPatcher"/> class.</summary>
    internal NewForgeMenuCraftItemPatcher()
    {
        this.Target = this.RequireMethod<NewForgeMenu>(nameof(NewForgeMenu.CraftItem));
    }

    #region harmony patches

    /// <summary>Allow forging Glowstone Ring.</summary>
    [HarmonyPrefix]
    private static bool NewForgeMenuCraftItemPrefix(ref Item? __result, Item? left_item, Item? right_item, bool forReal)
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
                right_item!.ParentSheetIndex == ObjectIds.GlowRing:
                __result = new Ring(ObjectIds.GlowstoneRing);
                break;
        }

        return false; // run original logic
    }

    #endregion harmony patches
}
