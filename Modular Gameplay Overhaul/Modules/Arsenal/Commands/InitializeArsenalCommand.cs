namespace DaLion.Overhaul.Modules.Arsenal.Commands;

#region using directives

using System.Linq;
using DaLion.Overhaul.Modules.Arsenal.Extensions;
using DaLion.Shared.Commands;
using DaLion.Shared.Extensions.Stardew;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class InitializeArsenalCommand : ConsoleCommand
{
    /// <summary>Initializes a new instance of the <see cref="InitializeArsenalCommand"/> class.</summary>
    /// <param name="handler">The <see cref="CommandHandler"/> instance that handles this command.</param>
    internal InitializeArsenalCommand(CommandHandler handler)
        : base(handler)
    {
    }

    /// <inheritdoc />
    public override string[] Triggers { get; } = { "initialize", "init" };

    /// <inheritdoc />
    public override string Documentation =>
        "Applies necessary changes to make existing saves work with the Arsenal module. You should need to run this command on new saves.";

    /// <inheritdoc />
    public override void Callback(string[] args)
    {
        if (Context.IsMainPlayer)
        {
            Utility.iterateAllItems(item =>
            {
                if (item is not MeleeWeapon weapon || weapon.isScythe())
                {
                    return;
                }

                weapon.AddIntrinsicEnchantments();

                if (Collections.StabbingSwords.Contains(weapon.InitialParentTileIndex))
                {
                    weapon.type.Value = MeleeWeapon.stabbingSword;
                }

                if (ArsenalModule.Config.Weapons.RebalancedStats)
                {
                    weapon.RefreshStats();
                }
            });
        }
        else
        {
            foreach (var weapon in Game1.player.Items.OfType<MeleeWeapon>())
            {
                if (weapon.isScythe())
                {
                    continue;
                }

                if (ArsenalModule.Config.InfinityPlusOne)
                {
                    weapon.AddIntrinsicEnchantments();
                }

                if (Collections.StabbingSwords.Contains(weapon.InitialParentTileIndex))
                {
                    weapon.type.Value = MeleeWeapon.stabbingSword;
                }

                if (ArsenalModule.Config.Weapons.RebalancedStats)
                {
                    weapon.RefreshStats();
                }
            }
        }

        if (Game1.player.hasOrWillReceiveMail("galaxySword"))
        {
            Game1.player.WriteIfNotExists(DataFields.GalaxyArsenalObtained, Constants.GalaxySwordIndex.ToString());
        }

        Log.I("Arsenal is now initialized.");
    }
}
