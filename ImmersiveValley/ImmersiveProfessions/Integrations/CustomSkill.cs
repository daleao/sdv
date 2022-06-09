using DaLion.Common.Extensions.Collections;

#nullable enable
namespace DaLion.Stardew.Professions.Integrations;

#region using directives

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using StardewValley;

using Common.Integrations;

#endregion using directives

public class CustomSkill : ICustomSkill
{
    private readonly Func<Farmer, string, int> _getCurrentExpFor =
        (Func<Farmer, string, int>) Delegate.CreateDelegate(typeof(Func<Farmer, string, int>),
            SpaceCoreIntegration.GetCustomSkillExp);

    /// <inheritdoc />
    public object Instance { get; }

    /// <inheritdoc />
    public string StringId { get; }

    /// <inheritdoc />
    public string DisplayName { get; }

    /// <inheritdoc />
    public int CurrentExp => _getCurrentExpFor(Game1.player, StringId);

    /// <inheritdoc />
    public int CurrentLevel => ModEntry.SpaceCoreApi?.GetLevelForCustomSkill(Game1.player, StringId) ?? 0;

    /// <inheritdoc />
    public int[] NewLevels =>
        ((List<KeyValuePair<string, int>>) SpaceCoreIntegration.GetCustomSkillNewLevels.GetValue(null)!)
        .Select(kvp => kvp.Value).ToArray();

    /// <inheritdoc />
    public int[] ProfessionIds { get; }

    /// <inheritdoc />
    public int[] TierOneProfessionIds { get; }

    /// <inheritdoc />
    public int[] TierTwoProfessionIds { get; }

    /// <inheritdoc />
    public Dictionary<int, (int left, int right)> ProfessionsByBranch { get; } = new();

    /// <inheritdoc />
    public Dictionary<int, string> ProfessionNamesById { get; } = new();

    /// <summary>Construct an instance.</summary>
    internal CustomSkill(string id)
    {
        StringId = id;
        Instance = SpaceCoreIntegration.GetCustomSkill.Invoke(null, new object?[] {id})!;
        DisplayName = (string) SpaceCoreIntegration.GetSkillName.Invoke(Instance, null)!;

        var professionPairs = ((IEnumerable) SpaceCoreIntegration.GetProfessionsForLevel.Invoke(Instance, null)!)
                .Cast<object>().ToList();
        var levelTenPairs =
            professionPairs.Where(pair => (int) SpaceCoreIntegration.GetPairLevel.Invoke(pair, null)! == 10).ToArray();
        var levelTenProfessions = levelTenPairs
            .Select(pair => SpaceCoreIntegration.GetFirstProfession.Invoke(pair, null))
            .Concat(levelTenPairs.Select(pair => SpaceCoreIntegration.GetSecondProfession.Invoke(pair, null)))
            .ToArray();
        var levelTenStringIds = levelTenProfessions.Select(p => (string) SpaceCoreIntegration.GetProfessionStringId.Invoke(p, null)!);
        var levelTenIds = levelTenStringIds.Select(pid => ModEntry.SpaceCoreApi!.GetProfessionId(StringId, pid)).ToArray();
        TierTwoProfessionIds = levelTenIds;

        var allProfessions = ((IEnumerable) SpaceCoreIntegration.GetProfessions.Invoke(Instance, null)!).Cast<object>()
            .ToList();
        var allStringIds =
            allProfessions.Select(p => (string) SpaceCoreIntegration.GetProfessionStringId.Invoke(p, null)!);
        var allIds = allStringIds.Select(pid => ModEntry.SpaceCoreApi!.GetProfessionId(StringId, pid)).ToArray();
        ProfessionIds = allIds;

        var levelFiveIds = allIds.Except(levelTenIds).ToArray();
        TierOneProfessionIds = levelFiveIds;

        ProfessionsByBranch[TierOneProfessionIds[0]] = (TierTwoProfessionIds[0], TierTwoProfessionIds[1]);
        ProfessionsByBranch[TierOneProfessionIds[1]] = (TierTwoProfessionIds[2], TierTwoProfessionIds[3]);

        for (var i = 0; i < 6; ++i)
            ProfessionNamesById[allIds[i]] = (string) SpaceCoreIntegration.GetProfessionName.Invoke(allProfessions[i], null)!;
    }
}