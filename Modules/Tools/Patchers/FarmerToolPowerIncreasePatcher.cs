namespace DaLion.Overhaul.Modules.Tools.Patchers;

#region using directives

using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Shared.Extensions.Xna;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class FarmerToolPowerIncreasePatcher : HarmonyPatcher
{
    private static readonly Random Random = new(Guid.NewGuid().GetHashCode());

    /// <summary>Initializes a new instance of the <see cref="FarmerToolPowerIncreasePatcher"/> class.</summary>
    internal FarmerToolPowerIncreasePatcher()
    {
        this.Target = this.RequireMethod<Farmer>("toolPowerIncrease");
    }

    #region harmony patches

    [HarmonyPrefix]
    private static bool FarmerToolPowerIncreasePrefix(Farmer __instance)
    {
        if (__instance.toolPower == 0)
        {
            Reflector.GetUnboundFieldSetter<Farmer, int>(__instance, "toolPitchAccumulator").Invoke(__instance, 0);
        }

        __instance.toolPower++;
        var powerUpColor = Color.White;
        var frameOffset = __instance.FacingDirection == Game1.up
            ? 4
            : __instance.FacingDirection == Game1.down
                ? 2
                : 0;
        switch (__instance.toolPower)
        {
            case 1:
                powerUpColor = Color.Orange;
                if (__instance.CurrentTool is not WateringCan)
                {
                    __instance.FarmerSprite.CurrentFrame = 72 + frameOffset;
                }

                __instance.jitterStrength = 0.25f;
                break;
            case 2:
                powerUpColor = Color.LightSteelBlue;
                if (__instance.CurrentTool is not WateringCan)
                {
                    __instance.FarmerSprite.CurrentFrame++;
                }

                __instance.jitterStrength = 0.5f;
                break;
            case 3:
                powerUpColor = Color.Gold;
                __instance.jitterStrength = 1f;
                break;
            case 4:
                powerUpColor = Color.Violet;
                __instance.jitterStrength = 2f;
                break;
            case 5:
                powerUpColor = __instance.CurrentTool.UpgradeLevel >= 5 ? Color.GreenYellow : Color.BlueViolet;
                __instance.jitterStrength = 3f;
                break;
            case 6:
                powerUpColor = __instance.CurrentTool.UpgradeLevel >= 6
                    ? Color.Cyan
                    : Color.YellowGreen;
                __instance.jitterStrength = 4f;
                break;
            case 7:
                powerUpColor = Color.DarkTurquoise;
                __instance.jitterStrength = 5f;
                break;
        }

        int xAnimation, yAnimation;
        if (__instance.CurrentTool is WateringCan)
        {
            xAnimation = __instance.FacingDirection == Game1.right
                ? -48
                : __instance.FacingDirection == Game1.left
                    ? 48
                    : 0;

            yAnimation = 128;
        }
        else
        {
            xAnimation = __instance.FacingDirection == Game1.right
                ? 40
                : __instance.FacingDirection == Game1.left
                    ? -40
                    : __instance.FacingDirection == Game1.down
                        ? 32
                        : 0;
            yAnimation = 192;
        }

        Game1.currentLocation.temporarySprites.Add(
            new TemporaryAnimatedSprite(
                21,
                __instance.Position - new Vector2(xAnimation, yAnimation),
                powerUpColor,
                8,
                false,
                70f,
                0,
                64,
                (__instance.getStandingY() / 10000f) + 0.005f,
                128));
        Game1.currentLocation.temporarySprites.Add(
            new TemporaryAnimatedSprite(
                "TileSheets\\animations",
                new Rectangle(192, 1152, 64, 64),
                50f,
                4,
                0,
                __instance.Position - new Vector2(__instance.FacingDirection != 1 ? -64 : 0, 128f),
                false,
                __instance.FacingDirection == 1,
                __instance.getStandingY() / 10000f,
                0.01f,
                Color.White,
                1f,
                0f,
                0f,
                0f));
        if (Game1.soundBank == null)
        {
            return false; // don't run original logic
        }

        var cue = Game1.soundBank.GetCue("toolCharge");
        cue.SetVariable("Pitch", (Random.Next(12, 16) * 100) + (__instance.toolPower * 100));
        cue.Play();
        return false; // don't run original logic
    }

    #endregion harmony patches
}
