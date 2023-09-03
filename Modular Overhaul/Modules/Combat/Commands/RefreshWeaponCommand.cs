namespace DaLion.Overhaul.Modules.Combat.Commands;

#region using directives

using DaLion.Overhaul.Modules.Combat.Enums;
using DaLion.Overhaul.Modules.Combat.Extensions;
using DaLion.Overhaul.Modules.Combat.VirtualProperties;
using DaLion.Shared.Commands;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class RefreshWeaponCommand : ConsoleCommand
{
    /// <summary>Initializes a new instance of the <see cref="RefreshWeaponCommand"/> class.</summary>
    /// <param name="handler">The <see cref="CommandHandler"/> instance that handles this command.</param>
    internal RefreshWeaponCommand(CommandHandler handler)
        : base(handler)
    {
    }

    /// <inheritdoc />
    public override string[] Triggers { get; } = { "revalidate", "refresh", "randomize" };

    /// <inheritdoc />
    public override string Documentation =>
        "Refreshes the stats of the currently selected weapon, randomizing if necessary.";

    /// <inheritdoc />
    public override void Callback(string trigger, string[] args)
    {
        if (args.Length == 0 || !string.Equals(args[0], "all", StringComparison.InvariantCultureIgnoreCase))
        {
            if (Game1.player.CurrentTool is not MeleeWeapon weapon || weapon.isScythe())
            {
                Log.W("You must select a weapon first.");
                return;
            }
        }

        if (string.Equals(trigger, "revalidate", StringComparison.InvariantCultureIgnoreCase))
        {
            if (args.Length > 0 && string.Equals(args[0], "all", StringComparison.InvariantCultureIgnoreCase))
            {
                CombatModule.RevalidateAllWeapons();
                MeleeWeapon_Stats.Values.Clear();
                Log.I("Revalidated all weapons.");
            }
            else if (Game1.player.CurrentTool is not MeleeWeapon weapon1 || weapon1.isScythe())
            {
                Log.W("You must select a weapon first.");
            }
            else
            {
                CombatModule.RevalidateSingleWeapon(weapon1);
                weapon1.Invalidate();
                Log.I($"Revalidated the {weapon1.Name}.");
            }

            return;
        }

        var randomize = string.Equals(trigger, "randomize", StringComparison.InvariantCultureIgnoreCase);
        var action = randomize ? "Randomized" : "Refreshed";
        if (args.Length > 0 && args[0].ToLowerInvariant() == "all")
        {
            CombatModule.RefreshAllWeapons(randomize ? WeaponRefreshOption.Randomized : WeaponRefreshOption.Initial);
            Log.I($"{action} all weapons.");
        }

        if (Game1.player.CurrentTool is not MeleeWeapon weapon2 || weapon2.isScythe())
        {
            Log.W("You must select a weapon first.");
            return;
        }

        weapon2.RefreshStats(randomize ? WeaponRefreshOption.Randomized : WeaponRefreshOption.Initial);
        weapon2.Invalidate();
        Log.I($"{action} the {weapon2.Name}.");
    }
}
