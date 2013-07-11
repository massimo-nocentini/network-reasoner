using System;

namespace it.unifi.dsi.stlab.networkreasoner.model.rdfinterface
{
	public class NamespaceRepository
	{
		private String Namespace { get; set; }

		public static readonly NamespaceRepository rdf = new NamespaceRepository (
			"http://www.w3.org/1999/02/22-rdf-syntax-ns#");

		public NamespaceRepository (String aNamespace)
		{
			this.Namespace = aNamespace;
		}

		public NamespaceRepository Append (String suffix)
		{
			return new NamespaceRepository (this.Namespace + suffix);
		}

		public String Value(){
			return this.Namespace;
		}
	}
}

