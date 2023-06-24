namespace DaLion.Overhaul.Modules.Tools;

#region using directives

using Microsoft.Xna.Framework.Graphics;

#endregion using directives

/// <summary>Caches custom mod textures and related functions.</summary>
internal static class Textures
{
    internal static Texture2D RadioactiveToolsTx { get; } =
        ModHelper.GameContent.Load<Texture2D>($"{Manifest.UniqueID}/RadioactiveTools");
}
