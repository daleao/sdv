// ReSharper disable EqualExpressionComparison
namespace DaLion.Overhaul.Modules.Enchantments.Melee;

#region using directives

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;
using DaLion.Overhaul.Modules.Enchantments.VirtualProperties;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Reflection;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley.Monsters;

#endregion using directives

/// <summary>Causes a random unpredictable effect.</summary>
/// <remarks>Can spawn illegal items.</remarks>
[XmlType("Mods_DaLion_WabbajackEnchantment")]
public sealed class WabbajackEnchantment : BaseWeaponEnchantment
{
    private static readonly Type[] MonsterTypes = AccessTools
        .GetTypesFromAssembly(Assembly.GetAssembly(typeof(Monster)))
        .Where(t => t.IsAssignableTo(typeof(Monster)) && !t.IsAbstract)
        .ToArray();

    private static readonly string[] AnimalNames =
        Game1.content.Load<Dictionary<string, string>>("Data\\FarmAnimals").Keys.ToArray();

    private static readonly int[] ItemIds =
        Game1.content.Load<Dictionary<int, string>>("Data\\ObjectInformation").Keys.ToArray();

    private readonly Random _random = new(Guid.NewGuid().GetHashCode());

    /// <inheritdoc />
    public override string GetName()
    {
        return I18n.Get("enchantments.wabbajack.name");
    }

    /// <inheritdoc />
    protected override void _OnDealDamage(Monster monster, GameLocation location, Farmer who, ref int amount)
    {
        if (!who.IsLocalPlayer)
        {
            return;
        }

        if (this._random.NextDouble() > this._random.NextDouble() * 0.5)
        {
            return;
        }

        try
        {
            // deal damage or heal
            if (this._random.NextDouble() < this._random.NextDouble())
            {
                switch (this._random.Next(3))
                {
                    case 0:
                    case 1:
                        var damage = amount * this._random.NextDouble();
                        monster.Health -= (int)damage;
                        location.debris.Add(new Debris(
                            (int)damage,
                            new Vector2(monster.getStandingX(), monster.getStandingY()),
                            Color.Red,
                            1f,
                            monster));
                        if (monster.Health <= 0)
                        {
                            monster.deathAnimation();
                        }

                        Log.D($"[ENCH]: {monster.Name} suffered additional {damage} damage.");
                        break;
                    case 2:
                        var restored = monster.MaxHealth * this._random.NextDouble();
                        monster.Health += (int)restored;
                        location.debris.Add(new Debris(
                            (int)restored,
                            new Vector2(monster.getStandingX(), monster.getStandingY()),
                            Color.Lime,
                            1f,
                            monster));
                        Game1.playSound("healSound");
                        Log.D($"[ENCH]: {monster.Name} restored {restored} health.");
                        break;
                }

                return;
            }

            // reduce or increase stats
            if (this._random.NextDouble() < this._random.NextDouble())
            {
                switch (this._random.Next(2))
                {
                    case 0:
                    {
                        var value = (int)(monster.DamageToFarmer * this._random.NextDouble(-0.5, 0.5));
                        monster.DamageToFarmer -= value;
                        Log.D($"[ENCH]: {monster.Name}'s damage was changed by {value}.");
                        break;
                    }

                    case 1:
                    {
                        var value = this._random.Next(-2, 3);
                        monster.resilience.Value -= value;
                        Log.D($"[ENCH]: {monster.Name}'s resistance was changed by {value}.");
                        break;
                    }
                }

                return;
            }

            // transform into different creature
            if (this._random.NextDouble() < this._random.NextDouble())
            {
                location.characters.Remove(monster);
                location.temporarySprites.Add(new TemporaryAnimatedSprite(
                    5,
                    monster.Position,
                    Color.White,
                    8,
                    Game1.random.NextDouble() < 0.5,
                    50f));
                location.playSound("wand");

                switch (this._random.Next(2))
                {
                    case 0:
                    {
                        var transfigure = (Monster)MonsterTypes
                            .Choose(this._random)
                            .RequireConstructor(1)
                            .Invoke(new object?[] { monster.Position });
                        location.characters.Add(transfigure);
                        Log.D($"[ENCH]: {monster.Name} was transfigured into a {transfigure.Name}.");
                        break;
                    }

                    case 1:
                    {
                        var transfigure = new FarmAnimal(AnimalNames.Choose(this._random), -1, -1)
                        {
                            Position = monster.Position,
                        };

                        transfigure.age.Value = transfigure.ageWhenMature.Value;
                        transfigure.Sprite.LoadTexture("Animals\\" + transfigure.type.Value);
                        location
                            .Get_Animals()
                            .Add(transfigure);
                        Log.D($"[ENCH]: {monster.Name} was transfigured into a {transfigure.displayName}.");
                        break;
                    }
                }

                return;
            }

            // transform into item
            if (this._random.NextDouble() < this._random.NextDouble())
            {
                location.characters.Remove(monster);
                location.temporarySprites.Add(new TemporaryAnimatedSprite(
                    5,
                    monster.Position,
                    Color.White,
                    8,
                    Game1.random.NextDouble() < 0.5,
                    50f));
                location.playSound("wand");

                var transfigure = new SObject(ItemIds.Choose(this._random), 1);
                location.debris.Add(
                    new Debris(
                        transfigure,
                        new Vector2((int)monster.Position.X, (int)monster.Position.Y),
                        who.getStandingPosition()));
                Log.D($"{monster.Name} was transfigured into a {transfigure.Name}.");
            }
        }
        catch
        {
            // ignore
        }
    }
}
