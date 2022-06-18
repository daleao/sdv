namespace DaLion.Stardew.Professions.Framework.Events.Player;

#region using directives

using JetBrains.Annotations;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewModdingAPI.Events;

using Extensions;
using Utility;

#endregion using directives

[UsedImplicitly]
internal sealed class VerifyHudThemeWarpedEvent : WarpedEvent
{
    /// <inheritdoc />
    protected override void OnWarpedImpl(object sender, WarpedEventArgs e)
    {
        if (e.NewLocation.Equals(e.OldLocation) || e.NewLocation.GetType() == e.OldLocation.GetType()) return;

        if (e.NewLocation.IsDungeon())
            Textures.UltimateMeterTx = Game1.content.Load<Texture2D>($"{ModEntry.Manifest.UniqueID}/UltimateMeter");
    }
}