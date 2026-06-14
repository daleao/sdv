namespace DaLion.Professions.Framework.Events.Input.ButtonPressed;

#region using directives

using DaLion.Shared.Events;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Utilities;
using StardewValley.Tools;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="SpelunkerButtonDoublePressedEvent"/> class.</summary>
[UsedImplicitly]
internal sealed class SpelunkerButtonDoublePressedEvent : ButtonDoublePressedEvent
{
    /// <summary>Initializes a new instance of the <see cref="SpelunkerButtonDoublePressedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    public SpelunkerButtonDoublePressedEvent(EventManager? manager = null)
        : base(manager ?? ProfessionsMod.EventManager)
    {
        this.OnButtonDoublePressed = this.OnButtonDoublePressedImpl;
    }

    /// <inheritdoc />
    public override KeybindList KeybindList => Config.ModKey;

    /// <inheritdoc />
    public override bool IsEnabled => Game1.player.HasProfession(Profession.Spelunker);

    /// <inheritdoc />
    public override void OnButtonDoublePressedImpl()
    {
        var player = Game1.player;
        if (Game1.activeClickableMenu is not null || player.CurrentTool is not Pickaxe)
        {
            return;
        }

        if (State.SpelunkerLadderStreak < 10)
        {
            Game1.playSound("cancel");
            return;
        }

        var recovered = State.SpelunkerLadderStreak * 2;
        player.Stamina = Math.Min(player.Stamina + recovered, player.MaxStamina);
        player.currentLocation.debris.Add(new Debris(
            recovered,
            new Vector2(player.StandingPixel.X, player.StandingPixel.Y),
            Color.Yellow,
            1f,
            player));
        Game1.playSound("healSound");

        State.SpelunkerLadderStreak = 0;
    }
}
