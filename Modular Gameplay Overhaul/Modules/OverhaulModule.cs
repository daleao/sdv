namespace DaLion.Overhaul.Modules;

#region using directives

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using DaLion.Shared.Commands;
using DaLion.Shared.Extensions.SMAPI;
using DaLion.Shared.Harmony;

#endregion using directives

/// <summary>The individual modules within the Overhaul mod.</summary>
internal abstract class OverhaulModule
{
    #region enum entries

    /// <summary>The Core module.</summary>
    public static readonly OverhaulModule Core = new CoreModule();

    /// <summary>The Professions module.</summary>
    public static readonly OverhaulModule Professions = new ProfessionsModule();

    /// <summary>The Combat module.</summary>
    public static readonly OverhaulModule Combat = new CombatModule();

    /// <summary>The Weapons module.</summary>
    public static readonly OverhaulModule Weapons = new WeaponsModule();

    /// <summary>The Slingshots module.</summary>
    public static readonly OverhaulModule Slingshots = new SlingshotsModule();

    /// <summary>The Tools module.</summary>
    public static readonly OverhaulModule Tools = new ToolsModule();

    /// <summary>The Enchantments module.</summary>
    public static readonly OverhaulModule Enchantments = new EnchantmentsModule();

    /// <summary>The Rings module.</summary>
    public static readonly OverhaulModule Rings = new RingsModule();

    /// <summary>The Ponds module.</summary>
    public static readonly OverhaulModule Ponds = new PondsModule();

    /// <summary>The Taxes module.</summary>
    public static readonly OverhaulModule Taxes = new TaxesModule();

    /// <summary>The Tweex module.</summary>
    public static readonly OverhaulModule Tweex = new TweexModule();

    #endregion enum entries

    private Harmonizer? _harmonizer;
    private CommandHandler? _commandHandler;

    /// <summary>Initializes a new instance of the <see cref="OverhaulModule"/> class.</summary>
    /// <param name="name">The module name.</param>
    /// <param name="entry">The entry keyword for the module's <see cref="IConsoleCommand"/>s.</param>
    protected OverhaulModule(string name, string entry)
    {
        this.Name = name;
        this.DisplayName = "Modular Overhaul :: " + name;
        this.Namespace = "DaLion.Overhaul.Modules." + name;
        this.EntryCommand = entry;
    }

    /// <summary>Gets the internal name of the module.</summary>
    internal string Name { get; }

    /// <summary>Gets the human-readable name of the module.</summary>
    internal string DisplayName { get; }

    /// <summary>Gets the namespace of the module.</summary>
    internal string Namespace { get; }

    /// <summary>Gets the entry command of the module.</summary>
    internal string EntryCommand { get; }

    /// <summary>Gets a value indicating whether the module is currently active.</summary>
    [MemberNotNullWhen(true, nameof(_harmonizer), nameof(_commandHandler))]
    internal bool IsActive { get; private set; }

    /// <summary>Enumerates all modules.</summary>
    /// <returns>A <see cref="IEnumerable{T}"/> of <see cref="OverhaulModule"/>s.</returns>
    internal static IEnumerable<OverhaulModule> EnumerateModules()
    {
        yield return Core;
        yield return Professions;
        yield return Combat;
        yield return Weapons;
        yield return Slingshots;
        yield return Tools;
        yield return Enchantments;
        yield return Rings;
        yield return Ponds;
        yield return Taxes;
        yield return Tweex;
    }

    /// <summary>Activates the module.</summary>
    /// <param name="helper">Provides simplified APIs for writing mods.</param>
    //[MemberNotNull(nameof(_harmonizer), nameof(_commandHandler))]
    internal virtual void Activate(IModHelper helper)
    {
        if (this.IsActive)
        {
            Log.D($"[Modules]: {this.Name} module is already active.");
            return;
        }

        EventManager.ManageNamespace(this.Namespace);
        this._harmonizer = Harmonizer.ApplyFromNamespace(helper.ModRegistry, this.Namespace);
        this._commandHandler ??= CommandHandler.HandleFromNamespace(
            helper.ConsoleCommands,
            this.Namespace,
            this.DisplayName,
            this.EntryCommand,
            () => this.IsActive);
        this.IsActive = true;
        this.InvalidateAssets();
        Log.T($"[Modules]: {this.Name} module activated.");
    }

    /// <summary>Deactivates the module.</summary>
    internal virtual void Deactivate()
    {
        if (!this.IsActive)
        {
            Log.D($"[Modules]: {this.Name} module is not active.");
            return;
        }

        EventManager.UnmanageNamespace(this.Namespace);
        this._harmonizer = this._harmonizer.Unapply();
        this.IsActive = false;
        this.InvalidateAssets();
        Log.T($"[Modules]: {this.Name} module deactivated.");
    }

    /// <summary>Causes SMAPI to reload all assets edited by this module.</summary>
    protected virtual void InvalidateAssets()
    {
    }

    #region implementations

    internal sealed class CoreModule : OverhaulModule
    {
        /// <summary>Initializes a new instance of the <see cref="OverhaulModule.CoreModule"/> class.</summary>
        internal CoreModule()
            : base("Core", "margo")
        {
        }

        /// <inheritdoc />
        internal override void Activate(IModHelper helper)
        {
            base.Activate(helper);
#if DEBUG
            // start FPS counter
            Globals.FpsCounter = new FrameRateCounter(GameRunner.instance);
            helper.Reflection.GetMethod(Globals.FpsCounter, "LoadContent").Invoke();
#endif
        }
    }

    internal sealed class ProfessionsModule : OverhaulModule
    {
        /// <summary>Initializes a new instance of the <see cref="OverhaulModule.ProfessionsModule"/> class.</summary>
        internal ProfessionsModule()
            : base("Professions", "profs")
        {
        }

        /// <summary>Gets a value indicating whether the <see cref="OverhaulModule.ProfessionsModule"/> is enabled.</summary>
        internal static bool IsEnabled => ModEntry.Config.EnableProfessions;

        /// <summary>Gets the config instance for the <see cref="OverhaulModule.ProfessionsModule"/>.</summary>
        internal static Professions.Config Config => ModEntry.Config.Professions;

        /// <summary>Gets the ephemeral runtime state for the <see cref="OverhaulModule.ProfessionsModule"/>.</summary>
        internal static Professions.State State => ModEntry.State.Professions;

        /// <inheritdoc />
        internal override void Activate(IModHelper helper)
        {
            if (IsEnabled)
            {
                base.Activate(helper);
            }
        }

        /// <inheritdoc />
        protected override void InvalidateAssets()
        {
            ModHelper.GameContent.InvalidateCacheAndLocalized("Data/achievements");
            ModHelper.GameContent.InvalidateCacheAndLocalized("Data/FishPondData");
            ModHelper.GameContent.InvalidateCacheAndLocalized("Data/mail");
            ModHelper.GameContent.InvalidateCacheAndLocalized("LooseSprite/Cursors");
            ModHelper.GameContent.InvalidateCache("TileSheets/BuffsIcons");
            ModHelper.GameContent.InvalidateCache("TileSheets/weapons");
        }
    }

    internal sealed class CombatModule : OverhaulModule
    {
        /// <summary>Initializes a new instance of the <see cref="OverhaulModule.CombatModule"/> class.</summary>
        internal CombatModule()
            : base("Combat", "combat")
        {
        }

        /// <summary>Gets a value indicating whether the <see cref="OverhaulModule.CombatModule"/> is enabled.</summary>
        internal static bool IsEnabled => ModEntry.Config.EnableCombat;

        /// <summary>Gets the config instance for the <see cref="OverhaulModule.CombatModule"/>.</summary>
        internal static Combat.Config Config => ModEntry.Config.Combat;

        /// <inheritdoc />
        internal override void Activate(IModHelper helper)
        {
            if (IsEnabled)
            {
                base.Activate(helper);
            }
        }
    }

    internal sealed class WeaponsModule : OverhaulModule
    {
        /// <summary>Initializes a new instance of the <see cref="OverhaulModule.WeaponsModule"/> class.</summary>
        internal WeaponsModule()
            : base("Weapons", "weapons")
        {
        }

        /// <summary>Gets a value indicating whether the <see cref="OverhaulModule.WeaponsModule"/> is enabled.</summary>
        internal static bool IsEnabled => ModEntry.Config.EnableWeapons;

        /// <summary>Gets the config instance for the <see cref="OverhaulModule.WeaponsModule"/>.</summary>
        internal static Weapons.Config Config => ModEntry.Config.Weapons;

        /// <summary>Gets the ephemeral runtime state for the <see cref="OverhaulModule.WeaponsModule"/>.</summary>
        internal static Weapons.State State => ModEntry.State.Weapons;

        /// <inheritdoc />
        internal override void Activate(IModHelper helper)
        {
            if (IsEnabled)
            {
                base.Activate(helper);
            }
        }

        /// <inheritdoc />
        internal override void Deactivate()
        {
            base.Deactivate();
            Modules.Weapons.Utils.RevalidateAllWeapons();
        }

        /// <inheritdoc />
        protected override void InvalidateAssets()
        {
            ModHelper.GameContent.InvalidateCacheAndLocalized("Data/ObjectInformation");
            ModHelper.GameContent.InvalidateCacheAndLocalized("Data/Events/AdventureGuild");
            ModHelper.GameContent.InvalidateCacheAndLocalized("Data/Events/Blacksmith");
            ModHelper.GameContent.InvalidateCacheAndLocalized("Data/Events/WizardHouse");
            ModHelper.GameContent.InvalidateCacheAndLocalized("Data/Monsters");
            ModHelper.GameContent.InvalidateCacheAndLocalized("Data/weapons");
            ModHelper.GameContent.InvalidateCacheAndLocalized("Strings/Locations");
            ModHelper.GameContent.InvalidateCache("TileSheets/BuffsIcons");
            ModHelper.GameContent.InvalidateCache("TileSheets/Projectiles");
            ModHelper.GameContent.InvalidateCache("TileSheets/weapons");
        }
    }

    internal sealed class SlingshotsModule : OverhaulModule
    {
        /// <summary>Initializes a new instance of the <see cref="OverhaulModule.SlingshotsModule"/> class.</summary>
        internal SlingshotsModule()
            : base("Slingshots", "slings")
        {
        }

        /// <summary>Gets a value indicating whether the <see cref="OverhaulModule.SlingshotsModule"/> is enabled.</summary>
        internal static bool IsEnabled => ModEntry.Config.EnableSlingshots;

        /// <summary>Gets the config instance for the <see cref="OverhaulModule.SlingshotsModule"/>.</summary>
        internal static Slingshots.Config Config => ModEntry.Config.Slingshots;

        /// <summary>Gets the ephemeral runtime state for the <see cref="OverhaulModule.SlingshotsModule"/>.</summary>
        internal static Slingshots.State State => ModEntry.State.Slingshots;

        /// <inheritdoc />
        internal override void Activate(IModHelper helper)
        {
            if (IsEnabled)
            {
                base.Activate(helper);
            }
        }

        /// <inheritdoc />
        internal override void Deactivate()
        {
            base.Deactivate();
            Modules.Weapons.Utils.RevalidateAllWeapons();
        }

        /// <inheritdoc />
        protected override void InvalidateAssets()
        {
            ModHelper.GameContent.InvalidateCacheAndLocalized("Data/ObjectInformation");
            ModHelper.GameContent.InvalidateCacheAndLocalized("Data/Events/AdventureGuild");
            ModHelper.GameContent.InvalidateCacheAndLocalized("Data/Events/Blacksmith");
            ModHelper.GameContent.InvalidateCacheAndLocalized("Data/Events/WizardHouse");
            ModHelper.GameContent.InvalidateCacheAndLocalized("Data/Monsters");
            ModHelper.GameContent.InvalidateCacheAndLocalized("Data/weapons");
            ModHelper.GameContent.InvalidateCacheAndLocalized("Strings/Locations");
            ModHelper.GameContent.InvalidateCache("TileSheets/BuffsIcons");
            ModHelper.GameContent.InvalidateCache("TileSheets/Projectiles");
            ModHelper.GameContent.InvalidateCache("TileSheets/weapons");
        }
    }

    internal sealed class ToolsModule : OverhaulModule
    {
        /// <summary>Initializes a new instance of the <see cref="OverhaulModule.ToolsModule"/> class.</summary>
        internal ToolsModule()
            : base("Tools", "tools")
        {
        }

        /// <summary>Gets a value indicating whether the <see cref="OverhaulModule.ToolsModule"/> is enabled.</summary>
        internal static bool IsEnabled => ModEntry.Config.EnableTools;

        /// <summary>Gets the config instance for the <see cref="OverhaulModule.ToolsModule"/>.</summary>
        internal static Tools.Config Config => ModEntry.Config.Tools;

        /// <summary>Gets the ephemeral runtime state for the <see cref="OverhaulModule.ToolsModule"/>.</summary>
        internal static Tools.State State => ModEntry.State.Tools;

        /// <inheritdoc />
        internal override void Activate(IModHelper helper)
        {
            if (IsEnabled)
            {
                base.Activate(helper);
            }
        }

        /// <inheritdoc />
        protected override void InvalidateAssets()
        {
            ModHelper.GameContent.InvalidateCacheAndLocalized("Data/weapons");
        }
    }

    internal sealed class EnchantmentsModule : OverhaulModule
    {
        /// <summary>Initializes a new instance of the <see cref="OverhaulModule.EnchantmentsModule"/> class.</summary>
        internal EnchantmentsModule()
            : base("Enchantments", "enchantments")
        {
        }

        /// <summary>Gets a value indicating whether the <see cref="OverhaulModule.EnchantmentsModule"/> is enabled.</summary>
        internal static bool IsEnabled => ModEntry.Config.EnableEnchantments;

        /// <summary>Gets the config instance for the <see cref="OverhaulModule.EnchantmentsModule"/>.</summary>
        internal static Enchantments.Config Config => ModEntry.Config.Enchantments;

        /// <inheritdoc />
        internal override void Activate(IModHelper helper)
        {
            if (IsEnabled)
            {
                base.Activate(helper);
            }
        }

        /// <inheritdoc />
        protected override void InvalidateAssets()
        {
            ModHelper.GameContent.InvalidateCache("TileSheets/BuffsIcons");
        }
    }

    internal sealed class RingsModule : OverhaulModule
    {
        /// <summary>Initializes a new instance of the <see cref="OverhaulModule.RingsModule"/> class.</summary>
        internal RingsModule()
            : base("Rings", "rings")
        {
        }

        /// <summary>Gets a value indicating whether the <see cref="OverhaulModule.RingsModule"/> is enabled.</summary>
        internal static bool IsEnabled => ModEntry.Config.EnableRings;

        /// <summary>Gets the config instance for the <see cref="OverhaulModule.RingsModule"/>.</summary>
        internal static Rings.Config Config => ModEntry.Config.Rings;

        /// <summary>Gets the ephemeral runtime state for the <see cref="OverhaulModule.RingsModule"/>.</summary>
        internal static Rings.State State => ModEntry.State.Rings;

        /// <inheritdoc />
        internal override void Activate(IModHelper helper)
        {
            if (IsEnabled)
            {
                base.Activate(helper);
            }
        }

        /// <inheritdoc />
        protected override void InvalidateAssets()
        {
            ModHelper.GameContent.InvalidateCacheAndLocalized("Data/CraftingRecipes");
            ModHelper.GameContent.InvalidateCacheAndLocalized("Data/ObjectInformation");
            ModHelper.GameContent.InvalidateCacheAndLocalized("Maps/springobjects");
        }
    }

    internal sealed class PondsModule : OverhaulModule
    {
        /// <summary>Initializes a new instance of the <see cref="OverhaulModule.PondsModule"/> class.</summary>
        internal PondsModule()
            : base("Ponds", "ponds")
        {
        }

        /// <summary>Gets a value indicating whether the <see cref="OverhaulModule.PondsModule"/> is enabled.</summary>
        internal static bool IsEnabled => ModEntry.Config.EnablePonds;

        /// <summary>Gets the config instance for the <see cref="OverhaulModule.PondsModule"/>.</summary>
        internal static Ponds.Config Config => ModEntry.Config.Ponds;

        /// <inheritdoc />
        internal override void Activate(IModHelper helper)
        {
            if (IsEnabled)
            {
                base.Activate(helper);
            }
        }

        /// <inheritdoc />
        protected override void InvalidateAssets()
        {
            ModHelper.GameContent.InvalidateCache("Data/FishPondData");
        }
    }

    internal sealed class TaxesModule : OverhaulModule
    {
        /// <summary>Initializes a new instance of the <see cref="OverhaulModule.TaxesModule"/> class.</summary>
        internal TaxesModule()
            : base("Taxes", "taxes")
        {
        }

        /// <summary>Gets a value indicating whether the <see cref="OverhaulModule.TaxesModule"/> is enabled.</summary>
        internal static bool IsEnabled => ModEntry.Config.EnableTaxes;

        /// <summary>Gets the config instance for the <see cref="OverhaulModule.TaxesModule"/>.</summary>
        internal static Taxes.Config Config => ModEntry.Config.Taxes;

        /// <summary>Gets the ephemeral runtime state for the <see cref="OverhaulModule.TaxesModule"/>.</summary>
        internal static Taxes.State State => ModEntry.State.Taxes;

        /// <inheritdoc />
        internal override void Activate(IModHelper helper)
        {
            if (IsEnabled)
            {
                base.Activate(helper);
            }
        }

        /// <inheritdoc />
        protected override void InvalidateAssets()
        {
            ModHelper.GameContent.InvalidateCacheAndLocalized("Data/mail");
        }
    }

    internal sealed class TweexModule : OverhaulModule
    {
        /// <summary>Initializes a new instance of the <see cref="OverhaulModule.TweexModule"/> class.</summary>
        internal TweexModule()
            : base("Tweex", "tweex")
        {
        }

        /// <summary>Gets a value indicating whether the <see cref="OverhaulModule.TweexModule"/> is enabled.</summary>
        internal static bool IsEnabled => ModEntry.Config.EnableTweex;

        /// <summary>Gets the config instance for the <see cref="OverhaulModule.TweexModule"/>.</summary>
        internal static Tweex.Config Config => ModEntry.Config.Tweex;

        /// <inheritdoc />
        internal override void Activate(IModHelper helper)
        {
            if (IsEnabled)
            {
                base.Activate(helper);
            }
        }
    }

    #endregion implementations
}
