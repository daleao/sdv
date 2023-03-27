namespace DaLion.Overhaul.Modules.Professions.Patchers.Prestige.Integration;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using SpaceCore.Interface;

#endregion using directives

[UsedImplicitly]
internal sealed class SkillLevelUpMenuCtorPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="SkillLevelUpMenuCtorPatcher"/> class.</summary>
    internal SkillLevelUpMenuCtorPatcher()
    {
        this.Target = this.RequireConstructor<SkillLevelUpMenu>(typeof(string), typeof(int));
    }

    #region harmony patches

    /// <summary>Patch to prevent duplicate profession acquisition + display end of level up dialogues.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? LevelUpMenuCtorTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        // This injection chooses the correct 2nd-tier profession choices based on the last selected level 5 profession.
        // From: profPair = null; foreach ( ... )
        // To: profPair = ChooseProfessionPair(skill);
        try
        {
            helper
                .Match(
                    new[]
                    {
                        // find index of initializing profPair to null
                        new CodeInstruction(OpCodes.Ldnull),
                        new CodeInstruction(OpCodes.Stfld, typeof(SkillLevelUpMenu).RequireField("profPair")),
                    })
                .ReplaceWith(
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(SkillLevelUpMenuUpdatePatcher).RequireMethod(nameof(SkillLevelUpMenuUpdatePatcher
                            .ChooseProfessionPair))))
                .Insert(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(
                            OpCodes.Ldfld,
                            typeof(SkillLevelUpMenu).RequireField("currentSkill")),
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(
                            OpCodes.Ldfld,
                            typeof(SkillLevelUpMenu).RequireField("currentLevel")),
                    })
                .Move(2)
                .Count(new[] { new CodeInstruction(OpCodes.Endfinally) }, out var count)
                .Remove(count); // remove the entire loop
        }
        catch (Exception ex)
        {
            Log.E(
                "Professions module failed patching 2nd-tier profession choices to reflect last chosen 1st-tier profession." +
                "\n—-- Do NOT report this to SpaceCore's author. ---" +
                $"\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}
