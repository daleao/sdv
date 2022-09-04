#nullable enable
namespace DaLion.Stardew.Professions.Framework;

#region using directives

using Common.Integrations.SpaceCore;
using LinqFasterer;
using System.Collections.Generic;
using System.Linq;

#endregion using directives

/// <summary>Represents a SpaceCore-provided custom skill.</summary>
public sealed class CustomSkill : ISkill
{
    private readonly ISpaceCoreAPI _api;

    /// <remarks>Also includes <see cref="LuckSkill"/>, despite it being technically a vanilla skill.</remarks>
    internal static Dictionary<string, ISkill> LoadedSkills { get; set; } = new();

    /// <inheritdoc />
    public string StringId { get; }

    /// <inheritdoc />
    public string DisplayName { get; }

    /// <inheritdoc />
    public int CurrentExp => ExtendedSpaceCoreAPI.GetCustomSkillExp.Value(Game1.player, StringId);

    /// <inheritdoc />
    public int CurrentLevel => _api.GetLevelForCustomSkill(Game1.player, StringId);

    /// <inheritdoc />
    public float BaseExperienceMultiplier => ModEntry.Config.CustomSkillExpMultipliers.TryGetValue(StringId, out var multiplier) ? multiplier : 1f;

    /// <inheritdoc />
    public IEnumerable<int> NewLevels => ExtendedSpaceCoreAPI.GetCustomSkillNewLevels.Value()
        .WhereF(pair => pair.Key == StringId).SelectF(pair => pair.Value);

    /// <inheritdoc />
    public IList<IProfession> Professions { get; } = new List<IProfession>();

    /// <inheritdoc />
    public IDictionary<int, ProfessionPair> ProfessionPairs { get; } = new Dictionary<int, ProfessionPair>();

    /// <summary>Construct an instance.</summary>
    internal CustomSkill(string id, ISpaceCoreAPI api)
    {
        _api = api;
        StringId = id;

        var instance = ExtendedSpaceCoreAPI.GetCustomSkillInstance.Value(id);
        DisplayName = ExtendedSpaceCoreAPI.GetSkillName.Value(instance);

        var professions = ExtendedSpaceCoreAPI.GetProfessions.Value(instance).Cast<object>()
            .ToList();
        var i = 0;
        foreach (var profession in professions)
        {
            var professionStringId = ExtendedSpaceCoreAPI.GetProfessionStringId.Value(profession);
            var displayName = ExtendedSpaceCoreAPI.GetProfessionDisplayName.Value(profession);
            var description = ExtendedSpaceCoreAPI.GetProfessionDescription.Value(profession);
            var vanillaId = ExtendedSpaceCoreAPI.GetProfessionVanillaId.Value(profession);
            var level = i++ < 2 ? 5 : 10;
            Professions.Add(new CustomProfession(professionStringId, displayName, description, vanillaId, level, this));
        }

        if (Professions.Count != 6)
            ThrowHelper.ThrowInvalidOperationException(
                $"The custom skill {id} did not provide the expected number of professions.");

        ProfessionPairs[-1] = new(Professions[0], Professions[1], null, 5);
        ProfessionPairs[Professions[0].Id] = new(Professions[2], Professions[3], Professions[0], 10);
        ProfessionPairs[Professions[1].Id] = new(Professions[4], Professions[5], Professions[1], 10);
    }
}