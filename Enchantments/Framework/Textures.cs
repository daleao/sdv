namespace DaLion.Enchantments.Framework;

#region using directives

using Microsoft.Xna.Framework.Graphics;

#endregion using directives

internal static class Textures
{
    private static readonly Lazy<Texture2D> _energizedTx =
        new(() => ModHelper.ModContent.Load<Texture2D>("assets/sprites/energized.png"));

    private static Lazy<Texture2D> _gemSocketTx =
        new(() => ModHelper.GameContent.Load<Texture2D>($"{Manifest.UniqueID}_GemstoneSockets"));

    internal static Texture2D EnergizedTx => _energizedTx.Value;

    internal static Texture2D GemSocketTx => _gemSocketTx.Value;

    internal static void Reload(IEnumerable<IAssetName> assets)
    {
        var names = assets.Select(a => a.BaseName).ToHashSet();
        if (names.Contains("GemstoneSockets"))
        {
            _gemSocketTx = new Lazy<Texture2D>(() =>
                ModHelper.GameContent.Load<Texture2D>($"{Manifest.UniqueID}_GemstoneSockets"));
        }
    }
}
