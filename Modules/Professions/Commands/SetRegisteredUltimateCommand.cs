namespace DaLion.Overhaul.Modules.Professions.Commands;

#region using directives

using System.Linq;
using DaLion.Overhaul.Modules.Professions.Extensions;
using DaLion.Overhaul.Modules.Professions.Ultimates;
using DaLion.Overhaul.Modules.Professions.VirtualProperties;
using DaLion.Shared.Commands;
using Shared.Extensions;

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
    public override string[] Triggers { get; } = { "register_ult", "set_ult", "register" };

    /// <inheritdoc />
    public override string Documentation => "Change the player's currently registered Limit Break.";

    /// <inheritdoc />
    public override void Callback(string trigger, string[] args)
    {
        if (args.Length == 0 || string.IsNullOrEmpty(args[0]))
        {
            Log.W("You must enter a valid 2nd-tier combat profession or Limit Break.");
            return;
        }

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

        if (!Game1.player.professions.Any(p => p.IsIn(26..29)))
        {
            Log.W("You don't have any 2nd-tier combat professions.");
            return;
        }

        if (!Ultimate.TryFromName(args[0], true, out var ultimate) &&
            !(int.TryParse(args[0], out var index) && Ultimate.TryFromValue(index, out ultimate)) &&
            !((Profession.TryFromName(args[0], true, out var profession) ||
               Profession.TryFromLocalizedName(args[0], true, out profession)) &&
              Ultimate.TryFromValue(profession, out ultimate)))
        {
            Log.W("You must enter a valid 2nd-tier combat profession or Limit Break.");
            return;
        }

        if (!Game1.player.HasProfession(ultimate.Profession))
        {
            Log.W("You don't have this profession.");
            return;
        }

        Game1.player.Set_Ultimate(ultimate);
    }
}
