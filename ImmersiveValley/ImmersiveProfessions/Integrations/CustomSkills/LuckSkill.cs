#nullable enable
namespace DaLion.Stardew.Professions.Integrations;

#region using directives

using System.Collections.Generic;
using System.Linq;
using StardewValley;
using StardewModdingAPI.Enums;

using Common.Extensions.Reflection;
using Common.Integrations;

#endregion using directives

public class LuckSkill : ICustomSkill
{
    /// <inheritdoc />
    public string StringId { get; }

    /// <inheritdoc />
    public string DisplayName { get; }

    /// <inheritdoc />
    public int CurrentExp => Game1.player.experiencePoints[(int) SkillType.Luck];

    /// <inheritdoc />
    public int CurrentLevel => Game1.player.LuckLevel;

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
    public Dictionary<int, (int first, int second)> ProfessionsByBranch { get; } = new();

    /// <inheritdoc />
    public Dictionary<int, string> ProfessionNamesById { get; } = new();

    /// <summary>Construct an instance.</summary>
    internal LuckSkill()
    {
        StringId = "spacechase0.LuckSkill";
        DisplayName = (string) "LuckSkill.I18n".ToType().RequireMethod("Skill_Name").Invoke(null, null)!;
        ProfessionIds = Enumerable.Range(30, 36).ToArray();
        TierOneProfessionIds = new[] {30, 31};
        TierTwoProfessionIds = new[] {32, 33, 34, 35};
        ProfessionsByBranch[TierOneProfessionIds[0]] = (TierTwoProfessionIds[0], TierTwoProfessionIds[1]);
        ProfessionsByBranch[TierOneProfessionIds[1]] = (TierTwoProfessionIds[2], TierTwoProfessionIds[3]);
        
        var professionsDict = ModEntry.LuckSkillApi!.GetProfessions();
        for (var i = 0; i < 6; ++i)
        {
            var professionId = professionsDict.Keys.ElementAt(i);
            var profession = professionsDict[professionId];
            var professionName = profession.Name;
            ProfessionNamesById[professionId] = professionName;
        }
    }
}