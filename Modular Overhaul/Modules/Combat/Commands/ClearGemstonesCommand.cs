namespace DaLion.Overhaul.Modules.Combat.Commands;

#region using directives

using DaLion.Shared.Commands;
using StardewValley.Objects;

#endregion using directives

[UsedImplicitly]
internal sealed class ClearGemstonesCommand : ConsoleCommand
{
    /// <summary>Initializes a new instance of the <see cref="ClearGemstonesCommand"/> class.</summary>
    /// <param name="handler">The <see cref="CommandHandler"/> instance that handles this command.</param>
    internal ClearGemstonesCommand(CommandHandler handler)
        : base(handler)
    {
    }

    /// <inheritdoc />
    public override string[] Triggers { get; } = { "clear_gemstones", "cear_gems", "desocket" };

    /// <inheritdoc />
    public override string Documentation => "Remove all gemstones from the selected infinity band.";

    /// <inheritdoc />
    public override void Callback(string trigger, string[] args)
    {
        if (!Globals.InfinityBandIndex.HasValue)
        {
            Log.W("The Infinity Band is not loaded.");
            return;
        }

        if (Game1.player.CurrentItem is not CombinedRing combined || combined.ParentSheetIndex != Globals.InfinityBandIndex.Value)
        {
            Log.W("You must select an Infinity Band first.");
            return;
        }

        combined.combinedRings.Clear();
    }
}
