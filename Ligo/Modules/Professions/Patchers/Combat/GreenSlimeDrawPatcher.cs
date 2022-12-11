namespace DaLion.Ligo.Modules.Professions.Patchers.Combat;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Shared.Attributes;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Monsters;

#endregion using directives

[UsedImplicitly]
[ImplicitIgnore]
internal sealed class GreenSlimeDrawPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="GreenSlimeDrawPatcher"/> class.</summary>
    internal GreenSlimeDrawPatcher()
    {
        this.Target = this.RequireMethod<GreenSlime>(nameof(GreenSlime.draw), new[] { typeof(SpriteBatch) });
    }

    #region harmony patches

    /// <summary>Patch to fix Green Slime eye and antenna position when inflated.</summary>
    [HarmonyTranspiler]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1114:Parameter list should follow declaration", Justification = "Transpiler benefits from line-by-line commentary.")]
    private static IEnumerable<CodeInstruction>? GreenSlimeDrawTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        // Injected: antenna position += GetAntennaOffset(this)
        //			 eyes position += GetEyesOffset(this)
        var drawInstructions = new CodeInstruction[]
        {
            new(OpCodes.Ldarg_1),
            new(OpCodes.Ldarg_0),
            new(OpCodes.Callvirt, typeof(Character).RequirePropertyGetter(nameof(Character.Sprite))),
            new(OpCodes.Callvirt, typeof(AnimatedSprite).RequirePropertyGetter(nameof(AnimatedSprite.Texture))),
            new(OpCodes.Ldarg_0),
            new(OpCodes.Ldsfld, typeof(Game1).RequireField(nameof(Game1.viewport))),
            new(OpCodes.Call, typeof(Character).RequireMethod(nameof(Character.getLocalPosition))),
        };

        try
        {
            helper
                .Match(drawInstructions) // the main sprite draw call
                .Match(drawInstructions) // find antenna draw call
                .Match(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Ldloc_S, helper.Locals[5]),
                    }) // advance until end of position argument
                .Move(-1)
                .Copy(out var got, moveStackPointer: true) // copy vector addition instruction
                .Insert(
                    new[]
                    {
                        // insert custom offset
                        new CodeInstruction(OpCodes.Ldarg_0), new CodeInstruction(
                            OpCodes.Call,
                            typeof(GreenSlimeDrawPatcher).RequireMethod(nameof(GetAntennaeOffset))),
                    })
                .Insert(got) // insert addition
                .Match(drawInstructions) // find eyes draw call
                .Match(
                    new[] { new CodeInstruction(OpCodes.Ldc_I4_S, 32) }) // advance until end of position argument
                .Insert(
                    new[]
                    {
                        // insert custom offset
                        new CodeInstruction(OpCodes.Ldarg_0), new CodeInstruction(
                            OpCodes.Call,
                            typeof(GreenSlimeDrawPatcher).RequireMethod(nameof(GetEyesOffset))),
                    })
                .Insert(got); // insert addition
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

        var x = MathHelper.Lerp(0, -32f, slime.Scale - 1f);
        var y = MathHelper.Lerp(0, -64f, slime.Scale - 1f);
        return new Vector2(x, y);
    }

    private static Vector2 GetEyesOffset(GreenSlime slime)
    {
        if (slime.Scale <= 1f)
        {
            return Vector2.Zero;
        }

        var x = MathHelper.Lerp(0, -32f, slime.Scale - 1f);
        var y = MathHelper.Lerp(0, -32f, slime.Scale - 1f);
        return new Vector2(x, y);
    }

    #endregion injected subroutines
}
