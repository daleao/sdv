namespace DaLion.Stardew.Alchemy.Framework.Textures;

#region using directives

using Microsoft.Xna.Framework.Graphics;
using StardewValley;

#endregion using directives

internal static class Textures
{
    #region textures

    public static Texture2D InterfaceTx { get; } =
        ModEntry.ModHelper.GameContent.Load<Texture2D>($"{ModEntry.Manifest.UniqueID}/Interface");

    public static Texture2D ObjectsTx { get; } =
        ModEntry.ModHelper.GameContent.Load<Texture2D>($"{ModEntry.Manifest.UniqueID}/Objects");

    public static Texture2D TalentsTx { get; } =
        ModEntry.ModHelper.GameContent.Load<Texture2D>($"{ModEntry.Manifest.UniqueID}/Talents");

    //public static Texture2D AnimationsTx { get; set; } =
    //    ModEntry.ModHelper.GameContent.Load<Texture2D>($"{ModEntry.Manifest.UniqueID}/Animations");

    public static Texture2D CursorsTx { get; } = Game1.content.Load<Texture2D>("LooseSprites\\Cursors");

    #endregion textures
}