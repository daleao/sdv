namespace DaLion.Redux.Framework.Professions.Patches.Combat;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Netcode;
using StardewValley.Projectiles;
using HarmonyPatch = DaLion.Shared.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class ProjectileUpdatePatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="ProjectileUpdatePatch"/> class.</summary>
    internal ProjectileUpdatePatch()
    {
        this.Target = this.RequireMethod<Projectile>(nameof(Projectile.update));
    }

    #region harmony patches

    /// <summary>Patch to detect bounced bullets.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? ProjectileUpdateTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new IlHelper(original, instructions);

        // Injected: this.DidBounce = true;
        // After: bouncesLeft.Value--;
        try
        {
            var projectile = generator.DeclareLocal(typeof(ReduxProjectile));
            var notTrickShot = generator.DefineLabel();
            helper
                .FindFirst(
                    new CodeInstruction(OpCodes.Ldfld, typeof(Projectile).RequireField("bouncesLeft")),
                    new CodeInstruction(OpCodes.Dup))
                .AdvanceUntil(
                    new CodeInstruction(
                        OpCodes.Callvirt,
                        typeof(NetFieldBase<int, NetInt>).RequirePropertySetter("Value")))
                .Advance()
                .AddLabels(notTrickShot)
                .InsertInstructions(
                    // check if this is BasicProjectile
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Isinst, typeof(ReduxProjectile)),
                    new CodeInstruction(OpCodes.Stloc_S, projectile),
                    new CodeInstruction(OpCodes.Ldloc_S, projectile),
                    new CodeInstruction(OpCodes.Brfalse_S, notTrickShot),
                    // check if is colliding with monster
                    new CodeInstruction(OpCodes.Ldarg_2),
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(
                        OpCodes.Callvirt,
                        typeof(Projectile).RequireMethod(nameof(Projectile.getBoundingBox))),
                    new CodeInstruction(OpCodes.Ldc_I4_0),
                    new CodeInstruction(
                        OpCodes.Callvirt,
                        typeof(GameLocation).RequireMethod(
                            nameof(GameLocation.doesPositionCollideWithCharacter),
                            new[] { typeof(Rectangle), typeof(bool) })),
                    new CodeInstruction(OpCodes.Ldnull),
                    new CodeInstruction(OpCodes.Bgt_Un_S, notTrickShot),
                    // add to bounced bullet set
                    new CodeInstruction(OpCodes.Ldloc_S, projectile),
                    new CodeInstruction(OpCodes.Ldc_I4_1),
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(ReduxProjectile).RequirePropertySetter(nameof(ReduxProjectile.DidBounce))));
        }
        catch (Exception ex)
        {
            Log.E($"Failed while patching prestiged Rascal trick shot.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}
