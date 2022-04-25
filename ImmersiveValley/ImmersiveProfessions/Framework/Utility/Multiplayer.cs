namespace DaLion.Stardew.Professions.Framework.Utility;

#region using directives

using System.Threading.Tasks;
using StardewValley;
using StardewValley.Menus;

#endregion using directives

/// <summary>Provides methods for asynchronous communication between remote online players.</summary>
public static class Multiplayer
{
    public static TaskCompletionSource<string> ResponseReceived;

    /// <summary>Send an asynchronous request to a multiplayer peer and await a response.</summary>
    /// <param name="message">The message to send.</param>
    /// <param name="messageType">The message type.</param>
    /// <param name="playerId">The unique id of the recipient.</param>
    public static async Task<string> SendRequestAsync(string message, string messageType, long playerId)
    {
        ModEntry.ModHelper.Multiplayer.SendMessage(message, messageType, new[] {ModEntry.Manifest.UniqueID},
            new[] {playerId});

        ResponseReceived = new();
        return await ResponseReceived.Task;
    }

    /// <summary>Send a chat message to all players.</summary>
    /// <param name="text">The chat text to send.</param>
    /// <param name="error">Whether to format the text as an error.</param>
    public static void SendPublicChat(string text, bool error = false)
    {
        // format text
        if (error)
        {
            Game1.chatBox.activate();
            Game1.chatBox.setText("/color red");
            Game1.chatBox.chatBox.RecieveCommandInput('\r');
        }

        // send chat message
        // (Bypass Game1.chatBox.setText which doesn't handle long text well)
        Game1.chatBox.activate();
        Game1.chatBox.chatBox.reset();
        Game1.chatBox.chatBox.finalText.Add(new ChatSnippet(text, LocalizedContentManager.LanguageCode.en));
        Game1.chatBox.chatBox.updateWidth();
        Game1.chatBox.chatBox.RecieveCommandInput('\r');
    }

    /// <summary>Send a private message to a specified player.</summary>
    /// <param name="playerID">The player ID.</param>
    /// <param name="text">The text to send.</param>
    public static void SendDirectMessage(long playerID, string text)
    {
        Game1.server.sendMessage(playerID, StardewValley.Multiplayer.chatMessage, Game1.player,
            ModEntry.ModHelper.Content.CurrentLocaleConstant, text);
    }
}