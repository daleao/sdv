namespace DaLion.Stardew.Professions.Commands;

#region using directives

using Common;
using Common.Commands;
using Common.Integrations;
using Framework;
using JetBrains.Annotations;
using StardewValley;
using System;
using System.Linq;

#endregion using directives

[UsedImplicitly]
internal sealed class ClearNewLevelsCommand : ConsoleCommand
{
    /// <summary>Construct an instance.</summary>
    /// <param name="handler">The <see cref="CommandHandler"/> instance that handles this command.</param>
    internal ClearNewLevelsCommand(CommandHandler handler)
        : base(handler) { }

    /// <inheritdoc />
    public override string Trigger => "clear_new_levels";

    /// <inheritdoc />
    public override string Documentation => "Clear the player's cache of new levels for te specified skills.";

    /// <inheritdoc />
    public override void Callback(string[] args)
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
                    var newLevels = ExtendedSpaceCoreAPI.GetCustomSkillNewLevels();
                    ExtendedSpaceCoreAPI.SetCustomSkillNewLevels(newLevels
                        .Where(pair => pair.Key != customSkill.StringId).ToList());
                }
                else
                {
                    Log.W($"Ignoring unknown skill {arg}.");
                }
            }
    }
}