namespace DaLion.Overhaul.Modules.Professions.Commands;

#region using directives

using DaLion.Overhaul.Modules.Professions.Integrations;
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
    public override void Callback(string trigger, string[] args)
    {
        Log.I(
            $"Farming level: {Game1.player.GetUnmodifiedSkillLevel(VanillaSkill.Farming)} ({Game1.player.experiencePoints[VanillaSkill.Farming]} exp)");
        Log.I(
            $"Fishing level: {Game1.player.GetUnmodifiedSkillLevel(VanillaSkill.Fishing)} ({Game1.player.experiencePoints[VanillaSkill.Fishing]} exp)");
        Log.I(
            $"Foraging level: {Game1.player.GetUnmodifiedSkillLevel(VanillaSkill.Foraging)} ({Game1.player.experiencePoints[VanillaSkill.Foraging]} exp)");
        Log.I(
            $"Mining level: {Game1.player.GetUnmodifiedSkillLevel(VanillaSkill.Mining)} ({Game1.player.experiencePoints[VanillaSkill.Mining]} exp)");
        Log.I(
            $"Combat level: {Game1.player.GetUnmodifiedSkillLevel(VanillaSkill.Combat)} ({Game1.player.experiencePoints[VanillaSkill.Combat]} exp)");

        if (LuckSkillIntegration.Instance?.IsRegistered == true)
        {
            Log.I(
                $"Luck level: {Game1.player.GetUnmodifiedSkillLevel(VanillaSkill.Luck)} ({Game1.player.experiencePoints[VanillaSkill.Luck]} exp)");
        }

        foreach (var skill in CustomSkill.Loaded.Values)
        {
            Log.I($"{skill.DisplayName} level: {skill.CurrentLevel} ({skill.CurrentExp} exp)");
        }
    }
}
