namespace DaLion.Professions.Framework.UI;

#region using directives

using DaLion.Professions.Framework.VirtualProperties;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley.Extensions;
using StardewValley.Menus;
using ScreenAnchorPosition = DaLion.Professions.ProfessionsConfig.ScreenAnchorPosition;

#endregion using directive

internal sealed class PipedMinionHud : IClickableMenu, IDisposable
{
    private List<PipedMinionHudComponent> _allMinionHudComponents = [];

    internal PipedMinionHud()
    {
        this.allClickableComponents = [];
        this.populateClickableComponentList();
        Game1.onScreenMenus.Add(this);
    }

    public void Dispose()
    {
        Game1.onScreenMenus.Remove(this);
    }

    public override void draw(SpriteBatch b)
    {
        foreach (var component in this._allMinionHudComponents)
        {
            component.draw(b);
        }

        foreach (var component in this._allMinionHudComponents)
        {
            if (component.containsPoint(Game1.getMouseX(), Game1.getMouseY()))
            {
                component.DrawTooltip(b);
            }
        }
    }

    public override void receiveLeftClick(int x, int y, bool playSound = true)
    {
        foreach (var component in this._allMinionHudComponents)
        {
            component.OnClick(x, y);
        }
    }

    public override void receiveGamePadButton(Buttons buttons)
    {
        if (Config.ModKey.JustPressed())
        {
            this.setCurrentlySnappedComponentTo(0);
            return;
        }

        if (!Config.ModKey.IsDown())
        {
            this.currentlySnappedComponent = null;
            return;
        }

        switch (buttons)
        {
            case Buttons.A:
                (this.currentlySnappedComponent as PipedMinionHudComponent)!.OnClick();
                break;
            case Buttons.DPadRight:
                this.setCurrentlySnappedComponentTo(this.currentlySnappedComponent!.myID + 1);
                break;
            case Buttons.DPadLeft:
                this.setCurrentlySnappedComponentTo(this.currentlySnappedComponent!.myID - 1);
                break;
        }
    }

    public override bool areGamePadControlsImplemented()
    {
        return true;
    }

    public override void populateClickableComponentList()
    {
        var piped = GreenSlime_Piped.Values.Select(pair => pair.Value).ToList();
        var hatSlimeIndex = piped.FindIndex(pip => pip.Hat is not null);
        if (hatSlimeIndex != -1)
        {
            piped.RemoveAt(hatSlimeIndex);
        }

        this.Reposition(piped.Count);
        this.allClickableComponents.Clear();
        this._allMinionHudComponents.Clear();
        for (var i = 0; i < piped.Count; i++)
        {
            var component = new PipedMinionHudComponent(piped[i], this, i);
            this.allClickableComponents.Add(component);
            this._allMinionHudComponents.Add(component);
        }
    }

    private void Reposition(int numberOfComponents)
    {
        const int sideLength = 9;
        const int padding = 2;
        const int healthBarHeight = 1;
        switch (Config.MinionHudAnchorPosition)
        {
            case ScreenAnchorPosition.Top:
                this.width = (((sideLength + padding) * numberOfComponents) - padding) * Game1.pixelZoom;
                this.height = (sideLength + healthBarHeight) * Game1.pixelZoom;
                break;
            default:
                this.width = (sideLength + healthBarHeight) * Game1.pixelZoom;
                this.height = (((sideLength + padding) * numberOfComponents) - padding) * Game1.pixelZoom;
                break;
        }

        var position = Config.MinionHudAnchorPosition switch
        {
            ScreenAnchorPosition.Top => new Vector2(
                Game1.graphics.GraphicsDevice.Viewport.GetTitleSafeArea().Left + (Game1.uiViewport.Width / 2) - (this.width / 2),
                Game1.graphics.GraphicsDevice.Viewport.GetTitleSafeArea().Top),
            ScreenAnchorPosition.Left => new Vector2(
                Game1.graphics.GraphicsDevice.Viewport.GetTitleSafeArea().Left,
                Game1.graphics.GraphicsDevice.Viewport.GetTitleSafeArea().Top + (Game1.uiViewport.Height / 2) - (this.height / 2)),
            ScreenAnchorPosition.Right => new Vector2(
                Game1.graphics.GraphicsDevice.Viewport.GetTitleSafeArea().Right,
                Game1.graphics.GraphicsDevice.Viewport.GetTitleSafeArea().Top + (Game1.uiViewport.Height / 2) - (this.height / 2)),
        };

        position += Config.MinionHudOffset * Game1.pixelZoom;
        this.xPositionOnScreen = (int)position.X;
        this.yPositionOnScreen = (int)position.Y;
    }
}
