namespace DaLion.Overhaul.Modules;

#region using directives

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using DaLion.Overhaul.Modules.Weapons.Extensions;
using DaLion.Overhaul.Modules.Weapons.VirtualProperties;
using DaLion.Shared.Commands;
using DaLion.Shared.Extensions.Collections;
using DaLion.Shared.Extensions.SMAPI;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using DaLion.Shared.Integrations;
using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

/// <summary>The individual modules within the Overhaul mod.</summary>
public abstract class OverhaulModule
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
        this.Ticker = entry;
    }

    /// <summary>Gets the internal name of the module.</summary>
    internal string Name { get; }

    /// <summary>Gets the human-readable name of the module.</summary>
    internal string DisplayName { get; }

    /// <summary>Gets a short description of the module.</summary>
    internal string? Description { get; private set; }

    /// <summary>Gets the namespace of the module.</summary>
    internal string Namespace { get; }

    /// <summary>Gets the ticker symbol of the module, which is used as the entry command.</summary>
    internal string Ticker { get; }

    /// <summary>Gets or sets a value indicating whether the module should be enabled.</summary>
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:Element should begin with upper-case letter", Justification = "Conflicts with static version.")]
    // ReSharper disable once InconsistentNaming
    internal abstract bool _ShouldEnable { get; set; }

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
            this.Ticker,
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

    /// <summary>Applies necessary fixes to the current save file.</summary>
    /// <returns><see langword="false"/> if the save was not ready for validation, otherwise <see langword="true"/>.</returns>
    internal virtual bool Revalidate()
    {
        return Context.IsWorldReady;
    }

    /// <summary>Registers module integrations with third-party mods.</summary>
    internal virtual void RegisterIntegrations()
    {
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
            : base("Core", "margx")
        {
        }

        /// <inheritdoc />
        internal override bool _ShouldEnable
        {
            get
            {
                return true;
            }

            set
            {
            }
        }

        /// <inheritdoc />
        internal override void Activate(IModHelper helper)
        {
            base.Activate(helper);
            //EventManager.ManageNamespace("DaLion.Shared");
#if DEBUG
            // start FPS counter
            Globals.FpsCounter = new FrameRateCounter(GameRunner.instance);
            helper.Reflection.GetMethod(Globals.FpsCounter, "LoadContent").Invoke();
#endif
        }

        /// <inheritdoc />
        internal override void RegisterIntegrations()
        {
            Modules.Core.ConfigMenu.GenericModConfigMenu.Instance?.Register();
        }
    }

    internal sealed class ProfessionsModule : OverhaulModule
    {
        /// <summary>Initializes a new instance of the <see cref="OverhaulModule.ProfessionsModule"/> class.</summary>
        internal ProfessionsModule()
            : base("Professions", "profs")
        {
            this.Description =
                "Overhauls professions with the goal of supporting more diverse and interesting playstyles. " +
                "Introduces all-new Prestige mechanics and Limit Breaks for combat professions.";
        }

        /// <summary>Gets a value indicating whether the <see cref="OverhaulModule.ProfessionsModule"/> is set to enabled.</summary>
        internal static bool ShouldEnable => ModEntry.Config.EnableProfessions;

        /// <summary>Gets the config instance for the <see cref="OverhaulModule.ProfessionsModule"/>.</summary>
        internal static Professions.Config Config => ModEntry.Config.Professions;

        /// <summary>Gets the ephemeral runtime state for the <see cref="OverhaulModule.ProfessionsModule"/>.</summary>
        internal static Professions.State State => ModEntry.State.Professions;

        /// <inheritdoc />
        internal override bool _ShouldEnable
        {
            get
            {
                return ModEntry.Config.EnableProfessions;
            }

            set
            {
                ModEntry.Config.EnableProfessions = value;
                ModHelper.WriteConfig(ModEntry.Config);
            }
        }

        /// <inheritdoc />
        internal override void Activate(IModHelper helper)
        {
            if (ShouldEnable)
            {
                base.Activate(helper);
            }
        }

        /// <inheritdoc />
        internal override void RegisterIntegrations()
        {
            new IModIntegration?[]
            {
                Modules.Professions.Integrations.SpaceCoreIntegration.Instance,
                Modules.Professions.Integrations.LuckSkillIntegration.Instance,
                Modules.Professions.Integrations.LoveOfCookingIntegration.Instance,
                Modules.Professions.Integrations.AutomateIntegration.Instance,
                Modules.Professions.Integrations.TehsFishingOverhaulIntegration.Instance,
                Modules.Professions.Integrations.CustomOreNodesIntegration.Instance,
                Modules.Professions.Integrations.StardewValleyExpandedIntegration.Instance,
            }.ForEach(integration => integration?.Register());
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
            : base("Combat", "cmbt")
        {
            this.Description =
                "Overhauls general combat mechanics with the goal of improving underwhelming combat stats. " +
                "Emphasis on a multiplicative defense algorithm, and knockback collision damage.";
        }

        /// <summary>Gets a value indicating whether the <see cref="OverhaulModule.CombatModule"/> is set to enabled.</summary>
        internal static bool ShouldEnable => ModEntry.Config.EnableCombat;

        /// <summary>Gets the config instance for the <see cref="OverhaulModule.CombatModule"/>.</summary>
        internal static Combat.Config Config => ModEntry.Config.Combat;

        /// <inheritdoc />
        internal override bool _ShouldEnable
        {
            get
            {
                return ModEntry.Config.EnableCombat;
            }

            set
            {
                ModEntry.Config.EnableCombat = value;
                ModHelper.WriteConfig(ModEntry.Config);
            }
        }

        /// <inheritdoc />
        internal override void Activate(IModHelper helper)
        {
            if (ShouldEnable)
            {
                base.Activate(helper);
            }
        }

        /// <inheritdoc />
        protected override void InvalidateAssets()
        {
            ModHelper.GameContent.InvalidateCacheAndLocalized("Strings/StringsFromCSFiles");
        }
    }

    internal sealed class WeaponsModule : OverhaulModule
    {
        /// <summary>Initializes a new instance of the <see cref="OverhaulModule.WeaponsModule"/> class.</summary>
        internal WeaponsModule()
            : base("Weapons", "wpnz")
        {
            this.Description =
                "Overhauls all aspects of melee weapons. " +
                "Rebalances weapons according to new color-coded tiers, introduces combo mechanics, swords with a stabbing special move, and many other features.";
        }

        /// <summary>Gets a value indicating whether the <see cref="OverhaulModule.WeaponsModule"/> is set to enabled.</summary>
        internal static bool ShouldEnable => ModEntry.Config.EnableWeapons;

        /// <summary>Gets the config instance for the <see cref="OverhaulModule.WeaponsModule"/>.</summary>
        internal static Weapons.Config Config => ModEntry.Config.Weapons;

        /// <summary>Gets the ephemeral runtime state for the <see cref="OverhaulModule.WeaponsModule"/>.</summary>
        internal static Weapons.State State => ModEntry.State.Weapons;

        /// <inheritdoc />
        internal override bool _ShouldEnable
        {
            get
            {
                return ModEntry.Config.EnableWeapons;
            }

            set
            {
                ModEntry.Config.EnableWeapons = value;
                ModHelper.WriteConfig(ModEntry.Config);
            }
        }

        internal static void RevalidateAllWeapons()
        {
            if (!Context.IsWorldReady)
            {
                return;
            }

            var player = Game1.player;
            Log.I(
                $"[WPNZ]: Performing {(Context.IsMainPlayer ? "global" : "local")} weapon revalidation.");
            if (Context.IsMainPlayer)
            {
                Utility.iterateAllItems(item =>
                {
                    if (item is MeleeWeapon weapon)
                    {
                        RevalidateSingleWeapon(weapon);
                    }
                });
            }
            else
            {
                for (var i = 0; i < player.Items.Count; i++)
                {
                    if (player.Items[i] is MeleeWeapon weapon)
                    {
                        RevalidateSingleWeapon(weapon);
                    }
                }
            }

            Log.I("[WPNZ]: Weapon revalidation complete.");
            ModHelper.GameContent.InvalidateCacheAndLocalized("Data/weapons");
        }

        internal static void RevalidateSingleWeapon(MeleeWeapon weapon)
        {
            // refresh intrinsic enchantments
            weapon.RemoveIntrinsicEnchantments();
            if (ShouldEnable)
            {
                weapon.AddIntrinsicEnchantments();
            }

            // refresh stabby swords
            if (ShouldEnable && weapon.type.Value == MeleeWeapon.defenseSword && weapon.ShouldBeStabbySword())
            {
                weapon.type.Value = MeleeWeapon.stabbingSword;
                Log.D($"[WPNZ]: The type of {weapon.Name} was converted to Stabbing sword.");
            }
            else if (!ShouldEnable || (weapon.type.Value == MeleeWeapon.stabbingSword && !weapon.ShouldBeStabbySword()))
            {
                weapon.type.Value = MeleeWeapon.defenseSword;
                Log.D($"[WPNZ]: The type of {weapon.Name} was converted to Defense sword.");
            }

            // refresh special status
            if (ShouldEnable && Config.InfinityPlusOne && (weapon.isGalaxyWeapon() || weapon.IsInfinityWeapon()
                || weapon.InitialParentTileIndex is ItemIDs.DarkSword or ItemIDs.HolyBlade or ItemIDs.NeptuneGlaive))
            {
                weapon.specialItem = true;
            }

            // refresh forges and stats
            weapon.RecalculateAppliedForges();
        }

        internal static void AddAllIntrinsicEnchantments()
        {
            if (Context.IsMainPlayer)
            {
                Utility.iterateAllItems(item =>
                {
                    if (item is MeleeWeapon weapon)
                    {
                        weapon.AddIntrinsicEnchantments();
                    }
                });
            }
            else
            {
                for (var i = 0; i < Game1.player.Items.Count; i++)
                {
                    if (Game1.player.Items[i] is MeleeWeapon weapon)
                    {
                        weapon.AddIntrinsicEnchantments();
                    }
                }
            }
        }

        internal static void RemoveAllIntrinsicEnchantments()
        {
            if (Context.IsMainPlayer)
            {
                Utility.iterateAllItems(item =>
                {
                    if (item is MeleeWeapon weapon)
                    {
                        weapon.RemoveIntrinsicEnchantments();
                    }
                });
            }
            else
            {
                for (var i = 0; i < Game1.player.Items.Count; i++)
                {
                    if (Game1.player.Items[i] is MeleeWeapon weapon)
                    {
                        weapon.RemoveIntrinsicEnchantments();
                    }
                }
            }
        }

        internal static void ConvertAllStabbingSwords()
        {
            if (Context.IsMainPlayer)
            {
                Utility.iterateAllItems(item =>
                {
                    if (item is MeleeWeapon sword && sword.ShouldBeStabbySword())
                    {
                        sword.type.Value = MeleeWeapon.stabbingSword;
                    }
                });
            }
            else
            {
                for (var i = 0; i < Game1.player.Items.Count; i++)
                {
                    if (Game1.player.Items[i] is MeleeWeapon sword && sword.ShouldBeStabbySword())
                    {
                        sword.type.Value = MeleeWeapon.stabbingSword;
                    }
                }
            }
        }

        internal static void RevertAllStabbingSwords()
        {
            if (Context.IsMainPlayer)
            {
                Utility.iterateAllItems(item =>
                {
                    if (item is MeleeWeapon { type.Value: MeleeWeapon.stabbingSword } sword)
                    {
                        sword.type.Value = MeleeWeapon.defenseSword;
                    }
                });
            }
            else
            {
                for (var i = 0; i < Game1.player.Items.Count; i++)
                {
                    if (Game1.player.Items[i] is MeleeWeapon { type.Value: MeleeWeapon.stabbingSword } sword)
                    {
                        sword.type.Value = MeleeWeapon.defenseSword;
                    }
                }
            }
        }

        internal static void RefreshAllWeapons(Weapons.RefreshOption option)
        {
            if (Context.IsMainPlayer)
            {
                Utility.iterateAllItems(item =>
                {
                    if (item is not MeleeWeapon weapon)
                    {
                        return;
                    }

                    weapon.RefreshStats(option);
                    weapon.Invalidate();
                });
            }
            else
            {
                for (var i = 0; i < Game1.player.Items.Count; i++)
                {
                    if (Game1.player.Items[i] is not MeleeWeapon weapon)
                    {
                        continue;
                    }

                    weapon.RefreshStats(option);
                    weapon.Invalidate();
                }
            }
        }

        internal static void PerformDarkSwordValidations()
        {
            if (!ShouldEnable || !Config.InfinityPlusOne)
            {
                return;
            }

            var player = Game1.player;
            var darkSword = player.Items.FirstOrDefault(item => item is MeleeWeapon { InitialParentTileIndex: ItemIDs.DarkSword });
            if (darkSword is null && player.IsCursed())
            {
                if (Config.CanStoreRuinBlade)
                {
                    foreach (var chest in Game1.game1.IterateAllChests())
                    {
                        darkSword = chest.items.FirstOrDefault(item => item is MeleeWeapon { InitialParentTileIndex: ItemIDs.DarkSword });
                        if (darkSword is not null)
                        {
                            break;
                        }
                    }

                    if (darkSword is null)
                    {
                        Log.W($"[WPNZ]: {player.Name} is Cursed by Viego, but is not in possession of the Dark Sword. A new copy will be forcefully added.");
                        darkSword = new MeleeWeapon(ItemIDs.DarkSword);
                        if (!player.addItemToInventoryBool(darkSword))
                        {
                            Game1.createItemDebris(darkSword, player.getStandingPosition(), -1, player.currentLocation);
                        }
                    }
                }
                else
                {
                    Log.W($"[WPNZ]: {player.Name} is Cursed by Viego, but is not in possession of the Dark Sword. A new copy will be forcefully added.");
                    darkSword = new MeleeWeapon(ItemIDs.DarkSword);
                    if (!player.addItemToInventoryBool(darkSword))
                    {
                        Game1.createItemDebris(darkSword, player.getStandingPosition(), -1, player.currentLocation);
                    }
                }
            }
            else if (darkSword is not null && !player.mailReceived.Contains("gotDarkSword"))
            {
                Log.W($"[WPNZ]: {player.Name} is not yet Cursed by Viego, but already carries a the Dark Sword. The appropriate mail flag will be added.");
                player.mailReceived.Add("gotDarkSword");
            }

            if (darkSword is not null && darkSword.Read<int>(Modules.Weapons.DataKeys.CursePoints) >= 50 && !player.hasOrWillReceiveMail("viegoCurse"))
            {
                Log.W($"[WPNZ]: {player.Name}'s Dark Sword has gathered enough kills, but the purification quest-line has not begun. The necessary mail will be added for tomorrow.");
                Game1.addMailForTomorrow("viegoCurse");
            }
        }

        /// <inheritdoc />
        internal override void Activate(IModHelper helper)
        {
            if (ShouldEnable)
            {
                base.Activate(helper);
            }
        }

        /// <inheritdoc />
        internal override bool Revalidate()
        {
            if (!Context.IsWorldReady)
            {
                return false;
            }

            RevalidateAllWeapons();
            PerformDarkSwordValidations();
            return true;
        }

        /// <inheritdoc />
        internal override void RegisterIntegrations()
        {
            new IModIntegration?[]
            {
                Modules.Weapons.Integrations.SpaceCoreIntegration.Instance,
                Modules.Weapons.Integrations.JsonAssetsIntegration.Instance,
                Modules.Weapons.Integrations.StardewValleyExpandedIntegration.Instance,
                Modules.Weapons.Integrations.VanillaTweaksIntegration.Instance,
                Modules.Weapons.Integrations.SimpleWeaponsIntegration.Instance,
            }.ForEach(integration => integration?.Register());
        }

        /// <inheritdoc />
        protected override void InvalidateAssets()
        {
            ModHelper.GameContent.InvalidateCacheAndLocalized("Characters/Dialogue/Gil");
            ModHelper.GameContent.InvalidateCacheAndLocalized("Data/ObjectInformation");
            ModHelper.GameContent.InvalidateCacheAndLocalized("Data/Events/AdventureGuild");
            ModHelper.GameContent.InvalidateCacheAndLocalized("Data/Events/Blacksmith");
            ModHelper.GameContent.InvalidateCacheAndLocalized("Data/Events/WizardHouse");
            ModHelper.GameContent.InvalidateCacheAndLocalized("Data/mail");
            ModHelper.GameContent.InvalidateCacheAndLocalized("Data/Monsters");
            ModHelper.GameContent.InvalidateCacheAndLocalized("Data/weapons");
            ModHelper.GameContent.InvalidateCacheAndLocalized("Strings/Locations");
            ModHelper.GameContent.InvalidateCache("TileSheets/Projectiles");
            ModHelper.GameContent.InvalidateCache("TileSheets/weapons");
        }
    }

    internal sealed class SlingshotsModule : OverhaulModule
    {
        /// <summary>Initializes a new instance of the <see cref="OverhaulModule.SlingshotsModule"/> class.</summary>
        internal SlingshotsModule()
            : base("Slingshots", "slngs")
        {
            this.Description =
                "Overhauls slingshots to bring them up to par with melee weapons. " +
                "Includes ranged critical strikes, slingshots enchantments, a stunning smack special move, and more.";
        }

        /// <summary>Gets a value indicating whether the <see cref="OverhaulModule.SlingshotsModule"/> is set to enabled.</summary>
        internal static bool ShouldEnable => ModEntry.Config.EnableSlingshots;

        /// <summary>Gets the config instance for the <see cref="OverhaulModule.SlingshotsModule"/>.</summary>
        internal static Slingshots.Config Config => ModEntry.Config.Slingshots;

        /// <summary>Gets the ephemeral runtime state for the <see cref="OverhaulModule.SlingshotsModule"/>.</summary>
        internal static Slingshots.State State => ModEntry.State.Slingshots;

        /// <inheritdoc />
        internal override bool _ShouldEnable
        {
            get
            {
                return ModEntry.Config.EnableSlingshots;
            }

            set
            {
                ModEntry.Config.EnableSlingshots = value;
                ModHelper.WriteConfig(ModEntry.Config);
            }
        }

        /// <inheritdoc />
        internal override void Activate(IModHelper helper)
        {
            if (ShouldEnable)
            {
                base.Activate(helper);
            }
        }

        /// <inheritdoc />
        protected override void InvalidateAssets()
        {
            ModHelper.GameContent.InvalidateCache("TileSheets/weapons");
        }
    }

    internal sealed class ToolsModule : OverhaulModule
    {
        /// <summary>Initializes a new instance of the <see cref="OverhaulModule.ToolsModule"/> class.</summary>
        internal ToolsModule()
            : base("Tools", "tols")
        {
            this.Description =
                "A one-stop-shop for tool augmentation, customization and quality-of-life. " +
                "Includes chargeable resource tools, automatic tool selection, expanded tool enchantments, among other features.";
        }

        /// <summary>Gets a value indicating whether the <see cref="OverhaulModule.ToolsModule"/> is set to enabled.</summary>
        internal static bool ShouldEnable => ModEntry.Config.EnableTools;

        /// <summary>Gets the config instance for the <see cref="OverhaulModule.ToolsModule"/>.</summary>
        internal static Tools.Config Config => ModEntry.Config.Tools;

        /// <summary>Gets the ephemeral runtime state for the <see cref="OverhaulModule.ToolsModule"/>.</summary>
        internal static Tools.State State => ModEntry.State.Tools;

        /// <inheritdoc />
        internal override bool _ShouldEnable
        {
            get
            {
                return ModEntry.Config.EnableTools;
            }

            set
            {
                ModEntry.Config.EnableTools = value;
                ModHelper.WriteConfig(ModEntry.Config);
            }
        }

        /// <inheritdoc />
        internal override void Activate(IModHelper helper)
        {
            if (ShouldEnable)
            {
                base.Activate(helper);
            }
        }

        /// <inheritdoc />
        internal override void RegisterIntegrations()
        {
            (Modules.Tools.Integrations.MoonMisadventuresIntegration.Instance as IModIntegration)?.Register();
        }
    }

    internal sealed class EnchantmentsModule : OverhaulModule
    {
        private static readonly HashSet<string> KnownEnchantmentType = AccessTools
            .GetTypesFromAssembly(Assembly.GetAssembly(typeof(Modules.Enchantments.Ranged.BaseSlingshotEnchantment)))
            .Where(t => t.Namespace is "DaLion.Overhaul.Modules.Enchantments.Melee"
                or "DaLion.Overhaul.Modules.Enchantments.Ranged" or "DaLion.Overhaul.Modules.Enchantments.Gemstone")
            .Select(t => t.FullName)
            .WhereNotNull()
            .ToHashSet();

        /// <summary>Initializes a new instance of the <see cref="OverhaulModule.EnchantmentsModule"/> class.</summary>
        internal EnchantmentsModule()
            : base("Enchantments", "ench")
        {
            this.Description =
                "A complete overhaul of combat enchantments. " +
                "Replaces boring vanilla weapon enchantments with entirely new and more interesting ones, while also introducing slingshot-specific enchantments.";
        }

        /// <summary>Gets a value indicating whether the <see cref="OverhaulModule.EnchantmentsModule"/> is set to enabled.</summary>
        internal static bool ShouldEnable => ModEntry.Config.EnableEnchantments;

        /// <summary>Gets the config instance for the <see cref="OverhaulModule.EnchantmentsModule"/>.</summary>
        internal static Enchantments.Config Config => ModEntry.Config.Enchantments;

        /// <summary>Gets the ephemeral runtime state for the <see cref="OverhaulModule.EnchantmentsModule"/>.</summary>
        internal static Enchantments.State State => ModEntry.State.Enchantments;

        /// <inheritdoc />
        internal override bool _ShouldEnable
        {
            get
            {
                return ModEntry.Config.EnableEnchantments;
            }

            set
            {
                ModEntry.Config.EnableEnchantments = value;
                ModHelper.WriteConfig(ModEntry.Config);
            }
        }

        internal static void RemoveInvalidEnchantments()
        {
            Utility.iterateAllItems(item =>
            {
                if (item is not (Tool tool and (MeleeWeapon or Slingshot)))
                {
                    return;
                }

                for (var i = tool.enchantments.Count - 1; i >= 0; i--)
                {
                    var enchantment = tool.enchantments[i];
                    var name = enchantment.GetType().FullName;
                    if (name is null || enchantment.IsSecondaryEnchantment())
                    {
                        continue;
                    }

                    if (!name.StartsWith("DaLion.Overhaul.Modules") || (ShouldEnable && KnownEnchantmentType.Contains(name)))
                    {
                        continue;
                    }

                    tool.RemoveEnchantment(enchantment);
                    Log.W($"[ENCH]: {enchantment.GetType()} was removed from {tool.Name} to avoid issues. You can try to re-add it with console commands.");
                }
            });
        }

        /// <inheritdoc />
        internal override void Activate(IModHelper helper)
        {
            if (ShouldEnable)
            {
                base.Activate(helper);
            }

            Reflector.GetStaticFieldSetter<List<BaseEnchantment>?>(typeof(BaseEnchantment), "_enchantments")
                .Invoke(null);
        }

        /// <inheritdoc />
        internal override void Deactivate()
        {
            base.Deactivate();
            Reflector.GetStaticFieldSetter<List<BaseEnchantment>?>(typeof(BaseEnchantment), "_enchantments")
                .Invoke(null);
        }

        /// <inheritdoc />
        internal override bool Revalidate()
        {
            if (!Context.IsWorldReady)
            {
                return false;
            }

            RemoveInvalidEnchantments();
            return true;
        }

        /// <inheritdoc />
        internal override void RegisterIntegrations()
        {
            (Modules.Enchantments.Integrations.SpaceCoreIntegration.Instance as IModIntegration)?.Register();
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
            : base("Rings", "rngs")
        {
            this.Description =
                "Snap your enemies to oblivion with the powerful new Infinity Band, which draws inspiration from real Music Theory to create a more balanced and immersive mechanic for combining multiple rings.";
        }

        /// <summary>Gets a value indicating whether the <see cref="OverhaulModule.RingsModule"/> is set to enabled.</summary>
        internal static bool ShouldEnable => ModEntry.Config.EnableRings;

        /// <summary>Gets the config instance for the <see cref="OverhaulModule.RingsModule"/>.</summary>
        internal static Rings.Config Config => ModEntry.Config.Rings;

        /// <summary>Gets the ephemeral runtime state for the <see cref="OverhaulModule.RingsModule"/>.</summary>
        internal static Rings.State State => ModEntry.State.Rings;

        /// <inheritdoc />
        internal override bool _ShouldEnable
        {
            get
            {
                return ModEntry.Config.EnableRings;
            }

            set
            {
                ModEntry.Config.EnableRings = value;
                ModHelper.WriteConfig(ModEntry.Config);
            }
        }

        /// <inheritdoc />
        internal override void Activate(IModHelper helper)
        {
            if (ShouldEnable)
            {
                base.Activate(helper);
            }
        }

        /// <inheritdoc />
        internal override void RegisterIntegrations()
        {
            new IModIntegration?[]
            {
                Modules.Rings.Integrations.BetterCraftingIntegration.Instance,
                Modules.Rings.Integrations.WearMoreRingsIntegration.Instance,
                Modules.Rings.Integrations.BetterRingsIntegration.Instance,
                Modules.Rings.Integrations.VanillaTweaksIntegration.Instance,
                Modules.Rings.Integrations.JsonAssetsIntegration.Instance,
            }.ForEach(integration => integration?.Register());
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
            : base("Ponds", "pnds")
        {
            this.Description =
                "Makes Fish Ponds great again, allowing roe products to scale in quality and quantity among other rebalances and new mechanics.";
        }

        /// <summary>Gets a value indicating whether the <see cref="OverhaulModule.PondsModule"/> is set to enabled.</summary>
        internal static bool ShouldEnable => ModEntry.Config.EnablePonds;

        /// <summary>Gets the config instance for the <see cref="OverhaulModule.PondsModule"/>.</summary>
        internal static Ponds.Config Config => ModEntry.Config.Ponds;

        /// <inheritdoc />
        internal override bool _ShouldEnable
        {
            get
            {
                return ModEntry.Config.EnablePonds;
            }

            set
            {
                ModEntry.Config.EnablePonds = value;
                ModHelper.WriteConfig(ModEntry.Config);
            }
        }

        /// <inheritdoc />
        internal override void Activate(IModHelper helper)
        {
            if (ShouldEnable)
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
            : base("Taxes", "txs")
        {
            this.Description =
                "A borderline realistic taxation system for added challenge and end-game gold sink.";
        }

        /// <summary>Gets a value indicating whether the <see cref="OverhaulModule.TaxesModule"/> is set to enabled.</summary>
        internal static bool ShouldEnable => ModEntry.Config.EnableTaxes;

        /// <summary>Gets the config instance for the <see cref="OverhaulModule.TaxesModule"/>.</summary>
        internal static Taxes.Config Config => ModEntry.Config.Taxes;

        /// <summary>Gets the ephemeral runtime state for the <see cref="OverhaulModule.TaxesModule"/>.</summary>
        internal static Taxes.State State => ModEntry.State.Taxes;

        /// <inheritdoc />
        internal override bool _ShouldEnable
        {
            get
            {
                return ModEntry.Config.EnableTaxes;
            }

            set
            {
                ModEntry.Config.EnableTaxes = value;
                ModHelper.WriteConfig(ModEntry.Config);
            }
        }

        /// <inheritdoc />
        internal override void Activate(IModHelper helper)
        {
            if (ShouldEnable)
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
            : base("Tweex", "twx")
        {
            this.Description =
                "A repository of smaller immersion tweaks and fixes for vanilla inconsistencies.";
        }

        /// <summary>Gets a value indicating whether the <see cref="OverhaulModule.TweexModule"/> is set to enabled.</summary>
        internal static bool ShouldEnable => ModEntry.Config.EnableTweex;

        /// <summary>Gets the config instance for the <see cref="OverhaulModule.TweexModule"/>.</summary>
        internal static Tweex.Config Config => ModEntry.Config.Tweex;

        /// <inheritdoc />
        internal override bool _ShouldEnable
        {
            get
            {
                return ModEntry.Config.EnableTweex;
            }

            set
            {
                ModEntry.Config.EnableTweex = value;
                ModHelper.WriteConfig(ModEntry.Config);
            }
        }

        /// <inheritdoc />
        internal override void Activate(IModHelper helper)
        {
            if (ShouldEnable)
            {
                base.Activate(helper);
            }
        }
    }

    #endregion implementations
}
