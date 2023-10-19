namespace DaLion.Overhaul.Modules.Combat.Patchers.Quests.Infinity;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Overhaul.Modules.Combat.Projectiles;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Projectiles;

#endregion using directives

[UsedImplicitly]
internal sealed class ProjectileDrawPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="ProjectileDrawPatcher"/> class.</summary>
    internal ProjectileDrawPatcher()
    {
        this.Target = typeof(Projectile).RequireMethod(nameof(Projectile.draw), new[] { typeof(SpriteBatch) });
    }

    #region harmony patches

    /// <summary>Remove light projectile shadow.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? ProjectileDrawTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        try
        {
            var drawSpecialOrder = generator.DefineLabel();
            var resumeExecution = generator.DefineLabel();
            helper
                .Match(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Ldsfld, typeof(Game1).RequireField(nameof(Game1.shadowTexture))),
                    })
                .Match(new[] { new CodeInstruction(OpCodes.Ble_Un) }, ILHelper.SearchOption.Previous)
                .GetOperand(out var skipShadow)
                .Move()
                .Insert(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(OpCodes.Isinst, typeof(BlessedProjectile)),
                        new CodeInstruction(OpCodes.Brtrue, skipShadow), new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(OpCodes.Isinst, typeof(InfinityProjectile)),
                        new CodeInstruction(OpCodes.Brtrue, skipShadow),
                    });
        }
        catch (Exception ex)
        {
            Log.E($"Failed removing light projectile shadow.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}
