using System;

namespace TheLion.Common
{
	public static class Functionals
	{
		// One-argument Y-Combinator.
		public static Func<T, TResult> Y<T, TResult>(Func<Func<T, TResult>, Func<T, TResult>> F)
		{
			return t => F(Y(F))(t);
		}

		// Two-argument Y-Combinator.
		public static Func<T1, T2, TResult> Y<T1, T2, TResult>(Func<Func<T1, T2, TResult>, Func<T1, T2, TResult>> F)
		{
			return (t1, t2) => F(Y(F))(t1, t2);
		}

		// Three-arugument Y-Combinator.
		public static Func<T1, T2, T3, TResult> Y<T1, T2, T3, TResult>(Func<Func<T1, T2, T3, TResult>, Func<T1, T2, T3, TResult>> F)
		{
			return (t1, t2, t3) => F(Y(F))(t1, t2, t3);
		}

		// Four-arugument Y-Combinator.
		public static Func<T1, T2, T3, T4, TResult> Y<T1, T2, T3, T4, TResult>(Func<Func<T1, T2, T3, T4, TResult>, Func<T1, T2, T3, T4, TResult>> F)
		{
			return (t1, t2, t3, t4) => F(Y(F))(t1, t2, t3, t4);
		}

		// Curry first argument
		public static Func<T1, Func<T2, TResult>> Curry<T1, T2, TResult>(Func<T1, T2, TResult> F)
		{
			return t1 => t2 => F(t1, t2);
		}

		// Curry second argument.
		public static Func<T2, Func<T1, TResult>> Curry2nd<T1, T2, TResult>(Func<T1, T2, TResult> F)
		{
			return t2 => t1 => F(t1, t2);
		}

		// Uncurry first argument.
		public static Func<T1, T2, TResult> Uncurry<T1, T2, TResult>(Func<T1, Func<T2, TResult>> F)
		{
			return (t1, t2) => F(t1)(t2);
		}

		// Uncurry second argument.
		public static Func<T1, T2, TResult> Uncurry2nd<T1, T2, TResult>(Func<T2, Func<T1, TResult>> F)
		{
			return (t1, t2) => F(t2)(t1);
		}
	}
}