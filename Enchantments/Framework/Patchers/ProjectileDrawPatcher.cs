namespace DaLion.Enchantments.Framework.Patchers;

#region using directives

using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Projectiles;

#endregion using directives

[UsedImplicitly]
internal sealed class ProjectileDrawPatcher : HarmonyPatcher
{
    private static int _frameTime;
    private static int _currentFrame;

    /// <summary>Initializes a new instance of the <see cref="ProjectileDrawPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal ProjectileDrawPatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireMethod<Projectile>(nameof(Projectile.draw), [typeof(SpriteBatch)]);
    }

    #region harmony patches

    /// <summary>Draw plasma effect.</summary>
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void ProjectileDrawPostfix(Projectile __instance, float? ____rotation, SpriteBatch b)
    {
        if (!Data.ReadAs<bool>(__instance, DataKeys.Energized))
        {
            return;
        }

        if (_frameTime++ % 5 == 0)
        {
            _currentFrame = (_currentFrame + 1) % 4;
        }

        b.Draw(
            Textures.EnergizedTx,
            Game1.GlobalToLocal(
                Game1.viewport,
                __instance.position.Value + new Vector2(32, 32)),
            new Rectangle(_currentFrame * 16, 0, 16, 16),
            Color.White,
            ____rotation ?? 0f,
            new Vector2(8f, 8f),
            __instance.localScale * 3.5f,
            SpriteEffects.None,
            (__instance.position.Y + 96f) / 10000f);
    }

    #endregion harmony patches
}
