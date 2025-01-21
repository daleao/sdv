namespace DaLion.Professions.Commands;

#region using directives

using System.Collections.Generic;
using System.Linq;
using DaLion.Shared.Commands;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="ClearNewLevelsCommand"/> class.</summary>
/// <param name="handler">The <see cref="CommandHandler"/> instance that handles this command.</param>
[UsedImplicitly]
internal sealed class ClearNewLevelsCommand(CommandHandler handler)
    : ConsoleCommand(handler)
{
    /// <inheritdoc />
    public override string[] Triggers { get; } = ["clear_new_levels"];

    /// <inheritdoc />
    public override string Documentation =>
        "Clear the player's cache of new levels for the specified skills, or all vanilla skills if none are specified.";

    /// <inheritdoc />
    public override bool CallbackImpl(string trigger, string[] args)
    {
        if (args.Length == 0)
        {
            Game1.player.newLevels.Clear();
            return true;
        }

        foreach (var arg in args)
        {
            if (Skill.TryFromName(arg, true, out var skill))
            {
                Game1.player.newLevels.Set(Game1.player.newLevels
                    .Where(p => p.X != skill)
                    .ToList());
            }
            else
            {
                var customSkill = CustomSkill.Loaded.Values.FirstOrDefault(s =>
                    string.Equals(s.DisplayName, arg, StringComparison.CurrentCultureIgnoreCase));
                if (customSkill is null)
                {
                    Log.W($"Ignoring unknown skill {arg}.");
                    continue;
                }

                var newLevels = Reflector
                    .GetStaticFieldGetter<List<KeyValuePair<string, int>>>(typeof(SCSkills), "NewLevels")
                    .Invoke();
                Reflector
                    .GetStaticFieldSetter<List<KeyValuePair<string, int>>>(typeof(SCSkills), "NewLevels")
                    .Invoke(newLevels.Where(pair => pair.Key != customSkill.StringId).ToList());
            }
        }

        return true;
    }
}
