namespace DaLion.Shared.Extensions.Functional;

/// <summary>Extensions for <see cref="Func{TResult}"/> and <see cref="Action"/> delegates.</summary>
public static class DelegateExtensions
{
    #region curry function

    public static Func<T1, Func<T2, TResult>> Curry<T1, T2, TResult>(this Func<T1, T2, TResult> func) => arg1 => arg2 => func(arg1, arg2);

    public static Func<T1, Func<T2, Func<T3, TResult>>> Curry<T1, T2, T3, TResult>(this Func<T1, T2, T3, TResult> func) => arg1 => arg2 => arg3 => func(arg1, arg2, arg3);

    public static Func<T1, Func<T2, Func<T3, Func<T4, TResult>>>> Curry<T1, T2, T3, T4, TResult>(this Func<T1, T2, T3, T4, TResult> func) => arg1 => arg2 => arg3 => arg4 => func(arg1, arg2, arg3, arg4);

    #endregion curry functions

    #region uncurry functions

    public static Func<T1, T2, TResult> Uncurry<T1, T2, TResult>(this Func<T1, Func<T2, TResult>> curried) => (arg1, arg2) => curried(arg1)(arg2);

    public static Func<T1, T2, T3, TResult> Uncurry<T1, T2, T3, TResult>(this Func<T1, Func<T2, Func<T3, TResult>>> curried) => (arg1, arg2, arg3) => curried(arg1)(arg2)(arg3);

    public static Func<T1, T2, T3, T4, TResult> Uncurry<T1, T2, T3, T4, TResult>(this Func<T1, Func<T2, Func<T3, Func<T4, TResult>>>> curried) => (arg1, arg2, arg3, arg4) => curried(arg1)(arg2)(arg3)(arg4);

    #endregion uncurry functions

    #region partial application functions

    public static Func<TResult> Partial<T1, TResult>(this Func<T1, TResult> func, T1 arg1) => () => func(arg1);

    public static Func<T2, TResult> Partial<T1, T2, TResult>(this Func<T1, T2, TResult> func, T1 arg1) => arg2 => func(arg1, arg2);

    public static Func<T1, TResult> Partial<T1, T2, TResult>(this Func<T1, T2, TResult> func, T2 arg2) => arg1 => func(arg1, arg2);

    public static Func<T, TResult> Partial<T, TResult>(this Func<T, T, TResult> func, T arg1) => arg2 => func(arg1, arg2);

    public static Func<TResult> Partial<T1, T2, TResult>(this Func<T1, T2, TResult> func, T1 arg1, T2 arg2) => () => func(arg1, arg2);

    public static Func<T2, T3, TResult> Partial<T1, T2, T3, TResult>(this Func<T1, T2, T3, TResult> func, T1 arg1) => (arg2, arg3) => func(arg1, arg2, arg3);

    public static Func<T1, T3, TResult> Partial<T1, T2, T3, TResult>(this Func<T1, T2, T3, TResult> func, T2 arg2) => (arg1, arg3) => func(arg1, arg2, arg3);

    public static Func<T1, T2, TResult> Partial<T1, T2, T3, TResult>(this Func<T1, T2, T3, TResult> func, T3 arg3) => (arg1, arg2) => func(arg1, arg2, arg3);

    public static Func<T, T, TResult> Partial<T, TResult>(this Func<T, T, T, TResult> func, T arg1) => (arg2, arg3) => func(arg1, arg2, arg3);

    public static Func<T3, TResult> Partial<T1, T2, T3, TResult>(this Func<T1, T2, T3, TResult> func, T1 arg1, T2 arg2) => arg3 => func(arg1, arg2, arg3);

    public static Func<T2, TResult> Partial<T1, T2, T3, TResult>(this Func<T1, T2, T3, TResult> func, T1 arg1, T3 arg3) => arg2 => func(arg1, arg2, arg3);

    public static Func<T1, TResult> Partial<T1, T2, T3, TResult>(this Func<T1, T2, T3, TResult> func, T2 arg2, T3 arg3) => arg1 => func(arg1, arg2, arg3);

    public static Func<TResult> Partial<T1, T2, T3, TResult>(this Func<T1, T2, T3, TResult> func, T1 arg1, T2 arg2, T3 arg3) => () => func(arg1, arg2, arg3);

    #endregion partial application functions

    #region curry actions

    public static Func<T1, Action<T2>> Curry<T1, T2>(this Action<T1, T2> action) => arg1 => arg2 => action(arg1, arg2);

    public static Func<T1, Func<T2, Action<T3>>> Curry<T1, T2, T3>(this Action<T1, T2, T3> action) => arg1 => arg2 => arg3 => action(arg1, arg2, arg3);

    public static Func<T1, Func<T2, Func<T3, Action<T4>>>> Curry<T1, T2, T3, T4>(this Action<T1, T2, T3, T4> action) => arg1 => arg2 => arg3 => arg4 => action(arg1, arg2, arg3, arg4);

    #endregion curry actions

    #region uncurry actions

    public static Action<T1, T2> Uncurry<T1, T2>(this Func<T1, Action<T2>> curried) => (arg1, arg2) => curried(arg1)(arg2);

    public static Action<T1, T2, T3> Uncurry<T1, T2, T3>(this Func<T1, Func<T2, Action<T3>>> curried) => (arg1, arg2, arg3) => curried(arg1)(arg2)(arg3);

    public static Action<T1, T2, T3, T4> Uncurry<T1, T2, T3, T4>(this Func<T1, Func<T2, Func<T3, Action<T4>>>> curried) => (arg1, arg2, arg3, arg4) => curried(arg1)(arg2)(arg3)(arg4);

    #endregion uncurry actions

    #region partial application actions

    public static Action Partial<T1>(this Action<T1> action, T1 arg1) => () => action(arg1);

    public static Action<T2> Partial<T1, T2>(this Action<T1, T2> action, T1 arg1) => arg2 => action(arg1, arg2);

    public static Action<T1> Partial<T1, T2>(this Action<T1, T2> action, T2 arg2) => arg1 => action(arg1, arg2);

    public static Action<T> Partial<T>(this Action<T, T> action, T arg1) => arg2 => action(arg1, arg2);

    public static Action Partial<T1, T2>(this Action<T1, T2> action, T1 arg1, T2 arg2) => () => action(arg1, arg2);

    public static Action<T2, T3> Partial<T1, T2, T3>(this Action<T1, T2, T3> action, T1 arg1) => (arg2, arg3) => action(arg1, arg2, arg3);

    public static Action<T1, T3> Partial<T1, T2, T3>(this Action<T1, T2, T3> action, T2 arg2) => (arg1, arg3) => action(arg1, arg2, arg3);

    public static Action<T1, T2> Partial<T1, T2, T3>(this Action<T1, T2, T3> action, T3 arg3) => (arg1, arg2) => action(arg1, arg2, arg3);

    public static Action<T, T> Partial<T>(this Action<T, T, T> action, T arg1) => (arg2, arg3) => action(arg1, arg2, arg3);

    public static Action<T3> Partial<T1, T2, T3>(this Action<T1, T2, T3> action, T1 arg1, T2 arg2) => arg3 => action(arg1, arg2, arg3);

    public static Action<T2> Partial<T1, T2, T3>(this Action<T1, T2, T3> action, T1 arg1, T3 arg3) => (arg2) => action(arg1, arg2, arg3);

    public static Action<T1> Partial<T1, T2, T3>(this Action<T1, T2, T3> action, T2 arg2, T3 arg3) => (arg1) => action(arg1, arg2, arg3);

    public static Action Partial<T1, T2, T3>(this Action<T1, T2, T3> action, T1 arg1, T2 arg2, T3 arg3) => () => action(arg1, arg2, arg3);

    #endregion partial application actions
}
