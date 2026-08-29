namespace DaLion.Professions.Framework.UI;

#region using directives

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Buildings;
using StardewValley.Menus;

#endregion using directives

internal sealed class SiloMenuWrapper
{
    internal SiloMenuWrapper(Building silo, ItemGrabMenu menu)
    {
        this.Silo = silo;
        this.Menu = menu;

        menu.initializeUpperRightCloseButton();
        menu.setBackgroundTransparency(b: false);
        menu.lastShippedHolder = new ClickableTextureComponent(
            string.Empty,
            new Rectangle(
                menu.xPositionOnScreen + (menu.width / 2) - 48,
                menu.yPositionOnScreen + (menu.height / 2) - 80 - 64,
                96,
                96),
            string.Empty,
            Game1.content.LoadString("Strings\\UI:ShippingBin_LastItem"),
            Game1.mouseCursors,
            new Rectangle(293, 360, 24, 24),
            4f)
        {
            myID = 12598,
            region = 12598,
        };

        for (var i = 0; i < menu.GetColumnCount(); i++)
        {
            if (menu.inventory?.inventory?.Count >= menu.GetColumnCount())
            {
                menu.inventory.inventory[i].upNeighborID = -7777;
                if (i == 11)
                {
                    menu.inventory.inventory[i].rightNeighborID = 5948;
                }
            }
        }

        menu.populateClickableComponentList();
        if (Game1.options.SnappyMenus)
        {
            menu.snapToDefaultClickableComponent();
        }
    }

    internal static SObject? LastItemDeposited { get; set; }

    internal ItemGrabMenu Menu { get; }

    internal Building Silo { get; }

    internal void Draw(SpriteBatch b)
    {
        if (LastItemDeposited is null)
        {
            return;
        }

        var lastShippedHolder = this.Menu.lastShippedHolder;
        lastShippedHolder.draw(b);
        var x = lastShippedHolder.bounds.X;
        var y = lastShippedHolder.bounds.Y;
        var bottom = lastShippedHolder.bounds.Bottom;
        LastItemDeposited.drawInMenu(b, new Vector2(x + 16, y + 16), 1f);
        b.Draw(Game1.mouseCursors, new Vector2(x + -8, bottom - 100), new Rectangle(325, 448, 5, 14), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
        b.Draw(Game1.mouseCursors, new Vector2(x + 84, bottom - 100), new Rectangle(325, 448, 5, 14), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
        b.Draw(Game1.mouseCursors, new Vector2(x + -8, bottom - 44), new Rectangle(325, 452, 5, 13), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
        b.Draw(Game1.mouseCursors, new Vector2(x + 84, bottom - 44), new Rectangle(325, 452, 5, 13), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
    }

    internal void ReceiveLeftClick(int x, int y, bool playSound = true)
    {
        if (LastItemDeposited is null || !this.Menu.lastShippedHolder.containsPoint(x, y))
        {
            return;
        }

        var player = Game1.player;
        var originalStack = LastItemDeposited.Stack;
        // the stack may or may not be consumed here, depending on whether a pre-existing stack exists in the player's inventory
        if (!player.addItemToInventoryBool(LastItemDeposited))
        {
            return;
        }

        Game1.playSound("coin");
        this.Silo.RemovePiecesOfCropFeed(LastItemDeposited, originalStack);
        LastItemDeposited = null;
        if (player.ActiveObject is not null)
        {
            player.showCarrying();
            player.Halt();
        }
    }
}
