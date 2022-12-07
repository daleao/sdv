namespace DaLion.Ligo.Modules.Arsenal.Patchers.Infinity;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Ligo.Modules.Arsenal.Enchantments;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class ToolForgePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="ToolForgePatcher"/> class.</summary>
    internal ToolForgePatcher()
    {
        this.Target = this.RequireMethod<Tool>(nameof(Tool.Forge));
    }

    #region harmony patches

    /// <summary>Transform Galaxy Slingshot into Infinity Slingshot.</summary>
    [HarmonyPrefix]
    private static bool ToolForgePrefix(Tool __instance, ref bool __result, Item item, bool count_towards_stats)
    {
        if (__instance is not Slingshot { InitialParentTileIndex: Constants.GalaxySlingshotIndex } slingshot)
        {
            return true; // run original logic
        }

        var enchantment = BaseEnchantment.GetEnchantmentFromItem(__instance, item);
        if (ModEntry.Config.Arsenal.InfinityPlusOne)
        {
            if (enchantment is not InfinityEnchantment)
            {
                return true; // run original logic
            }
        }
        else
        {
            if (enchantment is not GalaxySoulEnchantment)
            {
                return true; // run original logic
            }
        }

        if (!slingshot.AddEnchantment(enchantment) || slingshot.GetEnchantmentLevel<GalaxySoulEnchantment>() < 3)
        {
            return true; // run original logic
        }

        slingshot.CurrentParentTileIndex = Constants.InfinitySlingshotIndex;
        slingshot.InitialParentTileIndex = Constants.InfinitySlingshotIndex;
        slingshot.IndexOfMenuItemView = Constants.InfinitySlingshotIndex;
        slingshot.BaseName = "Infinity Slingshot";
        slingshot.DisplayName = ModEntry.i18n.Get("infinityslingshot.name");
        slingshot.description = ModEntry.i18n.Get("infinityslingshot.desc");
        if (count_towards_stats)
        {
            DelayedAction.playSoundAfterDelay("discoverMineral", 400);
            ModEntry.Reflector.GetStaticFieldGetter<Multiplayer>(typeof(Game1), "multiplayer").Invoke()
                .globalChatInfoMessage("InfinityWeapon", Game1.player.Name, slingshot.DisplayName);

            slingshot.previousEnchantments.Insert(0, enchantment.GetName());
            while (slingshot.previousEnchantments.Count > 2)
            {
                slingshot.previousEnchantments.RemoveAt(slingshot.previousEnchantments.Count - 1);
            }

            Game1.stats.incrementStat("timesEnchanted", 1);
        }

        var galaxyEnchantment = __instance.GetEnchantmentOfType<GalaxySoulEnchantment>();
        if (galaxyEnchantment is not null)
        {
            __instance.RemoveEnchantment(galaxyEnchantment);
        }

        __result = true;
        return false; // don't run original logic
    }

    /// <summary>Require Hero Soul to transform Galaxy into Infinity.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? ToolForgeTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        // From: if (enchantment is GalaxySoulEnchantment)
        // To: if (enchantment is (Config.InfinityPlusOne ? InfinityEnchantment : GalaxySoulEnchantment))
        try
        {
            var checkForGalaxy = generator.DefineLabel();
            var resumeExecution = generator.DefineLabel();
            helper
                .FindFirst(
                    new CodeInstruction(OpCodes.Isinst, typeof(GalaxySoulEnchantment)))
                .AddLabels(checkForGalaxy)
                .InsertInstructions(
                    new CodeInstruction(OpCodes.Call, typeof(ModEntry).RequirePropertyGetter(nameof(ModEntry.Config))),
                    new CodeInstruction(
                        OpCodes.Callvirt,
                        typeof(ModConfig).RequirePropertyGetter(nameof(ModConfig.Arsenal))),
                    new CodeInstruction(
                        OpCodes.Callvirt,
                        typeof(Config).RequirePropertyGetter(nameof(Config.InfinityPlusOne))),
                    new CodeInstruction(OpCodes.Brfalse_S, checkForGalaxy),
                    new CodeInstruction(OpCodes.Isinst, typeof(InfinityEnchantment)),
                    new CodeInstruction(OpCodes.Br_S, resumeExecution))
                .Advance()
                .AddLabels(resumeExecution)
                .FindNext(
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(Tool)
                            .RequireMethod(nameof(Tool.GetEnchantmentOfType))
                            .MakeGenericMethod(typeof(GalaxySoulEnchantment))))
                .StripLabels(out var labels)
                .AdvanceUntil(
                    new CodeInstruction(OpCodes.Brfalse_S))
                .GetOperand(out var toRemove)
                .Return()
                .RemoveInstructionsUntil(
                    new CodeInstruction(OpCodes.Callvirt, typeof(Tool).RequireMethod(nameof(Tool.RemoveEnchantment))))
                .RemoveLabels((Label)toRemove)
                .AddLabels(labels);
        }
        catch (Exception ex)
        {
            Log.E($"Failed injecting Hero Soul condition for Infinity Blade.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}
