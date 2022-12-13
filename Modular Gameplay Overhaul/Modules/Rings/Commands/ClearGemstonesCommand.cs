namespace DaLion.Overhaul.Modules.Rings.Commands;

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
    public override string[] Triggers { get; } = { "clear_gemstones", "clear" };

    /// <inheritdoc />
    public override string Documentation => "Remove all gemstones from the selected infinity band.";

    /// <inheritdoc />
    public override void Callback(string[] args)
    {
        if (Game1.player.CurrentItem is not CombinedRing combined || combined.ParentSheetIndex != Globals.InfinityBandIndex)
        {
            Log.W("You must select an Infinity Band first.");
            return;
        }

        combined.combinedRings.Clear();
    }
}
