namespace DaLion.Taxes.Framework.Events;

#region using directives

using DaLion.Shared.Events;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
internal sealed class HostModMessageReceivedEvent : ModMessageReceivedEvent
{
    /// <summary>Initializes a new instance of the <see cref="HostModMessageReceivedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal HostModMessageReceivedEvent(EventManager? manager = null)
        : base(manager ?? TaxesMod.EventManager)
    {
    }

    /// <inheritdoc />
    public override bool IsEnabled => Context.IsMultiplayer && Context.IsMainPlayer;

    /// <inheritdoc />
    protected override void OnModMessageReceivedImpl(object? sender, ModMessageReceivedEventArgs e)
    {
        if (e.FromModID != UniqueId)
        {
            return;
        }

        var key = e.Type;
        if (string.IsNullOrWhiteSpace(key))
        {
            return;
        }

        var value = e.ReadAs<int>();
        switch (key)
        {
            case DataKeys.BusinessExpenses:
                if (value <= 0)
                {
                    return;
                }

                Log.I($"A farmhand has expended {value}g. This amount will be added to {Game1.player.farmName} farm's business expenses.");
                Data.Increment(Game1.player, key, value);
                break;
        }
    }
}
