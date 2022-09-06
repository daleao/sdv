namespace DaLion.Stardew.Rings.Extensions;

#region using directives

using Framework;
using Framework.VirtualProperties;
using Microsoft.Xna.Framework;
using StardewValley.Objects;
using System;
using System.Linq;

#endregion using directives

/// <summary>Extensions for the <see cref="CombinedRing"/> class.</summary>
public static class CombinedRingExtensions
{
    /// <summary>Apply resonant glow to the current location.</summary>
    /// <param name="location">The current location.</param>
    /// <param name="who">The wielder.</param>
    public static void ApplyResonanceGlow(this CombinedRing combined, GameLocation location, Farmer who)
    {
        if (combined.ParentSheetIndex != Constants.IRIDIUM_BAND_INDEX_I) return;

        var phases = combined.get_Phases().ToArray();
        if (phases.Length <= 0) return;

        for (var i = 0; i < 2; ++i)
        {
            var ph = phases[i];
            if (ph is null) continue;

            if (ph.LightSourceId.HasValue)
            {
                location.removeLightSource(ph.LightSourceId.Value);
                ph.LightSourceId = null;
            }

            ph.LightSourceId = ModEntry.Manifest.UniqueID.GetHashCode() + combined.GetHashCode() + (int)who.UniqueMultiplayerID;
            while (location.sharedLights.ContainsKey(ph.LightSourceId!.Value)) ++ph.LightSourceId;

            var factor = (float)(i == 0 ? Math.Sin(Phase.Angle) : Math.Cos(Phase.Angle));
            var color = factor > 0
                ? Color.Lerp(ph.Peak.Color, ph.Trough.Color, Math.Abs(factor))
                : Color.Lerp(ph.Trough.Color, ph.Peak.Color, Math.Abs(factor));
            var radius = MathHelper.Lerp((float)(3d / 4d) * ph.Intensity, (float)(5d / 4d) * ph.Intensity, Math.Abs(factor));
            Game1.currentLightSources.Add(new(1, new(who.Position.X + 21f, who.Position.Y + 64f),
                radius, color, ph.LightSourceId.Value, LightSource.LightContext.None, who.UniqueMultiplayerID));
        }
    }

    /// <summary>Remove resonant glow from the current location.</summary>
    /// <param name="location">The current location.</param>
    public static void UnapplyResonanceGlow(this CombinedRing combined, GameLocation location)
    {
        if (combined.ParentSheetIndex != Constants.IRIDIUM_BAND_INDEX_I) return;

        var phases = combined.get_Phases().ToArray();
        if (phases.Length <= 0) return;

        for (var i = 0; i < 2; ++i)
        {
            var ph = phases[i];
            if (ph?.LightSourceId.HasValue != true) continue;

            location.removeLightSource(ph.LightSourceId!.Value);
            ph.LightSourceId = null;
        }
    }

    /// <summary>Reposition resonant glow and apply phase shifts on update tick.</summary>
    /// <param name="location">The current location.</param>
    /// <param name="who">The wielder.</param>
    public static void UpdateResonanceGlow(this CombinedRing combined, GameLocation location, Farmer who)
    {
        if (combined.ParentSheetIndex != Constants.IRIDIUM_BAND_INDEX_I) return;

        var phases = combined.get_Phases().ToArray();
        if (phases.Length <= 0) return;

        for (var i = 0; i < 2; ++i)
        {
            var ph = phases[i];
            if (ph?.LightSourceId.HasValue != true) continue;

            // reposition
            var offset = Vector2.Zero;
            if (who.shouldShadowBeOffset)
                offset += who.drawOffset.Value;

            location.repositionLightSource(ph.LightSourceId!.Value, new Vector2(who.Position.X + 21f, who.Position.Y) + offset);

            // apply phase shift
            var factor = (float)(i == 0 ? Math.Sin(Phase.Angle) : Math.Cos(Phase.Angle));
            var color = factor > 0
                ? Color.Lerp(ph.Peak.Color, ph.Trough.Color, Math.Abs(factor))
                : Color.Lerp(ph.Trough.Color, ph.Peak.Color, Math.Abs(factor));
            var radius = MathHelper.Lerp((float)(3d / 4d) * ph.Intensity, (float)(5d / 4d) * ph.Intensity, Math.Abs(factor));
            location.sharedLights[ph.LightSourceId.Value].color.Value = color;
            location.sharedLights[ph.LightSourceId.Value].radius.Value = radius;
        }
    }
}