namespace DaLion.Overhaul.Modules.Combat.Patchers.Ranged;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Overhaul.Modules.Combat.Extensions;
using DaLion.Overhaul.Modules.Combat.VirtualProperties;
using DaLion.Shared.Attributes;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley.Monsters;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
[ImplicitIgnore]
internal sealed class GameLocationDamageMonsterPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="GameLocationDamageMonsterPatcher"/> class.</summary>
    internal GameLocationDamageMonsterPatcher()
    {
        this.Target = this.RequireMethod<GameLocation>(
            nameof(GameLocation.damageMonster),
            new[]
            {
                typeof(Rectangle), typeof(int), typeof(int), typeof(bool), typeof(float), typeof(int),
                typeof(float), typeof(float), typeof(bool), typeof(Farmer),
            });
    }

    #region harmony patches

    /// <summary>Slingshot special stun.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? GameLocationDamageMonsterTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        // From: else if (damageAmount > 0) { ... }
        // To: else { DoSlingshotSpecial(monster, who); if (damageAmount > 0) { ... } }
        try
        {
            helper
                .Match(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Ldloc_S, helper.Locals[8]),
                        new CodeInstruction(OpCodes.Ldc_I4_0),
                        new CodeInstruction(OpCodes.Ble),
                    },
                    ILHelper.SearchOption.First)
                .StripLabels(out var labels)
                .Insert(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Ldloc_2),
                        new CodeInstruction(OpCodes.Ldarg_S, (byte)10),
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(GameLocationDamageMonsterPatcher).RequireMethod(nameof(DoSlingshotSpecial))),
                    },
                    labels);
        }
        catch (Exception ex)
        {
            Log.E($"Failed adding slingshot special stun.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region injected subroutines

    private static void DoSlingshotSpecial(Monster monster, Farmer who)
    {
        if (who.CurrentTool is not Slingshot slingshot || !slingshot.Get_IsOnSpecial())
        {
            return;
        }

        var (x, y) = who.getTileLocation() * Game1.tileSize;
        if (slingshot.GetAreaOfEffect((int)x, (int)y, who).Intersects(monster.GetBoundingBox()))
        {
            monster.Stun(1000);
        }
    }

    #endregion injected subroutines
}
