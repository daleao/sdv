namespace DaLion.Ligo.Modules.Arsenal;

#region using directives

using Microsoft.Xna.Framework.Graphics;

#endregion using directives

/// <summary>Caches custom mod textures and related functions.</summary>
internal static class Textures
{
    internal static Texture2D ProjectilesTx { get; } =
        ModEntry.ModHelper.ModContent.Load<Texture2D>("assets/sprites/projectiles");

    internal static Texture2D BuffsSheetTx { get; } =
        ModEntry.ModHelper.ModContent.Load<Texture2D>("assets/sprites/buffs");
}
