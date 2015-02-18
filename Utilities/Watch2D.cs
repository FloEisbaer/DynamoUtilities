using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Autodesk.DesignScript.Runtime;
using Dynamo.Controls;
using Dynamo.Models;
using Dynamo.Wpf;
using ProtoCore.AST.AssociativeAST;
using Utilities.Properties;

namespace Utilities
{
    // The NodeName attribute is what will display on 
    // top of the node in Dynamo
    [NodeName("Watch2D")]

    // The NodeCategory attribute determines how your
    // node will be organized in the library. You can
    // specify your own category or use one of the 
    // built-ins provided in BuiltInNodeCategories.
    [NodeCategory("Utilities")]

    // The description will display in the tooltip
    // and in the help window for the node.
    [NodeDescription("UtilitiesDescription")]//,typeof(Resources))]

    [IsDesignScriptCompatible]
    public class Watch2D : NodeModel
    {
        #region private members

        private double _xmin;
        private double _ymin;
        private double _xmax;
        private double _ymax;

        #endregion

        #region properties

        public event EventHandler RequestChangeWatch2D;
        protected virtual void OnRequestChangeWatch2D(object sender, EventArgs e)
        {
            if (RequestChangeWatch2D != null)
                RequestChangeWatch2D(sender, e);
        }

        public double Xmin
        {
            get { return _xmin; }
            set 
            {
                _xmin = value;
                RaisePropertyChanged("Xmin");
            }
        }

        public double Ymin
        {
            get { return _ymin; }
            set
            {
                _ymin = value;
                RaisePropertyChanged("Ymin");
            }
        }

        public double Xmax
        {
            get { return _xmax; }
            set
            {
                _xmin = value;
                RaisePropertyChanged("Xmax");
            }
        }

        public double Ymax
        {
            get { return _ymax; }
            set
            {
                _ymin = value;
                RaisePropertyChanged("Ymax");
            }
        }

        #endregion

        #region constructor

        /// <summary>
        /// The constructor for a NodeModel is used to create
        /// the input and output ports and specify the argument
        /// lacing.
        /// </summary>
        public Watch2D()
        {
            // When you create a UI node, you need to do the
            // work of setting up the ports yourself. To do this,
            // you can populate the InPortData and the OutPortData
            // collections with PortData objects describing your ports.
            InPortData.Add(new PortData("1", Resources.UtilitiesPortDataInputToolTip));
            InPortData.Add(new PortData("2", Resources.UtilitiesPortDataInputToolTip));

            // Nodes can have an arbitrary number of inputs and outputs.
            // If you want more ports, just create more PortData objects.
            OutPortData.Add(new PortData("", Resources.UtilitiesPortDataOutputToolTip));

            // This call is required to ensure that your ports are
            // properly created.
            RegisterAllPorts();

            // The arugment lacing is the way in which Dynamo handles
            // inputs of lists. If you don't want your node to
            // support argument lacing, you can set this to LacingStrategy.Disabled.
            ArgumentLacing = LacingStrategy.Disabled;

            this.PropertyChanged += Watch2D_PropertyChanged;

            // never used in current build!!
            Xmin = 0;
            Xmax = 6.5;
        
            Ymin = -1.1;
            Ymax = 1.1;
        }

        private void Watch2D_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "IsUpdated")
                return;

            if (InPorts.Any(x => x.Connectors.Count == 0))
                return;

            OnRequestChangeWatch2D(this, EventArgs.Empty);
        }

        #endregion

        #region public methods

        /// <summary>
        /// If this method is not overriden, Dynamo will, by default
        /// pass data through this node. But we wouldn't be here if
        /// we just wanted to pass data through the node, so let's 
        /// try using the data.
        /// </summary>
        /// <param name="inputAstNodes"></param>
        /// <returns></returns>
        [IsVisibleInDynamoLibrary(false)]
        public override IEnumerable<AssociativeNode> BuildOutputAst(List<AssociativeNode> inputAstNodes)
        {
            // When you create your own UI node you are responsible
            // for generating the abstract syntax tree (AST) nodes which
            // specify what methods are called, or how your data is passed
            // when execution occurs.

            // WARNING!!!
            // Do not throw an exception during AST creation. If you
            // need to convey a failure of this node, then use
            // AstFactory.BuildNullNode to pass out null.

            // Using the AstFactory class, we can build AstNode objects
            // that assign doubles, assign function calls, build expression lists, etc.
            return new[]
            {
                // In these assignments, GetAstIdentifierForOutputIndex finds 
                // the unique identifier which represents an output on this node
                // and 'assigns' that variable the expression that you create.
                
                // For the first node, we'll build a double node that 
                // passes along our value for awesome.
                AstFactory.BuildAssignment(
                    GetAstIdentifierForOutputIndex(0),
                    AstFactory.BuildExprList(inputAstNodes))
            };
        }

        #endregion

    }

    /// <summary>
    ///     View customizer for Utilities Node Model.
    /// </summary>
    public class Watch2DNodeViewCustomization : INodeViewCustomization<Watch2D>
    {
        /// <summary>
        /// At run-time, this method is called during the node 
        /// creation. Here you can create custom UI elements and
        /// add them to the node view, but we recommend designing
        /// your UI declaratively using xaml, and binding it to
        /// properties on this node as the DataContext.
        /// </summary>
        /// <param name="model">The NodeModel representing the node's core logic.</param>
        /// <param name="nodeView">The NodeView representing the node in the graph.</param>
        public void CustomizeView(Watch2D model, NodeView nodeView)
        {
            // The view variable is a reference to the node's view.
            // In the middle of the node is a grid called the InputGrid.
            // We reccommend putting your custom UI in this grid, as it has
            // been designed for this purpose.

            var dm = nodeView.ViewModel.DynamoViewModel.Model;

            // Create an instance of our custom UI class (defined in xaml),
            // and put it into the input grid.
            var watch2DControl = new Watch2DControl();
            
            nodeView.inputGrid.Children.Add(watch2DControl);

            // Set the data context for our control to be this class.
            // Properties in this class which are data bound will raise 
            // property change notifications which will update the UI.
            watch2DControl.DataContext = model;

            model.RequestChangeWatch2D += delegate
            {
                model.DispatchOnUIThread(delegate
                {
                    var colorStartNode = model.InPorts[0].Connectors[0].Start.Owner;
                    var startIndex = model.InPorts[0].Connectors[0].Start.Index;
                    var colorEndNode = model.InPorts[1].Connectors[0].Start.Owner;
                    var endIndex = model.InPorts[1].Connectors[0].Start.Index;

                    var startId = colorStartNode.GetAstIdentifierForOutputIndex(startIndex).Name;
                    var endId = colorEndNode.GetAstIdentifierForOutputIndex(endIndex).Name;

                    var startMirror = dm.EngineController.GetMirror(startId);
                    var endMirror = dm.EngineController.GetMirror(endId);

                    object start = 0;
                    object end = 0;

                    if (startMirror == null)
                    {
                        start = -1.1;
                    }
                    else
                    {
                        if (startMirror.GetData().IsCollection)
                        {
                            start = startMirror.GetData().GetElements().
                                Select(x => x.Data).FirstOrDefault();
                        }
                        else
                        {
                            start = startMirror.GetData().Data;
                        }
                    }

                    if (endMirror == null)
                    {
                        end = 1.1;
                    }
                    else
                    {
                        if (endMirror.GetData().IsCollection)
                        {
                            end = endMirror.GetData().GetElements().
                                Select(x => x.Data).FirstOrDefault();
                        }
                        else
                        {
                            end = endMirror.GetData().Data;
                        }
                    }

                    watch2DControl.ymin = (start == null) ? 0 : (double) start;
                    watch2DControl.ymax = (end == null) ? 0 : (double) end;
                    watch2DControl.AddChart();
                });
            };

        }

        /// <summary>
        /// Here you can do any cleanup you require if you've assigned callbacks for particular 
        /// UI events on your node.
        /// </summary>
        public void Dispose() { }
    }

}
