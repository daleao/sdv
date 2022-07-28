namespace DaLion.Common.Extensions.Reflection;

#region using directives

using FastExpressionCompiler.LightExpression;
using HarmonyLib;
using System;
using System.Linq;
using System.Reflection;

#endregion using directives

/// <summary>Extensions for the <see cref="MethodInfo"/> class.</summary>
public static class MethodInfoExtensions
{
    /// <summary>Construct a <see cref="HarmonyMethod"/> instance from a <see cref="MethodInfo"/> object.</summary>
    /// <returns>A <see cref="HarmonyMethod"/> instance if <paramref name="method"/> is not null, or <see langword="null"/> otherwise.</returns>
    public static HarmonyMethod? ToHarmonyMethod(this MethodInfo? method) =>
        method is null ? null : new HarmonyMethod(method);

    /// <summary>Creates a delegate of the specified type that represents the specified instance method.</summary>
    /// <typeparam name="TDelegate">A delegate type which mirrors the desired method and accepts the target instance type as the first parameter.</typeparam>
    public static TDelegate CompileUnboundDelegate<TDelegate>(this MethodInfo method) where TDelegate : Delegate
    {
        if (method.IsStatic) ThrowHelper.ThrowInvalidOperationException("Method cannot be static.");

        var delegateInfo = typeof(TDelegate).GetMethodInfoFromDelegateType();
        var methodParamTypes = method.GetParameters().Select(m => m.ParameterType).ToArray();
        var delegateParamTypes = delegateInfo.GetParameters().Select(d => d.ParameterType).ToArray();
        if (delegateParamTypes.Length < 1)
            ThrowHelper.ThrowInvalidOperationException(
                "Delegate type must accept at least the target instance parameter.");

        var delegateInstanceType = delegateParamTypes[0];
        delegateParamTypes = delegateParamTypes.Skip(1).ToArray();
        if (delegateParamTypes.Length != methodParamTypes.Length)
            ThrowHelper.ThrowInvalidOperationException(
                "Mismatched method and delegate parameter count.");

        for (var i = 0; i < delegateParamTypes.Length; ++i)
        {
            if (!delegateParamTypes[i].IsAssignableTo(methodParamTypes[i]))
                ThrowHelper.ThrowArgumentException(
                    $"{delegateParamTypes[i].FullName} is not assignable to {methodParamTypes[i].FullName}");
        }

        // convert argument types if necessary
        var args = methodParamTypes.Zip(delegateParamTypes, (methodParamType, delegateParamType) =>
        {
            var delegateParamExp = Expression.Parameter(delegateParamType);
            return new
            {
                DelegateParamExp = delegateParamExp,
                ConvertedParamExp = methodParamType != delegateParamType
                    ? (Expression)Expression.Convert(delegateParamExp, methodParamType)
                    : delegateParamExp
            };
        }).ToArray();

        // convert instance type if necessary
        var delegateTargetExp = Expression.Parameter(delegateInstanceType);
        var convertedTargetExp = delegateInstanceType != method.DeclaringType
            ? (Expression)Expression.Convert(delegateTargetExp, method.DeclaringType!)
            : delegateTargetExp;

        // create method call
        var callExp = Expression.Call(convertedTargetExp, method, args.Select(a => a.ConvertedParamExp));

        // convert return type if necessary
        var convertedCallExp = delegateInfo.ReturnType != method.ReturnType
            ? Expression.Convert(callExp, delegateInfo.ReturnType)
            : (Expression)callExp;

        // collect args and target
        return Expression
            .Lambda<TDelegate>(convertedCallExp, delegateTargetExp.Collect(args.Select(a => a.DelegateParamExp)))
            .CompileFast();
    }

    /// <summary>Creates a delegate of the specified type that represents the specified static method.</summary>
    /// <typeparam name="TDelegate">A delegate type which mirrors the desired method signature.</typeparam>
    public static TDelegate CompileStaticDelegate<TDelegate>(this MethodInfo method) where TDelegate : Delegate
    {
        if (!method.IsStatic) ThrowHelper.ThrowInvalidOperationException("Method must be static.");

        var delegateInfo = typeof(TDelegate).GetMethodInfoFromDelegateType();
        var methodParamTypes = method.GetParameters().Select(m => m.ParameterType).ToArray();
        var delegateParamTypes = delegateInfo.GetParameters().Select(d => d.ParameterType).ToArray();
        if (delegateParamTypes.Length != methodParamTypes.Length)
            ThrowHelper.ThrowInvalidOperationException(
                "Mismatched method and delegate parameter count.");

        for (var i = 0; i < delegateParamTypes.Length; ++i)
        {
            if (!delegateParamTypes[i].IsAssignableTo(methodParamTypes[i]))
                ThrowHelper.ThrowArgumentException(
                    $"{delegateParamTypes[i].FullName} is not assignable to {methodParamTypes[i].FullName}");
        }

        // convert argument types if necessary
        var args = methodParamTypes.Zip(delegateParamTypes, (methodParamType, delegateParamType) =>
        {
            var delegateParamExp = Expression.Parameter(delegateParamType);
            return new
            {
                DelegateParamExp = delegateParamExp,
                ConvertedParamExp = methodParamType != delegateParamType
                    ? (Expression)Expression.Convert(delegateParamExp, methodParamType)
                    : delegateParamExp
            };
        }).ToArray();

        // create method call
        var callExp = Expression.Call(null, method, args.Select(a => a.ConvertedParamExp));

        // convert return type if necessary
        var convertedCallExp = delegateInfo.ReturnType != method.ReturnType
            ? Expression.Convert(callExp, delegateInfo.ReturnType)
            : (Expression)callExp;

        // collect args and target
        return Expression.Lambda<TDelegate>(convertedCallExp, args.Select(a => a.DelegateParamExp)).CompileFast();
    }
}