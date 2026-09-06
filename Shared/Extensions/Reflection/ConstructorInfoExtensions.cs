namespace DaLion.Shared.Extensions.Reflection;

#region using directives

using System.Linq;
using System.Reflection;
using FastExpressionCompiler.LightExpression;

#endregion using directives

/// <summary>Extensions for the <see cref="MethodInfo"/> class.</summary>
public static class ConstructorInfoExtensions
{
    /// <summary>Creates a delegate of type <typeparamref name="TDelegate"/> for the given instance <paramref name="constructor"/>.</summary>
    /// <typeparam name="TDelegate">A delegate type which mirrors the desired <paramref name="constructor"/> signature.</typeparam>
    /// <param name="constructor">The <see cref="ConstructorInfo"/>.</param>
    /// <returns>A delegate of type <typeparamref name="TDelegate"/> which invokes the corresponding <paramref name="constructor"/>.</returns>
    public static TDelegate CompileConstructorDelegate<TDelegate>(this ConstructorInfo constructor)
        where TDelegate : Delegate
    {
        var delegateInfo = typeof(TDelegate).GetMethodInfoFromDelegateType();
        var constructorParamTypes = constructor
            .GetParameters()
            .Select(m => m.ParameterType)
            .ToArray();
        var delegateParamTypes = delegateInfo
            .GetParameters()
            .Select(d => d.ParameterType)
            .ToArray();
        if (delegateParamTypes.Length != constructorParamTypes.Length)
        {
            ThrowHelper.ThrowInvalidOperationException(
                "Mismatched constructor and delegate parameter count.");
        }

        // convert argument types if necessary
        var args = constructorParamTypes.Zip(delegateParamTypes, (constructorParamType, delegateParamType) =>
        {
            var delegateParamExp = Expression.Parameter(delegateParamType);
            return new
            {
                DelegateParamExp = delegateParamExp,
                ConvertedParamExp = constructorParamType != delegateParamType
                    ? (Expression)Expression.Convert(delegateParamExp, constructorParamType)
                    : delegateParamExp,
            };
        }).ToArray();

        // create constructor call
        var newExp = Expression.New(constructor, args.Select(a => a.ConvertedParamExp));

        // convert return type if necessary
        var convertedNewExp = delegateInfo.ReturnType != constructor.DeclaringType
            ? Expression.Convert(newExp, delegateInfo.ReturnType)
            : (Expression)newExp;

        // collect args and target
        return Expression
            .Lambda<TDelegate>(
                convertedNewExp,
                args.Select(a => a.DelegateParamExp))
            .CompileFast();
    }
}
