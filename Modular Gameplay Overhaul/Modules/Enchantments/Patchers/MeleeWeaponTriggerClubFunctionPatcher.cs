    namespace DaLion.Overhaul.Modules.Enchantments.Patchers;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Overhaul.Modules.Core.Extensions;
using DaLion.Overhaul.Modules.Enchantments.Melee;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley.Monsters;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class MeleeWeaponTriggerClubFunctionPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="MeleeWeaponTriggerClubFunctionPatcher"/> class.</summary>
    internal MeleeWeaponTriggerClubFunctionPatcher()
    {
        this.Target = this.RequireMethod<MeleeWeapon>(nameof(MeleeWeapon.triggerClubFunction));
    }

    #region harmony patches

    /// <summary>Apply Stun after Artful club's special smash move.</summary>
    [HarmonyPostfix]
    private static void MeleeWeaponTriggerClubFunctionPostfix(MeleeWeapon __instance, Farmer ___lastUser, Farmer who)
    {
        if (!__instance.hasEnchantmentOfType<MeleeArtfulEnchantment>())
        {
            return;
        }

        var area = new Rectangle((int)___lastUser.Position.X - 192, ___lastUser.GetBoundingBox().Y - 192, 384, 384);
        area.Inflate(96, 96);
        for (var i = 0; i < who.currentLocation.characters.Count; i++)
        {
            var character = who.currentLocation.characters[i];
            if (character is Monster { IsMonster: true, Health: > 0 } monster && monster.TakesDamageFromHitbox(area))
            {
                monster.Stun(2000);
            }
        }
    }

    /// <summary>Doubles AoE of Artful club's special smash move.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? MeleeWeaponTriggerClubFunctionTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        // Injected: if (this.hasEnchantmentOfType<NewArtfulEnchantment>() areaOfEffect.Inflate(96, 96);
        // After: new Rectangle((int)lastUser.Position.X - 192, lastUser.GetBoundingBox().Y - 192, 384, 384)
        try
        {
            var notInfinity = generator.DefineLabel();
            var aoe = generator.DeclareLocal(typeof(Rectangle));
            helper
                .Match(new[] { new CodeInstruction(OpCodes.Newobj) })
                .Move()
                .Insert(new[] { new CodeInstruction(OpCodes.Stloc_S, aoe) })
                .Insert(new[] { new CodeInstruction(OpCodes.Ldloc_S, aoe) }, new[] { notInfinity })
                .Move(-1)
                .Insert(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(MeleeWeapon)
                                .RequireMethod(nameof(MeleeWeapon.hasEnchantmentOfType))
                                .MakeGenericMethod(typeof(MeleeArtfulEnchantment))),
                        new CodeInstruction(OpCodes.Brfalse_S, notInfinity),
                        new CodeInstruction(OpCodes.Ldloca_S, aoe),
                        new CodeInstruction(OpCodes.Ldc_I4_S, 96),
                        new CodeInstruction(OpCodes.Ldc_I4_S, 96),
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(Rectangle).RequireMethod(
                                nameof(Rectangle.Inflate),
                                new[] { typeof(int), typeof(int) })),
                    });
        }
        catch (Exception ex)
        {
            Log.E($"Failed adding Artful club effect.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}
