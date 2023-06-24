namespace DaLion.Overhaul.Modules.Weapons.Extensions;

#region using directives

using DaLion.Overhaul.Modules.Tools;
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

        if (tool is MeleeWeapon && WeaponsModule.Config.ColorCodedForYourConvenience)
        {
            var tier = WeaponTier.GetFor(tool);
            if (tier == WeaponTier.Untiered)
            {
                return Game1.textColor;
            }

            if (tier < WeaponTier.Legendary)
            {
                return tier.Color;
            }

            if (tool is MeleeWeapon weapon)
            {
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
                    case ItemIDs.DarkSword:
                        return Color.DarkSlateGray;
                    case ItemIDs.HolyBlade:
                        return Color.Gold;
                }
            }
        }
        else if (tool is Slingshot slingshot && SlingshotsModule.ShouldEnable && SlingshotsModule.Config.ColorCodedForYourConvenience)
        {
            switch (slingshot.InitialParentTileIndex)
            {
                case ItemIDs.GalaxySlingshot:
                    return Color.DarkViolet;
                case ItemIDs.InfinitySlingshot:
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
