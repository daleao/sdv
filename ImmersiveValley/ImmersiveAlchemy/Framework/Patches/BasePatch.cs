namespace DaLion.Stardew.Alchemy.Framework.Patches;

#region using directives

using System;
using System.Linq;
using System.Reflection;
using HarmonyLib;

using DaLion.Common.Extensions.Reflection;

#endregion using directives

/// <summary>Base implementation for Harmony patch classes.</summary>
public abstract class BasePatch : IPatch
{
    protected static bool transpilationFailed;

    /// <summary>Construct an instance.</summary>
    protected BasePatch()
    {
        (Prefix, Postfix, Transpiler) = GetHarmonyMethods();

        if (Prefix is not null) ++HarmonyPatcher.TotalPrefixCount;
        if (Postfix is not null) ++HarmonyPatcher.TotalPostfixCount;
        if (Transpiler is not null) ++HarmonyPatcher.TotalTranspilerCount;
        //if (ReversePatch is not null) ++PatchManager.TotalReversePatchCount;
    }

    protected MethodBase Original { get; set; }
    protected Type DeclaringType => Original.DeclaringType;
    protected HarmonyMethod Prefix { get; set; }
    protected HarmonyMethod Postfix { get; set; }
    protected HarmonyMethod Transpiler { get; set; }
    //protected HarmonyMethod ReversePatch { get; set; }

    /// <inheritdoc />
    void IPatch.Apply(Harmony harmony)
    {
        if (Original is null)
        {
            Log.D($"[Patch]: Ignoring {GetType().Name}. The patch target was not found.");

            if (Prefix is not null) ++HarmonyPatcher.IgnoredPrefixCount;
            if (Postfix is not null) ++HarmonyPatcher.IgnoredPostfixCount;
            if (Transpiler is not null) ++HarmonyPatcher.IgnoredTranspilerCount;
            //if (ReversePatch is not null) ++PatchManager.FailedReversePatchCount;

            return;
        }

        try
        {
            Log.D($"[Patch]: Applying {GetType().Name} to {Original.DeclaringType}::{Original.Name}.");
            harmony.Patch(Original, Prefix, Postfix, Transpiler);

            if (Prefix is not null) ++HarmonyPatcher.AppliedPrefixCount;
            if (Postfix is not null) ++HarmonyPatcher.AppliedPostfixCount;
            if (Transpiler is not null)
            {
                if (transpilationFailed)
                {
                    ++HarmonyPatcher.FailedTranspilerCount;
                    transpilationFailed = false;
                }
                else
                {
                    ++HarmonyPatcher.AppliedTranspilerCount;
                }
            }

            //if (ReversePatch is null) return;

            //harmony.CreateReversePatcher(Original, ReversePatch).Patch();
            //++PatchManager.AppliedReversePatchCount;
        }
        catch (Exception ex)
        {
            Log.E($"[Patch]: Failed to patch {Original.DeclaringType}::{Original.Name}.\nHarmony returned {ex}");

            if (Prefix is not null) ++HarmonyPatcher.FailedPrefixCount;
            if (Postfix is not null) ++HarmonyPatcher.FailedPostfixCount;
            if (Transpiler is not null) ++HarmonyPatcher.FailedTranspilerCount;
            //if (ReversePatch is not null) ++PatchManager.FailedReversePatchCount;
        }
    }

    /// <summary>Get a method and assert that it was found.</summary>
    /// <typeparam name="TTarget">The type containing the method.</typeparam>
    /// <param name="parameters">The method parameter types, or <c>null</c> if it's not overloaded.</param>
    /// <remarks>Credit to Pathoschild.</remarks>
    protected ConstructorInfo RequireConstructor<TTarget>(params Type[] parameters)
    {
        return typeof(TTarget).RequireConstructor(parameters);
    }

    /// <summary>Get a method and assert that it was found.</summary>
    /// <typeparam name="TTarget">The type containing the method.</typeparam>
    /// <param name="name">The method name.</param>
    /// <param name="parameters">The method parameter types, or <c>null</c> if it's not overloaded.</param>
    /// <remarks>Credit to Pathoschild.</remarks>
    protected MethodInfo RequireMethod<TTarget>(string name, Type[] parameters = null)
    {
        return typeof(TTarget).RequireMethod(name, parameters);
    }

    /// <summary>Get all Harmony patch methods in the current patch instance.</summary>
    protected (HarmonyMethod, HarmonyMethod, HarmonyMethod) GetHarmonyMethods()
    {
        // get all static and private inner methods of this class
        var methods = GetType().GetMethods(BindingFlags.Static | BindingFlags.NonPublic);

        // identify patch methods by custom Harmony annotations and create Harmony Method instances
        var prefix = methods.FirstOrDefault(m => m.GetCustomAttributes(typeof(HarmonyPrefix), false).Length > 0)
            .ToHarmonyMethod();
        var postfix = methods.FirstOrDefault(m => m.GetCustomAttributes(typeof(HarmonyPostfix), false).Length > 0)
            .ToHarmonyMethod();
        var transpiler = methods
            .FirstOrDefault(m => m.GetCustomAttributes(typeof(HarmonyTranspiler), false).Length > 0)
            .ToHarmonyMethod();

        return (prefix, postfix, transpiler);
    }
}