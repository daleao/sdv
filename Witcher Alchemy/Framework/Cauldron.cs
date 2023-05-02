namespace DaLion.Alchemy.Framework;

#region using directives

using System.Text;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

#endregion using directives

/// <summary>The Cauldron tool.</summary>
[XmlType("Mods_DaLion_Cauldron")]
public sealed class Cauldron : Tool
{
    private readonly Rectangle _sourceRect;

    /// <summary>Initializes a new instance of the <see cref="Cauldron"/> class.</summary>
    public Cauldron(int upgradeLevel)
        : base("Cauldron", upgradeLevel, -1, -1, false)
    {
        this._sourceRect = new Rectangle(16 * this.UpgradeLevel, 0, 16, 16);
    }

    /// <summary>Gets the internal name of the <see cref="Cauldron"/>.</summary>
    public static string InternalName { get; } = ModEntry.Manifest.UniqueID + ".Tool";

    /// <summary>Gets the <see cref="Cauldron"/> texture.</summary>
    public static Texture2D Texture { get; } =
        ModEntry.ModHelper.GameContent.Load<Texture2D>($"{ModEntry.Manifest.UniqueID}/Cauldron");

    /// <inheritdoc />
    public override Item getOne()
    {
        Cauldron cauldron = new(this.UpgradeLevel);
        cauldron._GetOneFrom(this);
        return cauldron;
    }

    /// <inheritdoc />
    protected override string loadDisplayName()
    {
        return ModEntry.i18n.Get("tool.name");
    }

    /// <inheritdoc />
    protected override string loadDescription()
    {
        return ModEntry.i18n.Get("tool.desc");
    }

    /// <inheritdoc />
    public override bool beginUsing(GameLocation location, int x, int y, Farmer who)
    {
        return base.beginUsing(location, x, y, who);
    }

    /// <inheritdoc />
    public override void DoFunction(GameLocation location, int x, int y, int power, Farmer who)
    {
        base.DoFunction(location, x, y, power, who);
    }

    /// <inheritdoc />
    public override void endUsing(GameLocation location, Farmer who)
    {
        base.endUsing(location, who);
    }

    /// <inheritdoc />
    public override void draw(SpriteBatch b)
    {
        base.draw(b);
    }

    /// <inheritdoc />
    public override void drawInMenu(
        SpriteBatch spriteBatch,
        Vector2 location,
        float scaleSize,
        float transparency,
        float layerDepth,
        StackDrawType drawStackNumber,
        Color color,
        bool drawShadow)
    {
        var offset = new Vector2(16f) / 2f;
        spriteBatch.Draw(
            Texture,
            location + (offset * 4f),
            this._sourceRect,
            color * transparency,
            0f,
            offset,
            scaleSize * 4f,
            SpriteEffects.None,
            layerDepth);
    }

    /// <inheritdoc />
    public override void drawTooltip(
        SpriteBatch spriteBatch,
        ref int x,
        ref int y,
        SpriteFont font,
        float alpha,
        StringBuilder overrideText)
    {
        base.drawTooltip(spriteBatch, ref x, ref y, font, alpha, overrideText);
    }
}
