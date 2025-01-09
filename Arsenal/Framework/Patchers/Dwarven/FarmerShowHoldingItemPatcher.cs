﻿namespace DaLion.Arsenal.Framework.Patchers.Dwarven;

#region using directives

using System.Reflection;
using DaLion.Overhaul.Modules.Combat.Integrations;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;

#endregion using directives

[UsedImplicitly]
internal sealed class FarmerShowHoldingItemPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="FarmerShowHoldingItemPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal FarmerShowHoldingItemPatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireMethod<Farmer>(nameof(Farmer.showHoldingItem));
    }

    #region harmony patches

    /// <summary>Show Dwarvish Blueprint on obtain.</summary>
    [HarmonyPrefix]
    private static bool FarmerShowHoldingItemPrefix(Farmer who)
    {
        try
        {
            if (!JsonAssetsIntegration.DwarvishBlueprintIndex.HasValue ||
                who.mostRecentlyGrabbedItem?.ParentSheetIndex != JsonAssetsIntegration.DwarvishBlueprintIndex.Value)
            {
                return true; // run original logic
            }

            Game1.currentLocation.temporarySprites.Add(
                new TemporaryAnimatedSprite(
                    $"{Manifest.UniqueID}_DwarvishBlueprint",
                    new Rectangle(0, 0, 25, 18),
                    2500f,
                    1,
                    0,
                    who.Position + new Vector2(-20f, -152f),
                    flicker: false,
                    flipped: false)
                { motion = new Vector2(0f, -0.1f), scale = 4f, layerDepth = 1f, });
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
