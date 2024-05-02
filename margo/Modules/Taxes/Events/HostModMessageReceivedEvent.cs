namespace DaLion.Overhaul.Modules.Taxes.Events;

#region using directives

using DaLion.Shared.Events;
using DaLion.Shared.Extensions.Stardew;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
internal sealed class HostModMessageReceivedEvent : ModMessageReceivedEvent
{
    /// <summary>Initializes a new instance of the <see cref="HostModMessageReceivedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal HostModMessageReceivedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    public override bool IsEnabled => Context.IsMultiplayer && Context.IsMainPlayer;

    /// <inheritdoc />
    protected override void OnModMessageReceivedImpl(object? sender, ModMessageReceivedEventArgs e)
    {
        if (e.FromModID != Manifest.UniqueID || !e.Type.Contains(OverhaulModule.Taxes.Namespace))
        {
            return;
        }

        var field = e.Type.Split('/')[1];
        if (string.IsNullOrWhiteSpace(field))
        {
            return;
        }

        var value = e.ReadAs<int>();
        switch (field)
        {
            case DataKeys.BusinessExpenses:
                if (value <= 0)
                {
                    return;
                }

                Log.I($"A farmhand has expended {value}g. This amount will be added to {Game1.player.farmName} farm's business expenses.");
                Game1.player.Increment(field, value);
                break;
        }
    }
}
