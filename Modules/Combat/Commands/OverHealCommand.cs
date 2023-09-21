namespace DaLion.Overhaul.Modules.Combat.Commands;

#region using directives

using DaLion.Overhaul.Modules.Combat.Events.Display.RenderedWorld;
using DaLion.Overhaul.Modules.Combat.Events.GameLoop.UpdateTicked;
using DaLion.Shared.Attributes;
using DaLion.Shared.Commands;

#endregion using directives

[UsedImplicitly]
[Debug]
internal sealed class OverHealCommand : ConsoleCommand
{
    /// <summary>Initializes a new instance of the <see cref="OverHealCommand"/> class.</summary>
    /// <param name="handler">The <see cref="CommandHandler"/> instance that handles this command.</param>
    internal OverHealCommand(CommandHandler handler)
        : base(handler)
    {
    }

    /// <inheritdoc />
    public override string[] Triggers { get; } = { "overheal" };

    /// <inheritdoc />
    public override string Documentation => "Heals the player, allowing health to go above the maximum value.";

    /// <inheritdoc />
    public override void Callback(string trigger, string[] args)
    {
        var player = Game1.player;
        if (args.Length == 0 || !int.TryParse(args[0], out var amount))
        {
            amount = (int)(player.maxHealth * 1.2f);
        }

        player.health = Math.Min(amount, (int)(player.maxHealth * 1.2f));
        if (player.health > player.maxHealth)
        {
            EventManager.Enable(typeof(BloodthirstyRenderedWorldEvent), typeof(BloodthirstyUpdateTickedEvent));
        }
    }
}
