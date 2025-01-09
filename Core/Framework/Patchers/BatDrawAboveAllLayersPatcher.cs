namespace DaLion.Core.Framework.Patchers;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.Monsters;

#endregion using directives

[UsedImplicitly]
internal sealed class BatDrawAboveAllLayersPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="BatDrawAboveAllLayersPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal BatDrawAboveAllLayersPatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireMethod<Bat>(nameof(Bat.drawAboveAllLayers));
        this.Prefix!.priority = Priority.First;
    }

    #region harmony patches

    /// <summary>Slow and damage-over-time effects for cursed dolls.</summary>
    [HarmonyPrefix]
    [HarmonyPriority(Priority.First)]
    [UsedImplicitly]
    private static bool BatDrawAboveAllLayersPrefix(Bat __instance, List<Vector2> ___previousPositions, NetBool ___seenPlayer, SpriteBatch b)
    {
        if (!__instance.cursedDoll.Value || !__instance.IsFrozen())
        {
            return true; // run original logic
        }

        if (!Utility.isOnScreen(__instance.Position, 128))
        {
            return false; // don't run original logic
        }

        try
        {
            if (__instance.hauntedSkull.Value)
            {
                var offset = Vector2.Zero;
                if (___previousPositions.Count > 2)
                {
                    offset = __instance.Position - ___previousPositions[1];
                }

                var direction = Math.Abs(offset.X) <= Math.Abs(offset.Y)
                    ? offset.Y >= 0f
                        ? 2
                        : 0
                    : offset.X > 0f
                        ? 1
                        : 3;

                b.Draw(
                    __instance.Sprite.Texture,
                    __instance.getLocalPosition(Game1.viewport) + new Vector2(32f, 32f),
                    Game1.getSourceRectForStandardTileSheet(
                        __instance.Sprite.Texture,
                        direction * 2,
                        16,
                        16),
                    Color.White,
                    0f,
                    new Vector2(8f, 16f),
                    Math.Max(0.2f, __instance.Scale) * 4f,
                    __instance.flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                    (__instance.position.Y + 128f + 1f) / 10000f);
            }
            else
            {
                b.Draw(
                    Game1.objectSpriteSheet,
                    __instance.getLocalPosition(Game1.viewport) + new Vector2(32f, 32f),
                    Game1.getSourceRectForStandardTileSheet(
                        Game1.objectSpriteSheet,
                        103,
                        16,
                        16),
                    new Color(255, 50, 50),
                    0f,
                    new Vector2(8f, 16f),
                    Math.Max(0.2f, __instance.Scale) * 4f,
                    __instance.flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                    (__instance.position.Y + 128f + 1f) / 10000f);
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
