using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley.Characters;

namespace DaLion.HorseHeadFix
{
    public class ModEntry : Mod
    {
        public override void Entry(IModHelper helper)
        {
            new Harmony(ModManifest.UniqueID).Patch(
                AccessTools.Method(typeof(Horse), nameof(Horse.draw), new[] { typeof(SpriteBatch) }), null, null,
                new HarmonyMethod(AccessTools.Method(GetType(), nameof(HorseDrawTranspiler))));
        }

        private static IEnumerable<CodeInstruction> HorseDrawTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            var j = instructions.ToList();
            for (var i = 0; i < j.Count; i++)
            {
                if (j[i].opcode == OpCodes.Ldc_R4 && j[i].operand is 48f)
                {
                    j[i++].operand = 4f;
                    j[i].operand = -52f;
                    continue;
                }

                if (j[i].opcode == OpCodes.Ldc_I4_S && j[i].operand is byte and 9)
                {
                    j[i++].operand = (sbyte)16;
                    j[i].operand = (sbyte)18;
                    break;
                }
            }

            return j.AsEnumerable();
        }
    }
}