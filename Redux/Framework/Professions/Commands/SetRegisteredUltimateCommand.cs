namespace DaLion.Redux.Framework.Professions.Commands;

#region using directives

using System.Linq;
using DaLion.Redux.Framework.Professions.Extensions;
using DaLion.Redux.Framework.Professions.Ultimates;
using DaLion.Redux.Framework.Professions.VirtualProperties;
using DaLion.Shared.Commands;

#endregion using directives

[UsedImplicitly]
internal sealed class SetRegisteredUltimateCommand : ConsoleCommand
{
    /// <summary>Initializes a new instance of the <see cref="SetRegisteredUltimateCommand"/> class.</summary>
    /// <param name="handler">The <see cref="CommandHandler"/> instance that handles this command.</param>
    internal SetRegisteredUltimateCommand(CommandHandler handler)
        : base(handler)
    {
    }

    /// <inheritdoc />
    public override string[] Triggers { get; } = { "set_ult" };

    /// <inheritdoc />
    public override string Documentation => "Change the player's currently registered Special Ability.";

    /// <inheritdoc />
    public override void Callback(string[] args)
    {
        if (args.Length > 1)
        {
            Log.W("Additional arguments beyond the first will be ignored.");
            return;
        }

        if (args[0].ToLowerInvariant() is "clear" or "null")
        {
            Game1.player.Set_Ultimate(null);
            return;
        }

        if (!Game1.player.professions.Any(p => p is >= 26 and < 30))
        {
            Log.W("You don't have any 2nd-tier combat professions.");
            return;
        }

        Profession? profession = null;
        if (!Ultimate.TryFromName(args[0], true, out var ultimate) &&
            (!Profession.TryFromLocalizedName(args[0], true, out profession) ||
             !Ultimate.TryFromValue(profession, out ultimate)))
        {
            Log.W("You must enter a valid 2nd-tier combat profession or special ability name.");
            return;
        }

        if (!Game1.player.HasProfession(profession ?? Profession.FromValue(ultimate)))
        {
            Log.W("You don't have this profession.");
            return;
        }

        Game1.player.Set_Ultimate(ultimate);
    }
}
