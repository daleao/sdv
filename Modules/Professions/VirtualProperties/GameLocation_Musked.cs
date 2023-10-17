namespace DaLion.Overhaul.Modules.Professions.VirtualProperties;

#region using directives

using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;

#endregion using directives

// ReSharper disable once InconsistentNaming
internal static class GameLocation_Musked
{
    internal static ConditionalWeakTable<GameLocation, List<Musk>> Values { get; } = new();

    internal static IEnumerable<FakeFarmer> Get_Musks(this GameLocation location)
    {
        if (!Values.TryGetValue(location, out var muskList))
        {
            yield break;
        }

        for (var i = 0; i < muskList.Count; i++)
        {
            var musk = muskList[i];
            if (musk.FakeFarmer is not null && musk.MuskTimer > 0)
            {
                yield return musk.FakeFarmer;
            }
        }
    }

    internal static void AddMusk(this GameLocation location, Vector2 position, int timer)
    {
        var fakeFarmer = new FakeFarmer
        {
            UniqueMultiplayerID = Guid.NewGuid().GetHashCode(),
            currentLocation = location,
            Position = position,
        };

        Values.GetOrCreateValue(location).Add(new Musk { MuskTimer = timer, FakeFarmer = fakeFarmer });

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
                        positionFollowsAttachedCharacter = false,
                    });
        }

        location.playSound("steam");
    }

    internal static void CountdownMusk(this GameLocation location)
    {
        if (!Values.TryGetValue(location, out var muskList))
        {
            return;
        }

        for (var i = muskList.Count - 1; i >= 0; i--)
        {
            var musk = muskList[i];
            musk.MuskTimer--;
            if (musk.MuskTimer <= 0)
            {
                Values.GetOrCreateValue(location).Remove(musk);
            }
            else if (musk.MuskTimer % 60 == 0)
            {
                Reflector.GetStaticFieldGetter<Multiplayer>(typeof(Game1), "multiplayer").Invoke()
                    .broadcastSprites(
                        location,
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
                            attachedCharacter = musk.FakeFarmer,
                        });
            }
        }
    }

    internal class Musk
    {
        public int MuskTimer { get; internal set; }

        public FakeFarmer? FakeFarmer { get; internal set; }
    }
}
