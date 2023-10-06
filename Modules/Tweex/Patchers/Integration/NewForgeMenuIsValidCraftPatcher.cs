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
internal sealed class NewForgeMenuIsValidCraftPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="NewForgeMenuIsValidCraftPatcher"/> class.</summary>
    internal NewForgeMenuIsValidCraftPatcher()
    {
        this.Target = this.RequireMethod<NewForgeMenu>(nameof(NewForgeMenu.IsValidCraft));
    }

    #region harmony patches

    /// <summary>Allow forging Glowstone Ring.</summary>
    [HarmonyPrefix]
    private static bool NewForgeMenuIsValidCraftPrefix(ref bool __result, Item? left_item, Item? right_item)
    {
        if (left_item is not Ring || right_item is not Ring || !TweexModule.Config.ImmersiveGlowstoneProgression)
        {
            return true; // run original logic
        }

        switch (left_item.ParentSheetIndex)
        {
            case ObjectIds.SmallGlowRing or ObjectIds.SmallMagnetRing
                when right_item.ParentSheetIndex == left_item.ParentSheetIndex:
            case ObjectIds.GlowRing when
                right_item.ParentSheetIndex == ObjectIds.MagnetRing:
            case ObjectIds.MagnetRing when
                right_item.ParentSheetIndex == ObjectIds.GlowRing:
                __result = true;
                break;
        }

        return false; // don't run original logic
    }

    #endregion harmony patches
}
