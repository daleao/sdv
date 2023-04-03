namespace DaLion.Overhaul.Modules.Weapons.Commands;

#region using directives

using DaLion.Overhaul.Modules.Weapons.Extensions;
using DaLion.Overhaul.Modules.Weapons.VirtualProperties;
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
    public override string[] Triggers { get; } = { "refresh_weapon", "refresh" };

    /// <inheritdoc />
    public override string Documentation =>
        "Refreshes the stats of the currently selected weapon, randomizing if necessary.";

    /// <inheritdoc />
    public override void Callback(string trigger, string[] args)
    {
        if (Game1.player.CurrentTool is not MeleeWeapon weapon || weapon.isScythe())
        {
            Log.W("You must select a weapon first.");
            return;
        }

        var bias = 0d;
        if (args.Length > 0 && double.TryParse(args[0], out bias))
        {
        }

        weapon.RandomizeDamage(bias);
        MeleeWeapon_Stats.Invalidate(weapon);
        Log.I($"Refreshed the stats of {weapon.Name}.");
    }
}
