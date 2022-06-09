using System;
using System.Linq;
using System.Linq.Expressions;

namespace DaLion.Common.Extensions.Reflection;

#region using directives

using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;

#endregion using directives

/// <summary>Extensions for the <see cref="MethodInfo"/> class.</summary>
public static class MethodInfoExtensions
{
    /// <summary>Construct a <see cref="HarmonyMethod" /> instance from a <see cref="MethodInfo" /> object.</summary>
    /// <returns>
    ///     Returns a new <see cref="HarmonyMethod" /> instance if <paramref name="mi" /> is not null, or <c>null</c>
    ///     otherwise.
    /// </returns>
    [CanBeNull]
    public static HarmonyMethod ToHarmonyMethod(this MethodInfo mi)
    {
        return mi is null ? null : new HarmonyMethod(mi);
    }

    public static Delegate CreateDelegate(this MethodInfo mi, Type targetType = null)
    {
        Func<Type[], Type> getType;
        var makeAction = mi.ReturnType == typeof(void);
        var paramTypes = mi.GetParameters().Select(p => p.ParameterType);

        if (makeAction)
        {
            getType = Expression.GetActionType;
        }
        else
        {
            getType = Expression.GetFuncType;
            paramTypes = paramTypes.Concat(new[] { mi.ReturnType });
        }

        if (!mi.IsStatic)
        {
            if (targetType is null)
                throw new InvalidOperationException("Non-static method requires a non-null target type.");

            paramTypes = targetType.Collect().Concat(paramTypes);
        }

        return Delegate.CreateDelegate(getType(paramTypes.ToArray()), mi);
    }
}