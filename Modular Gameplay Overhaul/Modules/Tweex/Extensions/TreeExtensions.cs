namespace DaLion.Overhaul.Modules.Tweex.Extensions;

#region using directives

using DaLion.Shared.Extensions.Stardew;
using StardewValley.TerrainFeatures;

#endregion using directives

/// <summary>Extensions for the <see cref="Tree"/> class.</summary>
internal static class TreeExtensions
{
    /// <summary>Determines whether the <paramref name="tree"/> can hold a Tapper.</summary>
    /// <param name="tree">The <see cref="Tree"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="tree"/>'s type accepts a Tapper, otherwise <see langword="false"/>.</returns>
    internal static bool CanBeTapped(this Tree tree)
    {
        return tree.treeType.Value is Tree.bushyTree or Tree.leafyTree or Tree.pineTree or Tree.mushroomTree
            or Tree.mahoganyTree;
    }

    /// <summary>Gets a string representation for the <paramref name="tree"/>'s species.</summary>
    /// <param name="tree">The <see cref="Tree"/>.</param>
    /// <returns>A human-readable <see cref="string"/> representation of the <paramref name="tree"/>'s type.</returns>
    internal static string NameFromType(this Tree tree)
    {
        return tree.treeType.Value switch
        {
            Tree.bushyTree => "Oak Tree",
            Tree.leafyTree => "Mahogany Tree",
            Tree.pineTree => "Pine Tree",
            Tree.winterTree1 => "Winter Tree",
            Tree.winterTree2 => "Winter Tree 2",
            Tree.palmTree => "Palm Tree",
            Tree.mushroomTree => "Mushroom Tree",
            Tree.mahoganyTree => "Mahogany Tree",
            Tree.palmTree2 => "Palm Tree 2",
            _ => "Unknown Tree",
        };
    }

    /// <summary>Gets an object quality value based on this <paramref name="tree"/>'s age.</summary>
    /// <param name="tree">The <see cref="Tree"/>.</param>
    /// <returns>A <see cref="SObject"/> quality value.</returns>
    internal static int GetQualityFromAge(this Tree tree)
    {
        var skillFactor = 1f + (Game1.player.ForagingLevel * 0.1f);
        if (ProfessionsModule.IsEnabled && Game1.player.professions.Contains(Farmer.lumberjack))
        {
            skillFactor++;
        }

        var age = (int)(tree.Read<int>(DataKeys.Age) * skillFactor * TweexModule.Config.TreeAgingFactor);
        if (TweexModule.Config.DeterministicAgeQuality)
        {
            return age switch
            {
                >= 336 => SObject.bestQuality,
                >= 224 => SObject.highQuality,
                >= 112 => SObject.medQuality,
                _ => SObject.lowQuality,
            };
        }

        return Game1.random.Next(age) switch
        {
            >= 336 => SObject.bestQuality,
            >= 224 => SObject.highQuality,
            >= 112 => SObject.medQuality,
            _ => SObject.lowQuality,
        };
    }
}
