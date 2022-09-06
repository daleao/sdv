namespace DaLion.Stardew.Professions.Commands;

#region using directives

using Common;
using Common.Commands;
using Extensions;
using Framework;
using Framework.Ultimates;
using Framework.VirtualProperties;
using System;
using System.Linq;

#endregion using directives

[UsedImplicitly]
internal sealed class SetRegisteredUltimateCommand : ConsoleCommand
{
    /// <summary>Construct an instance.</summary>
    /// <param name="handler">The <see cref="CommandHandler"/> instance that handles this command.</param>
    internal SetRegisteredUltimateCommand(CommandHandler handler)
        : base(handler) { }

    /// <inheritdoc />
    public override string[] Triggers { get; } = { "set_ult" };

    /// <inheritdoc />
    public override string Documentation => "Change the player's currently registered Special Ability.";

    /// <inheritdoc />
    public override void Callback(string[] args)
    {
        if (args.Length != 1)
        {
            Log.W("You must specify a single value.");
            return;
        }

        if (!Game1.player.professions.Any(p => p is >= 26 and < 30))
        {
            Log.W("You don't have any 2nd-tier combat professions.");
            return;
        }

        var index = Array.FindIndex(Enum.GetNames<UltimateIndex>(),
            name => name.Contains(args[0], StringComparison.InvariantCultureIgnoreCase));
        if (index < 0)
        {
            Log.W("You must enter a valid 2nd-tier combat profession or special ability name.");
            return;
        }

        var value = Enum.GetValues<UltimateIndex>()[index];
        var profession = Profession.FromValue((int)value);
        if (!Game1.player.HasProfession(profession))
        {
            Log.W("You don't have this profession.");
            return;
        }

        Game1.player.set_Ultimate(Ultimate.FromIndex(value));
    }
}