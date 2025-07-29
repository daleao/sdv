namespace DaLion.Professions.Commands;

#region using directives

using DaLion.Shared.Attributes;
using DaLion.Shared.Commands;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="StartTreasureHuntCommand"/> class.</summary>
/// <param name="handler">The <see cref="CommandHandler"/> instance that handles this command.</param>
[UsedImplicitly]
[Debug]
internal sealed class StartTreasureHuntCommand(CommandHandler handler)
    : ConsoleCommand(handler)
{
    /// <inheritdoc />
    public override string[] Triggers { get; } = ["hunt", "start"];

    /// <inheritdoc />
    public override string Documentation => "Triggers a hunt of the specified type at the current location.";

    /// <inheritdoc />
    public override bool CallbackImpl(string trigger, string[] args)
    {
        if (args.Length < 1)
        {
            Log.W("You must specify the type of treasure hunt. Either 'Scavenger' or 'Prospector' key and value.");
            return false;
        }

        if (args[0].ToLowerInvariant() == "scavenger")
        {
            if (State.ScavengerHunt is null)
            {
                Log.W("The Scavenger Hunt instance has not been created.");
                return false;
            }

            State.ScavengerHunt.TryStart(Game1.currentLocation);
            return true;
        }

        if (args[0].ToLowerInvariant() == "prospector")
        {
            if (State.ProspectorHunt is null)
            {
                Log.W("The Prospector Hunt instance has not been created.");
                return false;
            }

            State.ProspectorHunt.TryStart(Game1.currentLocation);
            return true;
        }

        return true;
    }
}
