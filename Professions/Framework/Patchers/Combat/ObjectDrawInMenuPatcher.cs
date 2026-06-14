namespace DaLion.Professions.Framework.Patchers.Combat;

#region using directives

using System.Reflection;
using System.Reflection.Emit;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Menus;

#endregion using directives

[UsedImplicitly]
internal sealed class ObjectDrawInMenuPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="ObjectDrawInMenuPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal ObjectDrawInMenuPatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireMethod<SObject>(
            nameof(SObject.drawInMenu),
            [
                typeof(SpriteBatch), typeof(Vector2), typeof(float), typeof(float), typeof(float),
                typeof(StackDrawType), typeof(Color), typeof(bool),
            ]);
    }

    #region harmony patches

    /// <summary>Draw Prismatic Brush + Slime Flute cooldown.</summary>
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void ObjectDrawInMenuPostfix(
        SObject __instance,
        SpriteBatch spriteBatch,
        Vector2 location,
        float scaleSize,
        float transparency,
        float layerDepth,
        StackDrawType drawStackNumber,
        bool drawShadow)
    {
        if (__instance.ItemId == PrismaticBrushId)
        {
            var itemData = ItemRegistry.GetDataOrErrorItem(__instance.QualifiedItemId);
            var offset = 1;
            var sourceRect = itemData.GetSourceRect(offset, __instance.ParentSheetIndex);
            var color = Utility.GetPrismaticColor(speedMultiplier: 2f);
            spriteBatch.Draw(
                itemData.GetTexture(),
                location + new Vector2(32f, 32f),
                sourceRect,
                color * transparency,
                0f,
                new Vector2(sourceRect.Width / 2, sourceRect.Height / 2),
                4f * scaleSize,
                SpriteEffects.None,
                layerDepth);
        }
        else if (__instance.ItemId == SlimeFluteId && State.SlimeFluteCooldown > 0)
        {
            var drawingAsDebris = drawShadow && drawStackNumber == StackDrawType.Hide;
            if (drawShadow && !drawingAsDebris && (Game1.activeClickableMenu is not ShopMenu || scaleSize != 1f))
            {
                var coolDownLevel = State.SlimeFluteCooldown / 60f;
                spriteBatch.Draw(Game1.staminaRect, new Rectangle((int)location.X, (int)location.Y + (64 - (int)(coolDownLevel * 64f)), 64, (int)(coolDownLevel * 64f)), Color.Red * 0.66f);
            }
        }
    }

    [HarmonyTranspiler]
    [UsedImplicitly]
    private static IEnumerable<CodeInstruction>? GameLocationDamageMonsterTranspiler(IEnumerable<CodeInstruction> instructions, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        // Injected: + State.SlimeFluteAddedScale;
        // After: float drawnScale = scaleSize;
        try
        {
            helper
                .PatternMatch([new CodeInstruction(OpCodes.Stloc_3)])
                .Insert(
                [
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(ProfessionsMod).RequirePropertyGetter(nameof(State))),
                    new CodeInstruction(
                        OpCodes.Callvirt,
                        typeof(ProfessionsState).RequirePropertyGetter(nameof(State.SlimeFluteAddedScale))),
                    new CodeInstruction(OpCodes.Add),
                ]);
        }
        catch (Exception ex)
        {
            Log.E($"Failed adding Slime Flute scale.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}
