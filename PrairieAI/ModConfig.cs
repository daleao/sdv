namespace DaLion.Stardew.Prairie.Training;

#region using directives

using StardewModdingAPI.Utilities;

#endregion using directives

/// <summary>The mod user-defined settings.</summary>
public class ModConfig
{
    /// <summary>Key used by advance the stage.</summary>
    public KeybindList DebugKey { get; set; } = KeybindList.Parse("LeftControl");

    public int PopulationSize { get; set; } = 100;
    public int SpecieCount { get; set; } = 100;
    public string SpeciationStrategy { get; set; } = "kmeans";
    public string DistanceMetric { get; set; } = "manhattan";
    public string ComplexityRegulationStrategy { get; set; } = "default";
    public string ComplexityCeilingType { get; set; } = "absolute";
    public double ComplexityCeilingValue { get; set; } = 100.0;
    public string ActivationScheme { get; set; } = "cyclic_fixed";
    public int TimestepsPerActivation { get; set; } = 1;
    public double SignalDeltaThreshold { get; set; } = 1.0;
    public int MaxTimesteps { get; set; } = 100;
    public int MaxDegreeOfParallelism { get; set; } = 0;
    public uint GenerationsPerLog { get; set;  } = 1;
    public int KillWeight { get; set; } = 1;
    public int CoinWeight { get; set; } = 5;
    public int DeathWeight { get; set; } = 20;
    public double ActivationThreshold { get; set; } = 0.85;
}