namespace DaLion.Stardew.Professions.Commands;

#region using directives

using System;
using System.Collections.Generic;
using System.Linq;
using StardewValley;

using Common;
using Common.Commands;
using Common.Integrations;
using Framework;

#endregion using directives

internal class ClearNewLevelsCommand : ICommand
{
    /// <inheritdoc />
    public string Trigger => "clear_new_levels";

    /// <inheritdoc />
    public string Documentation => "Clear the player's cache of new levels for te specified skills.";

    /// <inheritdoc />
    public void Callback(string[] args)
    {
        if (!args.Any())
            Game1.player.newLevels.Clear();
        else
            foreach (var arg in args)
            {
                if (Skill.TryFromName(arg, true, out var skill))
                {
                    Game1.player.newLevels.Set(Game1.player.newLevels.Where(p => p.X != skill).ToList());
                }
                else if (ModEntry.CustomSkills.Values.Any(s =>
                             string.Equals(s.DisplayName, arg, StringComparison.CurrentCultureIgnoreCase)))
                {
                    var customSkill = ModEntry.CustomSkills.Values.Single(s =>
                        string.Equals(s.DisplayName, arg, StringComparison.CurrentCultureIgnoreCase));
                    var newLevels =
                        (List<KeyValuePair<string, int>>)ExtendedSpaceCoreAPI.GetCustomSkillNewLevels.GetValue(null)!;
                    ExtendedSpaceCoreAPI.GetCustomSkillNewLevels.SetValue(null,
                        newLevels.Where(pair => pair.Key != customSkill.StringId).ToList());
                }
                else
                {
                    Log.W($"Ignoring unknown skill {arg}.");
                }
            }
    }
}