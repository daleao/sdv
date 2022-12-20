namespace DaLion.Shared.Exceptions;

#region using directives

using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;

#endregion using directives

/// <summary>Throws <see cref="Exception"/>s not available in <see cref="ThrowHelper"/>.</summary>
public static class ThrowHelperExtensions
{
    #region standard exceptions

    /// <summary>
    ///     Throws a new <see cref="FileLoadException"/>.
    /// </summary>
    /// <param name="message">The message to include in the exception.</param>
    /// <param name="innerException">The inner <see cref="Exception"/> to include.</param>
    /// <exception cref="FileLoadException">Thrown with the specified parameters.</exception>
    [DoesNotReturn]
    public static void ThrowFileLoadException(string? message = null, Exception? innerException = null)
    {
        throw new FileLoadException(message, innerException);
    }

    /// <summary>
    ///     Throws a new <see cref="IndexOutOfRangeException"/>.
    /// </summary>
    /// <param name="message">The message to include in the exception.</param>
    /// <param name="innerException">The inner <see cref="Exception"/> to include.</param>
    /// <exception cref="IndexOutOfRangeException">Thrown with the specified parameters.</exception>
    [DoesNotReturn]
    public static void ThrowIndexOutOfRangeException(string? message = null, Exception? innerException = null)
    {
        throw new IndexOutOfRangeException(message, innerException);
    }

    /// <summary>
    ///     Throws a new <see cref="MissingMethodException"/>.
    /// </summary>
    /// <param name="message">The message to include in the exception.</param>
    /// <param name="innerException">The inner <see cref="Exception"/> to include.</param>
    /// <exception cref="MissingMethodException">Thrown with the specified parameters.</exception>
    [DoesNotReturn]
    public static void ThrowMissingMethodException(string? message = null, Exception? innerException = null)
    {
        throw new MissingMethodException(message, innerException);
    }

    /// <summary>
    ///     Throws a new <see cref="NotImplementedException"/>.
    /// </summary>
    /// <param name="message">The message to include in the exception.</param>
    /// <param name="innerException">The inner <see cref="Exception"/> to include.</param>
    /// <exception cref="NotImplementedException">Thrown with the specified parameters.</exception>
    [DoesNotReturn]
    public static void ThrowNotImplementedException(string? message = null, Exception? innerException = null)
    {
        throw new NotImplementedException(message, innerException);
    }

    /// <summary>
    ///     Throws a new <see cref="NotImplementedException"/>.
    /// </summary>
    /// <param name="fullTypeName">The name of the type which failed to initialize.</param>
    /// <param name="innerException">The inner <see cref="Exception"/> to include.</param>
    /// <exception cref="NotImplementedException">Thrown with the specified parameters.</exception>
    [DoesNotReturn]
    public static void ThrowTypeInitializationException(string? fullTypeName = null, Exception? innerException = null)
    {
        throw new TypeInitializationException(fullTypeName, innerException);
    }

    /// <summary>
    ///     Throws a new <see cref="NotImplementedException"/>.
    /// </summary>
    /// <typeparam name="T">The type which failed to initialize.</typeparam>
    /// <param name="innerException">The inner <see cref="Exception"/> to include.</param>
    /// <returns>Nothing.</returns>
    /// <exception cref="NotImplementedException">Thrown with the specified parameters.</exception>
    [DoesNotReturn]
    public static T ThrowTypeInitializationException<T>(Exception? innerException = null)
    {
        throw new TypeInitializationException(nameof(T), innerException);
    }

    #endregion standard exceptions

    #region custom exceptions

    /// <summary>
    ///     Throws a new <see cref="UnexpectedEnumValueException{T}"/>.
    /// </summary>
    /// <typeparam name="TEnum">The enum type that received an unexpected value.</typeparam>
    /// <param name="value">The unexpected enum value.</param>
    /// <exception cref="NotImplementedException">Thrown with the specified parameters.</exception>
    [DoesNotReturn]
    public static void ThrowUnexpectedEnumValueException<TEnum>(int value)
    {
        throw new UnexpectedEnumValueException<TEnum>(value);
    }

    /// <summary>
    ///     Throws a new <see cref="UnexpectedEnumValueException{T}"/>.
    /// </summary>
    /// <typeparam name="TEnum">The enum type that received an unexpected value.</typeparam>
    /// <param name="value">The unexpected enum value.</param>
    /// <exception cref="NotImplementedException">Thrown with the specified parameters.</exception>
    [DoesNotReturn]
    public static void ThrowUnexpectedEnumValueException<TEnum>(TEnum value)
    {
        throw new UnexpectedEnumValueException<TEnum>(value);
    }

    /// <summary>
    ///     Throws a new <see cref="UnexpectedEnumValueException{T}"/>.
    /// </summary>
    /// <typeparam name="TEnum">The enum type that received an unexpected value.</typeparam>
    /// <typeparam name="TReturn">The return type expected by the method where the exception is thrown.</typeparam>
    /// <param name="value">The unexpected enum value.</param>
    /// <returns>Nothing.</returns>
    /// <exception cref="NotImplementedException">Thrown with the specified parameters.</exception>
    [DoesNotReturn]
    public static TReturn ThrowUnexpectedEnumValueException<TEnum, TReturn>(int value)
    {
        throw new UnexpectedEnumValueException<TEnum>(value);
    }

    /// <summary>
    ///     Throws a new <see cref="UnexpectedEnumValueException{T}"/>.
    /// </summary>
    /// <typeparam name="TEnum">The enum type that received an unexpected value.</typeparam>
    /// <typeparam name="TReturn">The return type expected by the method where the exception is thrown.</typeparam>
    /// <param name="value">The unexpected enum value.</param>
    /// <returns>Nothing.</returns>
    /// <exception cref="NotImplementedException">Thrown with the specified parameters.</exception>
    [DoesNotReturn]
    public static TReturn ThrowUnexpectedEnumValueException<TEnum, TReturn>(TEnum value)
    {
        throw new UnexpectedEnumValueException<TEnum>(value);
    }

    /// <summary>
    ///     Throws a new <see cref="PatternNotFoundException"/>.
    /// </summary>
    /// <param name="pattern">A sequence of <see cref="CodeInstruction"/> that could not be found.</param>
    /// <param name="target">The target method where the pattern was searched for.</param>
    /// <exception cref="PatternNotFoundException">Thrown with the specified parameters.</exception>
    [DoesNotReturn]
    public static void ThrowPatternNotFoundException(CodeInstruction[] pattern, MethodBase target)
    {
        throw new PatternNotFoundException(pattern, target);
    }

    /// <summary>
    ///     Throws a new <see cref="PatternNotFoundException"/>.
    /// </summary>
    /// <param name="pattern">A sequence of <see cref="CodeInstruction"/> that could not be found.</param>
    /// <param name="target">The target method where the pattern was searched for.</param>
    /// <param name="snitch">A callback to snitch on applied changes to the target method.</param>
    /// <exception cref="PatternNotFoundException">Thrown with the specified parameters.</exception>
    [DoesNotReturn]
    public static void ThrowPatternNotFoundException(CodeInstruction[] pattern, MethodBase target, Func<string> snitch)
    {
        throw new PatternNotFoundException(pattern, target, snitch);
    }

    /// <summary>
    ///     Throws a new <see cref="LabelNotFoundException"/>.
    /// </summary>
    /// <param name="label">The <see cref="Label"/> which could not be found.</param>
    /// <param name="target">The target method where the label was searched for.</param>
    /// <exception cref="LabelNotFoundException">Thrown with the specified parameters.</exception>
    [DoesNotReturn]
    public static void ThrowLabelNotFoundException(Label label, MethodBase target)
    {
        throw new LabelNotFoundException(label, target);
    }

    /// <summary>
    ///     Throws a new <see cref="LabelNotFoundException"/>.
    /// </summary>
    /// <param name="label">The <see cref="Label"/> which could not be found.</param>
    /// <param name="target">The target method where the label was searched for.</param>
    /// <param name="snitch">A callback to snitch on applied changes to the target method.</param>
    /// <exception cref="LabelNotFoundException">Thrown with the specified parameters.</exception>
    [DoesNotReturn]
    public static void ThrowLabelNotFoundException(Label label, MethodBase target, Func<string> snitch)
    {
        throw new LabelNotFoundException(label, target, snitch);
    }

    /// <summary>
    ///     Throws a new <see cref="MissingTypeException"/>.
    /// </summary>
    /// <param name="name">The name of the expected type.</param>
    /// <returns>Nothing.</returns>
    /// <exception cref="MissingTypeException">Thrown with the specified parameters.</exception>
    [DoesNotReturn]
    public static Type ThrowMissingTypeException(string name)
    {
        throw new MissingTypeException(name);
    }

    #endregion custom exceptions
}
