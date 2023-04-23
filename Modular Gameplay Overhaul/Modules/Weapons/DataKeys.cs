#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable SA1600 // Elements should be documented
namespace DaLion.Overhaul.Modules.Weapons;

/// <summary>Holds the string keys of mod data fields used by <see cref="OverhaulModule.Weapons"/>.</summary>
internal sealed class DataKeys
{
    // weapon
    internal const string CursePoints = "CursePoints";
    internal const string BaseMinDamage = "BaseMinDamage";
    internal const string BaseMaxDamage = "BaseMaxDamage";

    // farmer
    internal const string GalaxyArsenalObtained = "GalaxyArsenalObtained";
    internal const string BlueprintsFound = "BlueprintsFound";
    internal const string DaysLeftTranslating = "DaysLeftTranslating";
    internal const string HasReadHonor = "HasReadHonor";
    internal const string HasReadCompassion = "HasReadCompassion";
    internal const string HasReadWisdom = "HasReadWisdom";
    internal const string HasReadGenerosity = "HasReadGenerosity";
    internal const string HasReadValor = "HasReadValor";
    internal const string ProvenHonor = "ProvenHonor"; // max 8 points (-4)
    internal const string ProvenCompassion = "ProvenCompassion"; // max 9 points (-3)
    internal const string ProvenWisdom = "ProvenWisdom"; // max 8 points (-1)
    internal const string ProvenGenerosity = "ProvenGenerosity"; // awarded before the mail flag `pamHouseUpgrade`
    internal const string ProvenValor = "ProvenValor"; // awarded after 5th monster slayer quest completion

    internal const string SelectableWeapon = "AutoSelectableWeapon";

    internal const string MonsterDropAccumulator = "MonsterDropAccumulator";
    internal const string ContainerDropAccumulator = "ContainerDropAccumulator";
}
#pragma warning restore SA1600 // Elements should be documented
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
