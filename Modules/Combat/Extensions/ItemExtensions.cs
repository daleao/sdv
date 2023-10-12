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
    internal static Color GetTitleColorFor(Item? item)
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
