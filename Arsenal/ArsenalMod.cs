global using DaLion.Arsenal.Framework;
global using DaLion.Arsenal.Framework.Extensions;
global using static DaLion.Arsenal.ArsenalMod;

namespace DaLion.Arsenal;

#region using directives

using System.Reflection;
using DaLion.Shared;
using DaLion.Shared.Commands;
using DaLion.Shared.Data;
using DaLion.Shared.Events;
using DaLion.Shared.Extensions.SMAPI;
using DaLion.Shared.Harmony;
using StardewModdingAPI.Utilities;

#endregion using directives

/// <summary>The mod entry point.</summary>
public sealed class ArsenalMod : Mod
{
    /// <summary>The user's current ring texture style, based on installed texture mods.</summary>
    internal enum TextureStyle
    {
        /// <summary>No ring texture mods installed.</summary>
        Vanilla,

        /// <summary>Simple Weapons by dengdeng installed.</summary>
        SimpleWeapons,

        /// <summary>Vanilla Tweaks by Taiyokun installed.</summary>
        VanillaTweaks,
    }

    /// <summary>Gets the static <see cref="ArsenalMod"/> instance.</summary>
    internal static ArsenalMod Instance { get; private set; } = null!; // set in Entry

    /// <summary>Gets or sets the <see cref="ArsenalConfig"/> instance.</summary>
    internal static ArsenalConfig Config { get; set; } = null!; // set in Entry

    /// <summary>Gets the <see cref="PerScreen{T}"/> <see cref="ArsenalState"/>.</summary>
    internal static PerScreen<ArsenalState> PerScreenState { get; private set; } = null!; // set in Entry

    /// <summary>Gets or sets the <see cref="ArsenalState"/> of the local player.</summary>
    internal static ArsenalState State
    {
        get => PerScreenState.Value;
        set => PerScreenState.Value = value;
    }

    /// <summary>Gets the <see cref="Shared.Events.EventManager"/> instance.</summary>
    internal static EventManager EventManager { get; private set; } = null!; // set in Entry

    /// <summary>Gets the <see cref="ModDataManager"/> instance.</summary>
    internal static ModDataManager Data { get; private set; } = null!; // set in Entry

    /// <summary>Gets the <see cref="Logger"/> instance.</summary>
    internal static Logger Log { get; private set; } = null!; // set in Entry;

    /// <summary>Gets the <see cref="IModHelper"/> API.</summary>
    internal static IModHelper ModHelper => Instance.Helper;

    /// <summary>Gets the <see cref="IManifest"/> API.</summary>
    internal static IManifest Manifest => Instance.ModManifest;

    /// <summary>Gets the unique ID for this mod.</summary>
    internal static string UniqueId => Manifest.UniqueID;

    internal static string DwarvenBlueprintId { get; private set; } = null!; // set in Entry;

    internal static string DwarvenMetalId { get; private set; } = null!; // set in Entry;

    internal static string ElderwoodId { get; private set; } = null!; // set in Entry;

    internal static string HeroSoulId { get; private set; } = null!; // set in Entry;

    internal static bool CombatModLoaded { get; private set; }

    internal static TextureStyle WeaponTextureStyle { get; private set; }

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
        Config = helper.ReadConfig<ArsenalConfig>();
        PerScreenState = new PerScreen<ArsenalState>(() => new ArsenalState());
        EventManager = new EventManager(helper.Events, helper.ModRegistry, Log).ManageInitial(assembly);
        Data = new ModDataManager(UniqueId, Log);
        Harmonizer.ApplyAll(assembly, helper.ModRegistry, Log, UniqueId);
        CommandHandler.HandleAll(
            assembly,
            helper.ConsoleCommands,
            Log,
            UniqueId,
            "ars");
        this.ValidateMultiplayer();

        DwarvenMetalId = $"{UniqueId}_DwarvenBlueprint";
        DwarvenMetalId = $"{UniqueId}_DwarvenMetal";
        ElderwoodId = $"{UniqueId}_Elderwood";
        HeroSoulId = $"{UniqueId}_HeroSoulId";
        WeaponTextureStyle = helper.ModRegistry.IsLoaded("dengdeng.simpleweapons")
            ? TextureStyle.SimpleWeapons
            : helper.ModRegistry.IsLoaded("Taiyo.VanillaTweaks") ||
              helper.ModRegistry.IsLoaded("Taiyo.VanillaTweaks.Warrior")
                ? TextureStyle.VanillaTweaks
                : TextureStyle.Vanilla;

        CombatModLoaded = helper.ModRegistry.IsLoaded("DaLion.Combat");
    }
}
