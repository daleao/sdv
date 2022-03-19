namespace DaLion.Stardew.Arsenal;

/// <summary>The mod user-defined settings.</summary>
public class ModConfig
{
    #region weapon and ring changes

    /// <summary>Make weapons more unique and useful.</summary>
    public bool RebalanceWeapons { get; set; } = false;
    
    /// <summary>Improves certain underwhelming forges.</summary>
    public bool RebalanceForges { get; set; } = true;

    /// <summary>Improves certain underwhelming rings.</summary>
    public bool RebalanceRings { get; set; } = true;

    /// <summary>Replaces the crafting recipe for Glowstone Ring, adds new recipes for Magnet Ring, Glow Ring and gemstone rings.</summary>
    public bool ImmersiveRingRecipes { get; set; } = true;

    /// <summary>Weapons should cost energy to use.</summary>
    public bool WeaponsCostStamina { get; set; } = true;

    /// <summary>Projectiles should not be useless for the first 100ms.</summary>
    public bool RemoveSlingshotGracePeriod { get; set; } = true;

    /// <summary>Damage mitigation should not be soft-capped at 50%.</summary>
    public bool RemoveDefenseSoftCap { get; set; } = true;

    #endregion weapon and ring changes

    #region foraging and farming changes
    
    /// <summary>Tree sap quality should improve as the tree ages.</summary>
    public bool AgeTapperTrees { get; set; } = true;

    /// <summary>If crab pots reward experience, so should tappers.</summary>
    public bool TappersRewardExp { get; set; } = true;

    /// <summary>Honey quality should improve as the hive gets older.</summary>
    public bool AgeBeeHouses { get; set; } = true;

    /// <summary>If wild forage rewards experience, berry bushes should qualify.</summary>
    public bool BerryBushesRewardExp { get; set; } = true;

    /// <summary>If regular trees can't grow in winter, neither should fruit trees.</summary>
    public bool PreventFruitTreeGrowthInWinter { get; set; } = true;

    /// <summary>Mead should take after Honey type.</summary>
    public bool KegsRememberHoneyFlower { get; set; } = true;

    /// <summary>Large input products should yield more processed output instead of higher quality.</summary>
    public bool LargeProducsYieldQuantityOverQuality { get; set; } = true;

    #endregion foraging and farming changes

    #region integrations for other mods

    /// <summary>The visual style for different honey mead icons, if using BetterArtisanGoodIcons. Allowed values: 'ColoredBottles', 'ColoredCaps'.</summary>
    public string HoneyMeadStyle { get; set; } = "ColoredBottles";

    #endregion integrations for other mods
}