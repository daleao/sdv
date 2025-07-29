namespace DaLion.Professions.Commands;

#region using directives

using DaLion.Shared.Attributes;
using DaLion.Shared.Commands;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="PipeSlimeCommand"/> class.</summary>
/// <param name="handler">The <see cref="CommandHandler"/> instance that handles this command.</param>
[UsedImplicitly]
[Debug]
internal sealed class PipeSlimeCommand(CommandHandler handler)
    : ConsoleCommand(handler)
{
    /// <inheritdoc />
    public override string[] Triggers { get; } = ["pipe"];

    /// <inheritdoc />
    public override string Documentation => "Causes a nearby Slime to become Piped.";

    /// <inheritdoc />
    public override bool CallbackImpl(string trigger, string[] args)
    {
        if (Game1.player.HasProfession(Profession.Brute))
        {
            State.BruteRageCounter += 20;
        }

        return true;
    }
}
