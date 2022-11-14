namespace DaLion.Ligo.Modules.Arsenal.Weapons.Patches;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Tools;
using HarmonyPatch = DaLion.Shared.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class MeleeWeaponDrawInMenuPatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="MeleeWeaponDrawInMenuPatch"/> class.</summary>
    internal MeleeWeaponDrawInMenuPatch()
    {
        this.Target = this.RequireMethod<MeleeWeapon>(
            nameof(MeleeWeapon.drawInMenu),
            new[]
            {
                typeof(SpriteBatch), typeof(Vector2), typeof(float), typeof(float), typeof(float),
                typeof(StackDrawType), typeof(Color), typeof(bool),
            });
    }

    #region harmony patches

    /// <summary>Draw stabby sword cooldown.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? MeleeWeaponDrawInMenuTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new IlHelper(original, instructions);

        // Injected: else if (attackSwordCooldown > 0)
        //     { coolDownLevel = (float)defenseCooldown / 1500f; }
        // Before: addedScale = addedSwordScale;
        try
        {
            var tryAttackSwordCooldown = generator.DefineLabel();
            helper
                .FindFirst(
                    new CodeInstruction(
                        OpCodes.Ldsfld,
                        typeof(MeleeWeapon).RequireField(nameof(MeleeWeapon.defenseCooldown))),
                    new CodeInstruction(OpCodes.Ldc_I4_0),
                    new CodeInstruction(OpCodes.Ble_S))
                .Advance(2)
                .GetOperand(out var resumeExecution)
                .SetOperand(tryAttackSwordCooldown)
                .AdvanceUntil(
                    new CodeInstruction(OpCodes.Ldsfld, typeof(MeleeWeapon).RequireField("addedSwordScale")))
                .InsertWithLabels(
                    new[] { tryAttackSwordCooldown },
                    new CodeInstruction(
                        OpCodes.Ldsfld,
                        typeof(MeleeWeapon).RequireField(nameof(MeleeWeapon.attackSwordCooldown))),
                    new CodeInstruction(OpCodes.Ldc_I4_0),
                    new CodeInstruction(OpCodes.Ble_S, resumeExecution),
                    new CodeInstruction(
                        OpCodes.Ldsfld,
                        typeof(MeleeWeapon).RequireField(nameof(MeleeWeapon.attackSwordCooldown))),
                    new CodeInstruction(OpCodes.Conv_R4),
                    new CodeInstruction(OpCodes.Ldc_R4, 2000f),
                    new CodeInstruction(OpCodes.Div),
                    new CodeInstruction(OpCodes.Stloc_0));
        }
        catch (Exception ex)
        {
            Log.E($"Failed adding attack sword cooldown in menu.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}
