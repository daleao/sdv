#region global using directives

#pragma warning disable SA1200 // Using directives should be placed correctly
global using static DaLion.Chargeable.ModEntry;
#pragma warning restore SA1200 // Using directives should be placed correctly

#endregion global using directives

namespace DaLion.Chargeable;

#region using directives

using System.Diagnostics.CodeAnalysis;
using System.Linq;
using DaLion.Chargeable.Framework;
using HarmonyLib;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley.Tools;

#endregion using directives

/// <summary>The mod entry point.</summary>
public sealed class ModEntry : Mod
{
    /// <summary>Gets the static <see cref="ModEntry"/> instance.</summary>
    internal static ModEntry Instance { get; private set; } = null!; // set in Entry

    /// <summary>Gets or sets the <see cref="ModConfig"/> instance.</summary>
    internal static ModConfig Config { get; set; } = null!; // set in Entry

    /// <summary>Gets the <see cref="PerScreen{T}"/> <see cref="ModState"/>.</summary>
    internal static PerScreen<ModState> PerScreenState { get; private set; } = null!; // set in Entry

    /// <summary>Gets or sets the <see cref="ModState"/> of the local player.</summary>
    internal static ModState State
    {
        get => PerScreenState.Value;
        set => PerScreenState.Value = value;
    }

    /// <summary>Gets the <see cref="IModHelper"/> API.</summary>
    internal static IModHelper ModHelper => Instance.Helper;

    /// <summary>The mod entry point, called after the mod is first loaded.</summary>
    /// <param name="helper">Provides simplified APIs for writing mods.</param>
    public override void Entry(IModHelper helper)
    {
        Instance = this;

        // initialize logger
        Log.Init(this.Monitor);

        // get configs
        Config = helper.ReadConfig<ModConfig>();
        Config.Validate(helper);

        // initialize mod state
        PerScreenState = new PerScreen<ModState>(() => new ModState());

        // hook events
        helper.Events.GameLoop.UpdateTicked += this.OnUpdateTicked;

        // apply patches
        new Harmony(this.ModManifest.UniqueID).PatchAll();

        // register console commands
        helper.ConsoleCommands.Add(
            "upgrade_tools",
            "Set the upgrade level of the currently held tool." + this.GetUsage(),
            this.UpgradeTools);
    }

    private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
    {
        if (State.Shockwaves.Count == 0 || (Config.TicksBetweenWaves > 1 && !e.IsMultipleOf(Config.TicksBetweenWaves)))
        {
            return;
        }

        var shockwaves = State.Shockwaves.ToList();
        shockwaves.ForEach(wave => wave.Update(Game1.currentGameTime.TotalGameTime.TotalMilliseconds));
    }

    [SuppressMessage("StyleCop.CSharp.SpacingRules", "SA1012:Opening braces should be spaced correctly", Justification = "Contradicting rules.")]
    private void UpgradeTools(string command, string[] args)
    {
        if (Game1.player.CurrentTool is not ({ } tool and (Axe or Hoe or Pickaxe or WateringCan or FishingRod)))
        {
            Log.W("You must select a tool first.");
            return;
        }

        if (args.Length < 1)
        {
            Log.W("You must specify a valid upgrade level." + this.GetUsage());
            return;
        }

        if (!Enum.TryParse<UpgradeLevel>(args[0], true, out var upgradeLevel))
        {
            Log.W($"Invalid upgrade level {args[0]}." + this.GetUsage());
            return;
        }

        if (upgradeLevel >= UpgradeLevel.Enchanted)
        {
            Log.W("To add enchantments use the `add_enchantment` command instead.");
            return;
        }

        tool.UpgradeLevel = (int)upgradeLevel;
    }

    private string GetUsage()
    {
        var result = $"\n\nUsage: upgrade_tools <level>";
        result += "\n\nParameters:";
        result += "\n\t- <level>: one of 'copper', 'steel', 'gold', 'iridium'";
        result += "\n\nExample:";
        result += $"\n\t- upgrade_tools iridium";
        return result;
    }
}
