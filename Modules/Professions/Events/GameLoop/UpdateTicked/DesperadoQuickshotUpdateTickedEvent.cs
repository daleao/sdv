namespace DaLion.Overhaul.Modules.Professions.Events.GameLoop.UpdateTicked;

#region using directives

using DaLion.Shared.Events;
using StardewModdingAPI.Events;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class DesperadoQuickshotUpdateTickedEvent : UpdateTickedEvent
{
    private readonly int _buffId = (Manifest.UniqueID + Profession.Desperado).GetHashCode();

    private int _timer;

    /// <summary>Initializes a new instance of the <see cref="DesperadoQuickshotUpdateTickedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal DesperadoQuickshotUpdateTickedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnEnabled()
    {
        this._timer = 50;
    }

    /// <inheritdoc />
    protected override void OnDisabled()
    {
        ProfessionsModule.State.LastDesperadoTarget = null;
    }

    /// <inheritdoc />
    protected override void OnUpdateTickedImpl(object? sender, UpdateTickedEventArgs e)
    {
        var player = Game1.player;
        if (player.CurrentTool is not Slingshot || --this._timer <= 0)
        {
            this.Disable();
            return;
        }

        if (player.hasBuff(this._buffId))
        {
            return;
        }

        Game1.buffsDisplay.addOtherBuff(
            new Buff(
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                1,
                "Desperado",
                _I18n.Get("desperado.title" + (Game1.player.IsMale ? ".male" : ".female")) + " " +
                I18n.Desperado_Buff_Name())
            {
                which = this._buffId,
                sheetIndex = Profession.DesperadoQuickshotSheetIndex,
                millisecondsDuration = 0,
                description = I18n.Desperado_Buff_Desc(),
            });
    }
}
