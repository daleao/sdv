namespace DaLion.Professions.Commands;

#region using directives

using DaLion.Shared.Attributes;
using DaLion.Shared.Commands;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Stardew;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="RevalidateBuildingsCommand"/> class.</summary>
/// <param name="handler">The <see cref="CommandHandler"/> instance that handles this command.</param>
[UsedImplicitly]
[Debug]
internal sealed class RevalidateBuildingsCommand(CommandHandler handler)
    : ConsoleCommand(handler)
{
    /// <inheritdoc />
    public override string[] Triggers { get; } = ["revalidate_buildings", "reval"];

    /// <inheritdoc />
    public override string Documentation => "Revalidates farm buildings, applying profession rules to Barns, Coops, Fish Ponds and Slime Hutches.";

    /// <inheritdoc />
    public override bool CallbackImpl(string trigger, string[] args)
    {
        if (args.Length > 1)
        {
            Log.W("Additional arguments will be ignored.");
        }

        Game1.game1.RevalidateAllBuildings();
        return true;
    }
}
