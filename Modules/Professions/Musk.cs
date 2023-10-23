namespace DaLion.Overhaul.Modules.Professions;

#region using directives

using DaLion.Overhaul.Modules.Professions.VirtualProperties;
using Microsoft.Xna.Framework;
using StardewValley.Monsters;

#endregion using directivesv

internal sealed class Musk
{

    /// <summary>Initializes a new instance of the <see cref="Musk"/> class, attached to a fixed <paramref name="position"/> on the specified <paramref name="location"/>.</summary>
    /// <param name="location">The <see cref="GameLocation"/>.</param>
    /// <param name="position">The <see cref="Vector2"/> absolute position.</param>
    /// <param name="duration">The duration, in seconds.</param>
    internal Musk(GameLocation location, Vector2 position, int duration)
    {
        this.Location = location;
        this.FakeFarmer = new FakeFarmer
        {
            UniqueMultiplayerID = Guid.NewGuid().GetHashCode(),
            currentLocation = location,
            Position = position,
        };

        this.Duration = duration;
        for (var i = 0; i < 3; i++)
        {
            Reflector.GetStaticFieldGetter<Multiplayer>(typeof(Game1), "multiplayer").Invoke()
                .broadcastSprites(
                    location,
                    new TemporaryAnimatedSprite(5, position + new Vector2(16f, -64f + (32f * i)), Color.Purple)
                    {
                        motion = new Vector2(Utility.RandomFloat(-1f, 1f), -0.5f),
                        scaleChange = 0.005f,
                        scale = 0.5f,
                        alpha = 1f,
                        alphaFade = 0.0075f,
                        shakeIntensity = 1f,
                        delayBeforeAnimationStart = 100 * i,
                        layerDepth = 0.9999f,
                        positionFollowsAttachedCharacter = false,
                        attachedCharacter = this.FakeFarmer,
                    });
        }

        location.playSound("steam");
    }

    /// <summary>Initializes a new instance of the <see cref="Musk"/> class.</summary>
    /// <param name="attachedMonster">The <see cref="Monster"/> this instance is attached to.</param>
    /// <param name="duration">The duration, in seconds.</param>
    internal Musk(Monster attachedMonster, int duration)
    {
        this.AttachedMonster = attachedMonster;
        this.Location = attachedMonster.currentLocation;
        this.FakeFarmer = new FakeFarmer
        {
            UniqueMultiplayerID = Guid.NewGuid().GetHashCode(),
            currentLocation = attachedMonster.currentLocation,
            Position = attachedMonster.position.Value,
        };

        this.Duration = duration;
        for (var i = 0; i < 3; i++)
        {
            Reflector.GetStaticFieldGetter<Multiplayer>(typeof(Game1), "multiplayer").Invoke()
                .broadcastSprites(
                    attachedMonster.currentLocation,
                    new TemporaryAnimatedSprite(
                        5,
                        attachedMonster.Position + new Vector2(16f, -64f + (32f * i)),
                        Color.Purple)
                    {
                        motion = new Vector2(Utility.RandomFloat(-1f, 1f), -0.5f),
                        scaleChange = 0.005f,
                        scale = 0.5f,
                        alpha = 1f,
                        alphaFade = 0.0075f,
                        shakeIntensity = 1f,
                        delayBeforeAnimationStart = 100 * i,
                        layerDepth = 0.9999f,
                        positionFollowsAttachedCharacter = false,
                        attachedCharacter = this.FakeFarmer,
                    });
        }

        attachedMonster.currentLocation.playSound("steam");
    }

    internal GameLocation Location { get; }

    internal FakeFarmer FakeFarmer { get; }

    internal Monster? AttachedMonster { get; }

    internal int Duration { get; private set; }

    internal void Update()
    {
        if (this.AttachedMonster is not null)
        {
            this.FakeFarmer.Position = this.AttachedMonster.Position;
        }

        this.Duration--;
        if (this.Duration <= 0)
        {
            this.Location.RemoveMusk(this);
            if (this.AttachedMonster is not null)
            {
                Monster_Musked.Values.Remove(this.AttachedMonster);
            }

            return;
        }

        Reflector.GetStaticFieldGetter<Multiplayer>(typeof(Game1), "multiplayer").Invoke()
            .broadcastSprites(
                this.Location,
                new TemporaryAnimatedSprite(
                    5,
                    this.FakeFarmer.Position + new Vector2(16f, -32f),
                    Color.Purple)
                {
                    motion = new Vector2(Utility.RandomFloat(-1f, 1f), -0.5f),
                    scaleChange = 0.005f,
                    scale = 0.5f,
                    alpha = 1f,
                    alphaFade = 0.0075f,
                    shakeIntensity = 1f,
                    delayBeforeAnimationStart = 100,
                    layerDepth = 0.9999f,
                    positionFollowsAttachedCharacter = false,
                    attachedCharacter = this.FakeFarmer,
                });
    }
}
