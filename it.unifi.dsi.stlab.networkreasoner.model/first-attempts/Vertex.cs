using System;
using System.Collections;
using System.Collections.Generic;

namespace it.unifi.dsi.stlab.networkreasoner.model
{
	public class Vertex
	{
		private Dictionary<Property, PropertyImplementation> properties {get;set;}

		public Vertex (ICollection<KeyValuePair<Property, PropertyImplementation>> properties)
		{
			this.properties = new Dictionary<Property, PropertyImplementation>();
			foreach (var pair in properties) {
				this.properties.Add(pair.Key, pair.Value);
			}
		}

		public void doOnProperties(Action<Property, PropertyImplementation> anAction){

			foreach (var pair in properties) {
				anAction.Invoke(pair.Key, pair.Value);
			}

		}

		public Vertex filterProperties(Predicate<Property> aSelectionPredicate){
			return null;


		}

		public void DoSomethingWithPressure ()
		{
			if (Pressure > 0) {
			} else {
			}
		}

		public double Pressure {get;set;}
	}
}

