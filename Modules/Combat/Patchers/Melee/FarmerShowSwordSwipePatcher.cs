namespace DaLion.Overhaul.Modules.Combat.Patchers.Melee;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Overhaul.Modules.Combat.Extensions;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class FarmerShowSwordSwipePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="FarmerShowSwordSwipePatcher"/> class.</summary>
    internal FarmerShowSwordSwipePatcher()
    {
        this.Target = this.RequireMethod<Farmer>(nameof(Farmer.showSwordSwipe));
    }

    #region harmony patches

    /// <summary>Show combo swipe.</summary>
    [HarmonyPrefix]
    private static bool FarmerShowSwordSwipePrefix(Farmer who)
    {
        if (who.CurrentTool is not MeleeWeapon weapon || weapon.isScythe() ||
            !CombatModule.Config.EnableComboHits)
        {
            return true; // run original logic
        }

        try
        {
            const int minSwipeInterval = 20;
            var actionTile = who.GetToolLocation(ignoreClick: true);
            var sprite = who.FarmerSprite;
            var index = sprite.currentAnimationIndex;

            TemporaryAnimatedSprite? tempSprite = null;
            weapon.DoDamage(who.currentLocation, (int)actionTile.X, (int)actionTile.Y, who.FacingDirection, 1, who);
            var isBackwardsSwipe = (int)CombatModule.State.ComboHitStep % 2 == 0;
            switch (who.FacingDirection)
            {
                case Game1.up:
                    switch (index)
                    {
                        case 1 or 7 or 13 or 19:
                            who.yVelocity = 0.5f;
                            break;
                        case 5 or 11 or 17 or 23:
                            who.yVelocity = -0.3f;

                            var flipped = sprite.currentAnimationIndex is 11 or 23 ||
                                          (sprite.currentAnimationIndex == 5 && isBackwardsSwipe);
                            var rotation = flipped
                                ? (float)Math.PI * -5f / 4f
                                : (float)Math.PI * 5f / 4f;
                            tempSprite = new TemporaryAnimatedSprite(
                                "LooseSprites\\Cursors",
                                new Rectangle(518, 274, 23, 31),
                                who.Position + (new Vector2(0f, -32f) * 4f),
                                flipped: flipped,
                                0.07f,
                                Color.White)
                            {
                                scale = 4f,
                                animationLength = 1,
                                interval = Math.Max(
                                    who.FarmerSprite.CurrentAnimationFrame.milliseconds,
                                    minSwipeInterval),
                                alpha = 0.5f,
                                rotation = rotation,
                            };
                            break;
                    }

                    break;

                case Game1.right:
                    switch (index)
                    {
                        case 1 or 7 or 13 or 19:
                            who.xVelocity = 0.5f;
                            break;
                        case 5 or 11 or 17 or 23:
                            who.xVelocity = -0.3f;

                            var flipped = sprite.currentAnimationIndex is 11 or 23 ||
                                          (sprite.currentAnimationIndex == 5 && isBackwardsSwipe);
                            var rotation = flipped
                                ? (float)Math.PI
                                : 0f;
                            var offset = flipped
                                ? new Vector2(12f, -32f) * 4f
                                : new Vector2(4f, -12f) * 4f;
                            tempSprite = new TemporaryAnimatedSprite(
                                "LooseSprites\\Cursors",
                                new Rectangle(518, 274, 23, 31),
                                who.Position + offset,
                                flipped: flipped,
                                0.07f,
                                Color.White)
                            {
                                scale = 4f,
                                animationLength = 1,
                                interval = Math.Max(
                                    who.FarmerSprite.CurrentAnimationFrame.milliseconds,
                                    minSwipeInterval),
                                alpha = 0.5f,
                                rotation = rotation,
                            };
                            break;
                    }

                    break;

                case Game1.down:
                    switch (index)
                    {
                        case 1 or 7 or 13 or 19:
                            who.yVelocity = -0.5f;
                            break;
                        case 5 or 11 or 17 or 23:
                            who.yVelocity = 0.3f;
                            tempSprite =
                                new TemporaryAnimatedSprite(
                                    "LooseSprites\\Cursors",
                                    new Rectangle(503, 256, 42, 17),
                                    who.Position + (new Vector2(-16f, -2f) * 4f),
                                    flipped: sprite.currentAnimationIndex is 11 or 23 || (sprite.currentAnimationIndex == 5 && isBackwardsSwipe),
                                    0.07f,
                                    Color.White)
                                {
                                    scale = 4f,
                                    animationLength = 1,
                                    interval = Math.Max(
                                        who.FarmerSprite.CurrentAnimationFrame.milliseconds,
                                        minSwipeInterval),
                                    alpha = 0.5f,
                                    layerDepth = (who.Position.Y + 64f) / 10000f,
                                };
                            break;
                    }

                    break;

                case Game1.left:
                    switch (index)
                    {
                        case 1 or 7 or 13 or 19:
                            who.xVelocity = -0.5f;
                            break;
                        case 5 or 11 or 17 or 23:
                            who.xVelocity = 0.3f;

                            var flipped = sprite.currentAnimationIndex is 11 or 23 ||
                                          (sprite.currentAnimationIndex == 5 && isBackwardsSwipe);
                            var rotation = flipped
                                ? (float)Math.PI
                                : 0f;
                            var offset = flipped
                                ? new Vector2(-18f, -28f) * 4f
                                : new Vector2(-15f, -12f) * 4f;
                            tempSprite = new TemporaryAnimatedSprite(
                                "LooseSprites\\Cursors",
                                new Rectangle(518, 274, 23, 31),
                                who.Position + offset,
                                flipped: !flipped,
                                0.07f,
                                Color.White)
                            {
                                scale = 4f,
                                animationLength = 1,
                                interval = Math.Max(
                                    who.FarmerSprite.CurrentAnimationFrame.milliseconds,
                                    minSwipeInterval),
                                alpha = 0.5f,
                                rotation = rotation,
                            };
                            break;
                    }

                    break;
            }

            if (tempSprite is null)
            {
                return false; // don't run original logic
            }

            if (CombatModule.Config.EnableWeaponOverhaul)
            {
                tempSprite.color = weapon.InitialParentTileIndex switch
                {
                    ItemIDs.LavaKatana => Color.Orange,
                    ItemIDs.YetiTooth => Color.PowderBlue,
                    _ => tempSprite.color,
                };
            }

            if (CombatModule.Config.EnableHeroQuest)
            {
                tempSprite.color = weapon.IsInfinityWeapon()
                    ? Color.HotPink
                    : weapon.InitialParentTileIndex switch
                    {
                        ItemIDs.DarkSword => Color.DarkSlateGray,
                        ItemIDs.HolyBlade => Color.Gold,
                        _ => tempSprite.color,
                    };
            }

            who.currentLocation.temporarySprites.Add(tempSprite);

            return false; // don't run original logic
        }
        catch (Exception ex)
        {
            Log.E($"Failed in {MethodBase.GetCurrentMethod()?.Name}:\n{ex}");
            return true; // default to original logic
        }
    }

    /// <summary>Show novelty colors if combo hits is disabled.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? FarmerShowSwordSwipeTranspiler(
        IEnumerable<CodeInstruction> instructions, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        try
        {
            helper
                .Match(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Ldloc_0),
                        new CodeInstruction(OpCodes.Brfalse_S),
                    },
                    ILHelper.SearchOption.Last)
                .Match(new[] { new CodeInstruction(OpCodes.Isinst, typeof(MeleeWeapon)) })
                .Match(new[] { new CodeInstruction(OpCodes.Ldarg_0) })
                .Insert(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Ldloc_0),
                        new CodeInstruction(OpCodes.Ldloc_0),
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(OpCodes.Callvirt, typeof(Farmer).RequirePropertyGetter(nameof(Farmer.CurrentTool))),
                        new CodeInstruction(OpCodes.Isinst, typeof(MeleeWeapon)),
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(FarmerShowSwordSwipePatcher).RequireMethod(nameof(GetSwipeColor))),
                    })
                .Remove(7);
        }
        catch (Exception ex)
        {
            Log.E($"Failed injecting novelty color VFX.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region injected subroutines

    private static Color GetSwipeColor(TemporaryAnimatedSprite tempSprite, MeleeWeapon weapon)
    {
        if (CombatModule.Config.EnableWeaponOverhaul)
        {
            return weapon.InitialParentTileIndex switch
            {
                ItemIDs.LavaKatana => Color.Orange,
                ItemIDs.YetiTooth => Color.PowderBlue,
                _ => tempSprite.color,
            };
        }

        if (CombatModule.Config.EnableHeroQuest)
        {
            return weapon.InitialParentTileIndex switch
                {
                    ItemIDs.DarkSword => Color.DarkSlateGray,
                    ItemIDs.HolyBlade => Color.Gold,
                    _ => tempSprite.color,
                };
        }

        return weapon.IsInfinityWeapon() ? Color.HotPink : tempSprite.color;
    }

    #endregion injected subroutines
}
