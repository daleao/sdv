namespace DaLion.Stardew.Tweex;

/// <summary>The mod user-defined settings.</summary>
public class ModConfig
{
    /// <summary>Honey quality should improve as the hive gets older.</summary>
    public bool AgeBeeHouses { get; set; } = true;

    /// <summary>Mushroom quality should improve as the boxes get older.</summary>
    public bool AgeMushroomBoxes { get; set; } = true;

    /// <summary>Tree sap quality should improve as the tree ages.</summary>
    public bool AgeSapTrees { get; set; } = true;

    /// <summary>Increases or decreases the default age threshold for quality increase for Bee Houses, Trees and Fruit Trees.</summary>
    public float AgeImproveQualityFactor { get; set; } = 1f;

    /// <summary>Whether age-dependent qualities should be deterministic (true) or stochastic/random (false).</summary>
    public bool DeterministicAgeQuality { get; set; } = true;

    /// <summary>If wild forage rewards experience, berry bushes should qualify.</summary>
    public bool BerryBushesRewardExp { get; set; } = true;

    /// <summary>If fruit bat cave rewards experience, so should mushroom cave.</summary>
    public bool MushroomBoxesRewardExp { get; set; } = true;

    /// <summary>If crab pots reward experience, so should tappers.</summary>
    public bool TappersRewardExp { get; set; } = true;

    /// <summary>If regular trees can't grow in winter, neither should fruit trees.</summary>
    public bool PreventFruitTreeGrowthInWinter { get; set; } = true;

    /// <summary>Large input products should yield more processed output instead of higher quality.</summary>
    public bool LargeProducsYieldQuantityOverQuality { get; set; } = true;

    /// <summary>Extends the perks from Botanist/Ecologist profession to Ginger Island dug-up ginger and shaken-off coconuts.</summary>
    public bool ProfessionalForagingInGingerIsland { get; set; } = true;

    /// <summary>Mead should take after Honey type.</summary>
    public bool KegsRememberHoneyFlower { get; set; } = true;

    /// <summary>Bombs within any explosion radius are immediately triggered.</summary>
    public bool ExplosionTriggeredBombs { get; set; } = true;
}