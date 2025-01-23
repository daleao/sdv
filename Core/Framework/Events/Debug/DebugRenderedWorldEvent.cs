namespace DaLion.Core.Framework.Events.Debug;

#region using directives

using System.Linq;
using DaLion.Shared.Attributes;
using DaLion.Shared.Events;
using DaLion.Shared.Extensions.Stardew;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using StardewValley.Monsters;
using StardewValley.Tools;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="DebugRenderedWorldEvent"/> class.</summary>
/// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
[UsedImplicitly]
[Debug]
internal sealed class DebugRenderedWorldEvent(EventManager? manager = null)
    : RenderedWorldEvent(manager ?? CoreMod.EventManager)
{
    /// <inheritdoc />
    public override bool IsEnabled => State.DebugMode;

    /// <inheritdoc />
    protected override void OnRenderedWorldImpl(object? sender, RenderedWorldEventArgs e)
    {
        Rectangle bb;
        foreach (var @object in Game1.currentLocation.Objects.Values)
        {
            bb = @object.GetBoundingBox();
            if (!Game1.viewport.Intersects(new xTile.Dimensions.Rectangle(bb.X, bb.Y, bb.Width, bb.Height)))
            {
                continue;
            }

            bb.X -= Game1.viewport.X;
            bb.Y -= Game1.viewport.Y;
            bb.Highlight(Color.Blue * 0.5f, e.SpriteBatch);
        }

        foreach (var feature in Game1.currentLocation.terrainFeatures.Values)
        {
            bb = feature.getBoundingBox();
            if (!Game1.viewport.Intersects(new xTile.Dimensions.Rectangle(bb.X, bb.Y, bb.Width, bb.Height)))
            {
                continue;
            }

            bb.X -= Game1.viewport.X;
            bb.Y -= Game1.viewport.Y;
            bb.Highlight(Color.Yellow * 0.5f, e.SpriteBatch);
        }

        foreach (var feature in Game1.currentLocation.largeTerrainFeatures)
        {
            bb = feature.getBoundingBox();
            if (!Game1.viewport.Intersects(new xTile.Dimensions.Rectangle(bb.X, bb.Y, bb.Width, bb.Height)))
            {
                continue;
            }

            bb.X -= Game1.viewport.X;
            bb.Y -= Game1.viewport.Y;
            bb.Highlight(Color.Yellow * 0.5f, e.SpriteBatch);
        }

        foreach (var character in Game1.currentLocation.characters.Cast<Character>()
                     .Concat(Game1.currentLocation.farmers))
        {
            bb = character.GetBoundingBox();
            if (!Game1.viewport.Intersects(new xTile.Dimensions.Rectangle(bb.X, bb.Y, bb.Width, bb.Height)))
            {
                continue;
            }

            bb.X -= Game1.viewport.X;
            bb.Y -= Game1.viewport.Y;
            var textHeight = Game1.dialogueFont.MeasureString("T").Y;
            switch (character)
            {
                case Monster monster:
                {
                    bb.Highlight(Color.Red * 0.5f, e.SpriteBatch);

                    var @string = character.Name + $" ({monster.Health} / {monster.MaxHealth})";
                    var textWidth = Game1.dialogueFont.MeasureString(@string).X;
                    e.SpriteBatch.DrawString(
                        Game1.dialogueFont,
                        @string,
                        new Vector2(bb.X - ((textWidth - bb.Width) / 2f), bb.Y - bb.Height - (textHeight * 2)),
                        Color.White);

                    @string = $"Position: {character.Position}";
                    textWidth = Game1.dialogueFont.MeasureString(@string).X;
                    e.SpriteBatch.DrawString(
                        Game1.dialogueFont,
                        @string,
                        new Vector2(bb.X - ((textWidth - bb.Width) / 2f), bb.Y - bb.Height - (textHeight * 3)),
                        Color.White);
                    bb = new Rectangle((int)character.Position.X - 2, (int)character.Position.Y - 2, 4, 4);
                    bb.X -= Game1.viewport.X;
                    bb.Y -= Game1.viewport.Y;
                    bb.Highlight(Color.White, e.SpriteBatch);

                    var bbc = character.GetBoundingBox().Center;
                    @string = $"BB Center: {bbc}";
                    textWidth = Game1.dialogueFont.MeasureString(@string).X;
                    e.SpriteBatch.DrawString(
                        Game1.dialogueFont,
                        @string,
                        new Vector2(bb.X - ((textWidth - bb.Width) / 2f), bb.Y - bb.Height - (textHeight * 2)),
                        Color.White);
                    bb = new Rectangle(bbc.X - 2, bbc.Y - 2, 4, 4);
                    bb.X -= Game1.viewport.X;
                    bb.Y -= Game1.viewport.Y;
                    bb.Highlight(Color.White, e.SpriteBatch);
                    @string = $"Damage: {monster.DamageToFarmer} | Defense: {monster.resilience.Value}";
                    textWidth = Game1.dialogueFont.MeasureString(@string).X;
                    e.SpriteBatch.DrawString(
                        Game1.dialogueFont,
                        @string,
                        new Vector2(bb.X - ((textWidth - bb.Width) / 2f), bb.Y - bb.Height - textHeight),
                        Color.White);
                    if (monster is Serpent serpent && serpent.IsRoyalSerpent())
                    {
                        var offset = new Vector2(bb.X - serpent.Position.X, bb.Y - serpent.Position.Y);
                        foreach (var segment in serpent.segments)
                        {
                            bb.X = (int)(segment.X + offset.X);
                            bb.Y = (int)(segment.Y + offset.Y);
                            bb.Highlight(Color.Red * 0.5f, e.SpriteBatch);
                        }
                    }

                    break;
                }

                case Farmer farmer:
                {
                    var tool = farmer.CurrentTool;
                    var toolLocation = farmer.GetToolLocation(true);
                    if (tool is not MeleeWeapon weapon || !farmer.UsingTool)
                    {
                        goto default;
                    }

                    var tileLocation1 = Vector2.Zero;
                    var tileLocation2 = Vector2.Zero;
                    bb = weapon.getAreaOfEffect(
                        (int)toolLocation.X,
                        (int)toolLocation.Y,
                        farmer.FacingDirection,
                        ref tileLocation1,
                        ref tileLocation2,
                        farmer.GetBoundingBox(),
                        farmer.FarmerSprite.currentAnimationIndex);
                    bb.X -= Game1.viewport.X;
                    bb.Y -= Game1.viewport.Y;
                    bb.Highlight(Color.Purple * 0.5f, e.SpriteBatch);
                    bb = farmer.GetBoundingBox();
                    goto default;
                }

                default:
                {
                    bb.Highlight(Color.Green * 0.5f, e.SpriteBatch);

                    var @string = character.Name;
                    var textWidth = Game1.dialogueFont.MeasureString(@string).X;
                    e.SpriteBatch.DrawString(
                        Game1.dialogueFont,
                        @string,
                        new Vector2(bb.X - ((textWidth - bb.Width) / 2f), bb.Y - bb.Height - (textHeight * 2)),
                        Color.White);
                    break;
                }
            }
        }
    }
}
