namespace DaLion.Common.Multiplayer;

#region using directives

using System.Threading.Tasks;
using StardewValley.Menus;
using Multiplayer = StardewValley.Multiplayer;

#endregion using directives

/// <summary>Provides methods for synchronous and asynchronous communication between remote online players.</summary>
public class Broadcaster
{
    /// <summary> The <see cref="IMultiplayerHelper"/> API of the current<see cref= "IMod"/>.</summary>
    private readonly IMultiplayerHelper _helper;

    /// <summary>The unique ID of the active mod.</summary>
    private readonly string _modId;

    /// <summary>Initializes a new instance of the <see cref="Broadcaster"/> class.</summary>
    /// <param name="helper">The <see cref="IMultiplayerHelper"/> API of the current <see cref="IMod"/>.</param>
    /// <param name="modId">The unique ID of the active mod.</param>
    public Broadcaster(IMultiplayerHelper helper, string modId)
    {
        this._helper = helper;
        this._modId = modId;
    }

    /// <summary>Gets the cached the response from the latest asynchronous <see cref="Task"/>.</summary>
    public TaskCompletionSource<string>? ResponseReceived { get; private set; }

    /// <summary>Sends a chat message to all players.</summary>
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

    /// <summary>Sends a private message to the specified player.</summary>
    /// <param name="playerId">The player ID.</param>
    /// <param name="code">The <see cref="LocalizedContentManager.LanguageCode"/>.</param>
    /// <param name="text">The text to send.</param>
    public static void SendDirectMessage(long playerId, LocalizedContentManager.LanguageCode code, string text)
    {
        Game1.server.sendMessage(playerId, Multiplayer.chatMessage, Game1.player, code, text);
    }

    /// <summary>Sends a synchronous <paramref name="message"/> to all online peer.</summary>
    /// <param name="message">The message to send.</param>
    /// <param name="messageType">The message type.</param>
    public void Broadcast(string message, string messageType)
    {
        this._helper.SendMessage(message, messageType, new[] { this._modId });
    }

    /// <summary>Sends a synchronous <paramref name="message"/> to a multiplayer peer.</summary>
    /// <param name="message">The message to send.</param>
    /// <param name="messageType">The message type.</param>
    /// <param name="playerId">The unique ID of the recipient.</param>
    public void Message(string message, string messageType, long playerId)
    {
        this._helper.SendMessage(message, messageType, new[] { this._modId }, new[] { playerId });
    }

    /// <summary>
    ///     Sends a synchronous <paramref name="message"/> to a multiplayer peer that should be received by an external
    ///     mod.
    /// </summary>
    /// <param name="message">The message to send.</param>
    /// <param name="messageType">The message type.</param>
    /// <param name="playerId">The unique ID of the recipient player.</param>
    /// <param name="modId">The unique ID of the recipient mod.</param>
    public void Message(string message, string messageType, long playerId, string modId)
    {
        this._helper.SendMessage(message, messageType, new[] { modId }, new[] { playerId });
    }

    /// <summary>Sends a synchronous <paramref name="message"/> to the multiplayer host.</summary>
    /// <param name="message">The message to send.</param>
    /// <param name="messageType">The message type.</param>
    public void MessageHost(string message, string messageType)
    {
        this._helper.SendMessage(
            message, messageType, new[] { this._modId }, new[] { Game1.MasterPlayer.UniqueMultiplayerID });
    }

    /// <summary>
    ///     Sends a synchronous <paramref name="message"/> to the multiplayer host that should be received by an external
    ///     mod.
    /// </summary>
    /// <param name="message">The message to send.</param>
    /// <param name="messageType">The message type.</param>
    /// <param name="modId">The unique ID of the recipient mod.</param>
    public void MessageHost(string message, string messageType, string modId)
    {
        this._helper.SendMessage(
            message, messageType, new[] { modId }, new[] { Game1.MasterPlayer.UniqueMultiplayerID });
    }

    /// <summary>Sends an asynchronous request to a multiplayer peer and await a response.</summary>
    /// <param name="message">The message to send.</param>
    /// <param name="messageType">The message type.</param>
    /// <param name="playerId">The unique ID of the recipient player.</param>
    /// <returns>A <see cref="Task"/> that should resolve to the peer's response.</returns>
    public async Task<string> RequestAsync(string message, string messageType, long playerId)
    {
        this._helper.SendMessage(message, messageType, new[] { this._modId }, new[] { playerId });

        this.ResponseReceived = new TaskCompletionSource<string>();
        return await this.ResponseReceived.Task;
    }
}
