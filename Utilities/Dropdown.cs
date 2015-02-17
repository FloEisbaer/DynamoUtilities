using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using Autodesk.DesignScript.Runtime;
using Dynamo.Controls;
using Dynamo.Models;
using Dynamo.UI.Commands;
using Dynamo.Wpf;
using ProtoCore.AST.AssociativeAST;
using Utilities.Properties;
using VMDataBridge;

namespace Utilities
{
    // The NodeName attribute is what will display on 
    // top of the node in Dynamo
    [NodeName("DropDown")]

    // The NodeCategory attribute determines how your
    // node will be organized in the library. You can
    // specify your own category or use one of the 
    // built-ins provided in BuiltInNodeCategories.
    [NodeCategory("Utilities")]

    // The description will display in the tooltip
    // and in the help window for the node.
    [NodeDescription("UtilitiesDescription")]//,typeof(Resources))]

    [IsDesignScriptCompatible]
    public class Dropdown : NodeModel
    {
        #region private members

        private IList<string> _list;
        private int _index;
                
        public event Action<Object> EvaluationComplete;
        public new object CachedValue;

        #endregion

        #region properties

        /// <summary>
        /// A value that will be bound to our
        /// custom UI's awesome slider.
        /// </summary>
        public IList<string> List
        {
            get { return _list; }
            set
            {
                _list = value;
                RaisePropertyChanged("List");

                //OnNodeModified();
            }
        }

        public int Index
        {
            get { return _index; }
            set 
            {
                _index = value;
                RaisePropertyChanged("Index");

                OnNodeModified();
            }
        }
        
        #endregion

        #region constructor

        /// <summary>
        /// The constructor for a NodeModel is used to create
        /// the input and output ports and specify the argument
        /// lacing.
        /// </summary>
        public Dropdown()
        {
            // When you create a UI node, you need to do the
            // work of setting up the ports yourself. To do this,
            // you can populate the InPortData and the OutPortData
            // collections with PortData objects describing your ports.
            InPortData.Add(new PortData("list", Resources.UtilitiesPortDataInputToolTip));

            // Nodes can have an arbitrary number of inputs and outputs.
            // If you want more ports, just create more PortData objects.
            OutPortData.Add(new PortData("item", Resources.UtilitiesPortDataOutputToolTip));
            OutPortData.Add(new PortData("fixedListItem", Resources.UtilitiesPortDataOutputToolTip));
            OutPortData.Add(new PortData("some awesome", Resources.UtilitiesPortDataOutputToolTip));

            // This call is required to ensure that your ports are
            // properly created.
            RegisterAllPorts();

            // The arugment lacing is the way in which Dynamo handles
            // inputs of lists. If you don't want your node to
            // support argument lacing, you can set this to LacingStrategy.Disabled.
            ArgumentLacing = LacingStrategy.Disabled;

            ShouldDisplayPreviewCore = false;

            List = new List<string> {"la", "le", "lu"};
            Index = 0;
        }

        #endregion

        #region public methods

        protected override void OnBuilt()
        {
            base.OnBuilt();
            DataBridge.Instance.RegisterCallback(GUID.ToString(), OnEvaluationComplete);
        }

        public override void Dispose()
        {
            base.Dispose();
            DataBridge.Instance.UnregisterCallback(GUID.ToString());
        }

        private void OnEvaluationComplete(object obj)
        {
            this.CachedValue = obj;

            if (EvaluationComplete != null)
            {
                EvaluationComplete(obj);
            }
        }

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

            var firstNode = inputAstNodes[0];
            var dfdf = firstNode.Name;

            var inPort = InPorts[0];

            // Using the AstFactory class, we can build AstNode objects
            // that assign doubles, assign function calls, build expression lists, etc.
            return new[]
            {
                // In these assignments, GetAstIdentifierForOutputIndex finds 
                // the unique identifier which represents an output on this node
                // and 'assigns' that variable the expression that you create.
                
                // For the first node, return an index of the input
                AstFactory.BuildAssignment(
                    GetAstIdentifierForOutputIndex(0),
                    AstFactory.BuildExprList(inputAstNodes)),//[_index])),


                    
                // For the second node, return an index of the fixed list
                AstFactory.BuildAssignment(
                    GetAstIdentifierForOutputIndex(1),
                    AstFactory.BuildStringNode(List[_index])),

                // For the third node, we'll build a double node that 
                // passes along our value for awesome.
                AstFactory.BuildAssignment(
                    GetAstIdentifierForOutputIndex(2),
                    AstFactory.BuildIntNode(_index))
            };
        }

        #endregion

    }

    /// <summary>
    ///     View customizer for Utilities Node Model.
    /// </summary>
    public class UtilitiesNodeViewCustomization : INodeViewCustomization<Dropdown>
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
        public void CustomizeView(Dropdown model, NodeView nodeView)
        {
            // The view variable is a reference to the node's view.
            // In the middle of the node is a grid called the InputGrid.
            // We reccommend putting your custom UI in this grid, as it has
            // been designed for this purpose.

            // Create an instance of our custom UI class (defined in xaml),
            // and put it into the input grid.
            var dropdownControl = new DropdownControl();
            nodeView.inputGrid.Children.Add(dropdownControl);

            // Set the data context for our control to be this class.
            // Properties in this class which are data bound will raise 
            // property change notifications which will update the UI.
            dropdownControl.DataContext = model;
        }

        /// <summary>
        /// Here you can do any cleanup you require if you've assigned callbacks for particular 
        /// UI events on your node.
        /// </summary>
        public void Dispose() { }
    }

}
