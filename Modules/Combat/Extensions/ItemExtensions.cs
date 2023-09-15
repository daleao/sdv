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
                    var tier = WeaponTier.GetFor(weapon);
                    if (tier == WeaponTier.Untiered)
                    {
                        return Game1.textColor;
                    }

                    if (tier < WeaponTier.Legendary)
                    {
                        return tier.Color;
                    }

                    if (weapon.isGalaxyWeapon())
                    {
                        return Color.DarkViolet;
                    }

                    if (weapon.IsInfinityWeapon())
                    {
                        return Color.DeepPink;
                    }

                    switch (weapon.InitialParentTileIndex)
                    {
                        case WeaponIds.DarkSword:
                            return Color.DarkSlateGray;
                        case WeaponIds.HolyBlade:
                            return Color.Gold;
                    }

                    break;
                }

                case Slingshot slingshot:
                    switch (slingshot.InitialParentTileIndex)
                    {
                        case WeaponIds.GalaxySlingshot:
                            return Color.DarkViolet;
                        case WeaponIds.InfinitySlingshot:
                            return Color.DeepPink;
                        default:
                            if (slingshot.Name.Contains("Copper"))
                            {
                                return UpgradeLevel.Copper.GetTextColor();
                            }

                            if (slingshot.Name.Contains("Steel"))
                            {
                                return UpgradeLevel.Steel.GetTextColor();
                            }

                            if (slingshot.Name.Contains("Gold"))
                            {
                                return UpgradeLevel.Gold.GetTextColor();
                            }

                            if (slingshot.Name.Contains("Iridium"))
                            {
                                return UpgradeLevel.Iridium.GetTextColor();
                            }

                            if (slingshot.Name.Contains("Yoba"))
                            {
                                return Color.Gold;
                            }

                            if (slingshot.Name.Contains("Dwarven"))
                            {
                                return Color.MonoGameOrange;
                            }

                            break;
                    }

                    break;
            }
        }
        else if (tool.UpgradeLevel > 0 && ToolsModule.ShouldEnable && ToolsModule.Config.ColorCodedForYourConvenience)
        {
            if (tool is FishingRod)
            {
                if (tool.UpgradeLevel > 2)
                {
                    return Color.Violet.ChangeValue(0.5f);
                }
            }

            return ((UpgradeLevel)tool.UpgradeLevel).GetTextColor();
        }

        return Game1.textColor;
    }
}
