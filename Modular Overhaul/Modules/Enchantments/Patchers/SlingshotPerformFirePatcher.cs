namespace DaLion.Overhaul.Modules.Enchantments.Patchers;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Overhaul.Modules.Enchantments.Ranged;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Netcode;
using StardewValley.Projectiles;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class SlingshotPerformFirePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="SlingshotPerformFirePatcher"/> class.</summary>
    internal SlingshotPerformFirePatcher()
    {
        this.Target = this.RequireMethod<Slingshot>(nameof(Slingshot.PerformFire));
    }

    #region harmony patches

    /// <summary>Apply Magnum and Preserving effect.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? SlingshotPerformFireTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        if (SlingshotsModule.ShouldEnable)
        {
            return null;
        }

        var helper = new ILHelper(original, instructions);

        try
        {
            helper
                .Match(
                    new[]
                    {
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(NetCollection<Projectile>).RequireMethod(nameof(NetCollection<Projectile>.Add))),
                    })
                .Insert(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(SlingshotPerformFirePatcher).RequireMethod(nameof(ApplyMagnumIfNecessary))),
                    });
        }
        catch (Exception ex)
        {
            Log.E($"Failed adding Magnum effect.\nHelper returned {ex}");
            return null;
        }

        try
        {
            var skipAmmoConsumption = generator.DefineLabel();
            helper
                .Match(
                    new[] { new CodeInstruction(OpCodes.Stloc_S, helper.Locals[4]) },
                    ILHelper.SearchOption.First)
                .Move()
                .Insert(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(Tool).RequireMethod(nameof(Tool.hasEnchantmentOfType)).MakeGenericMethod(typeof(PreservingEnchantment))),
                        new CodeInstruction(OpCodes.Brtrue_S, skipAmmoConsumption),
                    })
                .Match(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Ldc_I4_1),
                        new CodeInstruction(OpCodes.Stloc_S, helper.Locals[5]),
                    })
                .AddLabels(skipAmmoConsumption);
        }
        catch (Exception ex)
        {
            Log.E($"Failed adding Preserving effect.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region injected subroutines

    private static BasicProjectile ApplyMagnumIfNecessary(BasicProjectile projectile, Slingshot slingshot)
    {
        if (slingshot.hasEnchantmentOfType<MagnumEnchantment>())
        {
            projectile.startingScale.Value *= 2f;
        }

        return projectile;
    }

    #endregion injected subroutines
}
