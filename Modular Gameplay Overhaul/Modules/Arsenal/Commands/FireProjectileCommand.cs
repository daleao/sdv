namespace DaLion.Overhaul.Modules.Arsenal.Commands;

#region using directives

using DaLion.Shared.Attributes;
using DaLion.Shared.Commands;
using DaLion.Shared.Enums;
using Microsoft.Xna.Framework;
using StardewValley.Projectiles;

#endregion using directives

[UsedImplicitly]
[Debug]
internal sealed class FireProjectileCommand : ConsoleCommand
{
    /// <summary>Initializes a new instance of the <see cref="FireProjectileCommand"/> class.</summary>
    /// <param name="handler">The <see cref="CommandHandler"/> instance that handles this command.</param>
    internal FireProjectileCommand(CommandHandler handler)
        : base(handler)
    {
    }

    /// <inheritdoc />
    public override string[] Triggers { get; } = { "fire", "shoot" };

    /// <inheritdoc />
    public override string Documentation => "Fire the specified projectile.";

    /// <inheritdoc />
    public override void Callback(string[] args)
    {
        if (args.Length == 0 || string.IsNullOrEmpty(args[0]))
        {
            Log.W("You must specify a projectile sheet index.");
            return;
        }

        if (!int.TryParse(args[0], out var index) || index is < 0 or > 15)
        {
            Log.W(
                "Specified index either could not be parsed or was outside the tilesheet range. Please enter a valid integer index between 0 and 15.");
            return;
        }

        if (args.Length > 2)
        {
            Log.W("Additional arguments beyond the second will be ignored.");
        }

        var tail = 0;
        if (args.Length > 1 && (!int.TryParse(args[1], out tail) || tail < 0))
        {
            Log.W(
                "Specified tail length either could not be parsed or is invalid. Please enter a valid integer index greater than 0.");
            return;
        }

        var velocity = ((FacingDirection)Game1.player.FacingDirection).ToVector() * 10f;
        var origin = Game1.player.getStandingPosition() + new Vector2(32f, 32f);
        var projectile = new BasicProjectile(
            1,
            index,
            0,
            tail,
            0f,
            velocity.X,
            velocity.Y,
            origin,
            string.Empty,
            string.Empty,
            false,
            false,
            Game1.player.currentLocation,
            Game1.player)
        {
            height = { Value = 32f },
            startingRotation = { Value = (float)(Game1.player.FacingDirection * Math.PI / 2f) },
        };

        Game1.player.currentLocation.projectiles.Add(projectile);
    }
}
