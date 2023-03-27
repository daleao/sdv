namespace DaLion.Overhaul.Modules.Slingshots;

#region using directives

using Microsoft.Xna.Framework;
using StardewValley.Tools;

#endregion using directives

/// <summary>The runtime state for Slingshot variables.</summary>
internal sealed class State
{
    internal int SlingshotCooldown { get; set; }

    internal Vector2 DriftVelocity { get; set; }

    internal Slingshot? AutoSelectableSlingshot { get; set; }
}
