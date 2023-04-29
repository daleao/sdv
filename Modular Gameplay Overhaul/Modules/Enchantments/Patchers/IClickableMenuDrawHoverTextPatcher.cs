namespace DaLion.Overhaul.Modules.Enchantments.Patchers;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using DaLion.Overhaul.Modules.Enchantments.Gemstone;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Menus;

#endregion using directives

[UsedImplicitly]
internal sealed class IClickableMenuDrawHoverTextPatcher : HarmonyPatcher
{
    private static readonly Dictionary<Type, Color> ColorByEnchantmentType = new()
    {
        { typeof(RubyEnchantment), new Color(225, 57, 57) },
        { typeof(AquamarineEnchantment), new Color(35, 144, 170) },
        { typeof(AmethystEnchantment), new Color(111, 60, 196) },
        { typeof(GarnetEnchantment), new Color(152, 29, 45) },
        { typeof(EmeraldEnchantment), new Color(4, 128, 54) },
        { typeof(JadeEnchantment), new Color(117, 150, 99) },
        { typeof(TopazEnchantment), new Color(220, 143, 8) },
    };

    /// <summary>Initializes a new instance of the <see cref="IClickableMenuDrawHoverTextPatcher"/> class.</summary>
    internal IClickableMenuDrawHoverTextPatcher()
    {
        this.Target = this.RequireMethod<IClickableMenu>(
            nameof(IClickableMenu.drawHoverText),
            new[]
            {
                typeof(SpriteBatch), typeof(StringBuilder), typeof(SpriteFont), typeof(int), typeof(int),
                typeof(int), typeof(string), typeof(int), typeof(string[]), typeof(Item), typeof(int), typeof(int),
                typeof(int), typeof(int), typeof(int), typeof(float), typeof(CraftingRecipe), typeof(IList<Item>),
            });
    }

    #region harmony patches

    /// <summary>Replaces "Forged" text with socket icons.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? IClickableMenuDrawHoverTextTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        try
        {
            var addHeightInstructions = new[]
            {
                new CodeInstruction(OpCodes.Ldarg_2),
                new CodeInstruction(OpCodes.Ldstr, "T"),
                new CodeInstruction(
                    OpCodes.Callvirt,
                    typeof(SpriteFont).RequireMethod(nameof(SpriteFont.MeasureString), new[] { typeof(string) })),
                new CodeInstruction(OpCodes.Ldfld, typeof(Vector2).RequireField(nameof(Vector2.Y))),
                new CodeInstruction(OpCodes.Conv_I4),
                new CodeInstruction(OpCodes.Add),
            };

            var resumeExecution1 = generator.DefineLabel();
            var resumeExecution2 = generator.DefineLabel();
            helper
                .Match(
                    new[]
                    {
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(IClickableMenu).RequireMethod(
                                nameof(IClickableMenu.drawTextureBox),
                                new[]
                                {
                                    typeof(SpriteBatch), typeof(Texture2D), typeof(Rectangle), typeof(int),
                                    typeof(int), typeof(int), typeof(int), typeof(Color), typeof(float),
                                    typeof(bool), typeof(float),
                                })),
                    },
                    nth: 2)
                .Match(new[] { new CodeInstruction(OpCodes.Sub) }, ILHelper.SearchOption.Previous)
                .Move()
                .AddLabels(resumeExecution1)
                .Insert(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Ldarg_S, (byte)9),
                        new CodeInstruction(OpCodes.Brfalse_S, resumeExecution1),
                        new CodeInstruction(OpCodes.Ldarg_S, (byte)9),
                        new CodeInstruction(OpCodes.Isinst, typeof(Tool)),
                        new CodeInstruction(OpCodes.Brfalse_S, resumeExecution1),
                        new CodeInstruction(OpCodes.Ldarg_S, (byte)9),
                        new CodeInstruction(OpCodes.Isinst, typeof(Tool)),
                        new CodeInstruction(OpCodes.Callvirt, typeof(Tool).RequireMethod(nameof(Tool.GetMaxForges))),
                        new CodeInstruction(OpCodes.Ldc_I4_0),
                        new CodeInstruction(OpCodes.Ble_S, resumeExecution1),
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(ModEntry).RequirePropertyGetter(nameof(ModEntry.Config))),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(ModConfig).RequirePropertyGetter(nameof(ModConfig.Enchantments))),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(Config).RequirePropertyGetter(nameof(Config.DrawForgeSockets))),
                        new CodeInstruction(OpCodes.Brfalse_S, resumeExecution1), new CodeInstruction(
                            OpCodes.Call,
                            typeof(ModEntry).RequirePropertyGetter(nameof(ModEntry.Config))),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(ModConfig).RequirePropertyGetter(nameof(ModConfig.Enchantments))),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(Config).RequirePropertyGetter(nameof(Config.SocketPosition))),
                        new CodeInstruction(OpCodes.Ldc_I4_1),
                        new CodeInstruction(OpCodes.Bne_Un_S, resumeExecution1),
                    })
                .Insert(addHeightInstructions)
                .Match(new[] { new CodeInstruction(OpCodes.Sub) })
                .Move()
                .AddLabels(resumeExecution2)
                .Insert(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Ldarg_S, (byte)9),
                        new CodeInstruction(OpCodes.Brfalse_S, resumeExecution2),
                        new CodeInstruction(OpCodes.Ldarg_S, (byte)9),
                        new CodeInstruction(OpCodes.Isinst, typeof(Tool)),
                        new CodeInstruction(OpCodes.Brfalse_S, resumeExecution2),
                        new CodeInstruction(OpCodes.Ldarg_S, (byte)9),
                        new CodeInstruction(OpCodes.Isinst, typeof(Tool)),
                        new CodeInstruction(OpCodes.Callvirt, typeof(Tool).RequireMethod(nameof(Tool.GetMaxForges))),
                        new CodeInstruction(OpCodes.Ldc_I4_0),
                        new CodeInstruction(OpCodes.Ble_S, resumeExecution2),
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(ModEntry).RequirePropertyGetter(nameof(ModEntry.Config))),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(ModConfig).RequirePropertyGetter(nameof(ModConfig.Enchantments))),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(Config).RequirePropertyGetter(nameof(Config.DrawForgeSockets))),
                        new CodeInstruction(OpCodes.Brfalse_S, resumeExecution2),
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(ModEntry).RequirePropertyGetter(nameof(ModEntry.Config))),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(ModConfig).RequirePropertyGetter(nameof(ModConfig.Enchantments))),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(Config).RequirePropertyGetter(nameof(Config.SocketPosition))),
                        new CodeInstruction(OpCodes.Ldc_I4_1),
                        new CodeInstruction(OpCodes.Bne_Un_S, resumeExecution2),
                    })
                .Insert(addHeightInstructions);
        }
        catch (Exception ex)
        {
            Log.E($"Failed increasing title box height.\nHelper returned {ex}");
            return null;
        }

        try
        {
            var drawGenericText = generator.DefineLabel();
            var resumeExecution = generator.DefineLabel();
            helper
                .Match(
                    new[]
                    {
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(Tool).RequireMethod(nameof(Tool.GetTotalForgeLevels))),
                    },
                    ILHelper.SearchOption.First,
                    nth: 2)
                .Match(new[] { new CodeInstruction(OpCodes.Ldsfld) })
                .AddLabels(drawGenericText)
                .Insert(
                    new[]
                    {
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(ModEntry).RequirePropertyGetter(nameof(ModEntry.Config))),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(ModConfig).RequirePropertyGetter(nameof(ModConfig.Enchantments))),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(Config).RequirePropertyGetter(nameof(Config.DrawForgeSockets))),
                        new CodeInstruction(OpCodes.Brfalse_S, drawGenericText),
                        new CodeInstruction(OpCodes.Ldarg_S, (byte)9), // arg 9 = Item hoveredItem
                        new CodeInstruction(OpCodes.Isinst, typeof(Tool)),
                        new CodeInstruction(OpCodes.Ldloc_S, helper.Locals[5]), // local 5 = int x
                        new CodeInstruction(
                            OpCodes.Ldloca_S,
                            helper.Locals[6]), // local 6 = int y (as ref since we want to increment it)
                        new CodeInstruction(OpCodes.Ldarg_0), // arg 0 = SpriteBatch b
                        new CodeInstruction(OpCodes.Ldarg_2), // arg 2 = SpriteFont font
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(IClickableMenuDrawHoverTextPatcher).RequireMethod(nameof(DrawForgeIcons))),
                        new CodeInstruction(OpCodes.Br, resumeExecution),
                    })
                .Match(new[] { new CodeInstruction(OpCodes.Stloc_S, helper.Locals[6]) })
                .Move()
                .AddLabels(resumeExecution);
        }
        catch (Exception ex)
        {
            Log.E($"Failed injecting custom forge tooltip.\nHelper returned {ex}");
            return null;
        }

        try
        {
            helper
                .ForEach(
                    new[]
                    {
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(Tool).RequireMethod(nameof(Tool.GetTotalForgeLevels))),
                    },
                    () =>
                    {
                        var callTotalForgeLevels = generator.DefineLabel();
                        var resumeExecution = generator.DefineLabel();
                        helper
                            .Move(-1)
                            .AddLabels(callTotalForgeLevels)
                            .Insert(
                                new[]
                                {
                                    new CodeInstruction(
                                        OpCodes.Call,
                                        typeof(ModEntry).RequirePropertyGetter(nameof(ModEntry.Config))),
                                    new CodeInstruction(
                                        OpCodes.Callvirt,
                                        typeof(ModConfig).RequirePropertyGetter(nameof(ModConfig.Enchantments))),
                                    new CodeInstruction(
                                        OpCodes.Callvirt,
                                        typeof(Config).RequirePropertyGetter(nameof(Config.DrawForgeSockets))),
                                    new CodeInstruction(OpCodes.Brfalse_S, callTotalForgeLevels),
                                    new CodeInstruction(OpCodes.Callvirt, typeof(Tool).RequireMethod(nameof(Tool.GetMaxForges))),
                                    new CodeInstruction(OpCodes.Br_S, resumeExecution),
                                })
                            .Move(2)
                            .AddLabels(resumeExecution);
                    });
        }
        catch (Exception ex)
        {
            Log.E($"Failed always drawing gem sockets.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region injected subroutines

    private static void DrawForgeIcons(Tool tool, int x, ref int y, SpriteBatch b, SpriteFont font)
    {
        var position = new Vector2(x + 20, y + 24);
        if (EnchantmentsModule.Config.SocketPosition == Config.ForgeSocketPosition.AboveSeparator)
        {
            position.Y -= 24;
        }

        var sourceRect = new Rectangle(0, 0, 7, 7);
        for (var i = 0; i < tool.enchantments.Count; i++)
        {
            var enchantment = tool.enchantments[i];
            if (!enchantment.IsForge())
            {
                continue;
            }

            if (!ColorByEnchantmentType.TryGetValue(enchantment.GetType(), out var color))
            {
                continue;
            }

            var level = enchantment.GetLevel();
            for (var j = 0; j < level; j++)
            {
                b.Draw(
                    Textures.ForgeIconTx,
                    position,
                    sourceRect,
                    color,
                    0f,
                    Vector2.Zero,
                    4f,
                    SpriteEffects.None,
                    1f);

                position.X += (7 * 4) + 8;
            }
        }

        var emptySlots = tool.GetMaxForges() - tool.GetTotalForgeLevels();
        if (emptySlots > 0)
        {
            sourceRect = new Rectangle(7, 0, 7, 7);
            for (var i = 0; i < emptySlots; i++)
            {
                b.Draw(
                    Textures.ForgeIconTx,
                    position,
                    sourceRect,
                    Color.White,
                    0f,
                    Vector2.Zero,
                    4f,
                    SpriteEffects.None,
                    1f);
                position.X += (7 * 4) + 8;
            }
        }

        y += (int)font.MeasureString("T").Y;
    }

    #endregion injected subroutines
}
