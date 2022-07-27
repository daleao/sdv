using StardewValley;

#region using directives

namespace DaLion.Common.Extensions.Stardew;

#endregion using directives

public static class FarmAnimalExtensions
{
    /// <summary>Get the <see cref="Farmer"/> instance that owns this farm animal.</summary>
    public static Farmer GetOwner(this FarmAnimal animal) =>
        Game1.getFarmerMaybeOffline(animal.ownerID.Value) ?? Game1.MasterPlayer;
}