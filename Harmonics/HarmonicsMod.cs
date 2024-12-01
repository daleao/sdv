global using DaLion.Shared.Reflection;
global using static DaLion.Harmonics.HarmonicsMod;

namespace DaLion.Harmonics;

#region using directives

using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using DaLion.Shared;
using DaLion.Shared.Commands;
using DaLion.Shared.Data;
using DaLion.Shared.Events;
using DaLion.Shared.Extensions.SMAPI;
using DaLion.Shared.Harmony;
using DaLion.Shared.Networking;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;

#endregion using directives

/// <summary>The mod entry point.</summary>
public sealed class HarmonicsMod : Mod
{
    /// <summary>The user's current ring texture style, based on installed texture mods.</summary>
    internal enum TextureStyle
    {
        /// <summary>No ring texture mods installed.</summary>
        Vanilla,

        /// <summary>Better Rings by BBR installed.</summary>
        BetterRings,

        /// <summary>Vanilla Tweaks by Taiyokun installed.</summary>
        VanillaTweaks,
    }

    /// <summary>Gets the static <see cref="HarmonicsMod"/> instance.</summary>
    internal static HarmonicsMod Instance { get; private set; } = null!; // set in Entry

    /// <summary>Gets or sets the <see cref="HarmonicsConfig"/> instance.</summary>
    internal static HarmonicsConfig Config { get; set; } = null!; // set in Entry

    /// <summary>Gets the <see cref="PerScreen{T}"/> <see cref="HarmonicsState"/>.</summary>
    internal static PerScreen<HarmonicsState> PerScreenState { get; private set; } = null!; // set in Entry

    /// <summary>Gets or sets the <see cref="HarmonicsState"/> of the local player.</summary>
    internal static HarmonicsState State
    {
        get => PerScreenState.Value;
        set => PerScreenState.Value = value;
    }

    /// <summary>Gets the <see cref="ModDataManager"/> instance.</summary>
    internal static ModDataManager Data { get; private set; } = null!; // set in Entry

    /// <summary>Gets the <see cref="Shared.Events.EventManager"/> instance.</summary>
    internal static EventManager EventManager { get; private set; } = null!; // set in Entry

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

    internal static string GarnetStoneId { get; private set; } = null!; // set in Entry;

    internal static string GarnetRingId { get; private set; } = null!; // set in Entry;

    internal static string InfinityBandId { get; private set; } = null!; // set in Entry;

    internal static TextureStyle RingTextureStyle { get; private set; }

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
                "Woops, looks like you downloaded a clandestine version of this mod! Please make sure to download from the official mod page at XXX.");
            return;
        }

        var assembly = Assembly.GetExecutingAssembly();
        I18n.Init(helper.Translation);
        Config = helper.ReadConfig<HarmonicsConfig>();
        PerScreenState = new PerScreen<HarmonicsState>(() => new HarmonicsState());
        Data = new ModDataManager(UniqueId, Log);
        EventManager = new EventManager(helper.Events, helper.ModRegistry, Log).ManageInitial(assembly);
        Broadcaster = new Broadcaster(helper.Multiplayer, UniqueId);
        Harmonizer.ApplyAll(assembly, helper.ModRegistry, Log, UniqueId);
        CommandHandler.HandleAll(
            assembly,
            helper.ConsoleCommands,
            Log,
            UniqueId,
            "hrmn");
        this.ValidateMultiplayer();

        GarnetStoneId = $"{UniqueId}_GarnetGemstone";
        GarnetRingId = $"{UniqueId}_GarnetRing";
        InfinityBandId = $"{UniqueId}_InfinityBand";
        RingTextureStyle = helper.ModRegistry.IsLoaded("BBR.BetterRings")
            ? TextureStyle.BetterRings
            : helper.ModRegistry.IsLoaded("Taiyo.VanillaTweaks")
                ? TextureStyle.VanillaTweaks
                : TextureStyle.Vanilla;
    }
}
