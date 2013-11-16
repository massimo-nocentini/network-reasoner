using System;
using NUnit.Framework;

namespace it.unifi.dsi.stlab.recursion
{


	abstract class RecursionSchema<R> where R : RecursionThroughSelf<R>
	{
		public abstract R makeInstance ();
	}

	interface RecursionThroughSelf<T>
	{
		T Self { get; set; }
	}

	class SelfRecursion<F, R> : RecursionSchema<R> where F :R where R:RecursionThroughSelf<R>
	{
		Func<F> Ctor {
			get;
			set;
		}

		public SelfRecursion (Func<F> ctor)
		{
			this.Ctor = ctor;
		}

		#region implemented abstract members of it.unifi.dsi.stlab.recursion.RecursionSchema
		public override R makeInstance ()
		{
			F instance = Ctor.Invoke ();
			instance.Self = instance;

			return instance;
		}
		#endregion
	}

	class ForeignRecursion<F, R> : RecursionSchema<R> where F : R where R:RecursionThroughSelf<R>
	{
		R Wrapped {
			get;
			set;
		}

		Func<F> RecurOnCtor {
			get;
			set;
		}

		public ForeignRecursion (R wrapped, Func<F> recurSelfCtor)
		{
			this.Wrapped = wrapped;
			this.RecurOnCtor = recurSelfCtor;
		}

		#region implemented abstract members of it.unifi.dsi.stlab.recursion.RecursionSchema
		public override R makeInstance ()
		{
			F instance = RecurOnCtor.Invoke ();

			Wrapped.Self = instance;

			return instance;
		}
		#endregion
	}
}

