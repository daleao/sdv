namespace DaLion.Stardew.Professions.Framework.Extensions;

#region using directives

using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.TerrainFeatures;

using Common.Extensions;

using SObject = StardewValley.Object;

#endregion using directives

internal static class TreeExtensions
{
    /// <summary>Whether a given common tree satisfies all conditions to advance a stage.</summary>
    internal static bool CanGrow(this Tree tree)
    {
        var tileLocation = tree.currentTileLocation;
        var environment = tree.currentLocation;
        if (Game1.GetSeasonForLocation(tree.currentLocation) == "winter" &&
            !tree.treeType.Value.IsAnyOf(Tree.palmTree, Tree.palmTree2) &&
            !environment.CanPlantTreesHere(-1, (int) tileLocation.X, (int) tileLocation.Y) &&
            !tree.fertilized.Value)
            return false;

        var s = environment.doesTileHaveProperty((int) tileLocation.X, (int) tileLocation.Y, "NoSpawn", "Back");
        if (s is not null && s.IsAnyOf("All", "Tree", "True")) return false;

        var growthRect = new Rectangle((int) ((tileLocation.X - 1f) * 64f), (int) ((tileLocation.Y - 1f) * 64f),
            192, 192);
        switch (tree.growthStage.Value)
        {
            case 4:
            {
                foreach (var pair in environment.terrainFeatures.Pairs)
                    if (pair.Value is Tree otherTree && !otherTree.Equals(tree) &&
                        otherTree.growthStage.Value >= 5 &&
                        otherTree.getBoundingBox(pair.Key).Intersects(growthRect))
                        return false;
                break;
            }
            case 0 when environment.objects.ContainsKey(tileLocation):
                return false;
        }

        return true;
    }

    /// <summary>Get an object quality value based on this tree's age.</summary>
    internal static int GetQualityFromAge(this Tree tree)
    {
        var age = tree.ReadDataAs<int>("Age");
        return age switch
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
    internal static string ReadData(this Tree tree, string field, string defaultValue = "")
    {
        return tree.modData.Read($"{ModEntry.Manifest.UniqueID}/{field}", defaultValue);
    }

    /// <summary>Read a field from this tree's <see cref="ModDataDictionary" /> as <typeparamref name="T" />.</summary>
    /// <param name="field">The field to read from.</param>
    /// <param name="defaultValue"> The default value to return if the field does not exist.</param>
    internal static T ReadDataAs<T>(this Tree tree, string field, T defaultValue = default)
    {
        return tree.modData.ReadAs($"{ModEntry.Manifest.UniqueID}/{field}", defaultValue);
    }

    /// <summary>Write to a field in this tree's <see cref="ModDataDictionary" />, or remove the field if supplied with a null or empty value.</summary>
    /// <param name="field">The field to write to.</param>
    /// <param name="value">The value to write, or <c>null</c> to remove the field.</param>
    internal static void WriteData(this Tree tree, string field, string value)
    {
        tree.modData.Write($"{ModEntry.Manifest.UniqueID}/{field}", value);
        Log.D($"[ModData]: Wrote {value} to {tree.treeType.Value} Tree's {field}.");
    }

    /// <summary>Write to a field in this tree's <see cref="ModDataDictionary" />, only if it doesn't yet have a value.</summary>
    /// <param name="field">The field to write to.</param>
    /// <param name="value">The value to write, or <c>null</c> to remove the field.</param>
    internal static bool WriteDataIfNotExists(this Tree tree, string field, string value)
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
    internal static void IncrementData<T>(this Tree tree, string field, T amount)
    {
        tree.modData.Increment($"{ModEntry.Manifest.UniqueID}/{field}", amount);
        Log.D($"[ModData]: Incremented {tree.treeType.Value} Tree's {field} by {amount}.");
    }

    /// <summary>Increment the value of a numeric field in this tree's <see cref="ModDataDictionary" /> by 1.</summary>
    /// <param name="field">The field to update.</param>
    internal static void IncrementData<T>(this Tree tree, string field)
    {
        tree.modData.Increment($"{ModEntry.Manifest.UniqueID}/{field}",
            "1".Parse<T>());
        Log.D($"[ModData]: Incremented {tree.treeType.Value} Tree's {field} by 1.");
    }
}