namespace DaLion.Enchantments.Framework.Patchers;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Enchantments.Framework.Enchantments;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Netcode;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class MeleeWeaponDoDamagePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="MeleeWeaponDoDamagePatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal MeleeWeaponDoDamagePatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireMethod<MeleeWeapon>(nameof(MeleeWeapon.DoDamage));
    }

    #region harmony patches

    /// <summary>Override `special = false` for stabby lunge.</summary>
    [HarmonyTranspiler]
    [UsedImplicitly]
    private static IEnumerable<CodeInstruction>? MeleeWeaponDoDamageTranspiler(
        IEnumerable<CodeInstruction> instructions,
        MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        // From: isOnSpecial = false;
        // To: isOnSpecial = (type.Value == MeleeWeapon.defenseSword && isOnSpecial && hasEnchantmentOfType<StabbingSword>());
        try
        {
            helper
                .PatternMatch([
                    new CodeInstruction(
                        OpCodes.Stfld,
                        typeof(MeleeWeapon).RequireField(nameof(MeleeWeapon.isOnSpecial))),
                ])
                .Move(-1)
                .Insert([
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldfld, typeof(MeleeWeapon).RequireField(nameof(MeleeWeapon.type))),
                    new CodeInstruction(
                        OpCodes.Callvirt,
                        typeof(NetInt).RequireMethod("get_Value")),
                    new CodeInstruction(OpCodes.Ldc_I4_3), // 3 = MeleeWeapon.defenseSword
                    new CodeInstruction(OpCodes.Ceq),
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(
                        OpCodes.Ldfld,
                        typeof(MeleeWeapon).RequireField(nameof(MeleeWeapon.isOnSpecial))),
                    new CodeInstruction(OpCodes.And),
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(
                        OpCodes.Callvirt,
                        typeof(Tool).RequireMethod(nameof(Tool.hasEnchantmentOfType))
                            .MakeGenericMethod(typeof(StabbingEnchantment))),
                    new CodeInstruction(OpCodes.And),
                ])
                .Remove();
        }
        catch (Exception ex)
        {
            Log.E($"Failed to prevent special stabby lunge override.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}
