namespace DaLion.Common.Extensions.Reflection;

#region using directives

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

#endregion using directives

/// <summary>Extensions for the <see cref="FieldInfo"/> class.</summary>
public static class FieldInfoExtensions
{
    #region getters

    /// <summary>Creates a delegate of the specified type that represents the specified unbound instance field getter.</summary>
    /// <typeparam name="TDelegate">A delegate type which returns the desired field type and accepts the target instance type as a parameter.</typeparam>
    public static TDelegate CompileUnboundFieldGetterDelegate<TDelegate>(this FieldInfo field) where TDelegate : Delegate
    {
        if (field.IsStatic) throw new InvalidOperationException("Field cannot be static");

        var delegateInfo = typeof(TDelegate).GetMethodInfoFromDelegateType();
        var delegateParamTypes = delegateInfo.GetParameters().Select(d => d.ParameterType).ToArray();
        if (delegateParamTypes.Length != 1)
            throw new InvalidOperationException(
                "Delegate type must accept a single target instance parameter.");

        var delegateTargetType = delegateParamTypes[0];

        // convert target type if necessary
        var delegateTargetExpression = Expression.Parameter(delegateTargetType);
        var convertedTargetExpression = delegateTargetType != field.DeclaringType
            ? (Expression)Expression.Convert(delegateTargetExpression, field.DeclaringType!)
            : delegateTargetExpression;

        // create field call
        var fieldExpression = Expression.Field(
            convertedTargetExpression,
            field
        );

        // convert return type if necessary
        var convertedFieldExpression = delegateInfo.ReturnType != field.FieldType
            ? Expression.Convert(fieldExpression, delegateInfo.ReturnType)
            : (Expression)fieldExpression;

        return Expression.Lambda<TDelegate>(convertedFieldExpression, delegateTargetExpression)
            .Compile();
    }

    /// <summary>Creates a delegate of the specified type that represents the specified static field getter.</summary>
    /// <typeparam name="TDelegate">A delegate type which returns the desired field type and accepts no parameters.</typeparam>
    public static TDelegate CompileStaticFieldGetterDelegate<TDelegate>(this FieldInfo field) where TDelegate : Delegate
    {
        if (!field.IsStatic) throw new InvalidOperationException("Field must be static");

        var delegateInfo = typeof(TDelegate).GetMethodInfoFromDelegateType();

        // create field call
        var fieldExpression = Expression.Field(
            null,
            field
        );

        // convert return type if necessary
        var convertedFieldExpression = delegateInfo.ReturnType != field.FieldType
            ? Expression.Convert(fieldExpression, delegateInfo.ReturnType)
            : (Expression)fieldExpression;

        return Expression.Lambda<TDelegate>(convertedFieldExpression)
            .Compile();
    }

    #endregion getters

    #region setters

    /// <summary>Creates a delegate of the specified type that represents the specified unbound instance field setter.</summary>
    /// <typeparam name="TDelegate">A delegate type which accepts the target instance and assignment value type parameters and returns void.</typeparam>
    public static TDelegate CompileUnboundFieldSetterDelegate<TDelegate>(this FieldInfo field) where TDelegate : Delegate
    {
        if (field.IsStatic) throw new InvalidOperationException("Field cannot be static");

        var delegateInfo = typeof(TDelegate).GetMethodInfoFromDelegateType();
        if (delegateInfo.ReturnType != typeof(void))
            throw new InvalidOperationException("Delegate return type must be void.");

        var delegateParamTypes = delegateInfo.GetParameters().Select(d => d.ParameterType).ToArray();
        if (delegateParamTypes.Length != 2)
            throw new InvalidOperationException(
                "Delegate type must accept both a target instance and assign value parameters.");

        var delegateTargetType = delegateParamTypes[0];
        var delegateValueType = delegateParamTypes[1];

        // convert target type if necessary
        var delegateTargetExpression = Expression.Parameter(delegateTargetType);
        var convertedTargetExpression = delegateTargetType != field.DeclaringType
            ? (Expression) Expression.Convert(delegateTargetExpression, field.DeclaringType!)
            : delegateTargetExpression;

        // convert assign value type if necessary
        var delegateValueExpression = Expression.Parameter(delegateValueType);
        var convertedValueExpression = delegateValueType != field.FieldType
            ? (Expression) Expression.Convert(delegateValueExpression, field.FieldType!)
            : delegateValueExpression;

        // create field call
        var fieldExpression = Expression.Field(
            convertedTargetExpression,
            field
        );

        // create assignment call
        var ssignExpression = Expression.Assign(
            fieldExpression,
            convertedValueExpression
        );

        return Expression.Lambda<TDelegate>(ssignExpression, delegateTargetExpression, delegateValueExpression)
            .Compile();
    }

    /// <summary>Creates a delegate of the specified type that represents the specified static field setter.</summary>
    /// <typeparam name="TDelegate">A delegate type which accepts the target instance type as a parameter and returns void.</typeparam>
    public static TDelegate CompileStaticFieldSetterDelegate<TDelegate>(this FieldInfo field) where TDelegate : Delegate
    {
        if (!field.IsStatic) throw new InvalidOperationException("Field must be static");

        var delegateInfo = typeof(TDelegate).GetMethodInfoFromDelegateType();
        if (delegateInfo.ReturnType != typeof(void))
            throw new InvalidOperationException("Delegate return type must be void.");

        var delegateParamTypes = delegateInfo.GetParameters().Select(d => d.ParameterType).ToArray();
        if (delegateParamTypes.Length != 1)
            throw new InvalidOperationException(
                "Delegate type must accept both a single assign value parameters.");

        var delegateValueType = delegateParamTypes[0];

        // convert assign value type if necessary
        var delegateValueExpression = Expression.Parameter(delegateValueType);
        var convertedValueExpression = delegateValueType != field.FieldType
            ? (Expression)Expression.Convert(delegateValueExpression, field.FieldType!)
            : delegateValueExpression;

        // create field call
        var fieldExpression = Expression.Field(
            null,
            field
        );

        // create assignment call
        var ssignExpression = Expression.Assign(
            fieldExpression,
            convertedValueExpression
        );

        return Expression.Lambda<TDelegate>(ssignExpression, delegateValueExpression)
            .Compile();
    }

    #endregion setters
}