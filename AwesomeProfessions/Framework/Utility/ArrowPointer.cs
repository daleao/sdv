using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

// ReSharper disable CompareOfFloatsByEqualityOperator

namespace TheLion.Stardew.Professions.Framework.Utility;

/// <summary>Vertical arrow indicator to reveal on-screen objects of interest for tracker professions.</summary>
public class ArrowPointer
{
    private const float MAX_STEP_F = 3f, MIN_STEP_F = -3f;

    private float _height = -42f, _jerk = 1f, _step;

    public Texture2D Texture { get; } =
        ModEntry.ModHelper.Content.Load<Texture2D>(Path.Combine("assets", "hud", "pointer.png"));

    /// <summary>Advance the pointer's vertical offset motion by one step, in a bobbing fashion.</summary>
    public void Bob()
    {
        if (_step == MAX_STEP_F || _step == MIN_STEP_F) _jerk = -_jerk;
        _step += _jerk;
        _height += _step;
    }

    /// <summary>Get the pointer's current vertical offset.</summary>
    public Vector2 GetOffset()
    {
        return new(0f, _height);
    }
}