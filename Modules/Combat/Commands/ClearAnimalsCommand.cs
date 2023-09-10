namespace DaLion.Overhaul.Modules.Combat.Commands;

#region using directives

using DaLion.Overhaul.Modules.Combat.VirtualProperties;
using DaLion.Shared.Attributes;
using DaLion.Shared.Commands;
using StardewValley;

#endregion using directives

[UsedImplicitly]
[Debug]
internal sealed class ClearAnimalsCommand : ConsoleCommand
{
    /// <summary>Initializes a new instance of the <see cref="ClearAnimalsCommand"/> class.</summary>
    /// <param name="handler">The <see cref="CommandHandler"/> instance that handles this command.</param>
    internal ClearAnimalsCommand(CommandHandler handler)
        : base(handler)
    {
    }

    /// <inheritdoc />
    public override string[] Triggers { get; } = { "clear_wabbajack", "clear_wab" };

    /// <inheritdoc />
    public override string Documentation => "Clears all transfigured animals in the current location.";

    /// <inheritdoc />
    public override void Callback(string trigger, string[] args)
    {
        Game1.player.currentLocation.Get_Animals().Clear();
    }
}
