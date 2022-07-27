namespace DaLion.Common.Extensions.Reflection;

#region using directives

using HarmonyLib;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

#endregion using directives

/// <summary>Extensions for the <see cref="MethodInfo"/> class.</summary>
public static class MethodInfoExtensions
{
    /// <summary>Construct a <see cref="HarmonyMethod" /> instance from a <see cref="MethodInfo" /> object.</summary>
    /// <returns>
    ///     Returns a new <see cref="HarmonyMethod" /> instance if <paramref name="method" /> is not null, or <c>null</c>
    ///     otherwise.
    /// </returns>
    public static HarmonyMethod? ToHarmonyMethod(this MethodInfo? method) =>
        method is null ? null : new HarmonyMethod(method);

    /// <summary>Creates a delegate of the specified type that represents the specified instance method.</summary>
    /// <typeparam name="TDelegate">A delegate type which mirrors the desired method and accepts the target instance type as the first parameter.</typeparam>
    public static TDelegate CompileUnboundDelegate<TDelegate>(this MethodInfo method) where TDelegate : Delegate
    {
        if (method.IsStatic) throw new InvalidOperationException("Method cannot be static.");

        var delegateInfo = typeof(TDelegate).GetMethodInfoFromDelegateType();
        if (!delegateInfo.ReturnType.IsAssignableFrom(method.ReturnType))
            throw new ArgumentException(
                $"{delegateInfo.ReturnType.FullName} is not assignable from {method.ReturnType.FullName}");

        var methodParamTypes = method.GetParameters().Select(m => m.ParameterType).ToArray();
        var delegateParamTypes = delegateInfo.GetParameters().Select(d => d.ParameterType).ToArray();
        if (delegateParamTypes.Length < 1)
            throw new InvalidOperationException(
                "Delegate type must accept at least the target instance parameter.");

        var delegateInstanceType = delegateParamTypes[0];
        if (!delegateInstanceType.IsAssignableTo(method.DeclaringType))
            throw new ArgumentException(
                $"{delegateInstanceType.FullName} is not assignable to {method.DeclaringType?.FullName}");

        delegateParamTypes = delegateParamTypes.Skip(1).ToArray();
        if (delegateParamTypes.Length != methodParamTypes.Length)
            throw new InvalidOperationException(
                "Mismatched method and delegate parameter count.");

        for (var i = 0; i < delegateParamTypes.Length; ++i)
        {
            if (!delegateParamTypes[i].IsAssignableTo(methodParamTypes[i]))
                throw new ArgumentException(
                    $"{delegateParamTypes[i].FullName} is not assignable to {methodParamTypes[i].FullName}");
        }

        // convert argument types if necessary
        var arguments = methodParamTypes.Zip(delegateParamTypes, (methodParamType, delegateParamType) =>
        {
            var delegateArgumentExpression = Expression.Parameter(delegateParamType);
            return new
            {
                DelegateArgumentExpression = delegateArgumentExpression,
                ConvertedArgumentExpression = methodParamType != delegateParamType
                    ? (Expression)Expression.Convert(delegateArgumentExpression, methodParamType)
                    : delegateArgumentExpression
            };
        }).ToArray();

        // convert target type if necessary
        var delegateTargetExpression = Expression.Parameter(delegateInstanceType);
        var convertedTargetExpression = delegateInstanceType != method.DeclaringType
            ? (Expression)Expression.Convert(delegateTargetExpression, method.DeclaringType!)
            : delegateTargetExpression;

        // create method call
        var callExpression = Expression.Call(
            convertedTargetExpression,
            method,
            arguments.Select(a => a.ConvertedArgumentExpression)
        );

        // convert return type if necessary
        var convertedCallExpression = delegateInfo.ReturnType != method.ReturnType
            ? Expression.Convert(callExpression, delegateInfo.ReturnType)
            : (Expression)callExpression;

        // collect arguments and target
        return Expression.Lambda<TDelegate>(
            convertedCallExpression,
            delegateTargetExpression.Collect(arguments.Select(a => a.DelegateArgumentExpression))
        ).Compile();
    }

    /// <summary>Creates a delegate of the specified type that represents the specified static method.</summary>
    /// <typeparam name="TDelegate">A delegate type which mirrors the desired method signature.</typeparam>
    public static TDelegate CompileStaticDelegate<TDelegate>(this MethodInfo method) where TDelegate : Delegate
    {
        if (!method.IsStatic) throw new InvalidOperationException("Method must be static.");

        var delegateInfo = typeof(TDelegate).GetMethodInfoFromDelegateType();
        if (!delegateInfo.ReturnType.IsAssignableFrom(method.ReturnType))
            throw new ArgumentException(
                $"{delegateInfo.ReturnType.FullName} is not assignable from {method.ReturnType.FullName}");

        var methodParamTypes = method.GetParameters().Select(m => m.ParameterType).ToArray();
        var delegateParamTypes = delegateInfo.GetParameters().Select(d => d.ParameterType).ToArray();
        if (delegateParamTypes.Length != methodParamTypes.Length)
            throw new InvalidOperationException(
                "Mismatched method and delegate parameter count.");

        for (var i = 0; i < delegateParamTypes.Length; ++i)
        {
            if (!delegateParamTypes[i].IsAssignableTo(methodParamTypes[i]))
                throw new ArgumentException(
                    $"{delegateParamTypes[i].FullName} is not assignable to {methodParamTypes[i].FullName}");
        }

        // convert argument types if necessary
        var arguments = methodParamTypes.Zip(delegateParamTypes, (methodParamType, delegateParamType) =>
        {
            var delegateArgumentExpression = Expression.Parameter(delegateParamType);
            return new
            {
                DelegateArgumentExpression = delegateArgumentExpression,
                ConvertedArgumentExpression = methodParamType != delegateParamType
                    ? (Expression)Expression.Convert(delegateArgumentExpression, methodParamType)
                    : delegateArgumentExpression
            };
        }).ToArray();

        // create method call
        var methodCall = Expression.Call(
            null,
            method,
            arguments.Select(a => a.ConvertedArgumentExpression)
        );

        // convert return type if necessary
        var convertedMethodCall = delegateInfo.ReturnType != method.ReturnType
            ? Expression.Convert(methodCall, delegateInfo.ReturnType)
            : (Expression)methodCall;

        // collect arguments and target
        return Expression.Lambda<TDelegate>(
            convertedMethodCall,
            arguments.Select(a => a.DelegateArgumentExpression)
        ).Compile();
    }
}