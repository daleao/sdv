namespace DaLion.Shared.Extensions.Reflection;

#region using directives

using System.Reflection;
using FastExpressionCompiler.LightExpression;

#endregion using directives

/// <summary>Extensions for the <see cref="FieldInfo"/> class.</summary>
public static class FieldInfoExtensions
{
    /// <summary>The full name of the <paramref name="field"/>, including the declaring type.</summary>
    /// <param name="field">The <see cref="FieldInfo"/>.</param>
    /// <returns>A <see cref="string"/> representation of the <paramref name="field"/>'s qualified full name.</returns>
    public static string GetFullName(this FieldInfo field)
    {
        return $"{field.DeclaringType}::{field.Name}";
    }

    #region getters

    /// <summary>
    ///     Creates a <see cref="Func{TInstance, TField}"/> delegate for the specified unbound instance
    ///     <paramref name="field"/> getter, performing type conversions if necessary.
    /// </summary>
    /// <typeparam name="TInstance">The type of the instance that will be received by the delegate, which should match a type assignable to the declaring type of the <paramref name="field"/>.</typeparam>
    /// <typeparam name="TField">The type that will be returned by the delegate, which should match a type assignable to the <paramref name="field"/>.</typeparam>
    /// <param name="field">The <see cref="FieldInfo"/>.</param>
    /// <returns>A <see cref="Func{TInstance, TField}"/> delegate which takes in an instance of type <typeparamref name="TInstance"/> and returns the <typeparamref name="TField"/> value of the corresponding <paramref name="field"/>.</returns>
    /// <exception cref="InvalidOperationException">If the <paramref name="field"/> is static.</exception>
    /// <exception cref="InvalidOperationException">If <typeparamref name="TInstance"/> does not match the declaring type of the <paramref name="field"/>.</exception>
    /// <exception cref="InvalidOperationException">If <typeparamref name="TField"/> does not match the type of the <paramref name="field"/>.</exception>
    public static Func<TInstance, TField> CompileUnboundFieldGetterDelegate<TInstance, TField>(this FieldInfo field)
    {
        if (field.IsStatic)
        {
            ThrowHelper.ThrowInvalidOperationException("Field cannot be static.");
        }

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

    /// <summary>Creates a <see cref="Func{TField}"/> delegate for the specified static <paramref name="field"/> getter, performing type conversions if necessary.</summary>
    /// <typeparam name="TField">The type that will be returned by the delegate, which should match a type assignable to the <paramref name="field"/>.</typeparam>
    /// <param name="field">The <see cref="FieldInfo"/>.</param>
    /// <returns>A <see cref="Func{TField}"/> delegate which returns the <typeparamref name="TField"/> value of the static <paramref name="field"/>.</returns>
    /// <exception cref="InvalidOperationException">If the <paramref name="field"/> is static.</exception>
    public static Func<TField> CompileStaticFieldGetterDelegate<TField>(this FieldInfo field)
    {
        if (!field.IsStatic)
        {
            ThrowHelper.ThrowInvalidOperationException("Field must be static");
        }

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

    /// <summary>
    ///     Creates an <see cref="Action{TInstance, TField}"/> delegate for the specified unbound instance
    ///     <paramref name="field"/> setter, performing type conversions if necessary.
    /// </summary>
    /// <typeparam name="TInstance">The type of the instance that will be received by the delegate, which should be a type assignable to the declaring type of the <paramref name="field"/>.</typeparam>
    /// <typeparam name="TField">The type that will be received by the <paramref name="field"/>, which should match a type assignable to the <paramref name="field"/>.</typeparam>
    /// <param name="field">The <see cref="FieldInfo"/>.</param>
    /// <returns>A <see cref="Action{TInstance, TField}"/> delegate which takes in an instance of type <typeparamref name="TInstance"/> and a value of type <typeparamref name="TField"/>, and assigns the latter to the corresponding <paramref name="field"/> in the former.</returns>
    /// <exception cref="InvalidOperationException">If the <paramref name="field"/> is static.</exception>
    public static Action<TInstance, TField> CompileUnboundFieldSetterDelegate<TInstance, TField>(this FieldInfo field)
    {
        if (field.IsStatic)
        {
            ThrowHelper.ThrowInvalidOperationException("Field cannot be static.");
        }

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

    /// <summary>
    ///     Creates an <see cref="Action{TInstance}"/> delegate for the specified static <paramref name="field"/>
    ///     setter, performing type conversions if necessary.
    /// </summary>
    /// <typeparam name="TField">The type that will be received by the field, which should match a type assignable to the <paramref name="field"/>.</typeparam>
    /// <param name="field">The <see cref="FieldInfo"/>.</param>
    /// <returns>A <see cref="Action{TField}"/> delegate which takes in value of type <typeparamref name="TField"/> and assigns it to the static <paramref name="field"/>.</returns>
    /// <exception cref="InvalidOperationException">If the <paramref name="field"/> is static.</exception>
    public static Action<TField> CompileStaticFieldSetterDelegate<TField>(this FieldInfo field)
    {
        if (!field.IsStatic)
        {
            ThrowHelper.ThrowInvalidOperationException("Field must be static");
        }

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
