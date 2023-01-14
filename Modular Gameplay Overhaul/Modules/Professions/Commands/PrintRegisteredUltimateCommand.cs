namespace DaLion.Overhaul.Modules.Professions.Commands;

#region using directives

using DaLion.Overhaul.Modules.Professions.VirtualProperties;
using DaLion.Shared.Commands;

#endregion using directives

[UsedImplicitly]
internal sealed class PrintRegisteredUltimateCommand : ConsoleCommand
{
    /// <summary>Initializes a new instance of the <see cref="PrintRegisteredUltimateCommand"/> class.</summary>
    /// <param name="handler">The <see cref="CommandHandler"/> instance that handles this command.</param>
    internal PrintRegisteredUltimateCommand(CommandHandler handler)
        : base(handler)
    {
    }

    /// <inheritdoc />
    public override string[] Triggers { get; } = { "print_ult", "which_ult", "ult" };

    /// <inheritdoc />
    public override string Documentation => "Print the player's currently registered Special Ability, if any.";

    /// <inheritdoc />
    public override void Callback(string trigger, string[] args)
    {
        var ultimate = Game1.player.Get_Ultimate();
        if (ultimate is null)
        {
            Log.I("Not registered to an Ultimate.");
            return;
        }

        Log.I($"Registered to {ultimate.Profession.Title}'s {ultimate.DisplayName}.");
    }
}
