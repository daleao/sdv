using System.Collections.Generic;

namespace DaLion.Stardew.Prairie.Training;

#region using directives

using System;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Minigames;
using SharpNeat.Core;
using SharpNeat.Genomes.Neat;

using Framework;
using Framework.Events;

#endregion using directives

/// <summary>The mod entry point.</summary>
public class ModEntry : Mod
{
    internal static ModConfig Config { get; private set; }
    internal static IEvolutionAlgorithm<NeatGenome> Network => NeatExperiment.EvolutionAlgorithm;

    internal static IModHelper ModHelper { get; private set; }
    internal static IManifest Manifest { get; private set; }
    internal static Action<string, LogLevel> Log { get; private set; }

    internal static Input[,] Inputs { get; } = new Input[16,16];
    internal static Dictionary<SButton, bool> Actions { get; } = new()
    {
        {SButton.W, false},
        {SButton.A, false},
        {SButton.S, false},
        {SButton.D, false},
        {SButton.Up, false},
        {SButton.Left, false},
        {SButton.Down, false},
        {SButton.Right, false}
    };

    internal static AbigailGame GameInstance => (AbigailGame) Game1.currentMinigame;
    internal static bool IsPlayingAbigailGame => Game1.currentMinigame?.minigameId() == "PrairieKing";
    internal static uint CoinsCollected { get; set; }
    internal static uint EnemiesDefeated { get; set; }
    internal static uint DeathCount { get; set; }

    /// <summary>The mod entry point, called after the mod is first loaded.</summary>
    /// <param name="helper">Provides simplified APIs for writing mods.</param>
    public override void Entry(IModHelper helper)
    {
        // store references to helper, mod manifest and logger
        ModHelper = helper;
        Manifest = ModManifest;
        Log = Monitor.Log;

        // get configs
        Config = helper.ReadConfig<ModConfig>();

        // hook events
        IEvent.HookAll();

        // add debug commands
        helper.ConsoleCommands.Register();
    }
}