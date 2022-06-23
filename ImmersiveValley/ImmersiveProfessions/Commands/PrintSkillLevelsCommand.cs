namespace DaLion.Stardew.Professions.Commands;

#region using directives

using System.Linq;
using StardewValley;

using Common;
using Common.Commands;
using Framework;

#endregion using directives

internal class PrintSkillLevelsCommand : ICommand
{
    /// <inheritdoc />
    public string Trigger => "levels";

    /// <inheritdoc />
    public string Documentation => "Print the current skill levels and experience.";

    /// <inheritdoc />
    public void Callback(string[] args)
    {
        Log.I(
            $"Farming level: {Game1.player.GetUnmodifiedSkillLevel(Skill.Farming)} ({Game1.player.experiencePoints[Skill.Farming]} exp)");
        Log.I(
            $"Fishing level: {Game1.player.GetUnmodifiedSkillLevel(Skill.Fishing)} ({Game1.player.experiencePoints[Skill.Fishing]} exp)");
        Log.I(
            $"Foraging level: {Game1.player.GetUnmodifiedSkillLevel(Skill.Foraging)} ({Game1.player.experiencePoints[Skill.Foraging]} exp)");
        Log.I(
            $"Mining level: {Game1.player.GetUnmodifiedSkillLevel(Skill.Mining)} ({Game1.player.experiencePoints[Skill.Mining]} exp)");
        Log.I(
            $"Combat level: {Game1.player.GetUnmodifiedSkillLevel(Skill.Combat)} ({Game1.player.experiencePoints[Skill.Combat]} exp)");

        if (ModEntry.LuckSkillApi is not null)
            Log.I(
                $"Luck level: {Game1.player.GetUnmodifiedSkillLevel(Skill.Luck)} ({Game1.player.experiencePoints[Skill.Luck]} exp)");

        foreach (var skill in ModEntry.CustomSkills.Values.OfType<CustomSkill>())
            Log.I($"{skill.DisplayName} level: {skill.CurrentLevel} ({skill.CurrentExp} exp)");
    }
}