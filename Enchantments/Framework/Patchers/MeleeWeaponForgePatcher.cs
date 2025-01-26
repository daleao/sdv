namespace DaLion.Enchantments.Framework.Patchers;

#region using directives

using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class MeleeWeaponForgePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="MeleeWeaponForgePatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal MeleeWeaponForgePatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireMethod<MeleeWeapon>(nameof(MeleeWeapon.Forge));
    }

    #region harmony patches

    /// <summary>Allow enchanting Scythe.</summary>
    [HarmonyTranspiler]
    [UsedImplicitly]
    private static IEnumerable<CodeInstruction> MeleeWeaponForgeTranspiler(IEnumerable<CodeInstruction> instructions)
    {
        // Skip: if (isScythe()) return false;
        return instructions.SkipWhile(instruction => instruction.opcode != OpCodes.Ldarg_1);
    }

    #endregion harmony patches
}
