namespace DaLion.Professions.Framework.Events.Player.InventoryChanged;

#region using directives

using DaLion.Shared.Events;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
[AlwaysEnabledEvent]
internal sealed class EcologistInventoryChangedEvent : InventoryChangedEvent
{
    /// <summary>Initializes a new instance of the <see cref="EcologistInventoryChangedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal EcologistInventoryChangedEvent(EventManager? manager = null)
        : base(manager ?? ProfessionsMod.EventManager)
    {
    }

    /// <inheritdoc />
    protected override void OnInventoryChangedImpl(object? sender, InventoryChangedEventArgs e)
    {
        foreach (var item in e.Added)
        {
            if (item is not SObject @object || !@object.isForage() || !e.Player.HasProfession(Profession.Ecologist) ||
                Data.ReadAs<bool>(item, DataKeys.EcologistBuffed))
            {
                continue;
            }

            @object.Edibility = e.Player.HasProfession(Profession.Ecologist, true)
                ? @object.Edibility * 2
                : (int)(@object.Edibility * 1.5f);
            Data.Write(item, DataKeys.EcologistBuffed, true.ToString());
        }

        foreach (var item in e.Removed)
        {
            if (item is not SObject @object || !@object.isForage() ||
                !Data.ReadAs<bool>(item, DataKeys.EcologistBuffed))
            {
                continue;
            }

            @object.Edibility = Game1.objectData[item.ItemId].Edibility;
            Data.Write(item, DataKeys.EcologistBuffed, false.ToString());
        }
    }
}
