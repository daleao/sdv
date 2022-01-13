using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Monsters;
using DaLion.Stardew.Common.Harmony;

namespace DaLion.Stardew.Professions.Framework.Patches.Combat;

internal class GreenSlimeDrawPatch : BasePatch
{
    /// <summary>Construct an instance.<w/ summary>
    internal GreenSlimeDrawPatch()
    {
        Original = null;
    }

    #region harmony patches

    /// <summary>Patch to fix Green Slime eye and antenna position when inflated.</summary>
    private static IEnumerable<CodeInstruction> GreenSlimeDrawTranspiler(IEnumerable<CodeInstruction> instructions,
        ILGenerator iLGenerator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        /// Injected: antenna position += GetAntennaOffset(this)
        ///			  eyes position += GetEyesOffset(this)

        try
        {
            helper
                .FindFirst( // find main sprite draw call
                    new CodeInstruction(OpCodes.Ldarg_1),
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Callvirt,
                        typeof(Character).PropertyGetter(nameof(Character.Sprite))),
                    new CodeInstruction(OpCodes.Callvirt,
                        typeof(AnimatedSprite).PropertyGetter(nameof(AnimatedSprite.Texture))),
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldsfld, typeof(Game1).Field(nameof(Game1.viewport))),
                    new CodeInstruction(OpCodes.Call,
                        typeof(Character).MethodNamed(nameof(Character.getLocalPosition)))
                )
                .FindNext( // find antenna draw call
                    new CodeInstruction(OpCodes.Ldarg_1),
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Callvirt,
                        typeof(Character).PropertyGetter(nameof(Character.Sprite))),
                    new CodeInstruction(OpCodes.Callvirt,
                        typeof(AnimatedSprite).PropertyGetter(nameof(AnimatedSprite.Texture))),
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldsfld, typeof(Game1).Field(nameof(Game1.viewport))),
                    new CodeInstruction(OpCodes.Call,
                        typeof(Character).MethodNamed(nameof(Character.getLocalPosition)))
                )
                .AdvanceUntil( // advance until end of position argument
                    new CodeInstruction(OpCodes.Ldloc_S, $"{typeof(int)} (5)")
                )
                .Retreat()
                .ToBuffer(advance: true) // copy vector addition instruction
                .Insert( // insert custom offset
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Call,
                        typeof(GreenSlimeDrawPatch).MethodNamed(nameof(GetAntennaeOffset)))
                )
                .InsertBuffer() // insert addition
                .FindNext( // find eyes draw call
                    new CodeInstruction(OpCodes.Ldarg_1),
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Callvirt,
                        typeof(Character).PropertyGetter(nameof(Character.Sprite))),
                    new CodeInstruction(OpCodes.Callvirt,
                        typeof(AnimatedSprite).PropertyGetter(nameof(AnimatedSprite.Texture))),
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldsfld, typeof(Game1).Field(nameof(Game1.viewport))),
                    new CodeInstruction(OpCodes.Call,
                        typeof(Character).MethodNamed(nameof(Character.getLocalPosition)))
                )
                .AdvanceUntil( // advance until end of position argument
                    new CodeInstruction(OpCodes.Ldc_I4_S, 32)
                )
                .Insert( // insert custom offset
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Call,
                        typeof(GreenSlimeDrawPatch).MethodNamed(nameof(GetEyesOffset)))
                )
                .InsertBuffer(); // insert addition
        }
        catch (Exception ex)
        {
            ModEntry.Log($"Failed while patching inflated Green Slime sprite.\nHelper returned {ex}",
                LogLevel.Error);
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region private methods

    private static Vector2 GetAntennaeOffset(GreenSlime slime)
    {
        if (slime.Scale <= 1f) return Vector2.Zero;

        var x = MathHelper.Lerp(0, -32f, slime.Scale - 1f);
        var y = MathHelper.Lerp(0, -64f, slime.Scale - 1f);
        return new(x, y);
    }

    private static Vector2 GetEyesOffset(GreenSlime slime)
    {
        if (slime.Scale <= 1f) return Vector2.Zero;

        var x = MathHelper.Lerp(0, -32f, slime.Scale - 1f);
        var y = MathHelper.Lerp(0, -32f, slime.Scale - 1f);
        return new(x, y);
    }

    #endregion private methods
}