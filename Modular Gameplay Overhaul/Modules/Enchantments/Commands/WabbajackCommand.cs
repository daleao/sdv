namespace DaLion.Overhaul.Modules.Enchantments.Commands;

#region using directives

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DaLion.Overhaul.Modules.Enchantments.VirtualProperties;
using DaLion.Shared.Attributes;
using DaLion.Shared.Commands;
using DaLion.Shared.Extensions.Collections;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Extensions.Stardew;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Monsters;

#endregion using directives

[UsedImplicitly]
[Debug]
internal sealed class WabbajackCommand : ConsoleCommand
{
    /// <summary>Initializes a new instance of the <see cref="WabbajackCommand"/> class.</summary>
    /// <param name="handler">The <see cref="CommandHandler"/> instance that handles this command.</param>
    internal WabbajackCommand(CommandHandler handler)
        : base(handler)
    {
    }

    /// <inheritdoc />
    public override string[] Triggers { get; } =
    {
        "wabbajack", "wabba", "wab", "transfigure", "transmorph", "transform", "transmute", "trans",
    };

    /// <inheritdoc />
    public override string Documentation => "Transforms the nearest monster.";

    /// <inheritdoc />
    public override void Callback(string trigger, string[] args)
    {
        var player = Game1.player;
        var location = player.currentLocation;
        var nearest = player.GetClosestCharacter<Monster>();
        if (nearest is null)
        {
            Log.W("There are no monsters nearby.");
            return;
        }

        var r = new Random(Guid.NewGuid().GetHashCode());
        location.characters.Remove(nearest);
        location.temporarySprites.Add(new TemporaryAnimatedSprite(
            5,
            nearest.Position,
            Color.White,
            8,
            Game1.random.NextDouble() < 0.5,
            50f));
        location.playSound("wand");

        if (args.Length == 0)
        {
            switch (r.Next(2))
            {
                case 0:
                    switch (r.Next(2))
                    {
                        case 0:
                        {
                            var transfigure = (Monster)AccessTools
                                .GetTypesFromAssembly(Assembly.GetAssembly(typeof(Monster)))
                                .Where(t => t.IsAssignableTo(typeof(Monster)) && !t.IsAbstract)
                                .Choose(r)!
                                .RequireConstructor(1)
                                .Invoke(new object?[] { nearest.Position });
                            location.characters.Add(transfigure);
                            Log.I($"{nearest.Name} was transfigured into a {transfigure.Name}.");
                            break;
                        }

                        case 1:
                        {
                            var transfigure = new FarmAnimal(
                                Game1.content.Load<Dictionary<string, string>>("Data\\FarmAnimals").Keys.Choose(r),
                                -1,
                                -1) { Position = nearest.Position };
                            transfigure.age.Value = transfigure.ageWhenMature.Value;
                            transfigure.Sprite.LoadTexture("Animals\\" + transfigure.type.Value);
                            location.Get_Animals().Add(transfigure);
                            Log.I($"{nearest.Name} was transfigured into a {transfigure.displayType}.");
                            break;
                        }
                    }

                    break;
                case 1:
                {
                    var transfigure = new SObject(
                        Game1.content.Load<Dictionary<int, string>>("Data\\ObjectInformation").Keys.Choose(r), 1);
                    location.debris.Add(
                        new Debris(
                            transfigure.ParentSheetIndex,
                            new Vector2((int)nearest.Position.X, (int)nearest.Position.Y),
                            player.getStandingPosition()));
                    Log.I($"{nearest.Name} was tarnsfigured into a {transfigure.Name}.");
                    break;
                }
            }

            return;
        }

        switch (args[0].ToLowerInvariant())
        {
            case "monster":
            case "enemy":
            {
                var transfigure = (Monster)AccessTools
                    .GetTypesFromAssembly(Assembly.GetAssembly(typeof(Monster)))
                    .Where(t => t.IsAssignableTo(typeof(Monster)) && !t.IsAbstract)
                    .Choose(r)!
                    .RequireConstructor(1)
                    .Invoke(new object?[] { nearest.Position });
                location.characters.Add(transfigure);
                Log.I($"{nearest.Name} was tarnsfigured into a {transfigure.Name}.");
                break;
            }

            case "animal":
            {
                var transfigure = new FarmAnimal(
                    Game1.content.Load<Dictionary<string, string>>("Data\\FarmAnimals").Keys.Choose(r),
                    -1,
                    -1) { Position = nearest.Position };
                transfigure.age.Value = transfigure.ageWhenMature.Value;
                transfigure.Sprite.LoadTexture("Animals\\" + transfigure.type.Value);
                location.Get_Animals().Add(transfigure);
                Log.I($"{nearest.Name} was tarnsfigured into a {transfigure.displayType}.");
                break;
            }

            case "item":
            {
                var transfigure =
                    new SObject(
                        Game1.content.Load<Dictionary<int, string>>("Data\\ObjectInformation").Keys.Choose(r),
                        1);
                location.debris.Add(
                    new Debris(
                        transfigure,
                        new Vector2((int)nearest.Position.X, (int)nearest.Position.Y),
                        player.getStandingPosition()));
                Log.I($"{nearest.Name} was tarnsfigured into a {transfigure.Name}.");
                break;
            }
        }
    }
}
