﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;
using Autodesk.DesignScript.Runtime;
using Dynamo.Controls;
using Dynamo.Models;
using Dynamo.Utilities;
using Dynamo.Wpf;
using DSCoreNodesUI;
using ProtoCore.AST.AssociativeAST;
using Utilities.Properties;

namespace Utilities
{
    [NodeName("DropDown")]
    [NodeCategory("Utilities")]
    [NodeDescription("DropdownDescription", typeof(Resources))]
    [IsDesignScriptCompatible]
    public class Dropdown : NodeModel
    {
        #region members

        private int _index;
        private ObservableCollection<DynamoDropDownItem> _items = new ObservableCollection<DynamoDropDownItem>();
        public event EventHandler RequestChangeDropdown;

        #endregion

        #region properties

        /// <summary>
        /// The selected Index
        /// </summary>

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

        public ObservableCollection<DynamoDropDownItem> Items
        {
            get { return _items; }
            set
            {
                _items = value;
                RaisePropertyChanged("Items");

                OnNodeModified();
            }
        }

        protected virtual void OnRequestChangeDropdown(object sender, EventArgs e)
        {
            if (RequestChangeDropdown != null)
                RequestChangeDropdown(sender, e);
        }

        private void Dropdown_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "IsUpdated")
                return;

            if (InPorts.Any(x => x.Connectors.Count == 0))
                return;

            OnRequestChangeDropdown(this, EventArgs.Empty);
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

            InPortData.Add(new PortData("list", Resources.DropdownPortDataInputToolTip));

            OutPortData.Add(new PortData("Item", Resources.DropdownPortDataOutputValueToolTip));
            OutPortData.Add(new PortData("Index", Resources.DropdownPortDataOutputIndexToolTip));
            //OutPortData.Add(new PortData("through", Resources.UtilitiesPortDataOutputToolTip));

            RegisterAllPorts();

            // The arugment lacing is the way in which Dynamo handles
            // inputs of lists. If you don't want your node to
            // support argument lacing, you can set this to LacingStrategy.Disabled.
            ArgumentLacing = LacingStrategy.Disabled;

            // removes preview button in the lower right corner of the node.
            ShouldDisplayPreviewCore = false;
            
            this.PropertyChanged += Dropdown_PropertyChanged;

            Items.Add(new DynamoDropDownItem("nothing selected", null));
            Index = 0;

        }

        #endregion

        #region public methods


        [IsVisibleInDynamoLibrary(false)]
        public override IEnumerable<AssociativeNode> BuildOutputAst(
            List<AssociativeNode> inputAstNodes)
        {
            return new[]
            {

                // In these assignments, GetAstIdentifierForOutputIndex finds 
                // the unique identifier which represents an output on this node
                // and 'assigns' that variable the expression that you create.
                
                // For the first node, return an index of the input
                AstFactory.BuildAssignment(
                    GetAstIdentifierForOutputIndex(0),
                    AstFactory.BuildPrimitiveNodeFromObject(Items[Index].Item)),

                AstFactory.BuildAssignment(
                    GetAstIdentifierForOutputIndex(1),
                    AstFactory.BuildIntNode(Index))//,

                //AstFactory.BuildAssignment(
                //GetAstIdentifierForOutputIndex(2),
                //AstFactory.BuildStringNode(inputAstNodes[0].ToString()))

            };
        }

        #endregion

    }

    /// <summary>
    ///     View customizer for Utilities Node Model.
    /// </summary>
    public class UtilitiesNodeViewCustomization : INodeViewCustomization<Dropdown>
    {

        public void CustomizeView(Dropdown model, NodeView nodeView)
        {
            var dm = nodeView.ViewModel.DynamoViewModel.Model;
            // Create an instance of our custom UI class (defined in xaml),
            // and put it into the input grid.
            var dropdownControl = new DropdownControl(model);
            nodeView.inputGrid.Children.Add(dropdownControl);

            // Set the data context for our control to be this class.
            // Properties in this class which are data bound will raise 
            // property change notifications which will update the UI.
            dropdownControl.DataContext = model;

            model.RequestChangeDropdown += delegate
            {
                model.DispatchOnUIThread(delegate
                {
                    var listNode = model.InPorts[0].Connectors[0].Start.Owner;
                    var listIndex = model.InPorts[0].Connectors[0].Start.Index;

                    var listId = listNode.GetAstIdentifierForOutputIndex(listIndex).Name;

                    var listMirror = dm.EngineController.GetMirror(listId);

                    model.Items.Clear();
                    
                    if (listMirror == null)
                    {
                        model.Items.Add(new DynamoDropDownItem("nothing selected", null));
                    }
                    else
                    {
                        if (listMirror.GetData().IsCollection)
                        {
                            foreach (var data in listMirror.GetData().GetElements())
                            {
                                model.Items.Add(new DynamoDropDownItem(data.Data.ToString(), data.Data));
                            }
                        }
                        else
                        {
                            var data = listMirror.GetData().Data;
                            model.Items.Add(new DynamoDropDownItem(data.ToString(), data));
                        }
                    }

                    var itemStrings = model.Items.Select(x => x.ToString());
                    dropdownControl.AddItems(itemStrings.ToObservableCollection());

                    //model.Value = dropdownControl.Selection;

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
