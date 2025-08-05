global using DaLion.Enchantments.Framework;
global using DaLion.Shared.Reflection;
global using StardewValley.Enchantments;
global using static DaLion.Enchantments.EnchantmentsMod;

namespace DaLion.Enchantments;

#region using directives

using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using DaLion.Shared.Commands;
using DaLion.Shared.Data;
using DaLion.Shared.Events;
using DaLion.Shared.Extensions.SMAPI;
using DaLion.Shared.Harmony;
using DaLion.Shared.Networking;
using StardewModdingAPI.Utilities;

#endregion using directives

/// <summary>The mod entry point.</summary>
public sealed class EnchantmentsMod : Mod
{
    /// <summary>Gets the static <see cref="EnchantmentsMod"/> instance.</summary>
    internal static EnchantmentsMod Instance { get; private set; } = null!; // set in Entry

    /// <summary>Gets or sets the <see cref="EnchantmentsConfig"/> instance.</summary>
    internal static EnchantmentsConfig Config { get; set; } = null!; // set in Entry

    /// <summary>Gets the <see cref="PerScreen{T}"/> <see cref="EnchantmentsState"/>.</summary>
    internal static PerScreen<EnchantmentsState> PerScreenState { get; private set; } = null!; // set in Entry

    /// <summary>Gets or sets the <see cref="EnchantmentsState"/> of the local player.</summary>
    internal static EnchantmentsState State
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
        Config = helper.ReadConfig<EnchantmentsConfig>();
        Broadcaster = new Broadcaster(helper.Multiplayer, UniqueId);
        Data = new ModDataManager(UniqueId, Log);
        PerScreenState = new PerScreen<EnchantmentsState>(() => new EnchantmentsState());
        EventManager = new EventManager(helper.Events, helper.ModRegistry, Log)
            .ManageInitial(assembly, "DaLion.Enchantments.Framework.Events");
        Harmonizer.ApplyFromNamespace(
            assembly,
            "DaLion.Enchantments.Framework.Patchers",
            helper.ModRegistry,
            Log,
            UniqueId);
        CommandHandler.HandleFromNamespace(
            assembly,
            "DaLion.Enchantments.Commands",
            helper.ConsoleCommands,
            Log,
            UniqueId,
            "ench");
        this.ValidateMultiplayer();
    }
}
