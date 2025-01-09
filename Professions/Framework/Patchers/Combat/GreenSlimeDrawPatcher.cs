namespace DaLion.Professions.Framework.Patchers.Combat;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Professions.Framework.VirtualProperties;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Monsters;

#endregion using directives

[UsedImplicitly]
internal sealed class GreenSlimeDrawPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="GreenSlimeDrawPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal GreenSlimeDrawPatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireMethod<GreenSlime>(nameof(GreenSlime.draw), [typeof(SpriteBatch)]);
    }

    #region harmony patches

    /// <summary>Draw Piped Slime health.</summary>
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void GreenSlimeDrawPostfix(GreenSlime __instance, SpriteBatch b)
    {
        if (__instance.Get_Piped() is null)
        {
            return;
        }

        const float fullBarWidth = Game1.tileSize * 0.67f;
        var position = __instance.getLocalPosition(Game1.viewport);
        position.Y += __instance.Sprite.SpriteHeight * 2.5f;
        var fillPercent = (float)__instance.Health / __instance.MaxHealth;
        var width = fullBarWidth * fillPercent;
        position.X += (__instance.Sprite.SpriteWidth * 2) - (width / 2f) + 2;
        const int height = 4;
        var color = Utility.getRedToGreenLerpColor(fillPercent);
        b.Draw(Game1.staminaRect, new Rectangle((int)position.X, (int)position.Y, (int)width, height), color);
    }

    /// <summary>Patch to fix Green Slime eye and antenna position when inflated.</summary>
    [HarmonyTranspiler]
    [UsedImplicitly]
    private static IEnumerable<CodeInstruction>? GreenSlimeDrawTranspiler(
        IEnumerable<CodeInstruction> instructions, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        // Injected: antenna position += GetAntennaOffset(this)
        //			 eyes position += GetEyesOffset(this)
        try
        {
            helper
                .PatternMatch(
                    [ // the antenna sprite draw call
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(Character).RequirePropertyGetter(nameof(Character.Sprite))),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(AnimatedSprite).RequirePropertyGetter(nameof(AnimatedSprite.Texture))),
                    ],
                    nth: 2)
                .PatternMatch([
                    new CodeInstruction(OpCodes.Ldloc_S, helper.Locals[4]), // stack_adjustment
                ])
                .Insert([
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(GreenSlimeDrawPatcher).RequireMethod(nameof(GetAntennaeOffset))),
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(Vector2).RequireMethod("op_Addition")),
                ])
                .PatternMatch([ // the eyes draw call
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(
                        OpCodes.Callvirt,
                        typeof(Character).RequirePropertyGetter(nameof(Character.Sprite))),
                    new CodeInstruction(
                        OpCodes.Callvirt,
                        typeof(AnimatedSprite).RequirePropertyGetter(nameof(AnimatedSprite.Texture))),
                ])
                .PatternMatch([
                    new CodeInstruction(OpCodes.Ldloc_S, helper.Locals[4]), // stack_adjustment
                ])
                .Insert([
                    // insert custom offset
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(GreenSlimeDrawPatcher).RequireMethod(nameof(GetEyesOffset))),
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(Vector2).RequireMethod("op_Addition")),
                ]);
        }
        catch (Exception ex)
        {
            Log.E($"Failed patching inflated Green Slime sprite.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region injected subroutines

    private static Vector2 GetAntennaeOffset(GreenSlime slime)
    {
        if (slime.Scale <= 1f)
        {
            return Vector2.Zero;
        }

        var x = MathHelper.Lerp(0, 0, slime.Scale - 1f);
        var y = MathHelper.Lerp(0, 0, slime.Scale - 1f);
        return new Vector2(x, y);
    }

    private static Vector2 GetEyesOffset(GreenSlime slime)
    {
        if (slime.Scale <= 1f)
        {
            return Vector2.Zero;
        }

        var x = MathHelper.Lerp(0, 0, slime.Scale - 1f);
        var y = MathHelper.Lerp(0, 0, slime.Scale - 1f);
        return new Vector2(x, y);
    }

    #endregion injected subroutines
}
