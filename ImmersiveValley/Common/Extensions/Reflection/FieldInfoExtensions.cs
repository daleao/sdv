namespace DaLion.Common.Extensions.Reflection;

#region using directives

using FastExpressionCompiler.LightExpression;
using System;
using System.Reflection;

#endregion using directives

/// <summary>Extensions for the <see cref="FieldInfo"/> class.</summary>
public static class FieldInfoExtensions
{
    #region getters

    /// <summary>Creates a delegate of the specified type that represents the specified unbound instance field getter.</summary>
    /// <typeparam name="TInstance">The type of the instance that will be received by the delegate.</typeparam>
    /// <typeparam name="TField">The type that will be returned by the delegate.</typeparam>
    public static Func<TInstance, TField> CompileUnboundFieldGetterDelegate<TInstance, TField>(this FieldInfo field)
    {
        if (field.IsStatic) ThrowHelper.ThrowInvalidOperationException("Field cannot be static.");

        var instanceType = typeof(TInstance);
        var returnType = typeof(TField);

        // convert instance type if necessary
        var instanceExp = Expression.Parameter(instanceType);
        var convertedInstanceExp = instanceType != field.DeclaringType
            ? (Expression)Expression.Convert(instanceExp, field.DeclaringType!)
            : instanceExp;

        // create field call
        var fieldExp = Expression.Field(convertedInstanceExp, field);

        // convert return type if necessary
        var convertedFieldExp = returnType != field.FieldType
            ? Expression.Convert(fieldExp, returnType)
            : (Expression)fieldExp;

        return Expression.Lambda<Func<TInstance, TField>>(convertedFieldExp, instanceExp).CompileFast();
    }

    /// <summary>Creates a delegate of the specified type that represents the specified static field getter.</summary>
    /// <typeparam name="TField">The type that will be returned by the delegate.</typeparam>
    public static Func<TField> CompileStaticFieldGetterDelegate<TField>(this FieldInfo field)
    {
        if (!field.IsStatic) ThrowHelper.ThrowInvalidOperationException("Field must be static");

        var returnType = typeof(TField);

        // create field call
        var fieldExp = Expression.Field(null, field);

        // convert return type if necessary
        var convertedFieldExp = returnType != field.FieldType
            ? Expression.Convert(fieldExp, returnType)
            : (Expression)fieldExp;

        return Expression.Lambda<Func<TField>>(convertedFieldExp).CompileFast();
    }

    #endregion getters

    #region setters

    /// <summary>Creates a delegate of the specified type that represents the specified unbound instance field setter.</summary>
    /// <typeparam name="TInstance">The type of the instance that will be received by the delegate.</typeparam>
    /// <typeparam name="TField">The type that will be received by the field.</typeparam>
    public static Action<TInstance, TField> CompileUnboundFieldSetterDelegate<TInstance, TField>(this FieldInfo field)
    {
        if (field.IsStatic) ThrowHelper.ThrowInvalidOperationException("Field cannot be static.");

        var instanceType = typeof(TInstance);
        var valueType = typeof(TField);

        // convert instance type if necessary
        var instanceExp = Expression.Parameter(instanceType);
        var convertedInstanceExp = instanceType != field.DeclaringType
            ? (Expression)Expression.Convert(instanceExp, field.DeclaringType!)
            : instanceExp;

        // convert assign value type if necessary
        var valueExp = Expression.Parameter(valueType);
        var convertedValueExp = valueType != field.FieldType
            ? (Expression)Expression.Convert(valueExp, field.FieldType)
            : valueExp;

        // create field call
        var fieldExp = Expression.Field(convertedInstanceExp, field);

        // create assignment call
        var assignExp = Expression.Assign(fieldExp, convertedValueExp);

        return Expression
            .Lambda<Action<TInstance, TField>>(assignExp, instanceExp, valueExp)
            .CompileFast();
    }

    /// <summary>Creates a delegate of the specified type that represents the specified static field setter.</summary>
    /// <typeparam name="TField">The type that will be received by the field.</typeparam>
    public static Action<TField> CompileStaticFieldSetterDelegate<TField>(this FieldInfo field)
    {
        if (!field.IsStatic) ThrowHelper.ThrowInvalidOperationException("Field must be static");

        var valueType = typeof(TField);

        // convert assign value type if necessary
        var valueExp = Expression.Parameter(valueType);
        var convertedValueExp = valueType != field.FieldType
            ? (Expression)Expression.Convert(valueExp, field.FieldType)
            : valueExp;

        // create field call
        var fieldExp = Expression.Field(null, field);

        // create assignment call
        var assignExp = Expression.Assign(fieldExp, convertedValueExp);

        return Expression.Lambda<Action<TField>>(assignExp, valueExp).CompileFast();
    }

    #endregion setters
}