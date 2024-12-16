global using DaLion.Combat.Framework;
global using DaLion.Combat.Framework.Extensions;
global using static DaLion.Combat.CombatMod;

namespace DaLion.Combat;

#region using directives

using System.Reflection;
using DaLion.Shared;
using DaLion.Shared.Commands;
using DaLion.Shared.Events;
using DaLion.Shared.Extensions.SMAPI;
using DaLion.Shared.Harmony;
using StardewModdingAPI.Utilities;

#endregion using directives

/// <summary>The mod entry point.</summary>
public sealed class CombatMod : Mod
{
    /// <summary>Gets the static <see cref="CombatMod"/> instance.</summary>
    internal static CombatMod Instance { get; private set; } = null!; // set in Entry

    /// <summary>Gets or sets the <see cref="CombatConfig"/> instance.</summary>
    internal static CombatConfig Config { get; set; } = null!; // set in Entry

    /// <summary>Gets the <see cref="PerScreen{T}"/> <see cref="CombatState"/>.</summary>
    internal static PerScreen<CombatState> PerScreenState { get; private set; } = null!; // set in Entry

    /// <summary>Gets or sets the <see cref="CombatState"/> of the local player.</summary>
    internal static CombatState State
    {
        get => PerScreenState.Value;
        set => PerScreenState.Value = value;
    }

    /// <summary>Gets the <see cref="Shared.Events.EventManager"/> instance.</summary>
    internal static EventManager EventManager { get; private set; } = null!; // set in Entry

    /// <summary>Gets the <see cref="Logger"/> instance.</summary>
    internal static Logger Log { get; private set; } = null!; // set in Entry;

    /// <summary>Gets the <see cref="IModHelper"/> API.</summary>
    internal static IModHelper ModHelper => Instance.Helper;

    /// <summary>Gets the <see cref="IManifest"/> API.</summary>
    internal static IManifest Manifest => Instance.ModManifest;

    /// <summary>Gets the unique ID for this mod.</summary>
    internal static string UniqueId => Manifest.UniqueID;

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
        Config = helper.ReadConfig<CombatConfig>();
        PerScreenState = new PerScreen<CombatState>(() => new CombatState());
        EventManager = new EventManager(helper.Events, helper.ModRegistry, Log).ManageInitial(assembly);
        Harmonizer.ApplyAll(assembly, helper.ModRegistry, Log, UniqueId);
        CommandHandler.HandleAll(
            assembly,
            helper.ConsoleCommands,
            Log,
            UniqueId,
            "cmbt");
        this.ValidateMultiplayer();
    }

    /// <inheritdoc />
    public override object GetApi()
    {
        return new CombatApi();
    }
}
