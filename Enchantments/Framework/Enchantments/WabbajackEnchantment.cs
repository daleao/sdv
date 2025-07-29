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

    /// <inheritdoc />
    public override void OnDealtDamage(Monster monster, GameLocation location, Farmer who, bool fromBomb, int amount)
    {
        base.OnDealtDamage(monster, location, who, fromBomb, amount);
        var chance = ((MathConstants.PHI - 1d) / (4d * MathConstants.PHI)) - who.DailyLuck;
        if (this._random.NextBool(chance))
        {
            DoWabbajack(monster, location, who, ref amount, chance, this._random);
        }
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

            // lucky 7 damage or heal
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
                        location.playSound("healSound");
                        break;
                }

                return;
            }

            // transformations
            location.temporarySprites.Add(new TemporaryAnimatedSprite(
                5,
                monster.Position,
                Color.White,
                8,
                r.NextBool(),
                50f));
            location.playSound("wand");
            switch (r.Next(9))
            {
                // debuff
                case 0:
                case 1:
                case 2:
                    switch (r.Next(8))
                    {
                        case 0:
                            monster.Bleed(who);
                            break;
                        case 1:
                            monster.Blind();
                            break;
                        case 2:
                            monster.Burn(who);
                            break;
                        case 3:
                            monster.Fear();
                            break;
                        case 4:
                            monster.Freeze();
                            break;
                        case 5:
                            monster.Poison(who);
                            break;
                        case 6:
                            monster.Slow();
                            break;
                        case 7:
                            monster.Stun();
                            break;
                    }

                    break;

                // shrink/grow/clone
                case 3:
                case 4:
                case 5:
                    switch (r.Next(4))
                    {
                        // shrink
                        case 0:
                        case 1:
                            monster.Scale /= 2f;
                            if (monster is GreenSlime slime)
                            {
                                slime.ageUntilFullGrown.Value = 1;
                            }

                            monster.Speed++;
                            monster.Health /= 2;
                            monster.DamageToFarmer = 1;
                            break;

                        // grow
                        case 2:
                            monster.Scale *= 2f;
                            monster.Speed--;
                            monster.Health *= 2;
                            monster.DamageToFarmer *= 2;
                            break;

                        // clone
                        case 3:
                            var clone = monster.DeepClone();
                            location.characters.Add(clone);
                            Log.D($"{monster.Name} was split in two.");
                            break;
                    }

                    break;

                // transfigure
                case 6:
                case 7:
                case 8:
                    location.characters.Remove(monster);
                    switch (r.Next(3))
                    {
                        // critter
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

                        // farm animal
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

                        // cheese
                        case 2:
                            var stack = (int)Math.Exp(r.Next(100) * 0.03);
                            for (var i = 0; i < stack; i++)
                            {
                                var cheese = ItemRegistry.Create<SObject>(
                                    r.NextBool()
                                        ? QIDs.Cheese
                                        : QIDs.GoatCheese);
                                location.debris.Add(
                                    new Debris(
                                        cheese,
                                        new Vector2((int)monster.Position.X, (int)monster.Position.Y),
                                        who.getStandingPosition()));
                            }

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
}
