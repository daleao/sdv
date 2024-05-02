namespace DaLion.Overhaul.Modules.Combat.Configs;

#region using directives

using DaLion.Overhaul.Modules.Combat.Enums;
using DaLion.Overhaul.Modules.Combat.Integrations;
using DaLion.Shared.Extensions.Collections;
using DaLion.Shared.Extensions.SMAPI;
using DaLion.Shared.Integrations.GMCM.Attributes;
using Newtonsoft.Json;

#endregion using directives

/// <summary>The user-configurable settings for CMBT.</summary>
public sealed class QuestsConfig
{
    private bool _dwarvenLegacy = true;
    private bool _enableHeroQuest = true;
    private int _iridiumBarsPerGalaxyWeapon = 10;
    private float _ruinBladeDotMultiplier = 1f;
    private QuestDifficulty _heroQuestDifficulty = QuestDifficulty.Medium;

    #region dropdown enums

    /// <summary>The difficulty level of the proven conditions for the virtue trials.</summary>
    public enum QuestDifficulty
    {
        /// <summary>Easy.</summary>
        Easy,

        /// <summary>Medium.</summary>
        Medium,

        /// <summary>Hard.</summary>
        Hard,
    }

    #endregion dropdown enums

    /// <summary>Gets a value indicating whether to enable Clint's forging mechanic for Masterwork weapons.</summary>
    [JsonProperty]
    [GMCMPriority(300)]
    public bool DwarvenLegacy
    {
        get => this._dwarvenLegacy;
        internal set
        {
            if (value == this._dwarvenLegacy)
            {
                return;
            }

            this._dwarvenLegacy = value;
            ModHelper.GameContent.InvalidateCacheAndLocalized("Data/Events/Blacksmith");
            ModHelper.GameContent.InvalidateCacheAndLocalized("Data/Quests");
            ModHelper.GameContent.InvalidateCacheAndLocalized("Data/Monsters");
            ModHelper.GameContent.InvalidateCacheAndLocalized("Data/weapons");
        }
    }

    /// <summary>Gets a value indicating whether replace lame Galaxy and Infinity weapons with something truly legendary.</summary>
    [JsonProperty]
    [GMCMPriority(301)]
    public bool EnableHeroQuest
    {
        get => this._enableHeroQuest;
        internal set
        {
            if (value == this._enableHeroQuest)
            {
                return;
            }

            this._enableHeroQuest = value;
            ModHelper.GameContent.InvalidateCacheAndLocalized("Data/Events/WizardHouse");
            ModHelper.GameContent.InvalidateCacheAndLocalized("Data/ObjectInformation");
            ModHelper.GameContent.InvalidateCacheAndLocalized("Data/weapons");
            ModHelper.GameContent.InvalidateCacheAndLocalized("Strings/Locations");
            ModHelper.GameContent.InvalidateCacheAndLocalized("Strings/StringsFromCSFiles");
            ModHelper.GameContent.InvalidateCache("TileSheets/Projectiles");
            if ((VanillaTweaksIntegration.IsValueCreated && VanillaTweaksIntegration.Instance.IsRegistered) ||
                (SimpleWeaponsIntegration.IsValueCreated && SimpleWeaponsIntegration.Instance.IsRegistered))
            {
                ModHelper.GameContent.InvalidateCache("TileSheets/weapons");
            }
        }
    }

    /// <summary>Gets a value indicating the number of Iridium Bars required to receive a Galaxy weapon.</summary>
    [JsonProperty]
    [GMCMPriority(302)]
    [GMCMRange(0, 50)]
    public int IridiumBarsPerGalaxyWeapon
    {
        get => this._iridiumBarsPerGalaxyWeapon;
        internal set
        {
            this._iridiumBarsPerGalaxyWeapon = Math.Max(value, 0);
        }
    }

    /// <summary>Gets a factor that can be used to reduce the Ruined Blade's damage-over-time effect.</summary>
    [JsonProperty]
    [GMCMPriority(303)]
    [GMCMRange(0, 4)]
    public float RuinBladeDotMultiplier
    {
        get => this._ruinBladeDotMultiplier;
        internal set
        {
            this._ruinBladeDotMultiplier = Math.Max(value, 0f);
        }
    }

    /// <summary>Gets a value indicating whether the Blade of Ruin can be deposited in chests.</summary>
    [JsonProperty]
    [GMCMPriority(304)]
    public bool CanStoreRuinBlade { get; internal set; } = false;

    /// <summary>Gets a value indicating the difficulty of the proven conditions for each virtue trial.</summary>
    [JsonProperty]
    [GMCMPriority(305)]
    public QuestDifficulty HeroQuestDifficulty
    {
        get => this._heroQuestDifficulty;
        internal set
        {
            if (value == this._heroQuestDifficulty)
            {
                return;
            }

            this._heroQuestDifficulty = value;
            if (Context.IsWorldReady && CombatModule.State.HeroQuest is { } quest)
            {
                Virtue.List.ForEach(virtue => quest.UpdateTrialProgress(virtue));
            }
        }
    }

    /// <summary>Gets a value indicating whether replace the starting Rusty Sword with a Wooden Blade.</summary>
    [JsonProperty]
    [GMCMPriority(310)]
    public bool WoodyReplacesRusty { get; internal set; } = true;
}
