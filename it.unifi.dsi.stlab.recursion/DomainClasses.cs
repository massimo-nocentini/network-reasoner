using System;
using System.Text;

namespace it.unifi.dsi.stlab.recursion
{
	abstract class AbstractBase : RecursionThroughSelf<AbstractBase>
	{
		#region RecursionThroughSelf implementation
		public AbstractBase Self {
			get;
			set;
		}
		#endregion

		public abstract void doSomething (StringBuilder b);

		public abstract void doSomethingElse (StringBuilder b);
	}

	class AtomicConcept : AbstractBase
	{
		public static AtomicConcept ctor ()
		{
			return new AtomicConcept ();
		}

		public override void doSomething (StringBuilder b)
		{
			b.AppendLine ("AtomicConcept: before doing something we need to do something else.");

			// this is classic method delegation, a special case of ``recursion''
			Self.doSomethingElse (b);

			b.AppendLine ("AtomicConcept: now that we did something else we can do something.");

		}

		public override void doSomethingElse (StringBuilder b)
		{
			b.AppendLine ("AtomicConcept: we're doing something else now.");
		}
	}

	abstract class ForwardingConcept : AbstractBase
	{
		AbstractBase Wrapped {
			get;
			set;
		}

		protected ForwardingConcept (AbstractBase wrapped)
		{
			this.Wrapped = wrapped;
		}

//		public static Func<ForwardingConcept> ctor (AbstractBase wrapped)
//		{
//			return () => new ForwardingConcept (wrapped);
//		}
	
		public override void doSomething (StringBuilder b)
		{
			Wrapped.doSomething (b);
		}

		public override void doSomethingElse (StringBuilder b)
		{
			Wrapped.doSomethingElse (b);
		}
	}

	class DoSomethingDecorator : ForwardingConcept
	{
		protected DoSomethingDecorator (AbstractBase wrapped):base(wrapped)
		{
		}
		
		public static Func<DoSomethingDecorator> ctor (AbstractBase wrapped)
		{
			return () => new DoSomethingDecorator (wrapped);
		}

		public override void doSomething (StringBuilder b)
		{
			b.AppendLine ("DoSomethingDecorator: PRE decoration for do something method");

			base.doSomething (b);

			b.AppendLine ("DoSomethingDecorator: POST decoration for do something method");
		}
	}

	class DoSomethingElseDecorator : ForwardingConcept
	{
		protected DoSomethingElseDecorator (AbstractBase wrapped):base(wrapped)
		{
		}
		
		public static Func<DoSomethingElseDecorator> ctor (AbstractBase wrapped)
		{
			return () => new DoSomethingElseDecorator (wrapped);
		}

		public override void doSomethingElse (StringBuilder b)
		{
			b.AppendLine ("DoSomethingElseDecorator: PRE decoration for do something else method");

			base.doSomethingElse (b);

			b.AppendLine ("DoSomethingElseDecorator: POST decoration for do something else method");
		}
	}
}

