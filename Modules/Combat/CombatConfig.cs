namespace DaLion.Overhaul.Modules.Combat;

#region using directives

using DaLion.Overhaul.Modules.Combat.Configs;
using DaLion.Shared.Constants;
using DaLion.Shared.Extensions.SMAPI;
using DaLion.Shared.Integrations.GMCM.Attributes;
using Newtonsoft.Json;
using StardewValley.Objects;

#endregion using directives

/// <summary>The user-configurable settings for CMBT.</summary>
public sealed class CombatConfig
{
    private bool _newResistanceFormula = true;

    /// <inheritdoc cref="WeaponsSlingshotsConfig"/>
    [JsonProperty]
    [GMCMInnerConfig("DaLion.Overhaul.Modules.Combat/WeaponsSlingshots", "cmbt.weapons_slingshots", true)]
    public WeaponsSlingshotsConfig WeaponsSlingshots { get; internal set; } = new();

    /// <inheritdoc cref="RingsEnchantmentsConfig"/>
    [JsonProperty]
    [GMCMInnerConfig("DaLion.Overhaul.Modules.Combat/RingsEnchantments", "cmbt.rings_enchantments", true)]
    public RingsEnchantmentsConfig RingsEnchantments { get; internal set; } = new();

    /// <inheritdoc cref="QuestsConfig"/>
    [JsonProperty]
    [GMCMInnerConfig("DaLion.Overhaul.Modules.Combat/Quests", "cmbt.quests", true)]
    public QuestsConfig Quests { get; internal set; } = new();

    /// <inheritdoc cref="EnemiesConfig"/>
    [JsonProperty]
    [GMCMInnerConfig("DaLion.Overhaul.Modules.Combat/Enemies", "cmbt.enemies", true)]
    public EnemiesConfig Enemies { get; internal set; } = new();

    /// <inheritdoc cref="ControlsUiConfig"/>
    [JsonProperty]
    [GMCMInnerConfig("DaLion.Overhaul.Modules.Combat/ControlsUi", "controls_ui", true)]
    public ControlsUiConfig ControlsUi { get; internal set; } = new();

    /// <summary>Gets a value indicating whether to overhaul the defense stat with better scaling and other features.</summary>
    [JsonProperty]
    [GMCMSection("cmbt.general")]
    [GMCMPriority(0)]
    public bool NewResistanceFormula
    {
        get => this._newResistanceFormula;
        internal set
        {
            if (value == this._newResistanceFormula)
            {
                return;
            }

            this._newResistanceFormula = value;
            ModHelper.GameContent.InvalidateCacheAndLocalized("Data/ObjectInformation");
            if (!Context.IsWorldReady)
            {
                return;
            }

            Utility.iterateAllItems(item =>
            {
                if (item is not Ring { ParentSheetIndex: ObjectIds.TopazRing } topaz)
                {
                    return;
                }

                var key = "rings.topaz.desc" + (value ? "resist" : "defense");
                topaz.description = _I18n.Get(key);
            });
        }
    }

    /// <summary>Gets a value indicating whether to overhaul the knockback stat adding collision damage.</summary>
    [JsonProperty]
    [GMCMSection("cmbt.general")]
    [GMCMPriority(1)]
    public bool KnockbackHurts { get; internal set; } = true;

    /// <summary>Gets a value indicating whether back attacks gain double crit. chance.</summary>
    [JsonProperty]
    [GMCMSection("cmbt.general")]
    [GMCMPriority(3)]
    public bool CriticalBackAttacks { get; internal set; } = true;

    /// <summary>Gets a value indicating whether to enable status conditions like Bleed and Stun on enemies.</summary>
    [JsonProperty]
    [GMCMSection("cmbt.general")]
    [GMCMPriority(4)]
    public bool EnableStatusConditions { get; internal set; } = true;
}
