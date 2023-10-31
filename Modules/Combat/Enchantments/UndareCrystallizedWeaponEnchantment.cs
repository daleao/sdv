namespace DaLion.Overhaul.Modules.Combat.Enchantments;

#region using directives

using System.Xml.Serialization;
using DaLion.Overhaul.Modules.Combat.Integrations;
using DaLion.Shared.Constants;
using StardewValley.Monsters;

#endregion using directives

/// <summary>The secondary <see cref="BaseWeaponEnchantment"/> which characterizes the Lava Katana.</summary>
[XmlType("Mods_DaLion_UndareCrystallizedWeaponEnchantment")]
public sealed class UndareCrystallizedWeaponEnchantment : BaseWeaponEnchantment
{
    /// <inheritdoc />
    public override bool IsSecondaryEnchantment()
    {
        return true;
    }

    /// <inheritdoc />
    public override bool IsForge()
    {
        return false;
    }

    /// <inheritdoc />
    public override int GetMaximumLevel()
    {
        return 1;
    }

    /// <inheritdoc />
    public override bool ShouldBeDisplayed()
    {
        return false;
    }

    protected override void _OnMonsterSlay(Monster m, GameLocation location, Farmer who)
    {
        base._OnMonsterSlay(m, location, who);
        int seedIndex, gemSeedIndex;
        switch (who.CurrentTool?.Name)
        {
            case "Blueglazer":
                seedIndex = ObjectIds.BlueberrySeeds;
                gemSeedIndex = JsonAssetsIntegration.Instance?.ModApi?.GetObjectId("Crystallized Blueberry Seeds") ?? -1;
                break;
            case "Crystallight":
                seedIndex = ObjectIds.RareSeed;
                gemSeedIndex = JsonAssetsIntegration.Instance?.ModApi?.GetObjectId("Sweet Crystallized Gem Berry Seeds") ?? -1;
                break;
            case "Grapemaul":
                seedIndex = ObjectIds.GrapeStarter;
                gemSeedIndex = JsonAssetsIntegration.Instance?.ModApi?.GetObjectId("Crystallized Grape Starter") ?? -1;
                break;
            case "Heartichoker":
                seedIndex = ObjectIds.ArtichokeSeeds;
                gemSeedIndex = JsonAssetsIntegration.Instance?.ModApi?.GetObjectId("Crystallized Artichoke Seeds") ?? -1;
                break;
            case "Strawblaster":
                seedIndex = ObjectIds.StrawberrySeeds;
                gemSeedIndex = JsonAssetsIntegration.Instance?.ModApi?.GetObjectId("Crystallized Strawberry Seeds") ?? -1;
                break;
            case "Sunspark":
                seedIndex = ObjectIds.SunflowerSeeds;
                gemSeedIndex = JsonAssetsIntegration.Instance?.ModApi?.GetObjectId("Crystallized Sunflower Seeds") ?? -1;
                break;
            default:
                seedIndex = -1;
                gemSeedIndex = -1;
                break;
        }

        if (seedIndex != -1 && Game1.random.NextDouble() < 0.15)
        {
            Game1.createItemDebris(
                new SObject(seedIndex, 1),
                m.getTileLocation(),
                -1,
                location);
        }

        if (gemSeedIndex != -1 && Game1.random.NextDouble() < 0.05)
        {
            Game1.createItemDebris(
                new SObject(seedIndex, 1),
                m.getTileLocation(),
                -1,
                location);
        }
    }
}
