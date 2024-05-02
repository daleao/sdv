namespace DaLion.Overhaul.Modules.Tools.Extensions;

#region using directives

using DaLion.Shared.Enums;

#endregion using directives

/// <summary>Extensions for the <see cref="SObject"/> class.</summary>
internal static class SObjectExtensions
{
    /// <summary>Determines whether the <paramref name="object"/> can be harvested with a sickle.</summary>
    /// <param name="object">The <see cref="SObject"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="object"/>'s category should be harvestable as defined by the player's config settings, otherwise <see langword="false"/>.</returns>
    internal static bool CanBeSickleHarvested(this SObject @object)
    {
        return (@object.Category is (int)ObjectCategory.Vegetables or (int)ObjectCategory.Fruits &&
                ToolsModule.Config.Scythe.HarvestCrops) ||
               (@object.Category == (int)ObjectCategory.Flowers && ToolsModule.Config.Scythe.HarvestFlowers) ||
               (@object.Category == (int)ObjectCategory.Greens && ToolsModule.Config.Scythe.HarvestForage);
    }
}
