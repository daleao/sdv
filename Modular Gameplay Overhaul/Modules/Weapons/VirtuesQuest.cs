namespace DaLion.Overhaul.Modules.Weapons;

#region using directives

using System.Collections.Generic;
using System.Linq;
using DaLion.Shared.Extensions.Collections;
using DaLion.Shared.Extensions.Stardew;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Quests;

#endregion using directives

internal sealed class VirtuesQuest : IQuest
{
    private readonly string _name = I18n.Get("quests.hero.journey.name");
    private readonly string _description = I18n.Get("quests.hero.journey.desc");

    private readonly List<string> _objectiveDescriptions =
        Virtue.List
            .OrderBy(virtue => virtue.Value)
            .Select(virtue => I18n.Get("virtues.trial") + virtue.DisplayName)
            .ToList();

    private readonly int[] _objectiveProgresses = new int[5];
    private readonly bool[] _completed = new bool[5];

    private bool _viewed;

    /// <summary>Initializes a new instance of the <see cref="VirtuesQuest"/> class.</summary>
    public VirtuesQuest()
    {
        Virtue.List.ForEach(virtue => this.UpdateVirtueProgress(virtue, true));
        this._viewed = Game1.player.Read<bool>(DataKeys.VirtueQuestViewed);
    }

    /// <inheritdoc />
    public string GetName()
    {
        return this._name;
    }

    /// <inheritdoc />
    public string GetDescription()
    {
        return this._description;
    }

    /// <inheritdoc />
    public List<string> GetObjectiveDescriptions()
    {
        return this._objectiveDescriptions;
    }

    /// <inheritdoc />
    public bool CanBeCancelled()
    {
        return false;
    }

    /// <inheritdoc />
    public void MarkAsViewed()
    {
        if (this._viewed)
        {
            return;
        }

        this._viewed = true;
        Game1.player.Write(DataKeys.VirtueQuestViewed, true.ToString());
    }

    /// <inheritdoc />
    public bool ShouldDisplayAsNew()
    {
        return !this._viewed;
    }

    /// <inheritdoc />
    public bool ShouldDisplayAsComplete()
    {
        return this._completed.All(i => i);
    }

    /// <inheritdoc />
    public bool IsTimedQuest()
    {
        return false;
    }

    /// <inheritdoc />
    public int GetDaysLeft()
    {
        return -1;
    }

    /// <inheritdoc />
    public bool IsHidden()
    {
        return false;
    }

    /// <inheritdoc />
    public bool HasReward()
    {
        return false;
    }

    /// <inheritdoc />
    public bool HasMoneyReward()
    {
        return false;
    }

    /// <inheritdoc />
    public int GetMoneyReward()
    {
        return 0;
    }

    /// <inheritdoc />
    public void OnMoneyRewardClaimed()
    {
    }

    /// <inheritdoc />
    public bool OnLeaveQuestPage()
    {
        return false;
    }

    /// <summary>Draws the objective with the specified <paramref name="index"/> inside the specified <see cref="QuestLog"/>.</summary>
    /// <param name="index">The objective index.</param>
    /// <param name="log">The <see cref="QuestLog"/>.</param>
    /// <param name="yPos">The current y-position in the <see cref="SpriteBatch"/>.</param>
    /// <param name="b">The <see cref="SpriteBatch"/> to draw to.</param>
    public void DrawObjective(int index, QuestLog log, ref float yPos, SpriteBatch b)
    {
        var virtue = Virtue.FromValue(index);
        var darkBarColor = Color.DarkRed;
        var barColor = Color.Red;
        if (this._objectiveProgresses[index] >= virtue.ProvenCondition)
        {
            barColor = Color.LimeGreen;
            darkBarColor = Color.Green;
        }

        const int inset = 64;
        const int objectiveCountDrawWidth = 160;
        const int barHorizontalPadding = 3;
        const int barVerticalPadding = 3;
        const int sliceWidth = 5;
        var notches = 4;
        var barBackgroundSource = new Rectangle(0, 224, 47, 12);
        var barNotchSource = new Rectangle(47, 224, 1, 12);
        string objectiveCountText;
        int maxTextWidth;
        if (virtue == Virtue.Generosity)
        {
            objectiveCountText = $"{Math.Min(this._objectiveProgresses[index], virtue.ProvenCondition):#,##0}g" + "/" +
                                 $"{virtue.ProvenCondition:#,##0}g";
            maxTextWidth = (int)Game1.dialogueFont
                .MeasureString($"{virtue.ProvenCondition:#,##0}g" + "/" + $"{virtue.ProvenCondition:#,##0}g").X;
        }
        else
        {
            objectiveCountText = Math.Min(this._objectiveProgresses[index], virtue.ProvenCondition) + "/" +
                                 virtue.ProvenCondition;
            maxTextWidth = (int)Game1.dialogueFont
                .MeasureString(this._objectiveProgresses[index] + "/" + virtue.ProvenCondition).X;
        }

        var countTextWidth = (int)Game1.dialogueFont.MeasureString(objectiveCountText).X;
        var textDrawPosition = log.xPositionOnScreen + log.width - inset - countTextWidth;
        var maxTextDrawPosition = log.xPositionOnScreen + log.width - inset - maxTextWidth;
        Utility.drawTextWithShadow(
            b,
            objectiveCountText,
            Game1.dialogueFont,
            new Vector2(textDrawPosition, yPos),
            Color.DarkBlue);
        //if (virtue == Virtue.Generosity)
        //{
        //    var targetTextWidth = Game1.dialogueFont.MeasureString($"{virtue.ProvenCondition:#,##0} ").X;
        //    var separatorWidth = Game1.dialogueFont.MeasureString("/").X;
        //    var drawX = textDrawPosition + targetTextWidth;
        //    if (WeaponsModule.Config.VirtueTrialTrialDifficulty == Config.TrialDifficulty.Hard)
        //    {
        //        drawX += 3;
        //    }

        //    Game1.spriteBatch.Draw(
        //        Game1.debrisSpriteSheet,
        //        new Vector2(drawX, yPos + 24),
        //        Game1.getSourceRectForStandardTileSheet(Game1.debrisSpriteSheet, 8, 16, 16),
        //        Color.White,
        //        0f,
        //        new Vector2(8f, 8f),
        //        4f,
        //        SpriteEffects.None,
        //        0.95f);

        //    drawX += targetTextWidth + separatorWidth + 9;
        //    Game1.spriteBatch.Draw(
        //        Game1.debrisSpriteSheet,
        //        new Vector2(drawX, yPos + 24),
        //        Game1.getSourceRectForStandardTileSheet(Game1.debrisSpriteSheet, 8, 16, 16),
        //        Color.White,
        //        0f,
        //        new Vector2(8f, 8f),
        //        4f,
        //        SpriteEffects.None,
        //        0.95f);
        //}

        var barDrawPosition = new Rectangle(
            log.xPositionOnScreen + inset,
            (int)yPos,
            log.width - (inset * 2) - objectiveCountDrawWidth,
            barBackgroundSource.Height * 4);
        if (barDrawPosition.Right > maxTextDrawPosition - 16)
        {
            var adjustment = barDrawPosition.Right - (maxTextDrawPosition - 16);
            barDrawPosition.Width -= adjustment;
        }

        b.Draw(
            Game1.mouseCursors2,
            new Rectangle(
                barDrawPosition.X,
                barDrawPosition.Y,
                sliceWidth * 4,
                barDrawPosition.Height),
            new Rectangle(
                barBackgroundSource.X,
                barBackgroundSource.Y,
                sliceWidth,
                barBackgroundSource.Height),
            Color.White,
            0f,
            Vector2.Zero,
            SpriteEffects.None,
            0.5f);
        b.Draw(
            Game1.mouseCursors2,
            new Rectangle(
                barDrawPosition.X + (sliceWidth * 4),
                barDrawPosition.Y,
                barDrawPosition.Width - (2 * sliceWidth * 4),
                barDrawPosition.Height),
            new Rectangle(
                barBackgroundSource.X + sliceWidth,
                barBackgroundSource.Y,
                barBackgroundSource.Width - (2 * sliceWidth),
                barBackgroundSource.Height),
            Color.White,
            0f,
            Vector2.Zero,
            SpriteEffects.None,
            0.5f);
        b.Draw(
            Game1.mouseCursors2,
            new Rectangle(
                barDrawPosition.Right - (sliceWidth * 4),
                barDrawPosition.Y,
                sliceWidth * 4,
                barDrawPosition.Height),
            new Rectangle(
                barBackgroundSource.Right - sliceWidth,
                barBackgroundSource.Y,
                sliceWidth,
                barBackgroundSource.Height),
            Color.White,
            0f,
            Vector2.Zero,
            SpriteEffects.None,
            0.5f);

        var questProgress = Math.Min((float)this._objectiveProgresses[index] / virtue.ProvenCondition, 1f);
        if (virtue.ProvenCondition < notches)
        {
            notches = virtue.ProvenCondition;
        }

        barDrawPosition.X += 4 * barHorizontalPadding;
        barDrawPosition.Width -= 4 * barHorizontalPadding * 2;
        for (var k = 1; k < notches; k++)
        {
            b.Draw(
                Game1.mouseCursors2,
                new Vector2(
                    barDrawPosition.X + (barDrawPosition.Width * ((float)k / notches)),
                    barDrawPosition.Y),
                barNotchSource,
                Color.White,
                0f,
                Vector2.Zero,
                4f,
                SpriteEffects.None,
                0.5f);
        }

        barDrawPosition.Y += 4 * barVerticalPadding;
        barDrawPosition.Height -= 4 * barVerticalPadding * 2;
        var rect = new Rectangle(
            barDrawPosition.X,
            barDrawPosition.Y,
            (int)(barDrawPosition.Width * questProgress) - 4,
            barDrawPosition.Height);
        b.Draw(
            Game1.staminaRect,
            rect,
            null,
            barColor,
            0f,
            Vector2.Zero,
            SpriteEffects.None,
            rect.Y / 10000f);

        rect.X = rect.Right;
        rect.Width = 4;
        b.Draw(
            Game1.staminaRect,
            rect,
            null,
            darkBarColor,
            0f,
            Vector2.Zero,
            SpriteEffects.None,
            rect.Y / 10000f);
        yPos += (barBackgroundSource.Height + 4) * 4;
    }

    /// <summary>Marks the corresponding objective as complete if the specified <paramref name="virtue"/> has been proven.</summary>
    /// <param name="virtue">The <see cref="Virtue"/> to be checked.</param>
    /// <param name="silent">Whether to prevent question completion notifications.</param>
    internal void UpdateVirtueProgress(Virtue virtue, bool silent = false)
    {
        if (this._completed[virtue])
        {
            return;
        }

        this._objectiveProgresses[virtue] = virtue.GetProgress();
        if (!virtue.Proven())
        {
            return;
        }

        this._completed[virtue] = true;
        var player = Game1.player;
        if (!silent)
        {
            Game1.playSound("questcomplete");
            Shared.Networking.Broadcaster.SendPublicChat($"{player.Name} has proven their {virtue}.");
        }

        if (!Virtue.AllProven(player))
        {
            return;
        }

        WeaponsModule.State.Quest = null;
        player.Write(DataKeys.VirtueQuestState, VirtuesQuestState.Completed.ToString());
        player.Write(DataKeys.ProvenHonor, null);
        player.Write(DataKeys.ProvenCompassion, null);
        player.Write(DataKeys.ProvenWisdom, null);
        player.Write(DataKeys.ProvenGenerosity, null);
        player.addQuest((int)Quest.HeroReward);
    }
}
