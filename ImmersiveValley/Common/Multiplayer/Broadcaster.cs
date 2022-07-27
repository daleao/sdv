namespace DaLion.Common.Multiplayer;

#region using directives

using StardewValley.Menus;
using System.Threading.Tasks;

#endregion using directives

/// <summary>Provides methods for synchronous and asynchronous communication between remote online players.</summary>
public class Broadcaster
{
    private readonly IMultiplayerHelper _Helper;
    private readonly string _modID;

    public TaskCompletionSource<string> ResponseReceived = null!;

    /// <summary>Construct an instance.</summary>
    /// <param name="modID">The unique ID of the mod requesting an instance.</param>
    public Broadcaster(IMultiplayerHelper helper, string modID)
    {
        _Helper = helper;
        _modID = modID;
    }

    /// <summary>Send a synchronous message to all online peer.</summary>
    /// <param name="message">The message to send.</param>
    /// <param name="messageType">The message type.</param>
    public void Broadcast(string message, string messageType)
    {
        _Helper.SendMessage(message, messageType, new[] { _modID });
    }

    /// <summary>Send a synchronous message to a multiplayer peer.</summary>
    /// <param name="message">The message to send.</param>
    /// <param name="messageType">The message type.</param>
    /// <param name="playerId">The unique ID of the recipient.</param>
    public void Message(string message, string messageType, long playerId)
    {
        _Helper.SendMessage(message, messageType, new[] { _modID }, new[] { playerId });
    }

    /// <summary>Send a synchronous message to a multiplayer peer that should be received by an external mod.</summary>
    /// <param name="message">The message to send.</param>
    /// <param name="messageType">The message type.</param>
    /// <param name="playerId">The unique ID of the recipient.</param>
    public void Message(string message, string messageType, long playerId, string modId)
    {
        _Helper.SendMessage(message, messageType, new[] { modId }, new[] { playerId });
    }

    /// <summary>Send a synchronous message to the multiplayer host.</summary>
    /// <param name="message">The message to send.</param>
    /// <param name="messageType">The message type.</param>
    public void MessageHost(string message, string messageType)
    {
        _Helper.SendMessage(message, messageType, new[] { _modID }, new[] { Game1.MasterPlayer.UniqueMultiplayerID });
    }

    /// <summary>Send a synchronous message to the multiplayer host that should be received by an external mod.</summary>
    /// <param name="message">The message to send.</param>
    /// <param name="messageType">The message type.</param>
    public void MessageHost(string message, string messageType, string modId)
    {
        _Helper.SendMessage(message, messageType, new[] { modId }, new[] { Game1.MasterPlayer.UniqueMultiplayerID });
    }

    /// <summary>Send an asynchronous request to a multiplayer peer and await a response.</summary>
    /// <param name="message">The message to send.</param>
    /// <param name="messageType">The message type.</param>
    /// <param name="playerId">The unique ID of the recipient.</param>
    public async Task<string> RequestAsync(string message, string messageType, long playerId)
    {
        _Helper.SendMessage(message, messageType, new[] { _modID }, new[] { playerId });

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
    public static void SendDirectMessage(long playerID, LocalizedContentManager.LanguageCode code, string text)
    {
        Game1.server.sendMessage(playerID, Multiplayer.chatMessage, Game1.player, code, text);
    }
}