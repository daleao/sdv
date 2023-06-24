namespace DaLion.Overhaul.Modules.Rings;

#region using directives

using Microsoft.Xna.Framework.Graphics;

#endregion using directives

/// <summary>Caches custom mod textures and related functions.</summary>
internal static class Textures
{
    internal static Texture2D GemstonesTx { get; } = ModHelper.ModContent.Load<Texture2D>("assets/sprites/gemstones");

    internal static Texture2D RingsTx { get; } = ModHelper.ModContent.Load<Texture2D>("assets/sprites/rings");

    internal static Texture2D PatternedResonanceTx { get; } =
        ModHelper.ModContent.Load<Texture2D>("assets/sprites/resonance_patterned");

    internal static Texture2D StrongerResonanceTx { get; } =
        ModHelper.ModContent.Load<Texture2D>("assets/sprites/resonance_stronger");

    internal static Texture2D ShieldTx { get; set; } = ModHelper.ModContent.Load<Texture2D>("assets/sprites/shield.png");
}
