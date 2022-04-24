namespace DaLion.Stardew.Prairie.Training.Framework;

#region using directives

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using SharpNeat.Core;
using SharpNeat.Decoders;
using SharpNeat.Decoders.Neat;
using SharpNeat.DistanceMetrics;
using SharpNeat.EvolutionAlgorithms;
using SharpNeat.EvolutionAlgorithms.ComplexityRegulation;
using SharpNeat.Genomes.Neat;
using SharpNeat.Phenomes;
using SharpNeat.SpeciationStrategies;

#endregion using directives

/// <summary>Hides most of the details of setting up an experiment.</summary>
internal static class NeatExperiment
{
    private static Thread Thread { get; set; }

    internal static int InputCount { get; private set; }
    internal static int OutputCount { get; private set; }
    internal static int PopulationSize { get; private set; }
    internal static int SpecieCount { get; private set; }
    
    internal static List<NeatGenome> GenomeList { get; private set; }
    internal static NeatGenomeFactory GenomeFactory { get; private set; }
    internal static NeatGenomeParameters GenomeParams { get; private set; }

    internal static IEvolutionAlgorithm<NeatGenome> EvolutionAlgorithm { get; private set; }
    internal static NeatEvolutionAlgorithmParameters EvolutionAlgorithmParams { get; private set; }

    internal static IComplexityRegulationStrategy ComplexityRegulationStrategy { get; private set; }
    internal static ISpeciationStrategy<NeatGenome> SpeciationStrategy { get; private set; }
    
    internal static NetworkActivationScheme ActivationScheme { get; private set; }
    internal static IGenomeDecoder<NeatGenome, IBlackBox> GenomeDecoder { get; private set; }

    internal static ParallelOptions ParallelOptions { get; private set; }
    internal static ParallelGenomeListEvaluator<NeatGenome, IBlackBox> GenomeListEvaluator { get; private set; }

    internal static IPhenomeEvaluator<IBlackBox> PhenomeEvaluator { get; private set; }

    /// <summary>Initialize the experiment with parameters specified by <see cref="ModConfig"/>..</summary>
    internal static void Initialize()
    {
        Log.D("Initializing network...");

        InputCount = Constants.NUM_INPUTS_I;
        OutputCount = Constants.NUM_OUTPUTS_I;

        PopulationSize = ModEntry.Config.PopulationSize;
        SpecieCount = ModEntry.Config.SpecieCount;

        GenomeFactory = new(InputCount, OutputCount);
        GenomeList = GenomeFactory.CreateGenomeList(SpecieCount, 0);
        GenomeParams = new();

        EvolutionAlgorithmParams = new() {SpecieCount = SpecieCount};
        
        var speciationStr = ModEntry.Config.SpeciationStrategy;
        var distanceMetricStr = ModEntry.Config.DistanceMetric;
        IDistanceMetric distanceMetric = distanceMetricStr switch
        {
            "euclidean" => new EuclideanDistanceMetric(),
            "manhattan" => new ManhattanDistanceMetric(),
            _ => throw new ArgumentOutOfRangeException($"{distanceMetricStr} is not a valid distance metric.")
        };

        SpeciationStrategy = speciationStr switch
        {
            "kmeans" => new KMeansClusteringStrategy<NeatGenome>(distanceMetric),
            "parallel_kmeans" => new ParallelKMeansClusteringStrategy<NeatGenome>(distanceMetric),
            "random" => new RandomClusteringStrategy<NeatGenome>(),
            _ => throw new ArgumentOutOfRangeException($"{speciationStr} is not a valid speciation strategy.")
        };

        var complexityRegulationStr = ModEntry.Config.ComplexityRegulationStrategy;
        var complexityCeilingType = Enum.Parse<ComplexityCeilingType>(ModEntry.Config.ComplexityCeilingType, true);
        var complexityCeilingValue = ModEntry.Config.ComplexityCeilingValue;
        ComplexityRegulationStrategy = complexityRegulationStr switch
        {
            "default" => new DefaultComplexityRegulationStrategy(complexityCeilingType, complexityCeilingValue),
            "null" => new NullComplexityRegulationStrategy(),
            _ => throw new ArgumentOutOfRangeException($"{complexityRegulationStr} is not a valid complexity regulation strategy.")
        };

        EvolutionAlgorithm = new NeatEvolutionAlgorithm<NeatGenome>(EvolutionAlgorithmParams, SpeciationStrategy,
            ComplexityRegulationStrategy);

        var activationSchemeStr = ModEntry.Config.ActivationScheme;
        var timestepsPerActivation = ModEntry.Config.TimestepsPerActivation;
        var signalDeltaThreshold = ModEntry.Config.SignalDeltaThreshold;
        var maxTimesteps = ModEntry.Config.MaxTimesteps;
        ActivationScheme = activationSchemeStr switch
        {
            "cyclic_fixed" => NetworkActivationScheme.CreateCyclicFixedTimestepsScheme(timestepsPerActivation),
            "cyclic_relaxing" => NetworkActivationScheme.CreateCyclicRelaxingActivationScheme(signalDeltaThreshold,
                maxTimesteps),
            "acyclic" => NetworkActivationScheme.CreateAcyclicScheme(),
            _ => throw new ArgumentOutOfRangeException($"{activationSchemeStr} is not a valid activation scheme.")
        };

        GenomeDecoder = new NeatGenomeDecoder(ActivationScheme);
        PhenomeEvaluator = new PhenomeEvaluator(ModEntry.Config.KillWeight, ModEntry.Config.CoinWeight,
            ModEntry.Config.DeathWeight, ModEntry.Config.ActivationThreshold);

        var maxDegreeOfParallelism = ModEntry.Config.MaxDegreeOfParallelism;
        ParallelOptions = maxDegreeOfParallelism > 0 ? new() {MaxDegreeOfParallelism = maxDegreeOfParallelism} : null;
        GenomeListEvaluator = ParallelOptions?.MaxDegreeOfParallelism > 0
            ? new(GenomeDecoder, PhenomeEvaluator, ParallelOptions)
            : new(GenomeDecoder, PhenomeEvaluator);

        var updateScheme = ModEntry.Config.GenerationsPerLog;
        EvolutionAlgorithm.UpdateScheme = new(updateScheme);

        Thread = new Thread(() => EvolutionAlgorithm.Initialize(GenomeListEvaluator, GenomeFactory, GenomeList));
        Log.D("Initialization complete.");
    }

    /// <summary>Start the experiment in a child thread.</summary>
    internal static void Start()
    {
        Thread.Start();
    }

    /// <summary>Load a population of genomes from an <see cref="XmlReader"/>.</summary>
    internal static void LoadPopulation(XmlReader xr, bool nodeFnIds)
    {
        var genomeList = NeatGenomeXmlIO.ReadCompleteGenomeList(xr, nodeFnIds, GenomeFactory);
        var speciesCount = genomeList.Count;
        for (var i = speciesCount - 1; speciesCount > -1; --speciesCount)
        {
            var genome = genomeList[i];
            if (genome.InputNodeCount != InputCount)
            {
                Log.E(
                    $"Loaded genome has wrong number of inputs for current experiment. Has {genome.InputNodeCount}, expected {InputCount}.");
                genomeList.RemoveAt(i);
            }
            else if (genome.OutputNodeCount != OutputCount)
            {
                Log.E(
                    $"Loaded genome has wrong number of outputs for current experiment. Has {genome.OutputNodeCount}, expected {OutputCount}.");
                genomeList.RemoveAt(i);
            }
        }

        GenomeList = genomeList;
    }

    /// <summary>Save a population of genomes to an <see cref="XmlWriter"/>.</summary>
    internal static void SavePopulation(XmlWriter xw)
    {
        NeatGenomeXmlIO.WriteComplete(xw, GenomeList, false);
    }
}
