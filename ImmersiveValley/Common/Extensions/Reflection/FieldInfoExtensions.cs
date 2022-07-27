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
    /// <typeparam name="TInstance">The type of the instance that will be received by the delegate.</typeparam>
    /// <typeparam name="TField">The type that will be returned by the delegate.</typeparam>
    public static Func<TInstance, TField> CompileUnboundFieldGetterDelegate<TInstance, TField>(this FieldInfo field)
    {
        if (field.IsStatic) throw new InvalidOperationException("Field cannot be static.");

        var delegateInstanceType = typeof(TInstance);
        if (!delegateInstanceType.IsAssignableTo(field.DeclaringType))
            throw new ArgumentException(
                $"{delegateInstanceType.FullName} is not assignable to {field.DeclaringType?.FullName}");

        var delegateReturnType = typeof(TField);
        if (!delegateReturnType.IsAssignableFrom(field.FieldType))
            throw new ArgumentException(
                $"{delegateReturnType.FullName} is not assignable from {field.FieldType.FullName}");

        // convert target type if necessary
        var delegateTargetExpression = Expression.Parameter(delegateInstanceType);
        var convertedTargetExpression = delegateInstanceType != field.DeclaringType
            ? (Expression)Expression.Convert(delegateTargetExpression, field.DeclaringType)
            : delegateTargetExpression;

        // create field call
        var fieldExpression = Expression.Field(
            convertedTargetExpression,
            field
        );

        // convert return type if necessary
        var convertedFieldExpression = delegateReturnType != field.FieldType
            ? Expression.Convert(fieldExpression, delegateReturnType)
            : (Expression)fieldExpression;

        return Expression.Lambda<Func<TInstance, TField>>(convertedFieldExpression, delegateTargetExpression).Compile();
    }

    /// <summary>Creates a delegate of the specified type that represents the specified static field getter.</summary>
    /// <typeparam name="TField">The type that will be returned by the delegate.</typeparam>
    public static Func<TField> CompileStaticFieldGetterDelegate<TField>(this FieldInfo field)
    {
        if (!field.IsStatic) throw new InvalidOperationException("Field must be static");

        var delegateReturnType = typeof(TField);
        if (!delegateReturnType.IsAssignableFrom(field.FieldType))
            throw new ArgumentException(
                $"{delegateReturnType.FullName} is not assignable from {field.FieldType.FullName}");

        // create field call
        var fieldExpression = Expression.Field(
            null,
            field
        );

        // convert return type if necessary
        var convertedFieldExpression = delegateReturnType != field.FieldType
            ? Expression.Convert(fieldExpression, delegateReturnType)
            : (Expression)fieldExpression;

        return Expression.Lambda<Func<TField>>(convertedFieldExpression).Compile();
    }

    #endregion getters

    #region setters

    /// <summary>Creates a delegate of the specified type that represents the specified unbound instance field setter.</summary>
    /// <typeparam name="TInstance">The type of the instance that will be received by the delegate.</typeparam>
    /// <typeparam name="TField">The type that will be received by the field.</typeparam>
    public static Action<TInstance, TField> CompileUnboundFieldSetterDelegate<TInstance, TField>(this FieldInfo field)
    {
        if (field.IsStatic) throw new InvalidOperationException("Field cannot be static.");

        var delegateInstanceType = typeof(TInstance);
        if (!delegateInstanceType.IsAssignableTo(field.DeclaringType))
            throw new ArgumentException(
                $"{delegateInstanceType.FullName} is not assignable to {field.DeclaringType?.FullName}");

        var delegateValueType = typeof(TField);
        if (!delegateValueType.IsAssignableTo(field.FieldType))
            throw new ArgumentException(
                $"{delegateValueType.FullName} is not assignable to {field.FieldType.FullName}");

        // convert target type if necessary
        var delegateTargetExpression = Expression.Parameter(delegateInstanceType);
        var convertedTargetExpression = delegateInstanceType != field.DeclaringType
            ? (Expression)Expression.Convert(delegateTargetExpression, field.DeclaringType)
            : delegateTargetExpression;

        // convert assign value type if necessary
        var delegateValueExpression = Expression.Parameter(delegateValueType);
        var convertedValueExpression = delegateValueType != field.FieldType
            ? (Expression)Expression.Convert(delegateValueExpression, field.FieldType)
            : delegateValueExpression;

        // create field call
        var fieldExpression = Expression.Field(
            convertedTargetExpression,
            field
        );

        // create assignment call
        var assignExpression = Expression.Assign(
            fieldExpression,
            convertedValueExpression
        );

        return Expression
            .Lambda<Action<TInstance, TField>>(assignExpression, delegateTargetExpression, delegateValueExpression)
            .Compile();
    }

    /// <summary>Creates a delegate of the specified type that represents the specified static field setter.</summary>
    /// <typeparam name="TField">The type that will be received by the field.</typeparam>
    public static Action<TField> CompileStaticFieldSetterDelegate<TField>(this FieldInfo field)
    {
        if (!field.IsStatic) throw new InvalidOperationException("Field must be static");

        var delegateValueType = typeof(TField);
        if (!delegateValueType.IsAssignableTo(field.FieldType))
            throw new ArgumentException(
                $"{delegateValueType.FullName} is not assignable to {field.FieldType.FullName}");

        // convert assign value type if necessary
        var delegateValueExpression = Expression.Parameter(delegateValueType);
        var convertedValueExpression = delegateValueType != field.FieldType
            ? (Expression)Expression.Convert(delegateValueExpression, field.FieldType)
            : delegateValueExpression;

        // create field call
        var fieldExpression = Expression.Field(
            null,
            field
        );

        // create assignment call
        var assignExpression = Expression.Assign(
            fieldExpression,
            convertedValueExpression
        );

        return Expression.Lambda<Action<TField>>(assignExpression, delegateValueExpression).Compile();
    }

    #endregion setters
}