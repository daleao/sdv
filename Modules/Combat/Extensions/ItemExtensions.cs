namespace DaLion.Overhaul.Modules.Combat.Extensions;

#region using directives

using DaLion.Overhaul.Modules.Combat.Enums;
using DaLion.Overhaul.Modules.Tools;
using DaLion.Shared.Constants;
using DaLion.Shared.Extensions.Xna;
using Microsoft.Xna.Framework;
using StardewValley.Tools;

#endregion using directives

/// <summary>Extensions for the <see cref="Item"/> class.</summary>
internal static class ItemExtensions
{
    internal static int GetAmmoDamage(this Item item)
    {
        if (item is not SObject)
        {
            return 0;
        }

        switch (item.ParentSheetIndex)
        {
            case ObjectIds.Wood:
                return 2;
            case ObjectIds.Coal:
                return CombatModule.Config.EnableWeaponOverhaul ? 2 : 15;
            case ObjectIds.ExplosiveAmmo:
                return CombatModule.Config.EnableWeaponOverhaul ? 1 : 20;
            case ObjectIds.Stone:
                return 5;
            case ObjectIds.CopperOre:
                return 10;
            case ObjectIds.IronOre:
                return 20;
            case ObjectIds.GoldOre:
                return 30;
            case ObjectIds.IridiumOre:
                return 50;
            case ObjectIds.RadioactiveOre:
                return 80;
            case ObjectIds.Slime:
                return Game1.player.professions.Contains(Farmer.acrobat) ? 10 : 1;
            case ObjectIds.Emerald:
            case ObjectIds.Aquamarine:
            case ObjectIds.Ruby:
            case ObjectIds.Amethyst:
            case ObjectIds.Topaz:
            case ObjectIds.Jade:
                return 40;
            case ObjectIds.Diamond:
                return 100;
            case ObjectIds.PrismaticShard:
                return 60;
            default: // fish, fruit or vegetable
                return 1;
        }
    }

    internal static Color GetTitleColorFor(this Item item)
    {
        if (item is not Tool tool)
        {
            return Game1.textColor;
        }

        if (CombatModule.Config.ColorCodedForYourConvenience)
        {
            switch (tool)
            {
                case MeleeWeapon weapon:
                {
                    return WeaponTier.GetFor(weapon).Color;
                }

                case Slingshot slingshot:
                    switch (slingshot.InitialParentTileIndex)
                    {
                        case WeaponIds.GalaxySlingshot:
                        case WeaponIds.InfinitySlingshot:
                            return CombatModule.Config.LegendaryTierColor;
                        default:
                            if (slingshot.Name.Contains("Yoba"))
                            {
                                return CombatModule.Config.LegendaryTierColor;
                            }

                            if (slingshot.Name.Contains("Dwarven"))
                            {
                                return CombatModule.Config.MasterworkTierColor;
                            }

                            break;
                    }

                    break;
            }
        }
        else if (tool.UpgradeLevel > 0 && ToolsModule.ShouldEnable && ToolsModule.Config.ColorCodedForYourConvenience)
        {
            if (tool is not FishingRod)
            {
                return ((UpgradeLevel)tool.UpgradeLevel).GetTextColor();
            }

            return tool.UpgradeLevel > 2
                ? Color.Violet.ChangeValue(0.5f)
                : Game1.textColor;
        }

        return Game1.textColor;
    }
}
