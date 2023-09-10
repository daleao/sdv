namespace DaLion.Overhaul.Modules.Combat.Patchers.Ranged;

#region using directives

using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class FarmerShowToolSwipeEffectPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="FarmerShowToolSwipeEffectPatcher"/> class.</summary>
    internal FarmerShowToolSwipeEffectPatcher()
    {
        this.Target = this.RequireMethod<Farmer>(nameof(Farmer.showToolSwipeEffect));
    }

    #region harmony patches

    /// <summary>Adjust swipe effect during slingshot special.</summary>
    [HarmonyPrefix]
    private static bool FarmerShowToolSwipeEffectPrefix(Farmer who)
    {
        if (who.CurrentTool is not Slingshot slingshot)
        {
            return true; // run original logic
        }

        TemporaryAnimatedSprite? tempSprite = null;
        switch (who.FacingDirection)
        {
            case Game1.up:
                tempSprite = new TemporaryAnimatedSprite(
                    18,
                    who.Position + new Vector2(0f, -132f),
                    Color.White,
                    4,
                    flipped: false,
                    who.stamina <= 0f ? 100f : 50f,
                    0,
                    64,
                    1f,
                    64) { layerDepth = (who.getStandingY() - 9) / 10000f, };
                break;
            case Game1.right:
                tempSprite = new TemporaryAnimatedSprite(
                    15,
                    who.Position + new Vector2(12f, -132f),
                    Color.White,
                    4,
                    flipped: false,
                    who.stamina <= 0f ? 80f : 40f,
                    0,
                    128,
                    1f,
                    128) { layerDepth = (who.GetBoundingBox().Bottom + 1) / 10000f, };
                break;
            case Game1.down:
                tempSprite = new TemporaryAnimatedSprite(
                    19,
                    who.Position + new Vector2(-4f, -128f),
                    Color.White,
                    4,
                    flipped: false,
                    (who.stamina <= 0f) ? 80f : 40f,
                    0,
                    128,
                    1f,
                    128) { layerDepth = (who.GetBoundingBox().Bottom + 1) / 10000f, };
                break;

            case Game1.left:
                tempSprite = new TemporaryAnimatedSprite(
                    15,
                    who.Position + new Vector2(-78f, -132f),
                    Color.White,
                    4,
                    flipped: true,
                    who.stamina <= 0f ? 80f : 40f,
                    0,
                    128,
                    1f,
                    128) { layerDepth = (who.GetBoundingBox().Bottom + 1) / 10000f, };
                break;
        }

        if (tempSprite is null)
        {
            return false; // don't run original logic
        }

        if (slingshot.InitialParentTileIndex == ItemIDs.InfinitySlingshot && CombatModule.Config.EnableHeroQuest)
        {
            tempSprite.color = Color.HotPink;
        }

        who.currentLocation.temporarySprites.Add(tempSprite);

        return false; // don't run original logic
    }

    #endregion harmony patches
}
