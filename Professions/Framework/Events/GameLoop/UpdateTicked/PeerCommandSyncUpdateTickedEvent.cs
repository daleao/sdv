namespace DaLion.Professions.Framework.Events.GameLoop.UpdateTicked;

#region using directives

using DaLion.Professions.Commands;
using DaLion.Shared.Events;
using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="PeerCommandSyncUpdateTickedEvent"/> class.</summary>
/// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
[UsedImplicitly]
internal sealed class PeerCommandSyncUpdateTickedEvent(EventManager? manager = null)
    : UpdateTickedEvent(manager ?? ProfessionsMod.Events)
{
    /// <inheritdoc />
    public override bool IsEnabled => Context.IsSplitScreen && !Context.IsMainPlayer &&
        (AddCommand.ProfessionsToAddPerScreen.Keys.Any() || AddCommand.RecipesToAddPerScreen.Keys.Any() ||
        RemoveCommand.ProfessionsToRemovePerScreen.Keys.Any() || SetCommand.TokensToSetPerScreen.Keys.Any());

    /// <inheritdoc />
    protected override void OnUpdateTickedImpl(object? sender, UpdateTickedEventArgs e)
    {
        if (e.Ticks % 10 != 0)
        {
            return;
        }

        var player = Game1.player;
        var screenId = Game1.game1.instanceId;
        if (AddCommand.ProfessionsToAddPerScreen.TryGetValue(screenId, out var professionsToAdd))
        {
            AddCommand.AddProfessionsStatic(professionsToAdd, player);
            AddCommand.ProfessionsToAddPerScreen.Remove(screenId);
        }

        if (AddCommand.RecipesToAddPerScreen.TryGetValue(screenId, out var recipesToAdd))
        {
            AddCommand.AddRecipesStatic(recipesToAdd, player);
            AddCommand.RecipesToAddPerScreen.Remove(screenId);
        }

        if (RemoveCommand.ProfessionsToRemovePerScreen.TryGetValue(screenId, out var professionsToRemove))
        {
            RemoveCommand.RemoveProfessionsStatic(professionsToRemove, player);
            RemoveCommand.ProfessionsToRemovePerScreen.Remove(screenId);
        }

        if (SetCommand.TokensToSetPerScreen.TryGetValue(screenId, out var tokens))
        {
            SetCommand.SetStatic(tokens, player);
            SetCommand.TokensToSetPerScreen.Remove(screenId);
        }
    }
}
