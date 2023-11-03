namespace DaLion.Overhaul.Modules.Combat.Commands;

#region using directives

using DaLion.Overhaul.Modules.Combat.Enums;
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
        var action = trigger == "randomize" ? "Randomized" : "Revalidated";
        if (args.Length > 0)
        {
            var all = args[0] is "-a" or "--all";
            if (all)
            {
                CombatModule.RevalidateAllWeapons(trigger == "randomize" ? WeaponRefreshOption.Randomized : WeaponRefreshOption.Initial);
                MeleeWeapon_Stats.Values.Clear();
                Log.I($"{action} all weapons.");
                return;
            }

            Log.W($"Invalid argument {args[0]} will be ignored.");
        }

        if (Game1.player.CurrentTool is not MeleeWeapon weapon || weapon.isScythe())
        {
            Log.W("You must select a weapon first.");
            return;
        }

        CombatModule.RevalidateSingleWeapon(weapon, trigger == "randomize" ? WeaponRefreshOption.Randomized : WeaponRefreshOption.Initial);
        Log.I($"{action} the {weapon.Name}.");
    }
}
