namespace DaLion.Professions.Framework.UI;

#region using directives

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;

#endregion using directives

/// <summary>Warning dialogue box to inform players when they might be locked out of Skill Reset.</summary>
public sealed class MasteryWarningBox : DialogueBox
{
    private readonly MasteryTrackerMenu _masteryTrackerMenu;

    /// <summary>Initializes a new instance of the <see cref="MasteryWarningBox"/> class.</summary>
    /// <param name="location">The <see cref="GameLocation"/>.</param>
    /// <param name="masteryTrackerMenu">The <see cref="MasteryTrackerMenu"/>.</param>
    public MasteryWarningBox(GameLocation location, MasteryTrackerMenu masteryTrackerMenu)
        : base(
            Config.Masteries.LockMasteryUntilFullReset
                ? I18n.Prestige_Mastery_Warning()
                : I18n.Prestige_Mastery_Lock(),
            Config.Masteries.LockMasteryUntilFullReset
                ? [new Response("OK", Game1.content.LoadString("Strings\\UI:Confirm")).SetHotKey(Keys.Escape)]
                : location.createYesNoResponses())
    {
        this._masteryTrackerMenu = masteryTrackerMenu;
        location.afterQuestion = this.AfterQuestionBehavior;

        var text = Game1.parseText(this.getCurrentString(), Game1.dialogueFont, 1000);
        var textWidth = (int)Game1.dialogueFont.MeasureString(text).X;
        if (this.width <= textWidth)
        {
            return;
        }

        var delta = this.width - textWidth;
        this.x += (delta / 2) - 16;
        this.width = textWidth + 32;
        this.exitFunction = () => State.WarningBox = null;
    }

    /// <inheritdoc />
    public override void draw(SpriteBatch b)
    {
        b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.6f);

        var text = Game1.parseText(this.getCurrentString(), Game1.dialogueFont, 1000);
        drawTextureBox(
            b,
            Game1.mouseCursors_1_6,
            new Rectangle(1, 85, 21, 21),
            this.x,
            this.y - (this.heightForQuestions - this.height),
            this.width,
            this.heightForQuestions + 8,
            Color.White,
            4f,
            drawShadow: false);

        var pos = new Vector2(this.x + 24, this.y + 24 - (this.heightForQuestions - this.height));
        b.DrawString(
            Game1.dialogueFont,
            text,
            pos + new Vector2(-3f, 3f),
            Color.Black * 0.15f * 0.9f);
        b.DrawString(
            Game1.dialogueFont,
            text,
            pos + new Vector2(0f, 3f),
            Color.Black * 0.15f * 0.9f);
        b.DrawString(
            Game1.dialogueFont,
            text,
            pos,
            Color.Black * 0.9f);
        if (this.characterIndexInDialogue < this.getCurrentString().Length - 1)
        {
            return;
        }

        var responseY = this.y - (this.heightForQuestions - this.height) +
                        SpriteText.getHeightOfString(this.getCurrentString(), this.width - 16) + 48;
        for (var i = 0; i < this.responses.Length; i++)
        {
            if (i == this.selectedResponse)
            {
                drawTextureBox(
                    b,
                    Game1.mouseCursors,
                    new Rectangle(375, 357, 3, 3),
                    this.x + 4,
                    responseY - 8,
                    this.width - 8,
                    SpriteText.getHeightOfString(this.responses[i].responseText, this.width) + 16,
                    Color.White,
                    4f,
                    drawShadow: false);
            }

            pos.Y = responseY;
            b.DrawString(
                Game1.dialogueFont,
                this.responses[i].responseText,
                pos,
                Color.Black * 0.9f * (this.selectedResponse == i ? 1f : 0.6f));

            responseY += SpriteText.getHeightOfString(this.responses[i].responseText, this.width) + 16;
        }

        this.drawMouse(b);
    }

    /// <inheritdoc />
    public override void receiveGamePadButton(Buttons button)
    {
        if (button is not (Buttons.DPadUp or Buttons.DPadDown))
        {
            return;
        }

        if (this.currentlySnappedComponent is null)
        {
            this.snapToDefaultClickableComponent();
        }
        else
        {
            this.setCurrentlySnappedComponentTo((this.currentlySnappedComponent.myID + 1) % 2);
        }

        this.performHoverAction(Game1.getMouseX(), Game1.getMouseY());
    }

    private void AfterQuestionBehavior(Farmer who, string whichAnswer)
    {
        if (whichAnswer != "Yes")
        {
            State.WarningBox = null;
            return;
        }

        Game1.playSound("cowboy_monsterhit");
        DelayedAction.playSoundAfterDelay("cowboy_monsterhit", 200);
        Reflector.GetUnboundMethodDelegate<Action<MasteryTrackerMenu>>(this._masteryTrackerMenu, "claimReward")
            .Invoke(this._masteryTrackerMenu);
        if (Game1.activeClickableMenu == this._masteryTrackerMenu)
        {
            Game1.activeClickableMenu.exitThisMenuNoSound();
            Game1.playSound("discoverMineral");
        }

        State.WarningBox = null;
    }
}
