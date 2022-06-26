namespace DaLion.Stardew.Professions.Commands;

#region using directives

using System.Linq;
using JetBrains.Annotations;

using Common;
using Common.Commands;

#endregion using directives

[UsedImplicitly]
internal sealed class SetUltimateChargeCommand : ConsoleCommand
{
    /// <summary>Construct an instance.</summary>
    /// <param name="handler">The <see cref="CommandHandler"/> instance that handles this command.</param>
    internal SetUltimateChargeCommand(CommandHandler handler)
        : base(handler) { }

    /// <inheritdoc />
    public override string Trigger => "ready_ult";

    /// <inheritdoc />
    public override string Documentation => "Max-out the player's Special Ability charge, or set it to the specified percentage.";

    /// <inheritdoc />
    public override void Callback(string[] args)
    {
        if (ModEntry.PlayerState.RegisteredUltimate is null)
        {
            Log.W("Not registered to an Ultimate.");
            return;
        }

        if (!args.Any())
        {
            ModEntry.PlayerState.RegisteredUltimate.ChargeValue = ModEntry.PlayerState.RegisteredUltimate.MaxValue;
            return;
        }

        if (args.Length > 1)
        {
            Log.W("Too many arguments. Specify a single value between 0 and 100.");
            return;
        }

        if (!int.TryParse(args[0], out var value) || value is < 0 or > 100)
        {
            Log.W("Bad arguments. Specify an integer value between 0 and 100.");
            return;
        }

        ModEntry.PlayerState.RegisteredUltimate.ChargeValue =
            (double) value * ModEntry.PlayerState.RegisteredUltimate.MaxValue / 100.0;
    }
}