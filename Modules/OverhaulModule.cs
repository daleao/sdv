namespace DaLion.Overhaul.Modules;

#region using directives

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using DaLion.Overhaul.Modules.Combat.Enums;
using DaLion.Overhaul.Modules.Combat.Extensions;
using DaLion.Overhaul.Modules.Combat.VirtualProperties;
using DaLion.Shared.Commands;
using DaLion.Shared.Constants;
using DaLion.Shared.Extensions.Collections;
using DaLion.Shared.Extensions.SMAPI;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using DaLion.Shared.Integrations;
using HarmonyLib;
using StardewValley.Objects;
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

    /// <summary>The Tools module.</summary>
    public static readonly OverhaulModule Tools = new ToolsModule();

    /// <summary>The Ponds module.</summary>
    public static readonly OverhaulModule Ponds = new PondsModule();

    /// <summary>The Taxes module.</summary>
    public static readonly OverhaulModule Taxes = new TaxesModule();

    /// <summary>The Tweex module.</summary>
    public static readonly OverhaulModule Tweex = new TweexModule();

    #endregion enum entries

    /// <summary>Initializes a new instance of the <see cref="OverhaulModule"/> class.</summary>
    /// <param name="name">The module name.</param>
    /// <param name="entry">The entry keyword for the module's <see cref="IConsoleCommand"/>s.</param>
    protected OverhaulModule(string name, string entry)
    {
        this.Name = name;
        this.Namespace = "DaLion.Overhaul.Modules." + name;
        this.DisplayName = _I18n.Get("gmcm.modules." + entry + ".name");
        this.Description = _I18n.Get("gmcm.modules." + entry + ".desc");
        this.Ticker = entry;
    }

    /// <summary>Gets the internal name of the module.</summary>
    internal string Name { get; }

    /// <summary>Gets the namespace of the module.</summary>
    internal string Namespace { get; }

    /// <summary>Gets the human-readable and localized name of the module.</summary>
    internal string DisplayName { get; init; }

    /// <summary>Gets a short localized description of the module.</summary>
    internal string Description { get; init; }

    /// <summary>Gets the ticker symbol of the module, which is used as the entry command.</summary>
    internal string Ticker { get; }

    /// <summary>Gets the <see cref="Harmonizer"/> instance of the module.</summary>
    internal Harmonizer? Harmonizer { get; private set; }

    /// <summary>Gets the <see cref="CommandHandler"/> instance of the module.</summary>
    internal CommandHandler? CommandHandler { get; private set; }

    /// <summary>Gets or sets a value indicating whether the module should be enabled.</summary>
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:Element should begin with upper-case letter", Justification = "Conflicts with static version.")]
    // ReSharper disable once InconsistentNaming
    internal abstract bool _ShouldEnable { get; set; }

    /// <summary>Gets a value indicating whether the module is currently active.</summary>
    [MemberNotNullWhen(true, nameof(Harmonizer), nameof(CommandHandler))]
    internal bool IsActive { get; private set; }

    /// <summary>Enumerates all modules.</summary>
    /// <returns>A <see cref="IEnumerable{T}"/> of <see cref="OverhaulModule"/>s.</returns>
    internal static IEnumerable<OverhaulModule> EnumerateModules()
    {
        yield return Core;
        yield return Professions;
        yield return Combat;
        yield return Tools;
        yield return Ponds;
        yield return Taxes;
        yield return Tweex;
    }

    /// <summary>Activates the module.</summary>
    /// <param name="helper">Provides simplified APIs for writing mods.</param>
    //[MemberNotNull(nameof(_harmonizer), nameof(_commandHandler))]
    internal virtual void Activate(IModHelper helper)
    {
        if (!this._ShouldEnable)
        {
            return;
        }

        if (this.IsActive)
        {
            Log.D($"[Modules]: {this.Name} module is already active.");
            return;
        }

        EventManager.ManageNamespace(this.Namespace + ".Events");
        this.Harmonizer =
            Harmonizer.ApplyFromNamespace(helper.ModRegistry, this.Namespace + ".Patchers", this.Namespace);
        this.CommandHandler ??= CommandHandler.HandleFromNamespace(
            helper.ConsoleCommands,
            this.Namespace + ".Commands",
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
        this.Harmonizer = this.Harmonizer.Unapply();
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
        if (this.IsActive)
        {
            IntegrationRegistry.RegisterFromNamespace(this.Namespace);
        }
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
            : base("Core", "mrg")
        {
            this.DisplayName = string.Empty;
            this.Description = string.Empty;
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

#if DEBUG
            EventManager.ManageNamespace(this.Namespace + ".Debug");
            this.Harmonizer = Harmonizer.ApplyFromNamespace(helper.ModRegistry, this.Namespace + ".Debug");
            this.CommandHandler ??= CommandHandler.HandleFromNamespace(
                helper.ConsoleCommands,
                this.Namespace + ".Debug",
                this.DisplayName,
                this.Ticker,
                () => this.IsActive);
            Log.I("[Modules]: Debug features activated.");
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
            : base("Professions", "prfs")
        {
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
        private static readonly HashSet<string> KnownEnchantmentType = AccessTools
            .GetTypesFromAssembly(Assembly.GetAssembly(typeof(Combat.Enchantments.BaseSlingshotEnchantment)))
            .Where(t => t.Namespace?.StartsWith("DaLion.Overhaul.Modules.Combat.Enchantments") == true)
            .Select(t => t.FullName)
            .WhereNotNull()
            .ToHashSet();

        /// <summary>Initializes a new instance of the <see cref="OverhaulModule.CombatModule"/> class.</summary>
        internal CombatModule()
            : base("Combat", "cmbt")
        {
        }

        /// <summary>Gets a value indicating whether the <see cref="OverhaulModule.CombatModule"/> is set to enabled.</summary>
        internal static bool ShouldEnable => ModEntry.Config.EnableCombat;

        /// <summary>Gets the config instance for the <see cref="OverhaulModule.CombatModule"/>.</summary>
        internal static Combat.Config Config => ModEntry.Config.Combat;

        /// <summary>Gets the ephemeral runtime state for the <see cref="OverhaulModule.CombatModule"/>.</summary>
        internal static Combat.State State => ModEntry.State.Combat;

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

        internal static void RevalidateAllWeapons()
        {
            if (!Context.IsWorldReady)
            {
                return;
            }

            var player = Game1.player;
            Log.I(
                $"[CMBT]: Performing {(Context.IsMainPlayer ? "global" : "local")} weapon revalidation.");
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

            Log.I("[CMBT]: Weapon revalidation complete.");
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
                Log.D($"[CMBT]: The type of {weapon.Name} was converted to Stabbing sword.");
            }
            else if (!ShouldEnable || (weapon.type.Value == MeleeWeapon.stabbingSword && !weapon.ShouldBeStabbySword()))
            {
                weapon.type.Value = MeleeWeapon.defenseSword;
                Log.D($"[CMBT]: The type of {weapon.Name} was converted to Defense sword.");
            }

            // refresh special status
            if (ShouldEnable && Config.EnableHeroQuest && (weapon.isGalaxyWeapon() || weapon.IsInfinityWeapon()
                || weapon.InitialParentTileIndex is WeaponIds.DarkSword or WeaponIds.HolyBlade or WeaponIds.NeptuneGlaive))
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

        internal static void RefreshAllWeapons(WeaponRefreshOption option)
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
            if (!ShouldEnable || !Config.EnableHeroQuest)
            {
                return;
            }

            var player = Game1.player;
            var darkSword = player.Items.FirstOrDefault(item => item is MeleeWeapon { InitialParentTileIndex: WeaponIds.DarkSword });
            if (darkSword is null && player.IsCursed())
            {
                if (Config.CanStoreRuinBlade)
                {
                    foreach (var chest in Game1.game1.IterateAll<Chest>())
                    {
                        darkSword = chest.items.FirstOrDefault(item => item is MeleeWeapon { InitialParentTileIndex: WeaponIds.DarkSword });
                        if (darkSword is not null)
                        {
                            break;
                        }
                    }

                    if (darkSword is null)
                    {
                        Log.W($"[CMBT]: {player.Name} is Cursed by Viego, but is not in possession of the Dark Sword. A new copy will be forcefully added.");
                        darkSword = new MeleeWeapon(WeaponIds.DarkSword);
                        if (!player.addItemToInventoryBool(darkSword))
                        {
                            Game1.createItemDebris(darkSword, player.getStandingPosition(), -1, player.currentLocation);
                        }
                    }
                }
                else
                {
                    Log.W($"[CMBT]: {player.Name} is Cursed by Viego, but is not in possession of the Dark Sword. A new copy will be forcefully added.");
                    darkSword = new MeleeWeapon(WeaponIds.DarkSword);
                    if (!player.addItemToInventoryBool(darkSword))
                    {
                        Game1.createItemDebris(darkSword, player.getStandingPosition(), -1, player.currentLocation);
                    }
                }
            }
            else if (darkSword is not null && !player.mailReceived.Contains("gotDarkSword"))
            {
                Log.W($"[CMBT]: {player.Name} is not yet Cursed by Viego, but already carries a the Dark Sword. The appropriate mail flag will be added.");
                player.mailReceived.Add("gotDarkSword");
            }

            if (darkSword is not null && darkSword.Read<int>(Modules.Combat.DataKeys.CursePoints) >= 50 && !player.hasOrWillReceiveMail("viegoCurse"))
            {
                Log.W($"[CMBT]: {player.Name}'s Dark Sword has gathered enough kills, but the purification quest-line has not begun. The necessary mail will be added for tomorrow.");
                Game1.addMailForTomorrow("viegoCurse");
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
                    Log.W($"[CMBT]: {enchantment.GetType()} was removed from {tool.Name} to avoid issues. You can try to re-add it with console commands.");
                }
            });
        }

        /// <inheritdoc />
        internal override void Activate(IModHelper helper)
        {
            base.Activate(helper);
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

            RevalidateAllWeapons();
            PerformDarkSwordValidations();
            RemoveInvalidEnchantments();
            return true;
        }

        /// <inheritdoc />
        protected override void InvalidateAssets()
        {
            ModHelper.GameContent.InvalidateCacheAndLocalized("Characters/Dialogue/Gil");
            ModHelper.GameContent.InvalidateCacheAndLocalized("Data/CraftingRecipes");
            ModHelper.GameContent.InvalidateCacheAndLocalized("Data/Events/AdventureGuild");
            ModHelper.GameContent.InvalidateCacheAndLocalized("Data/Events/Blacksmith");
            ModHelper.GameContent.InvalidateCacheAndLocalized("Data/Events/WizardHouse");
            ModHelper.GameContent.InvalidateCacheAndLocalized("Data/mail");
            ModHelper.GameContent.InvalidateCacheAndLocalized("Data/Monsters");
            ModHelper.GameContent.InvalidateCacheAndLocalized("Data/ObjectInformation");
            ModHelper.GameContent.InvalidateCacheAndLocalized("Data/Quests");
            ModHelper.GameContent.InvalidateCacheAndLocalized("Data/weapons");
            ModHelper.GameContent.InvalidateCacheAndLocalized("Maps/springobjects");
            ModHelper.GameContent.InvalidateCacheAndLocalized("Strings/Locations");
            ModHelper.GameContent.InvalidateCacheAndLocalized("Strings/StringsFromCSFiles");
            ModHelper.GameContent.InvalidateCache("TileSheets/BuffsIcons");
            ModHelper.GameContent.InvalidateCache("TileSheets/Projectiles");
            ModHelper.GameContent.InvalidateCache("TileSheets/weapons");
        }
    }

    internal sealed class ToolsModule : OverhaulModule
    {
        /// <summary>Initializes a new instance of the <see cref="OverhaulModule.ToolsModule"/> class.</summary>
        internal ToolsModule()
            : base("Tools", "tols")
        {
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
    }

    internal sealed class PondsModule : OverhaulModule
    {
        /// <summary>Initializes a new instance of the <see cref="OverhaulModule.PondsModule"/> class.</summary>
        internal PondsModule()
            : base("Ponds", "pnds")
        {
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
        }

        /// <summary>Gets a value indicating whether the <see cref="OverhaulModule.TaxesModule"/> is set to enabled.</summary>
        internal static bool ShouldEnable => ModEntry.Config.EnableTaxes;

        /// <summary>Gets the config instance for the <see cref="OverhaulModule.TaxesModule"/>.</summary>
        internal static Taxes.Config Config => ModEntry.Config.Taxes;

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
    }

    #endregion implementations
}
