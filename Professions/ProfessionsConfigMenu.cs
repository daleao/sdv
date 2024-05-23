namespace DaLion.Professions;

#region using directives

using System.Collections.Generic;
using System.Linq;
using DaLion.Shared.Integrations.GMCM;

#endregion using directives

internal sealed class ProfessionsConfigMenu : GMCMBuilder<ProfessionsConfigMenu>
{
    private readonly Dictionary<string, object> _changedFields = [];
    private bool _reload;

    /// <summary>Initializes a new instance of the <see cref="ProfessionsConfigMenu"/> class.</summary>
    internal ProfessionsConfigMenu()
        : base(ModHelper.Translation, ModHelper.ModRegistry, ProfessionsMod.Manifest)
    {
    }

    /// <inheritdoc />
    protected override void BuildMenu()
    {
        this.BuildImplicitly(() => Config);
        this.OnFieldChanged((name, newValue) =>
        {
            if (this._changedFields.TryGetValue(name, out var oldValue))
            {
                if (oldValue != newValue)
                {
                    this._changedFields[name] = newValue;
                    this._reload = true;
                }
            }
            else
            {
                this._changedFields[name] = newValue;
                this._reload = true;
            }
        });
    }

    /// <inheritdoc />
    protected override void ResetConfig()
    {
        Config = new ProfessionsConfig();
    }

    /// <inheritdoc />
    protected override void SaveAndApply()
    {
        ModHelper.WriteConfig(Config);

        if (this._changedFields.Count > 0)
        {
            var changedText = this._changedFields.Aggregate(
                "[Config]: Changed the following config settings:",
                (current, next) => current + $"\n\"{next.Key}\": \"{next.Value}\"");
            Log.D(changedText);
            this._changedFields.Clear();
        }

        if (!this._reload)
        {
            return;
        }

        this.Reload();
        this._reload = false;
    }

    [UsedImplicitly]
    private static void ArtisanMachinesOverride()
    {
        Instance!.AddDynamicListOption(
            I18n.Gmcm_ArtisanMachines_Title,
            I18n.Gmcm_ArtisanMachines_Desc,
            () => [.. Config.ArtisanMachines],
            values => Config.ArtisanMachines = values.ToHashSet(),
            id: "ArtisanMachines");
    }

    [UsedImplicitly]
    private static void AnimalDerivedGoodsOverride()
    {
        Instance!.AddDynamicListOption(
            I18n.Gmcm_AnimalDerivedGoods_Title,
            I18n.Gmcm_AnimalDerivedGoods_Desc,
            () => Config.AnimalDerivedGoods.ToList(),
            values => Config.AnimalDerivedGoods = values.ToHashSet(),
            id: "AnimalDerivedGoods");
    }

    [UsedImplicitly]
    private static void SkillExpMultipliersOverride()
    {
        foreach (var (skillId, multiplier) in Config.Skills.BaseMultipliers)
        {
            if (Skill.TryFromName(skillId, out var skill))
            {
                Instance!.AddFloatSlider(
                    () => I18n.Gmcm_SkillExpMultipliers_Title(skill.DisplayName),
                    () => I18n.Gmcm_SkillExpMultipliers_Desc(skill.DisplayName),
                    config => config.Skills.BaseMultipliers[skill.Name],
                    (config, value) => config.Skills.BaseMultipliers[skill.Name] = value,
                    () => Config,
                    0.2f,
                    2f,
                    id: "SkillExpMultipliers." + skill.Name);
                continue;
            }

            if (CustomSkill.Loaded.TryGetValue(skillId, out var customSkill))
            {
                Instance!.AddFloatSlider(
                    () => I18n.Gmcm_SkillExpMultipliers_Title(customSkill.DisplayName),
                    () => I18n.Gmcm_SkillExpMultipliers_Desc(customSkill.DisplayName),
                    config => config.Skills.BaseMultipliers[customSkill.StringId],
                    (config, value) => config.Skills.BaseMultipliers[customSkill.StringId] = value,
                    () => Config,
                    0.2f,
                    2f,
                    id: "SkillExpMultipliers." + customSkill.StringId);
            }
        }
    }
}
