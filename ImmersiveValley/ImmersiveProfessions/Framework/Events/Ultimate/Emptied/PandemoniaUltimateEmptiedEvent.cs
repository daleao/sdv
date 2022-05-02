namespace DaLion.Stardew.Professions.Framework.Events.Ultimate;

#region using directives

using System.Linq;
using JetBrains.Annotations;
using StardewValley;
using StardewValley.Monsters;

#endregion using directives

/// <inheritdoc />
[UsedImplicitly]
internal class PandemoniaUltimateEmptiedEvent : UltimateEmptiedEvent
{
    /// <inheritdoc cref="OnUltimateEmptied" />
    protected override void OnUltimateEmptiedImpl(object sender, UltimateEmptiedEventArgs e)
    {
        foreach (var slime in Game1.player.currentLocation.characters.OfType<GreenSlime>())
        {
            slime.addedSpeed = 0;
            slime.focusedOnFarmers = false;
        }
    }
}