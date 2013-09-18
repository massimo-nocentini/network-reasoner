public static RisultatiCalcoloGas SimulaNew(ImpostazioniCalcolo impostazioniCalcolo, ImpostazioniGas impostazioniGas)
        {
            GasNetwork gasNetwork = new GasNetwork();

            Dictionary<NodoGas, GasNodeAbstract> ourNodesByTheirNodes = new Dictionary<NodoGas, GasNodeAbstract>();
            Dictionary<GasNodeAbstract, NodoGas> theirNodesByOurNodes = new Dictionary<GasNodeAbstract, NodoGas>();

            Dictionary<RamoGas, GasEdgeAbstract> ourEdgesByTheirEdges = new Dictionary<RamoGas, GasEdgeAbstract>();
            Dictionary<GasEdgeAbstract, RamoGas> theirEdgesByOurEdges = new Dictionary<GasEdgeAbstract, RamoGas>();

            Random rand = new Random();

            AdaptNodes(impostazioniGas, gasNetwork, ourNodesByTheirNodes, theirNodesByOurNodes, rand);

            AdaptEdges(impostazioniGas, gasNetwork, ourNodesByTheirNodes, ourEdgesByTheirEdges, theirEdgesByOurEdges, rand);

            AmbientParameters ambientParameters = AdaptParametriCalcoloGasToAmbientParameters(impostazioniGas.parametri);

            Dictionary<GasNodeAbstract, double> unknownsByOurNodes;
            Dictionary<GasEdgeAbstract, double> QvaluesByOurEdges;
            SolveNewtonRaphsonSystem(gasNetwork, ambientParameters, out unknownsByOurNodes, out QvaluesByOurEdges);

            RisultatiCalcoloGas result = new RisultatiCalcoloGas();

            FillMeasuresForNodes(ourNodesByTheirNodes, unknownsByOurNodes, result);

            FillMeasuresForEdges(ourEdgesByTheirEdges, QvaluesByOurEdges, result);

            return result;
        }

        private static void FillMeasuresForEdges(Dictionary<RamoGas, GasEdgeAbstract> ourEdgesByTheirEdges, Dictionary<GasEdgeAbstract, double> QvaluesByOurEdges, RisultatiCalcoloGas result)
        {
            ourEdgesByTheirEdges.Keys.ToList().ForEach(theirEdge =>
            {
                GasEdgeAbstract ourEdge = ourEdgesByTheirEdges[theirEdge];

                result.misure.setMisura(theirEdge, new MisuraRamoGas(
                    QvaluesByOurEdges[ourEdge], TipoMisuraRamoGas.MisuraCalcolata));
            });
        }

        private static void FillMeasuresForNodes(Dictionary<NodoGas, GasNodeAbstract> ourNodesByTheirNodes, Dictionary<GasNodeAbstract, double> unknownsByOurNodes, RisultatiCalcoloGas result)
        {
            ourNodesByTheirNodes.Keys.ToList().ForEach(
                theirNode =>
                {
                    MisuraNodoGas misuraNodoGas = null;

                    GasNodeAbstract ourNode = ourNodesByTheirNodes[theirNode];

                    if (ourNode is GasNodeWithGadget)
                    {
                        if ((ourNode as GasNodeWithGadget).Gadget is GasNodeGadgetSupply)
                        {
                            misuraNodoGas = new MisuraNodoGas(
                                unknownsByOurNodes[ourNode], null, TipoMisuraNodoGas.MisuraCalcolata);
                        }
                        else if ((ourNode as GasNodeWithGadget).Gadget is GasNodeGadgetLoad)
                        {
                            misuraNodoGas = new MisuraNodoGas(
                                null, unknownsByOurNodes[ourNode], TipoMisuraNodoGas.MisuraCalcolata);
                        }
                    }
                    else
                    {
                        // nel caso non si ha un gadget significa che e' un nodo passivo 
                        // e quindi non immette pressione, al massimo ha un prelievo
                        misuraNodoGas = new MisuraNodoGas(
                            null, unknownsByOurNodes[ourNode], TipoMisuraNodoGas.MisuraCalcolata);
                    }

                    result.misure.setMisura(theirNode, misuraNodoGas);
                });
        }

        private static void SolveNewtonRaphsonSystem(GasNetwork gasNetwork, AmbientParameters ambientParameters, out Dictionary<GasNodeAbstract, double> unknownsByOurNodes, out Dictionary<GasEdgeAbstract, double> QvaluesByOurEdges)
        {

            //ILog log = LogManager.GetLogger(typeof(NetwonRaphsonSystem));

            //XmlConfigurator.Configure(new FileInfo(
            //    "log4net-configurations/for-three-nodes-network.xml")
            //);


            var formulaVisitor = new GasFormulaVisitorExactlyDimensioned
            {
                AmbientParameters = ambientParameters
            };

            NetwonRaphsonSystem system = new NetwonRaphsonSystem
            {
                FormulaVisitor = formulaVisitor
                //,              EventsListener = new NetwonRaphsonSystemEventsListenerForLogging
                //{
                //    Log = log
                //}

            };

            system.initializeWith(gasNetwork);

            unknownsByOurNodes = null;
            QvaluesByOurEdges = null;
            var resultsAfterOneMutation = system.repeatMutateUntilRevertingDomainBack(
                new List<UntilConditionAbstract>{
				new UntilConditionAdimensionalRatioPrecisionReached{
					Precision = 1e-10
				}
			},
                out unknownsByOurNodes,
                out QvaluesByOurEdges
            );
        }

        private static void AdaptEdges(ImpostazioniGas impostazioniGas, GasNetwork gasNetwork, Dictionary<NodoGas, GasNodeAbstract> ourNodesByTheirNodes, Dictionary<RamoGas, GasEdgeAbstract> ourEdgesByTheirEdges, Dictionary<GasEdgeAbstract, RamoGas> theirEdgesByOurEdges, Random rand)
        {
            impostazioniGas.rete.getRami().ForEach(
                aGasEdge =>
                {

                    String edgeIdentifier = "edge_" + (DateTime.Now.Ticks + rand.NextDouble() * 100).ToString();

                    GasEdgeAbstract anAdaptedEdge = new GasEdgeTopological
                    {
                        StartNode = ourNodesByTheirNodes[aGasEdge.nodoInizio as NodoGas],
                        EndNode = ourNodesByTheirNodes[aGasEdge.nodoFine as NodoGas]
                    };

                    anAdaptedEdge = new GasEdgePhysical
                    {
                        Described = anAdaptedEdge,
                        Diameter = aGasEdge.diametro,
                        Length = (long)aGasEdge.lunghezza,
                        Roughness = aGasEdge.scabrezza
                    };

                    ourEdgesByTheirEdges.Add(aGasEdge, anAdaptedEdge);
                    theirEdgesByOurEdges.Add(anAdaptedEdge, aGasEdge);

                    gasNetwork.Edges.Add(edgeIdentifier, anAdaptedEdge);
                });
        }

        private static void AdaptNodes(ImpostazioniGas impostazioniGas, GasNetwork gasNetwork, Dictionary<NodoGas, GasNodeAbstract> ourNodesByTheirNodes, Dictionary<GasNodeAbstract, NodoGas> theirNodesByOurNodes, Random rand)
        {
            impostazioniGas.rete.getNodi().ForEach(
                aGasNode =>
                {

                    String nodeIdentifier = "node_" + (DateTime.Now.Ticks + rand.NextDouble() * 100).ToString();

                    GasNodeAbstract anAdaptedNode = new GasNodeTopological
                    {
                        Height = (long)aGasNode.quota,
                        Identifier = nodeIdentifier
                    };

                    MisuraNodoGas misuraNodoGas = impostazioniGas.misure.getMisura(aGasNode);
                    if (misuraNodoGas.tipo == TipoMisuraNodoGas.PressioneImposta &&
                        misuraNodoGas.pressione.HasValue)
                    {

                        GasNodeGadgetSupply supplyGadget = new GasNodeGadgetSupply
                        {
                            SetupPressure = misuraNodoGas.pressione.Value
                        };

                        anAdaptedNode = new GasNodeWithGadget
                        {
                            Equipped = anAdaptedNode,
                            Gadget = supplyGadget
                        };
                    }
                    else if (misuraNodoGas.tipo == TipoMisuraNodoGas.PrelievoImposto &&
                        misuraNodoGas.prelievo.HasValue)
                    {

                        GasNodeGadgetLoad loadGadget = new GasNodeGadgetLoad
                        {
                            Load = misuraNodoGas.prelievo.Value
                        };

                        anAdaptedNode = new GasNodeWithGadget
                        {
                            Equipped = anAdaptedNode,
                            Gadget = loadGadget
                        };
                    }

                    ourNodesByTheirNodes.Add(aGasNode, anAdaptedNode);
                    theirNodesByOurNodes.Add(anAdaptedNode, aGasNode);

                    gasNetwork.Nodes.Add(nodeIdentifier, anAdaptedNode);
                });
        }

        public static AmbientParameters AdaptParametriCalcoloGasToAmbientParameters(
            ParametriCalcoloGas parametriCalcoloGas)
        {

            AmbientParameters result = new AmbientParameters();

            return result;
        }
