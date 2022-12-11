namespace DaLion.Ligo.Modules.Core.Commands;

#region using directives

using System.Linq;
using DaLion.Shared.Attributes;
using DaLion.Shared.Commands;
using Professions.Extensions;
using StardewValley.Monsters;

#endregion using directives

[UsedImplicitly]
[Debug]
internal sealed class DebugCommand : ConsoleCommand
{
    /// <summary>Initializes a new instance of the <see cref="DebugCommand"/> class.</summary>
    /// <param name="handler">The <see cref="CommandHandler"/> instance that handles this command.</param>
    internal DebugCommand(CommandHandler handler)
        : base(handler)
    {
    }

    /// <inheritdoc />
    public override string[] Triggers { get; } = { "debug" };

    /// <inheritdoc />
    public override string Documentation => "Wildcard command for on-demand debugging.";

    /// <inheritdoc />
    public override void Callback(string[] args)
    {
        var slimes = Game1.player.GetRaisedSlimes().ToList();
        Log.D($"{Game1.player.Name} owns {slimes.Count} slimes.");

        slimes = Game1.getFarm().characters.OfType<GreenSlime>().ToList();
        var playerPos = Game1.player.Position;
    }
}
