#nullable enable
namespace DaLion.Stardew.Professions.Framework;

#region using directives

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using StardewValley;

using Common.Integrations;

#endregion using directives

/// <summary>Represents a SpaceCore-provided custom skill.</summary>
public sealed class CustomSkill : ISkill
{
    private delegate int GetExperienceForDelegate(Farmer farmer, string skillName);

    private readonly GetExperienceForDelegate _GetCurrentExpFor =
        ExtendedSpaceCoreAPI.GetCustomSkillExp.CreateDelegate<GetExperienceForDelegate>();

    private readonly ISpaceCoreAPI _api;

    /// <inheritdoc />
    public string StringId { get; }

    /// <inheritdoc />
    public string DisplayName { get; }

    /// <inheritdoc />
    public int CurrentExp => _GetCurrentExpFor(Game1.player, StringId);

    /// <inheritdoc />
    public int CurrentLevel => _api.GetLevelForCustomSkill(Game1.player, StringId);

    /// <inheritdoc />
    public IEnumerable<int> NewLevels =>
        ((List<KeyValuePair<string, int>>) ExtendedSpaceCoreAPI.GetCustomSkillNewLevels.GetValue(null)!)
        .Where(pair => pair.Key == StringId).Select(pair => pair.Value);

    /// <inheritdoc />
    public IList<IProfession> Professions { get; } = new List<IProfession>();

    /// <inheritdoc />
    public IDictionary<int, ProfessionPair> ProfessionPairs { get; } = new Dictionary<int, ProfessionPair>();

    /// <summary>Construct an instance.</summary>
    internal CustomSkill(string id, ISpaceCoreAPI api)
    {
        _api = api;
        StringId = id;
        
        var instance = ExtendedSpaceCoreAPI.GetCustomSkillInstance.Invoke(null, new object?[] {id})!;
        DisplayName = (string) ExtendedSpaceCoreAPI.GetSkillName.Invoke(instance, null)!;

        var professions = ((IEnumerable) ExtendedSpaceCoreAPI.GetProfessions.Invoke(instance, null)!).Cast<object>()
            .ToList();
        var i = 0;
        foreach (var profession in professions)
        {
            var professionStringId = (string) ExtendedSpaceCoreAPI.GetProfessionStringId.Invoke(profession, null)!;
            var displayName = (string) ExtendedSpaceCoreAPI.GetProfessionDisplayName.Invoke(profession, null)!;
            var description = (string) ExtendedSpaceCoreAPI.GetProfessionDescription.Invoke(profession, null)!;
            var vanillaId = (int) ExtendedSpaceCoreAPI.GetProfessionVanillaId.Invoke(profession, null)!;
            var level = i++ < 2 ? 5 : 10;
            Professions.Add(new CustomProfession(professionStringId, displayName, description, vanillaId, level, this));
        }

        if (Professions.Count != 6)
            throw new InvalidOperationException(
                $"The custom skill {id} did not provide the expected number of professions.");

        ProfessionPairs[-1] = new(Professions[0], Professions[1], null, 5);
        ProfessionPairs[Professions[0].Id] = new(Professions[2], Professions[3], Professions[0], 10);
        ProfessionPairs[Professions[1].Id] = new(Professions[4], Professions[5], Professions[1], 10);
    }
}