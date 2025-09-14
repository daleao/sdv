global using DaLion.Professions.Framework;
global using DaLion.Professions.Framework.Extensions;
global using DaLion.Shared.Constants;
global using DaLion.Shared.Reflection;
global using static DaLion.Professions.ProfessionsMod;

namespace DaLion.Professions;

#region using directives

using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using DaLion.Shared.Commands;
using DaLion.Shared.Data;
using DaLion.Shared.Events;
using DaLion.Shared.Extensions.SMAPI;
using DaLion.Shared.Harmony;
using DaLion.Shared.Networking;
using DaLion.Shared.Pathfinding;
using StardewModdingAPI.Utilities;

#endregion using directives

/// <summary>The mod entry point.</summary>
public sealed class ProfessionsMod : Mod
{
    /// <summary>Gets the static <see cref="ProfessionsMod"/> instance.</summary>
    internal static ProfessionsMod Instance { get; private set; } = null!; // set in Entry

    /// <summary>Gets or sets the <see cref="ProfessionsConfig"/> instance.</summary>
    internal static ProfessionsConfig Config { get; set; } = null!; // set in Entry

    /// <summary>Gets the <see cref="PerScreen{T}"/> <see cref="ProfessionsState"/>.</summary>
    internal static PerScreen<ProfessionsState> PerScreenState { get; private set; } = null!; // set in Entry

    /// <summary>Gets or sets the <see cref="ProfessionsState"/> of the local player.</summary>
    internal static ProfessionsState State
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

    /// <summary>Gets or sets the <see cref="PathfindingManager"/> instance.</summary>
    internal static PathfindingManager? Pathfinder { get; set; }

    /// <summary>Gets or sets the <see cref="PathfindingManagerAsync"/> instance.</summary>
    internal static PathfindingManagerAsync? PathfinderAsync { get; set; }

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

    /// <summary>Gets a value indicating whether the Skill Reset feature is enabled.</summary>
    internal static bool ShouldEnableSkillReset => Config.Skills.EnableSkillReset;

    /// <summary>Gets a value indicating whether the Skill Reset feature is enabled.</summary>
    internal static bool ShouldEnablePrestigeLevels => Config.Masteries.EnablePrestigeLevels;

    /// <summary>Gets a value indicating whether the Skill Reset feature is enabled.</summary>
    internal static bool ShouldEnableLimitBreaks => Config.Masteries.EnableLimitBreaks;

    internal static string GoldenMayoId { get; private set; } = null!; // set in Entry;

    internal static string OstrichMayoId { get; private set; } = null!; // set in Entry;

    internal static string SlimeMayoId { get; private set; } = null!; // set in Entry;

    internal static string SlimeCheeseId { get; private set; } = null!; // set in Entry;

    internal static string RedBrushId { get; private set; } = null!; // set in Entry;

    internal static string GreenBrushId { get; private set; } = null!; // set in Entry;

    internal static string BlueBrushId { get; private set; } = null!; // set in Entry;

    internal static string PurpleBrushId { get; private set; } = null!; // set in Entry;

    internal static string PrismaticBrushId { get; private set; } = null!; // set in Entry;

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
        Broadcaster = new Broadcaster(helper.Multiplayer, UniqueId);
        Config = helper.ReadConfig<ProfessionsConfig>();
        Data = new ModDataManager(UniqueId, Log);
        PerScreenState = new PerScreen<ProfessionsState>(() => new ProfessionsState());
        Harmonizer.ApplyFromNamespace(
            assembly,
            "DaLion.Professions.Framework.Patchers",
            helper.ModRegistry,
            Log,
            UniqueId);

        var handler = CommandHandler.HandleFromNamespace(
            assembly,
            "DaLion.Professions.Commands",
            helper.ConsoleCommands,
            Log,
            UniqueId,
            "prfs");
        EventManager = new EventManager(helper.Events, helper.ModRegistry, Log, handler)
            .ManageInitial(assembly, "DaLion.Professions.Framework.Events");

        this.ValidateMultiplayer();

        GoldenMayoId = $"{UniqueId}_GoldenMayo";
        OstrichMayoId = $"{UniqueId}_OstrichMayo";
        SlimeMayoId = $"{UniqueId}_SlimeMayo";
        SlimeCheeseId = $"{UniqueId}_SlimeCheese";
        RedBrushId = $"{UniqueId}_RedPaintBrush";
        GreenBrushId = $"{UniqueId}_GreenPaintBrush";
        BlueBrushId = $"{UniqueId}_BluePaintBrush";
        PurpleBrushId = $"{UniqueId}_PurplePaintBrush";
        PrismaticBrushId = $"{UniqueId}_PrismaticPaintBrush";
    }

    /// <inheritdoc />
    public override object GetApi()
    {
        return new ProfessionsApi();
    }
}
