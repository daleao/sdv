using System.Threading.Tasks;

namespace TheLion.Stardew.Professions.Framework.Utility;

public static class Multiplayer
{
    public static TaskCompletionSource<string> ResponseReceived;

    public static async Task<string> SendRequestAsync(string message, string messageType, long playerId)
    {
        ModEntry.ModHelper.Multiplayer.SendMessage(message, messageType, new[] {ModEntry.Manifest.UniqueID},
            new[] {playerId});

        ResponseReceived = new();
        return await ResponseReceived.Task;
    }
}