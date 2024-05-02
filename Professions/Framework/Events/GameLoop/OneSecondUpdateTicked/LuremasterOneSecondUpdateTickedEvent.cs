namespace DaLion.Professions.Framework.Events.GameLoop.OneSecondUpdateTicked;

#region using directives

using System.Threading.Tasks;
using DaLion.Shared.Events;
using DaLion.Shared.Extensions;
using StardewModdingAPI.Events;
using StardewValley.Extensions;

#endregion using directives

[UsedImplicitly]
internal sealed class LuremasterOneSecondUpdateTickedEvent : OneSecondUpdateTickedEvent
{
    /// <summary>Initializes a new instance of the <see cref="LuremasterOneSecondUpdateTickedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal LuremasterOneSecondUpdateTickedEvent(EventManager? manager = null)
        : base(manager ?? ProfessionsMod.EventManager)
    {
    }

    /// <inheritdoc />
    protected override void OnOneSecondUpdateTickedImpl(object? sender, OneSecondUpdateTickedEventArgs e)
    {
        Parallel.ForEach(Game1.game1.EnumerateAllCrabPots(), crabPot =>
        {
            if (crabPot.heldObject.Value is null || !crabPot.Location.farmers.Any() ||
                Data.Read(crabPot, DataKeys.TrappedHaul).ParseList<string>() is not { Count: > 0 } haul ||
                haul[0]?.Split('/') is not { Length: 3 } split ||
                !Game1.random.NextBool())
            {
                return;
            }

            var held = crabPot.heldObject.Value;
            haul.Add($"{held.QualifiedItemId}/{held.Stack}/{held.Quality}");

            var item = ItemRegistry.Create<SObject>(
                split[0],
                amount: int.Parse(split[1]),
                quality: int.Parse(split[2]));
            crabPot.heldObject.Value = item;
            Data.Write(crabPot, DataKeys.TrappedHaul, string.Join(',', haul.Skip(1)));
        });
    }
}
