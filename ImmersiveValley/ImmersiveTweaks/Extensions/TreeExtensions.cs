namespace DaLion.Stardew.Tweaks.Extensions;

#region using directives

using StardewValley;
using StardewValley.TerrainFeatures;

using Common.Extensions;
using Common.Extensions.Stardew;

using SObject = StardewValley.Object;

#endregion using directives

/// <summary>Extensions for the <see cref="Tree"/> class.</summary>
public static class TreeExtensions
{
   /// <summary>Whether a given tree can hold a Tapper.</summary>
    public static bool CanBeTapped(this Tree tree)
    {
        return tree.treeType.Value is Tree.bushyTree or Tree.leafyTree or Tree.pineTree or Tree.mushroomTree
            or Tree.mahoganyTree;
    }

    /// <summary>Get a string representation of a given tree's species.</summary>
    public static string NameFromType(this Tree tree)
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
            _ => "Unknown Tree"
        };
    }

    /// <summary>Get an object quality value based on this tree's age.</summary>
    public static int GetQualityFromAge(this Tree tree)
    {
        var skillFactor = 1f + Game1.player.ForagingLevel * 0.1f;
        var age = (int) (tree.ReadDataAs<int>("Age") * skillFactor * ModEntry.Config.AgeImproveQualityFactor);
        if (ModEntry.HasProfessionsMod && Game1.player.professions.Contains(Farmer.lumberjack)) age *= 2;

        if (ModEntry.Config.DeterministicAgeQuality)
        {
            return age switch
            {
                >= 336 => SObject.bestQuality,
                >= 224 => SObject.highQuality,
                >= 112 => SObject.medQuality,
                _ => SObject.lowQuality
            };
        }

        return Game1.random.Next(age) switch
        {
            >= 336 => SObject.bestQuality,
            >= 224 => SObject.highQuality,
            >= 112 => SObject.medQuality,
            _ => SObject.lowQuality
        };
    }

    /// <summary>Read a string from this tree's <see cref="ModDataDictionary" />.</summary>
    /// <param name="field">The field to read from.</param>
    /// <param name="defaultValue">The default value to return if the field does not exist.</param>
    public static string ReadData(this Tree tree, string field, string defaultValue = "")
    {
        return tree.modData.Read($"{ModEntry.Manifest.UniqueID}/{field}", defaultValue);
    }

    /// <summary>Read a field from this tree's <see cref="ModDataDictionary" /> as <typeparamref name="T" />.</summary>
    /// <param name="field">The field to read from.</param>
    /// <param name="defaultValue"> The default value to return if the field does not exist.</param>
    public static T ReadDataAs<T>(this Tree tree, string field, T defaultValue = default)
    {
        return tree.modData.ReadAs($"{ModEntry.Manifest.UniqueID}/{field}", defaultValue);
    }

    /// <summary>Write to a field in this tree's <see cref="ModDataDictionary" />, or remove the field if supplied with a null or empty value.</summary>
    /// <param name="field">The field to write to.</param>
    /// <param name="value">The value to write, or <c>null</c> to remove the field.</param>
    public static void WriteData(this Tree tree, string field, string value)
    {
        tree.modData.Write($"{ModEntry.Manifest.UniqueID}/{field}", value);
        Log.D($"[ModData]: Wrote {value} to {tree.NameFromType()}'s {field}.");
    }

    /// <summary>Write to a field in this tree's <see cref="ModDataDictionary" />, only if it doesn't yet have a value.</summary>
    /// <param name="field">The field to write to.</param>
    /// <param name="value">The value to write, or <c>null</c> to remove the field.</param>
    public static bool WriteDataIfNotExists(this Tree tree, string field, string value)
    {
        if (tree.modData.ContainsKey($"{ModEntry.Manifest.UniqueID}/{field}"))
        {
            Log.D($"[ModData]: The data field {field} already existed.");
            return true;
        }

        tree.WriteData(field, value);
        return false;
    }

    /// <summary>Increment the value of a numeric field in this tree's <see cref="ModDataDictionary" /> by an arbitrary amount.</summary>
    /// <param name="field">The field to update.</param>
    /// <param name="amount">Amount to increment by.</param>
    public static void IncrementData<T>(this Tree tree, string field, T amount)
    {
        tree.modData.Increment($"{ModEntry.Manifest.UniqueID}/{field}", amount);
        Log.D($"[ModData]: Incremented {tree.NameFromType()}'s {field} by {amount}.");
    }

    /// <summary>Increment the value of a numeric field in this tree's <see cref="ModDataDictionary" /> by 1.</summary>
    /// <param name="field">The field to update.</param>
    public static void IncrementData<T>(this Tree tree, string field)
    {
        tree.modData.Increment($"{ModEntry.Manifest.UniqueID}/{field}",
            "1".Parse<T>());
        Log.D($"[ModData]: Incremented {tree.NameFromType()}'s {field} by 1.");
    }
}