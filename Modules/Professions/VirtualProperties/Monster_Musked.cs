namespace DaLion.Overhaul.Modules.Professions.VirtualProperties;

#region using directives

using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using StardewValley.Monsters;

#endregion using directives

// ReSharper disable once InconsistentNaming
internal static class Monster_Musked
{
    internal static ConditionalWeakTable<Monster, Musk> Values { get; } = new();

    internal static bool Get_Musked(this Monster monster)
    {
        return Values.GetOrCreateValue(monster).MuskTimer > 0;
    }

    internal static void Set_Musked(this Monster monster, int value)
    {
        var musk = Values.GetOrCreateValue(monster);
        musk.MuskTimer = value;

        var location = monster.currentLocation;
        var fakeFarmer = new FakeFarmer
        {
            UniqueMultiplayerID = Guid.NewGuid().GetHashCode(),
            currentLocation = location,
            Position = monster.position.Value,
        };

        musk.FakeFarmer = fakeFarmer;

        for (var i = 0; i < 3; i++)
        {
            Reflector.GetStaticFieldGetter<Multiplayer>(typeof(Game1), "multiplayer").Invoke()
                .broadcastSprites(
                    location,
                    new TemporaryAnimatedSprite(5, new Vector2(16f, -64f + (32f * i)), Color.Purple)
                    {
                        motion = new Vector2(Utility.RandomFloat(-1f, 1f), -0.5f),
                        scaleChange = 0.005f,
                        scale = 0.5f,
                        alpha = 1f,
                        alphaFade = 0.0075f,
                        shakeIntensity = 1f,
                        delayBeforeAnimationStart = 100 * i,
                        layerDepth = 0.9999f,
                        positionFollowsAttachedCharacter = true,
                        attachedCharacter = fakeFarmer,
                    });
        }

        location.playSound("steam");
    }

    internal static FakeFarmer? Get_MuskFakeFarmer(this Monster monster)
    {
        return Values.TryGetValue(monster, out var musk) ? musk.FakeFarmer : null;
    }

    internal static void CountdownMusk(this Monster monster)
    {
        if (!Values.TryGetValue(monster, out var musk))
        {
            return;
        }

        musk.MuskTimer--;
        if (musk.MuskTimer <= 0 || musk.MuskTimer % 60 != 0)
        {
            return;
        }

        Reflector.GetStaticFieldGetter<Multiplayer>(typeof(Game1), "multiplayer").Invoke()
            .broadcastSprites(
                monster.currentLocation,
                new TemporaryAnimatedSprite(5, new Vector2(16f, -64f + 32f), Color.Purple)
                {
                    motion = new Vector2(Utility.RandomFloat(-1f, 1f), -0.5f),
                    scaleChange = 0.005f,
                    scale = 0.5f,
                    alpha = 1f,
                    alphaFade = 0.0075f,
                    shakeIntensity = 1f,
                    delayBeforeAnimationStart = 100,
                    layerDepth = 0.9999f,
                    positionFollowsAttachedCharacter = true,
                    attachedCharacter = monster,
                });
    }

    internal class Musk
    {
        public int MuskTimer { get; internal set; }

        public FakeFarmer? FakeFarmer { get; internal set; }
    }
}
