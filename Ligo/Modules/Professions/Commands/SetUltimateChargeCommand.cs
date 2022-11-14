namespace DaLion.Ligo.Modules.Professions.Commands;

#region using directives

using DaLion.Ligo.Modules.Professions.Events.GameLoop;
using DaLion.Ligo.Modules.Professions.VirtualProperties;
using DaLion.Shared.Commands;

#endregion using directives

[UsedImplicitly]
internal sealed class SetUltimateChargeCommand : ConsoleCommand
{
    /// <summary>Initializes a new instance of the <see cref="SetUltimateChargeCommand"/> class.</summary>
    /// <param name="handler">The <see cref="CommandHandler"/> instance that handles this command.</param>
    internal SetUltimateChargeCommand(CommandHandler handler)
        : base(handler)
    {
    }

    /// <inheritdoc />
    public override string[] Triggers { get; } = { "ready_ult", "rdy", "set_charge", "charge" };

    /// <inheritdoc />
    public override string Documentation =>
        "Max-out the player's Special Ability charge, or set it to the specified percentage.";

    /// <inheritdoc />
    public override void Callback(string[] args)
    {
        var ultimate = Game1.player.Get_Ultimate();
        if (ultimate is null)
        {
            Log.W("Not registered to an Ultimate.");
            return;
        }

        switch (args.Length)
        {
            case <= 0:
                ultimate.ChargeValue = ultimate.MaxValue;
                return;
            case > 1:
                Log.W("Additional arguments will be ignored.");
                break;
        }

        if (!int.TryParse(args[0], out var value) || value is < 0 or > 100)
        {
            Log.W("Bad argument. Specify an integer value between 0 and 100.");
            return;
        }

        if (ultimate.CanActivate && value < ultimate.MaxValue)
        {
            ModEntry.Events.Disable<UltimateGaugeShakeUpdateTickedEvent>();
        }

        ultimate.ChargeValue = (double)value * ultimate.MaxValue / 100d;
    }
}
