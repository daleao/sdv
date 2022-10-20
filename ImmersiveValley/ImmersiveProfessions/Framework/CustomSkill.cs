namespace DaLion.Stardew.Professions.Framework;

#region using directives

using System.Collections.Generic;
using System.Linq;
using DaLion.Common.Integrations.SpaceCore;

#endregion using directives

/// <summary>Represents a SpaceCore-provided custom skill.</summary>
public sealed class CustomSkill : ISkill
{
    private readonly ISpaceCoreApi _api;

    /// <summary>Initializes a new instance of the <see cref="CustomSkill"/> class.</summary>
    /// <param name="id">The unique id of skill.</param>
    /// <param name="api">The <see cref="ISpaceCoreApi"/>.</param>
    internal CustomSkill(string id, ISpaceCoreApi api)
    {
        this._api = api;
        this.StringId = id;

        var instance = ExtendedSpaceCoreApi.GetCustomSkillInstance.Value(id);
        this.DisplayName = ExtendedSpaceCoreApi.GetSkillName.Value(instance);

        var professions = ExtendedSpaceCoreApi.GetProfessions.Value(instance).Cast<object>()
            .ToList();
        var i = 0;
        foreach (var profession in professions)
        {
            var professionStringId = ExtendedSpaceCoreApi.GetProfessionStringId.Value(profession);
            var displayName = ExtendedSpaceCoreApi.GetProfessionDisplayName.Value(profession);
            var description = ExtendedSpaceCoreApi.GetProfessionDescription.Value(profession);
            var vanillaId = ExtendedSpaceCoreApi.GetProfessionVanillaId.Value(profession);
            var level = i++ < 2 ? 5 : 10;
            this.Professions.Add(
                new CustomProfession(professionStringId, displayName, description, vanillaId, level, this));
        }

        if (this.Professions.Count != 6)
        {
            ThrowHelper.ThrowInvalidOperationException(
                $"The custom skill {id} did not provide the expected number of professions.");
        }

        this.ProfessionPairs[-1] = new ProfessionPair(this.Professions[0], this.Professions[1], null, 5);
        this.ProfessionPairs[this.Professions[0].Id] =
            new ProfessionPair(this.Professions[2], this.Professions[3], this.Professions[0], 10);
        this.ProfessionPairs[this.Professions[1].Id] =
            new ProfessionPair(this.Professions[4], this.Professions[5], this.Professions[1], 10);
    }

    /// <inheritdoc />
    public string StringId { get; }

    /// <inheritdoc />
    public string DisplayName { get; }

    /// <inheritdoc />
    public int CurrentExp => ExtendedSpaceCoreApi.GetCustomSkillExp.Value(Game1.player, this.StringId);

    /// <inheritdoc />
    public int CurrentLevel => this._api.GetLevelForCustomSkill(Game1.player, this.StringId);

    /// <inheritdoc />
    public float BaseExperienceMultiplier =>
        ModEntry.Config.CustomSkillExpMultipliers.TryGetValue(this.StringId, out var multiplier) ? multiplier : 1f;

    /// <inheritdoc />
    public IEnumerable<int> NewLevels => ExtendedSpaceCoreApi.GetCustomSkillNewLevels.Value()
        .Where(pair => pair.Key == this.StringId).Select(pair => pair.Value);

    /// <inheritdoc />
    public IList<IProfession> Professions { get; } = new List<IProfession>();

    /// <inheritdoc />
    public IDictionary<int, ProfessionPair> ProfessionPairs { get; } = new Dictionary<int, ProfessionPair>();

    /// <summary>Gets the currently loaded <see cref="CustomSkill"/>s.</summary>
    /// <remarks>Also includes <see cref="LuckSkill"/>, despite it being technically a vanilla skill.</remarks>
    internal static Dictionary<string, ISkill> Loaded { get; } = new();
}
