namespace DaLion.Stardew.Professions.Extensions;

#region using directives

using Framework;

#endregion using directives

public static class CropExtensions
{
    /// <summary>Whether the player should track a given crop.</summary>
    public static bool ShouldBeTracked(this Crop crop) =>
        Game1.player.HasProfession(Profession.Scavenger) && crop.forageCrop.Value;
}