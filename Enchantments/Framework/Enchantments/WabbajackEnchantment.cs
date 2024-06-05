// ReSharper disable EqualExpressionComparison
namespace DaLion.Enchantments.Framework.Enchantments;

#region using directives

using System.Xml.Serialization;
using DaLion.Core.Framework.Extensions;
using DaLion.Shared.Constants;
using DaLion.Shared.Extensions;
using Force.DeepCloner;
using Microsoft.Xna.Framework;
using StardewValley.BellsAndWhistles;
using StardewValley.Monsters;

#endregion using directives

/// <summary>Causes random unpredictable effects.</summary>
[XmlType("Mods_DaLion_WabbajackEnchantment")]
public sealed class WabbajackEnchantment : BaseWeaponEnchantment
{
    private static readonly Lazy<string[]> AnimalNames = new(() => [.. DataLoader.FarmAnimals(Game1.content).Keys]);

    private readonly Random _random = new(Guid.NewGuid().GetHashCode());

    /// <inheritdoc />
    public override string GetName()
    {
        return I18n.Enchantments_Wabbajack_Name();
    }

    internal static void DoWabbajack(
        Monster monster,
        GameLocation location,
        Farmer who,
        ref int amount,
        double chance,
        Random? r = null)
    {
        r ??= Game1.random;

        try
        {
            if (!r.NextBool(chance))
            {
                return;
            }

            if (amount.ContainsDigit(7))
            {
                switch (r.Next(3))
                {
                    case 0:
                        amount = 7;
                        break;
                    case 1:
                        amount = 77;
                        break;
                    case 2:
                        amount = 777;
                        break;
                    case 3:
                        amount = -777;
                        location.playSound("heal");
                        break;
                }

                return;
            }

            location.temporarySprites.Add(new TemporaryAnimatedSprite(
                5,
                monster.Position,
                Color.White,
                8,
                r.NextBool(),
                50f));
            location.playSound("wand");
            switch (r.Next(10))
            {
                case 0: // debuff/shrink/grow/clone
                case 1:
                case 2:
                case 3:
                    switch (r.Next(4))
                    {
                        case 0:
                            switch (r.Next(7))
                            {
                                case 0:
                                    monster.Bleed(who);
                                    break;
                                case 1:
                                    monster.Burn(who);
                                    break;
                                case 2:
                                    monster.Fear();
                                    break;
                                case 3:
                                    monster.Freeze();
                                    break;
                                case 4:
                                    monster.Poison(who);
                                    break;
                                case 5:
                                    monster.Slow();
                                    break;
                                case 6:
                                    monster.Stun();
                                    break;
                            }

                            break;

                        case 1:
                            monster.Scale *= 0.5f;
                            monster.Speed += 2;
                            monster.Health /= 2;
                            monster.DamageToFarmer = 1;
                            break;

                        case 2:
                            monster.Scale *= 2f;
                            monster.Speed -= 2;
                            monster.Health *= 2;
                            monster.DamageToFarmer *= 2;
                            break;

                        case 3:
                            var clone = monster.DeepClone();
                            location.characters.Add(clone);
                            Log.D($"{monster.Name} was split in two.");
                            break;
                    }

                    break;

                case 4:
                case 5:
                case 6: // transform into critter/farm animal/cheese
                    location.characters.Remove(monster);
                    switch (r.Next(3))
                    {
                        case 0:
                            Critter critter;
                            switch (r.Next(5))
                            {
                                case 0:
                                    critter = new Frog(monster.Tile, false, r.NextBool());
                                    break;
                                case 1:
                                    critter = new Opossum(location, monster.Tile, r.NextBool());
                                    break;
                                case 2:
                                    critter = new Owl(monster.Position);
                                    break;
                                case 3:
                                    critter = new Rabbit(location, monster.Tile, r.NextBool());
                                    break;
                                case 4:
                                    critter = new Squirrel(monster.Tile, r.NextBool());
                                    break;
                                default:
                                    return;
                            }

                            location.critters.Add(critter);
                            Log.D($"{monster.Name} became a {critter.GetType().Name}.");
                            break;

                        case 1:
                            var animal = new FarmAnimal(AnimalNames.Value.Choose(r), -1, -1)
                            {
                                Position = monster.Position,
                            };

                            animal.growFully();
                            animal.Sprite.LoadTexture("Animals\\" + animal.type.Value);
                            location.Animals.Add(animal.myID.Value, animal);
                            Log.D($"{monster.Name} became a {animal.displayName}.");
                            break;

                        case 2:
                            var cheese = ItemRegistry.Create<SObject>(
                                r.NextBool()
                                    ? QualifiedObjectIds.Cheese
                                    : QualifiedObjectIds.GoatCheese,
                                (int)Math.Exp(r.Next(100) * 0.03));
                            location.debris.Add(
                                new Debris(
                                    cheese,
                                    new Vector2((int)monster.Position.X, (int)monster.Position.Y),
                                    who.getStandingPosition()));
                            Log.D($"{monster.Name} became cheese.");
                            break;
                    }

                    break;
            }
        }
        catch
        {
            // ignore
        }
    }

    /// <inheritdoc />
    protected override void _OnDealDamage(Monster monster, GameLocation location, Farmer who, ref int amount)
    {
        if (!who.IsLocalPlayer)
        {
            return;
        }

        var chance = ((MathConstants.PHI - 1d) / (4d * MathConstants.PHI)) - Game1.player.DailyLuck;
        if (this._random.NextBool(chance))
        {
            DoWabbajack(monster, location, who, ref amount, chance, this._random);
        }
    }
}
