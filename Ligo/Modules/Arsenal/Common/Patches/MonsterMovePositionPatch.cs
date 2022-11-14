namespace DaLion.Ligo.Modules.Arsenal.Common.Patches;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Ligo.Modules.Arsenal.Common.VirtualProperties;
using DaLion.Shared.Attributes;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley.Monsters;
using HarmonyPatch = DaLion.Shared.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
[Debug]
internal class MonsterMovePositionPatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="MonsterMovePositionPatch"/> class.</summary>
    internal MonsterMovePositionPatch()
    {
        this.Target = this.RequireMethod<Monster>(nameof(Monster.MovePosition));
    }

    #region harmony patches

    /// <summary>Add knockback damage.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? MonsterMovePositionTranspiler(
        IEnumerable<CodeInstruction> instructions, MethodBase original)
    {
        var helper = new IlHelper(original, instructions);

        // Injected: if (wasKnockedBack) CollisionDetectedSubroutine(this);
        // After: found_collision = true;
        try
        {
            helper
                .FindFirst(
                    new CodeInstruction(OpCodes.Ldc_I4_1),
                    new CodeInstruction(OpCodes.Stloc_S, helper.Locals[6]))
                .Advance(2)
                .InsertInstructions(
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Call, typeof(MonsterMovePositionPatch).RequireMethod(nameof(CollisionDetectedSubroutine))));
        }
        catch (Exception ex)
        {
            Log.E($"Failed applying debug transpiler.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region injected subroutines

    private static void CollisionDetectedSubroutine(Monster monster)
    {
        if (monster.Health <= 0 || !monster.Get_WasKnockedBack())
        {
            return;
        }

        var velocity = new Vector2(monster.xVelocity, monster.yVelocity);
        var speed = velocity.Length();
        var damage = (int)Math.Pow(speed / 10, 2);
        if (damage <= 0)
        {
            return;
        }

        monster.Health = Math.Max(monster.Health - damage, 0);

        var monsterBox = monster.GetBoundingBox();
        monster.currentLocation.debris.Add(new Debris(
            damage,
            new Vector2(monsterBox.Center.X + 16, monsterBox.Center.Y),
            Color.White,
            1f,
            monster));
        monster.Set_WasKnockedBack(false);
    }

    #endregion injected subroutines
}
