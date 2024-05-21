namespace DaLion.Professions.Commands;

#region using directives

using System.Linq;
using DaLion.Shared.Commands;
using DaLion.Shared.Extensions.Collections;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="ResetSkillLevelsCommand"/> class.</summary>
/// <param name="handler">The <see cref="CommandHandler"/> instance that handles this command.</param>
[UsedImplicitly]
internal sealed class ResetSkillLevelsCommand(CommandHandler handler)
    : ConsoleCommand(handler)
{
    /// <inheritdoc />
    public override string[] Triggers { get; } = ["reset"];

    /// <inheritdoc />
    public override string Documentation =>
        "Reset the level of the specified skills, or all skills if none are specified. Does not remove professions.";

    /// <inheritdoc />
    public override bool CallbackImpl(string trigger, string[] args)
    {
        if (args.Length == 0)
        {
            Skill.List.ForEach(s => s.Reset());
            CustomSkill.Loaded.ForEach(s => s.Value.Reset());
            return true;
        }

        foreach (var arg in args)
        {
            if (Skill.TryFromName(arg, true, out var skill))
            {
                skill.Reset();
            }
            else
            {
                var customSkill = CustomSkill.Loaded.Values.FirstOrDefault(s =>
                    string.Equals(s.DisplayName, arg, StringComparison.CurrentCultureIgnoreCase));
                if (customSkill is null)
                {
                    this.Handler.Log.W($"{arg}.is not a valid skill name.");
                    continue;
                }

                customSkill.Reset();
            }
        }

        return true;
    }
}
