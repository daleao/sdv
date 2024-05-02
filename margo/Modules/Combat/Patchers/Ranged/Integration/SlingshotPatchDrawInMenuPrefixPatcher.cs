namespace DaLion.Overhaul.Modules.Combat.Patchers.Ranged.Integration;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Shared.Attributes;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
[ModRequirement("PeacefulEnd.Archery", "Archery", "2.1.0")]
internal sealed class SlingshotPatchDrawInMenuPrefixPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="SlingshotPatchDrawInMenuPrefixPatcher"/> class.</summary>
    internal SlingshotPatchDrawInMenuPrefixPatcher()
    {
        this.Target = "Archery.Framework.Patches.Objects.SlingshotPatch"
            .ToType()
            .RequireMethod("DrawInMenuPrefix");
    }

    #region harmony patches

    /// <summary>Draw current ammo.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? SlingshotPatchDrawInMenuPrefixTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        try
        {
            var ammoModel = generator.DeclareLocal("Archery.Framework.Models.Weapons.AmmoModel".ToType());
            helper
                .Match(new[]
                {
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(Utility).RequireMethod(nameof(Utility.getWidthOfTinyDigitString))),
                })
                .Match(new[] { new CodeInstruction(OpCodes.Brfalse_S) }, ILHelper.SearchOption.Previous)
                .Move()
                .Insert(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Ldarg_0), // the slingshot
                        new CodeInstruction(OpCodes.Ldarg_1), // the sprite batch

                        // load ammo texture and source rect ++
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(
                            OpCodes.Call,
                            "Archery.Framework.Objects.Weapons.Bow".ToType().RequireMethod("GetAmmoItem")),
                        new CodeInstruction(
                            OpCodes.Call,
                            "Archery.Framework.Objects.InstancedObject".ToType().RequireMethod("GetModel")
                                .MakeGenericMethod("Archery.Framework.Models.Weapons.AmmoModel".ToType())),
                        new CodeInstruction(OpCodes.Stloc_S, ammoModel),
                        new CodeInstruction(OpCodes.Ldloc_S, ammoModel),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            "Archery.Framework.Models.BaseModel".ToType().RequirePropertyGetter("Texture")),
                        new CodeInstruction(OpCodes.Ldloc_S, ammoModel),
                        new CodeInstruction(OpCodes.Call, typeof(Game1).RequirePropertyGetter(nameof(Game1.player))),
                        new CodeInstruction(OpCodes.Ldnull),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            "Archery.Framework.Models.BaseModel".ToType().RequireMethod("GetIcon")),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            "Archery.Framework.Models.Display.ItemSpriteModel".ToType()
                                .RequirePropertyGetter("Source")),
                        // load ammo texture and source rect --

                        new CodeInstruction(OpCodes.Ldarg_2), // the location
                        new CodeInstruction(OpCodes.Ldarg_3), // the scale size
                        new CodeInstruction(OpCodes.Ldarg_S, (byte)4), // the transparency
                        new CodeInstruction(OpCodes.Ldarg_S, (byte)5), // the layer depth
                        new CodeInstruction(OpCodes.Ldarg_S, (byte)7), // the color
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(SlingshotPatchDrawInMenuPrefixPatcher).RequireMethod(nameof(DrawAmmo))),
                    });
        }
        catch (Exception ex)
        {
            Log.E($"Failed drawing Bow ammo.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region injected subroutines

    private static void DrawAmmo(
        Slingshot instance,
        SpriteBatch b,
        Texture2D texture,
        Rectangle source,
        Vector2 location,
        float scaleSize,
        float transparency,
        float layerDepth,
        Color color)
    {
        if (!CombatModule.Config.ControlsUi.DrawCurrentAmmo || instance.attachments?[0] is null)
        {
            return;
        }

        b.Draw(
            texture,
            location + new Vector2(44f, 43f),
            source,
            color * transparency,
            0f,
            new Vector2(8f, 8f),
            scaleSize * 2.5f,
            SpriteEffects.None,
            layerDepth);
    }

    #endregion injected subroutines
}
