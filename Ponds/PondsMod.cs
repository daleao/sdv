global using DaLion.Ponds.Framework;
global using DaLion.Ponds.Framework.Extensions;
global using DaLion.Shared.Constants;
global using static DaLion.Ponds.PondsMod;

namespace DaLion.Ponds;

#region using directives

using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using DaLion.Shared.Commands;
using DaLion.Shared.Data;
using DaLion.Shared.Extensions.SMAPI;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using DaLion.Shared.Networking;
using Newtonsoft.Json;
using StardewModdingAPI.Events;
using StardewValley.Buildings;
using StardewValley.GameData.FishPonds;

#endregion using directives

/// <summary>The mod entry point.</summary>
public sealed class PondsMod : Mod
{
    /// <summary>Gets the static <see cref="PondsMod"/> instance.</summary>
    internal static PondsMod Instance { get; private set; } = null!; // set in Entry

    /// <summary>Gets or sets the <see cref="PondsConfig"/> instance.</summary>
    internal static PondsConfig Config { get; set; } = null!; // set in Entry

    /// <summary>Gets the <see cref="ModDataManager"/> instance.</summary>
    internal static ModDataManager Data { get; private set; } = null!; // set in Entry

    /// <summary>Gets the <see cref="Broadcaster"/> instance.</summary>
    internal static Broadcaster Broadcaster { get; private set; } = null!; // set in Entry

    /// <summary>Gets the <see cref="Logger"/> instance.</summary>
    internal static Logger Log { get; private set; } = null!; // set in Entry;

    /// <summary>Gets the <see cref="IModHelper"/> API.</summary>
    internal static IModHelper ModHelper => Instance.Helper;

    /// <summary>Gets the <see cref="IManifest"/> API.</summary>
    internal static IManifest Manifest => Instance.ModManifest;

    /// <summary>Gets the unique ID for this mod.</summary>
    internal static string UniqueId => Manifest.UniqueID;

    /// <summary>Gets the <see cref="ITranslationHelper"/> API.</summary>
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:Element should begin with upper-case letter", Justification = "Distinguish from static Pathoschild.TranslationBuilder")]
    // ReSharper disable once InconsistentNaming
    internal static ITranslationHelper _I18n => ModHelper.Translation;

    internal static int BaselineFishPrice { get; private set; } = int.MaxValue;

    /// <summary>The mod entry point, called after the mod is first loaded.</summary>
    /// <param name="helper">Provides simplified APIs for writing mods.</param>
    public override void Entry(IModHelper helper)
    {
        Instance = this;
        Log = new Logger(this.Monitor);

        // pseudo-DRM for low-effort theft
        if (Manifest.Author != "DaLion" || UniqueId != this.GetType().Namespace)
        {
            Log.W(
                "Woops, looks like you downloaded a clandestine version of this mod! Please make sure to download from the official mod page at Nexus Mods.");
            return;
        }

        var assembly = Assembly.GetExecutingAssembly();
        I18n.Init(helper.Translation);
        Config = helper.ReadConfig<PondsConfig>();
        Broadcaster = new Broadcaster(helper.Multiplayer, UniqueId);
        Data = new ModDataManager(UniqueId, Log);
        helper.Events.Content.AssetRequested += OnAssetRequested;
        helper.Events.GameLoop.DayStarted += OnDayStarted;
        helper.Events.GameLoop.GameLaunched += OnGameLaunched;
        helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
        Harmonizer.ApplyFromNamespace(
            assembly,
            "DaLion.Ponds.Framework.Patchers",
            helper.ModRegistry,
            Log,
            UniqueId);
        CommandHandler.HandleFromNamespace(
            assembly,
            "DaLion.Ponds.Commands",
            helper.ConsoleCommands,
            Log,
            UniqueId,
            "pnds");
        this.ValidateMultiplayer();
    }

    private static void OnAssetRequested(object? sender, AssetRequestedEventArgs e)
    {
        if (!e.NameWithoutLocale.IsEquivalentTo("Data/FishPondData"))
        {
            return;
        }

        e.Edit(
            asset =>
            {
                // patch algae fish data
                var data = (List<FishPondData>)asset.Data;
                data.InsertRange(data.Count - 1, new List<FishPondData>
                {
                    new() // seaweed
                    {
                        Id = Manifest.UniqueID + "_Seaweed",
                        PopulationGates =
                            new Dictionary<int, List<string>> { { 4, ["368 3"] }, { 7, ["369 5"] }, },
                        ProducedItems =
                        [
                            new FishPondReward
                            {
                                Chance = 1f, ItemId = QIDs.Seaweed, MinStack = 1, MaxStack = 1,
                            },
                        ],
                        RequiredTags = ["item_seaweed"],
                        SpawnTime = 2,
                        Precedence = 0,
                    },
                    new() // green algae
                    {
                        Id = Manifest.UniqueID + "_GreenAlgae",
                        PopulationGates =
                            new Dictionary<int, List<string>> { { 4, ["368 3"] }, { 7, ["369 5"] }, },
                        ProducedItems =
                        [
                            new FishPondReward
                            {
                                Chance = 1f, ItemId = QIDs.GreenAlgae, MinStack = 1, MaxStack = 1,
                            },
                        ],
                        RequiredTags = ["item_green_algae"],
                        SpawnTime = 2,
                        Precedence = 0,
                    },
                    new() // white algae
                    {
                        Id = Manifest.UniqueID + "_WhiteAlgae",
                        PopulationGates =
                            new Dictionary<int, List<string>> { { 4, ["368 3"] }, { 7, ["369 5"] }, },
                        ProducedItems =
                        [
                            new FishPondReward
                            {
                                Chance = 1f, ItemId = QIDs.WhiteAlgae, MinStack = 1, MaxStack = 1,
                            },
                        ],
                        RequiredTags = ["item_white_algae"],
                        SpawnTime = 2,
                        Precedence = 0,
                    },
                });

                if (!Config.AddLegendaryFishPondData || ModHelper.ModRegistry.IsLoaded("DaLion.Professions"))
                {
                    return;
                }

                var file = Path.Combine(ModHelper.DirectoryPath, "assets", "data", "LegendaryFishPondData.json");

                try
                {
                    var json = File.ReadAllText(file);
                    var parsed = JsonConvert.DeserializeObject<List<FishPondData>>(json) ?? [];
                    data.AddRange(parsed);
                }
                catch (Exception ex)
                {
                    Log.E($"Failed loading Legendary Fish Pond data.\n{ex}");
                }
            },
            AssetEditPriority.Late);
    }

    private static void OnDayStarted(object? sender, DayStartedEventArgs e)
    {
        Utility.ForEachBuilding(building =>
        {
            if (building is FishPond pond && pond.IsOwnedBy(Game1.player) &&
                !pond.isUnderConstruction())
            {
                Data.Write(pond, DataKeys.CheckedToday, true.ToString());
            }

            return true; // continue enumeration
        });
    }

    private static void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
    {
        if (PondsConfigMenu.Instance?.IsLoaded ?? false)
        {
            PondsConfigMenu.Instance.Register();
        }
    }

    private static void OnSaveLoaded(object? sender, SaveLoadedEventArgs e)
    {
        Utility.ForEachBuilding(b =>
        {
            if (b is not FishPond pond)
            {
                return true; // continue enumeration
            }

            if (pond.FishCount > 0 && string.IsNullOrEmpty(Data.Read(pond, DataKeys.PondFish)))
            {
                pond.ResetPondFishData();
            }

            return true; // continue enumeration
        });

        foreach (var data in Game1.objectData.Values)
        {
            if (data.Category != SObject.FishCategory)
            {
                continue;
            }

            if (data.Price < BaselineFishPrice)
            {
                BaselineFishPrice = data.Price;
            }
        }
    }
}
