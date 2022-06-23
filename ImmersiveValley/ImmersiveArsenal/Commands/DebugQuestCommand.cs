namespace DaLion.Stardew.Arsenal.Commands;

#region using directives

using System.Linq;
using StardewValley;

using Common;
using Common.Commands;

#endregion using directives

internal class DebugQuestCommand : ICommand
{
    /// <inheritdoc />
    public string Trigger => "debug_quest";

    /// <inheritdoc />
    public string Documentation => "Advance to the final stage of Qi's Final Challenge quest.";

    /// <inheritdoc />
    public void Callback(string[] args)
    {
        if (Game1.player.hasOrWillReceiveMail("QiChallengeComplete"))
        {
            if (!args.Any(arg => arg is "--force" or "-f"))
            {
                Log.W("Already completed the Qi Challenge questline. Use parameter '--force', '-f' to forcefully reset.");
                return;
            }

            Game1.player.RemoveMail("QiChallengeComplete");
        }

        if (!Game1.player.hasOrWillReceiveMail("skullCave"))
        {
            Game1.player.mailReceived.Add("skullCave");
            Log.I("Added 'skullCave' to mail received.");
        }

        if (!Game1.player.hasOrWillReceiveMail("QiChallengeFirst"))
        {
            Game1.player.mailReceived.Add("QiChallengeFirst");
            Log.I("Added 'QiChallengeFirst' to mail received.");
        }

        Game1.player.addQuest(ModEntry.QiChallengeFinalQuestId);
        Log.I($"Added Qi's Final Challenge to {Game1.player.Name}'s active quests.");
    }
}