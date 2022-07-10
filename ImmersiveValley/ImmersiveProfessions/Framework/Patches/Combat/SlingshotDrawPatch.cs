namespace DaLion.Stardew.Professions.Framework.Patches.Combat;

#region using directives

using Extensions;
using HarmonyLib;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Tools;
using Ultimates;

#endregion using directives

[UsedImplicitly]
internal sealed class SlingshotDrawPatch : DaLion.Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal SlingshotDrawPatch()
    {
        Target = RequireMethod<Slingshot>(nameof(Slingshot.draw));
    }

    /// <summary>Patch to draw Slingshot overcharge meter for Desperado.</summary>
    [HarmonyPostfix]
    internal static void SlingshotDrawPostfix(Slingshot __instance, SpriteBatch b)
    {
        if (__instance.attachments[0] is null) return;

        var lastUser = __instance.getLastFarmerToUse();
        if (!lastUser.usingSlingshot || !lastUser.IsLocalPlayer || !lastUser.HasProfession(Profession.Desperado) ||
            ModEntry.PlayerState.RegisteredUltimate is DeathBlossom { IsActive: true })
            return;

        var overcharge = __instance.GetDesperadoOvercharge(Game1.player);
        if (overcharge <= 0f) return;

        b.Draw(Game1.mouseCursors,
            Game1.GlobalToLocal(Game1.viewport, lastUser.Position + new Vector2(-48f, -160f)),
            new(193, 1868, 47, 12), Color.White, 0f, Vector2.Zero, Game1.pixelZoom, SpriteEffects.None, 0.885f);

        b.Draw(Game1.staminaRect,
            new((int)Game1.GlobalToLocal(Game1.viewport, lastUser.Position).X - 36,
                (int)Game1.GlobalToLocal(Game1.viewport, lastUser.Position).Y - 148, (int)(164f * overcharge), 25),
            Game1.staminaRect.Bounds, Utility.getRedToGreenLerpColor(overcharge), 0f, Vector2.Zero, SpriteEffects.None,
            0.887f);
    }
}