#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable SA1600 // Elements should be documented
namespace DaLion.Overhaul.Modules.Combat;

/// <summary>Holds the string keys of mod data fields used by <see cref="OverhaulModule.Combat"/>.</summary>
internal sealed class DataKeys
{
    // weapon
    internal const string CursePoints = "CursePoints";
    internal const string BaseMinDamage = "BaseMinDamage";
    internal const string BaseMaxDamage = "BaseMaxDamage";
    internal const string SwordType = "SwordType";

    // farmer
    internal const string GalaxyArsenalObtained = "GalaxyArsenalObtained";
    internal const string BlueprintsFound = "BlueprintsFound";
    internal const string DaysLeftTranslating = "DaysLeftTranslating";

    internal const string InspectedHonor = "InspectedHonor";
    internal const string InspectedCompassion = "InspectedCompassion";
    internal const string InspectedWisdom = "InspectedWisdom";
    internal const string InspectedGenerosity = "InspectedGenerosity";
    internal const string InspectedValor = "InspectedValor";
    //internal const string ProvenHonor = "ProvenHonor"; // max 8 points (-4)
    //internal const string ProvenCompassion = "ProvenCompassion"; // max 9 points (-3)
    //internal const string ProvenWisdom = "ProvenWisdom"; // max 8 points (-1)
    //internal const string ProvenGenerosity = "ProvenGenerosity"; // awarded gifting or purchasing community upgrades
    //internal const string ProvenValor = "ProvenValor"; // awarded for slaying monsters
    internal const string VirtueQuestState = "VirtueQuestState";
    internal const string VirtueQuestViewed = "VirtueQuestViewed";
    internal const string HasUsedMayorShorts = "HasUsedMayorShorts";
    internal const string NumCompletedSlayerQuests = "NumCompletedSlayerQuests";

    internal const string SelectableMelee = "AutoSelectableMelee";
    internal const string SelectableRanged = "AutoSelectableRanged";

    internal const string MonsterDropAccumulator = "MonsterDropAccumulator";
    internal const string ContainerDropAccumulator = "ContainerDropAccumulator";
}
#pragma warning restore SA1600 // Elements should be documented
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
