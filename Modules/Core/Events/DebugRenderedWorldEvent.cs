namespace DaLion.Overhaul.Modules.Core.Events;

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

[UsedImplicitly]
[Debug]
internal sealed class DebugRenderedWorldEvent : RenderedWorldEvent
{
    /// <summary>Initializes a new instance of the <see cref="DebugRenderedWorldEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal DebugRenderedWorldEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    public override bool IsEnabled => State.DebugMode;

    /// <inheritdoc />
    protected override void OnRenderedWorldImpl(object? sender, RenderedWorldEventArgs e)
    {
        var bb = default(Rectangle);
        foreach (var @object in Game1.currentLocation.Objects.Values)
        {
            bb = @object.getBoundingBox(@object.TileLocation);
            bb.X -= Game1.viewport.X;
            bb.Y -= Game1.viewport.Y;
            bb.Highlight(Color.Blue * 0.5f, e.SpriteBatch);
        }

        foreach (var feature in Game1.currentLocation.terrainFeatures.Values)
        {
            bb = feature.getBoundingBox(feature.currentTileLocation);
            bb.X -= Game1.viewport.X;
            bb.Y -= Game1.viewport.Y;
            bb.Highlight(Color.Yellow * 0.5f, e.SpriteBatch);
        }

        foreach (var feature in Game1.currentLocation.largeTerrainFeatures)
        {
            bb = feature.getBoundingBox(feature.currentTileLocation);
            bb.X -= Game1.viewport.X;
            bb.Y -= Game1.viewport.Y;
            bb.Highlight(Color.Yellow * 0.5f, e.SpriteBatch);
        }

        foreach (var character in Game1.currentLocation.characters.Cast<Character>()
                     .Concat(Game1.currentLocation.farmers))
        {
            bb = character.GetBoundingBox();
            bb.X -= Game1.viewport.X;
            bb.Y -= Game1.viewport.Y;
            switch (character)
            {
                case Monster monster:
                    {
                        bb.Highlight(Color.Red * 0.5f, e.SpriteBatch);

                        var str = character.Name + $" ({monster.Health} / {monster.MaxHealth})";
                        var length = Game1.dialogueFont.MeasureString(str);
                        e.SpriteBatch.DrawString(
                            Game1.dialogueFont,
                            str,
                            new Vector2(bb.X - (length.X - bb.Width) / 2f, bb.Y - bb.Height - length.Y - length.Y),
                            Color.White);

                        str = $"Damage: {monster.DamageToFarmer} | Defense: {monster.resilience.Value}";
                        length = Game1.dialogueFont.MeasureString(str);
                        e.SpriteBatch.DrawString(
                            Game1.dialogueFont,
                            str,
                            new Vector2(bb.X - (length.X - bb.Width) / 2f, bb.Y - bb.Height - length.Y),
                            Color.White);
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
                        var length = Game1.dialogueFont.MeasureString(@string);
                        e.SpriteBatch.DrawString(
                            Game1.dialogueFont,
                            @string,
                            new Vector2(bb.X - (length.X - bb.Width) / 2f, bb.Y - bb.Height - length.Y),
                            Color.White);
                        break;
                    }
            }
        }
    }
}
