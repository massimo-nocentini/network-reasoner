using System;
using NUnit.Framework;
using System.Text;

namespace it.unifi.dsi.stlab.recursion
{
	[TestFixture()]
	public class Testing
	{
		[Test()]
		public void tying_recursion_on_self ()
		{
			var anObjectMaker = new SelfRecursion<AtomicConcept, AbstractBase> (
				AtomicConcept.ctor);

			AbstractBase anObject = anObjectMaker.makeInstance ();

			Assert.That (anObject.Self, Is.SameAs (anObject));

			StringBuilder accumulator = new StringBuilder ();
			anObject.doSomething (accumulator);
			StringBuilder expected = new StringBuilder ();
			expected.AppendLine ("AtomicConcept: before doing something we need to do something else.");
			expected.AppendLine ("AtomicConcept: we're doing something else now.");
			expected.AppendLine ("AtomicConcept: now that we did something else we can do something.");

			Assert.That (accumulator.ToString (), Is.EqualTo (expected.ToString ()));
		}
		
		[Test()]
		public void tying_recursion_on_foreing_object ()
		{
			var baseObjectMaker = new SelfRecursion<AtomicConcept, AbstractBase> (
				new Func<AtomicConcept> (() => new AtomicConcept ()));

			AbstractBase baseObject = baseObjectMaker.makeInstance ();

			var foreingSelfRecurionMaker = new ForeignRecursion<DoSomethingDecorator, AbstractBase> (
				baseObject, DoSomethingDecorator.ctor (baseObject));

			AbstractBase decorator = foreingSelfRecurionMaker.makeInstance ();

			Assert.That (baseObject.Self, Is.SameAs (decorator));
		}

		[Test()]
		public void tying_recursion_on_foreing_object_of_order_two ()
		{
			var baseObjectMaker = new SelfRecursion<AtomicConcept, AbstractBase> (
				new Func<AtomicConcept> (() => new AtomicConcept ()));

			AbstractBase baseObject = baseObjectMaker.makeInstance ();

			AbstractBase doSomethingDecorator = DoSomethingDecorator.ctor (baseObject).Invoke ();

			var doSomethingElseRecurionMaker = new ForeignRecursion<DoSomethingElseDecorator, AbstractBase> (
				baseObject, DoSomethingElseDecorator.ctor (doSomethingDecorator));

			var doSomethingElseDecorator = doSomethingElseRecurionMaker.makeInstance ();

			Assert.That (baseObject.Self, Is.SameAs (doSomethingElseDecorator));
		}
	}
}

