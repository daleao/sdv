﻿namespace DaLion.Taxes.Framework;

#region using directives

using DaLion.Shared.Extensions.SMAPI;

#endregion using directives

/// <summary>Delivers mail.</summary>
internal static class PostalService
{
    /// <summary>Sends the post to the local player.</summary>
    /// <param name="mail">The <see cref="Mail"/> to send.</param>
    internal static void Send(Mail mail)
    {
        ModHelper.GameContent.InvalidateCacheAndLocalized("Data/mail");
        Game1.player.mailForTomorrow.Add($"{UniqueId}_{mail}");
    }

    /// <summary>Checks whether the local player has received this post.</summary>
    /// <param name="mail">The <see cref="Mail"/> to be sent.</param>
    /// <returns><see langword="true"/> if the player has or will receive the post, otherwise <see langword="false"/>.</returns>
    internal static bool HasSent(Mail mail)
    {
        return Game1.player.hasOrWillReceiveMail($"{UniqueId}_{mail}");
    }
}
