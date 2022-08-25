namespace DaLion.Stardew.Rings.Framework;

#region using directives

using Common.Extensions;
using Common.Extensions.Stardew;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Objects;

#endregion using directives

public sealed class GarnetRing : Ring
{
    public static int Index = (ModEntry.Manifest.UniqueID + "GarnetRing").GetDeterministicHashCode();

    public static Texture2D Texture = ModEntry.ModHelper.GameContent.Load<Texture2D>($"{ModEntry.Manifest.UniqueID}/Garnet");

    public GarnetRing()
    {
        Category = SObject.ringCategory;
        Name = "Garnet Ring";
        price.Value = 7000;
        indexInTileSheet.Value = Index;
        ParentSheetIndex = Index;
        uniqueID.Value = Game1.year + Game1.dayOfMonth + Game1.timeOfDay + Index +
                         Game1.player.getTileX() + (int)Game1.stats.MonstersKilled + (int)Game1.stats.itemsCrafted;
        loadDisplayFields();
    }

    public override void drawInMenu(SpriteBatch spriteBatch, Vector2 location, float scaleSize, float transparency, float layerDepth,
        StackDrawType drawStackNumber, Color color, bool drawShadow)
    {
        base.drawInMenu(spriteBatch, location, scaleSize, transparency, layerDepth, drawStackNumber, color, drawShadow);
        spriteBatch.Draw(Texture, location + new Vector2(32f, 32f) * scaleSize,
            new Rectangle(ModEntry.IsBetterRingsLoaded ? 16 : 0, 0, 16, 16), color * transparency, 0f,
            new Vector2(8f, 8f) * scaleSize, scaleSize * 4f, SpriteEffects.None, layerDepth);
    }

    protected override bool loadDisplayFields()
    {
        displayName = ModEntry.i18n.Get("rings.garnet.name");
        description = ModEntry.i18n.Get("rings.garnet.desc");
        return true;
    }

    public override void onEquip(Farmer who, GameLocation location)
    {
        who.Increment("CooldownReduction", 0.1f);
    }

    public override void onUnequip(Farmer who, GameLocation location)
    {
        who.Increment("CooldownReduction", -0.1f);
    }
}