namespace DaLion.Stardew.Rings;

#region using directives

using System.Diagnostics.CodeAnalysis;
using DaLion.Common.Events;
using DaLion.Common.Harmony;
using DaLion.Common.Integrations.JsonAssets;
using StardewModdingAPI.Utilities;

#endregion using directives

/// <summary>The mod entry point.</summary>
public sealed class ModEntry : Mod
{
    /// <summary>Gets the static <see cref="ModEntry"/> instance.</summary>
    internal static ModEntry Instance { get; private set; } = null!;

    /// <summary>Gets or sets the <see cref="ModConfig"/> instance.</summary>
    internal static ModConfig Config { get; set; } = null!;

    /// <summary>Gets the <see cref="EventManager"/> instance.</summary>
    internal static EventManager Events { get; private set; } = null!;

    /// <summary>Gets the <see cref="PerScreen{T}"/> <see cref="ModState"/>.</summary>
    internal static PerScreen<ModState> PerScreenState { get; private set; } = null!;

    /// <summary>Gets or sets the <see cref="ModState"/> of the local player.</summary>
    internal static ModState State
    {
        get => PerScreenState.Value;
        set => PerScreenState.Value = value;
    }

    /// <summary>Gets the <see cref="IModHelper"/> API.</summary>
    internal static IModHelper ModHelper => Instance.Helper;

    /// <summary>Gets the <see cref="IManifest"/> for this mod.</summary>
    internal static IManifest Manifest => Instance.ModManifest;

    /// <summary>Gets the <see cref="ITranslationHelper"/> API.</summary>
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Preference.")]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:Element should begin with upper-case letter", Justification = "Preference.")]
    internal static ITranslationHelper i18n => ModHelper.Translation;

    /// <summary>Gets or sets the <see cref="IJsonAssetsApi"/>.</summary>
    internal static IJsonAssetsApi? JsonAssetsApi { get; set; }

    /// <summary>Gets a value indicating whether Better Rings mod is loaded in the current game session.</summary>
    internal static bool IsBetterRingsLoaded { get; private set; }

    /// <summary>Gets a value indicating whether Immersive Arsenal mod is loaded in the current game session.</summary>
    internal static bool IsImmersiveArsenalLoaded { get; private set; }

    /// <summary>Gets a value indicating whether Immersive Professions mod is loaded in the current game session.</summary>
    internal static bool IsImmersiveProfessionsLoaded { get; private set; }

    /// <summary>Gets or sets <see cref="Item"/> index of the Garnet gemstone (provided by Json Assets).</summary>
    internal static int GarnetIndex { get; set; }

    /// <summary>Gets or sets <see cref="Item"/> index of the Garnet Ring (provided by Json Assets).</summary>
    internal static int GarnetRingIndex { get; set; }

    /// <summary>Gets or sets <see cref="Item"/> index of the Infinity Band (provided by Json Assets).</summary>
    internal static int InfinityBandIndex { get; set; }

    /// <summary>The mod entry point, called after the mod is first loaded.</summary>
    /// <param name="helper">Provides simplified APIs for writing mods.</param>
    public override void Entry(IModHelper helper)
    {
        Instance = this;

        // initialize logger
        Log.Init(this.Monitor);

        // check for Better Rings
        IsBetterRingsLoaded = helper.ModRegistry.IsLoaded("BBR.BetterRings");
        IsImmersiveArsenalLoaded = helper.ModRegistry.IsLoaded("DaLion.ImmersiveArsenal");
        IsImmersiveProfessionsLoaded = helper.ModRegistry.IsLoaded("DaLion.ImmersiveProfessions");

        // get configs
        Config = helper.ReadConfig<ModConfig>();

        // enable events
        Events = new EventManager(helper.Events);

        // initialize mod state
        PerScreenState = new PerScreen<ModState>(() => new ModState());

        // apply patches
        new Harmonizer(helper.ModRegistry, this.ModManifest.UniqueID).ApplyAll();
    }
}
