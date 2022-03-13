namespace DaLion.Stardew.Professions.Framework.Patches;

#region using directives

using System;
using System.Linq;
using System.Reflection;
using HarmonyLib;

using Stardew.Common.Extensions;
using Stardew.Common.Harmony;

#endregion using directives

/// <summary>Base implementation for Harmony patch classes.</summary>
internal abstract class BasePatch : IPatch
{
    protected static bool transpilationFailed;

    /// <summary>Construct an instance.</summary>
    protected BasePatch()
    {
        (Prefix, Postfix, Transpiler) = GetHarmonyMethods();

        if (Prefix is not null) ++PatchManager.TotalPrefixCount;
        if (Postfix is not null) ++PatchManager.TotalPostfixCount;
        if (Transpiler is not null) ++PatchManager.TotalTranspilerCount;
        //if (ReversePatch is not null) ++PatchManager.TotalReversePatchCount;
    }

    protected MethodBase Original { get; set; }
    protected HarmonyMethod Prefix { get; set; }
    protected HarmonyMethod Postfix { get; set; }
    protected HarmonyMethod Transpiler { get; set; }
    //protected HarmonyMethod ReversePatch { get; set; }

    /// <inheritdoc />
    public virtual void Apply(Harmony harmony)
    {
        if (Original is null)
        {
            Log.D($"[Patch]: Ignoring {GetType().Name}. The patch target was not found.");

            if (Prefix is not null) ++PatchManager.IgnoredPrefixCount;
            if (Postfix is not null) ++PatchManager.IgnoredPostfixCount;
            if (Transpiler is not null) ++PatchManager.IgnoredTranspilerCount;
            //if (ReversePatch is not null) ++PatchManager.FailedReversePatchCount;

            return;
        }

        try
        {
            Log.D($"[Patch]: Applying {GetType().Name} to {Original.DeclaringType}::{Original.Name}.");
            harmony.Patch(Original, Prefix, Postfix, Transpiler);

            if (Prefix is not null) ++PatchManager.AppliedPrefixCount;
            if (Postfix is not null) ++PatchManager.AppliedPostfixCount;
            if (Transpiler is not null)
            {
                if (transpilationFailed)
                {
                    ++PatchManager.FailedTranspilerCount;
                    transpilationFailed = false;
                }
                else
                {
                    ++PatchManager.AppliedTranspilerCount;
                }
            }

            //if (ReversePatch is null) return;

            //harmony.CreateReversePatcher(Original, ReversePatch).Patch();
            //++PatchManager.AppliedReversePatchCount;
        }
        catch (Exception ex)
        {
            Log.E($"[Patch]: Failed to patch {Original.DeclaringType}::{Original.Name}.\nHarmony returned {ex}");

            if (Prefix is not null) ++PatchManager.FailedPrefixCount;
            if (Postfix is not null) ++PatchManager.FailedPostfixCount;
            if (Transpiler is not null) ++PatchManager.FailedTranspilerCount;
            //if (ReversePatch is not null) ++PatchManager.FailedReversePatchCount;
        }
    }

    /// <summary>Get a method and assert that it was found.</summary>
    /// <typeparam name="TTarget">The type containing the method.</typeparam>
    /// <param name="parameters">The method parameter types, or <c>null</c> if it's not overloaded.</param>
    /// <remarks>Credit to Pathoschild.</remarks>
    protected ConstructorInfo RequireConstructor<TTarget>(params Type[] parameters)
    {
        return typeof(TTarget).Constructor(parameters);
    }

    /// <summary>Get a method and assert that it was found.</summary>
    /// <typeparam name="TTarget">The type containing the method.</typeparam>
    /// <param name="name">The method name.</param>
    /// <param name="parameters">The method parameter types, or <c>null</c> if it's not overloaded.</param>
    /// <remarks>Credit to Pathoschild.</remarks>
    protected MethodInfo RequireMethod<TTarget>(string name, Type[] parameters = null)
    {
        return typeof(TTarget).MethodNamed(name, parameters);
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