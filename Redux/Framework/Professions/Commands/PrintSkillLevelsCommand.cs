namespace DaLion.Redux.Framework.Professions.Commands;

#region using directives

using System.Linq;
using DaLion.Shared.Commands;

#endregion using directives

[UsedImplicitly]
internal sealed class PrintSkillLevelsCommand : ConsoleCommand
{
    /// <summary>Initializes a new instance of the <see cref="PrintSkillLevelsCommand"/> class.</summary>
    /// <param name="handler">The <see cref="CommandHandler"/> instance that handles this command.</param>
    internal PrintSkillLevelsCommand(CommandHandler handler)
        : base(handler)
    {
    }

    /// <inheritdoc />
    public override string[] Triggers { get; } =
    {
        "print_levels", "levels", "print_skills", "skills", "print_exp", "experience", "exp",
    };

    /// <inheritdoc />
    public override string Documentation => "Print the player's current skill levels and experience.";

    /// <inheritdoc />
    public override void Callback(string[] args)
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

        if (Framework.Integrations.LuckSkillApi is not null)
        {
            Log.I(
                $"Luck level: {Game1.player.GetUnmodifiedSkillLevel(Skill.Luck)} ({Game1.player.experiencePoints[Skill.Luck]} exp)");
        }

        foreach (var skill in SCSkill.Loaded.Values)
        {
            Log.I($"{skill.DisplayName} level: {skill.CurrentLevel} ({skill.CurrentExp} exp)");
        }
    }
}
