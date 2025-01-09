namespace DaLion.Professions.Framework.Events.Input.ButtonPressed;

#region using directives

using DaLion.Core;
using DaLion.Professions.Framework.VirtualProperties;
using DaLion.Shared.Events;
using DaLion.Shared.Extensions.Stardew;
using StardewModdingAPI.Utilities;
using StardewValley.Extensions;
using StardewValley.Monsters;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="PiperButtonDoublePressedEvent"/> class.</summary>
/// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
[UsedImplicitly]
internal sealed class PiperButtonDoublePressedEvent(EventManager? manager = null)
    : ButtonDoublePressedEvent(manager ?? ProfessionsMod.EventManager)
{
    private static readonly Func<int, double> _pipeChance = x => 29f / (58f - x);

    /// <inheritdoc />
    public override bool IsEnabled => Game1.player.HasProfession(Profession.Demolitionist);

    /// <inheritdoc />
    protected override KeybindList Keybinds => Config.ModKey;

    /// <inheritdoc />
    protected override void OnButtonDoublePressedAndHeldImpl()
    {
        if (Game1.currentLocation is SlimeHutch || !CoreMod.State.AreEnemiesNearby)
        {
            return;
        }

        if (State.AlliedSlimes[0] is not null &&
            (State.AlliedSlimes[1] is not null || !Game1.player.HasProfession(Profession.Piper, true)))
        {
            return;
        }

        var nearestSlime = Game1.currentLocation.characters
            .OfType<GreenSlime>()
            .Where(slime => slime.withinPlayerThreshold())
            .OrderByDescending(slime => slime.DistanceToPlayer()).FirstOrDefault();
        if (nearestSlime is null)
        {
            Game1.playSound("cancel");
            return;
        }

        SoundBox.PiperProvoke.PlayLocal();
        var numberRaised = Game1.player.CountRaisedSlimes();
        if (!Game1.random.NextBool(_pipeChance(numberRaised)))
        {
            nearestSlime.focusedOnFarmers = true;
            return;
        }

        if (State.AlliedSlimes[0] is null)
        {
            nearestSlime.Set_Piped(Game1.player);
            State.AlliedSlimes[0] = nearestSlime.Get_Piped();
        }
        else if (Game1.player.HasProfession(Profession.Piper, true) && State.AlliedSlimes[1] is null)
        {
            nearestSlime.Set_Piped(Game1.player);
            State.AlliedSlimes[1] = nearestSlime.Get_Piped();
            SoundBox.PiperProvoke.PlayLocal();
        }
    }
}
