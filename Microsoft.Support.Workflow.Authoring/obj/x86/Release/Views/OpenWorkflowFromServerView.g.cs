﻿#pragma checksum "..\..\..\..\Views\OpenWorkflowFromServerView.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "58CA30586FB31BE7DDBF89034F5310F6"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18046
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Microsoft.Support.Workflow.Authoring.Behaviors;
using Microsoft.Support.Workflow.Authoring.UIControls;
using Microsoft.Support.Workflow.Authoring.ViewModels;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.Animation;
using Telerik.Windows.Controls.BulletGraph;
using Telerik.Windows.Controls.Carousel;
using Telerik.Windows.Controls.Data.PropertyGrid;
using Telerik.Windows.Controls.Docking;
using Telerik.Windows.Controls.DragDrop;
using Telerik.Windows.Controls.Gauge;
using Telerik.Windows.Controls.GridView;
using Telerik.Windows.Controls.HeatMap;
using Telerik.Windows.Controls.Map;
using Telerik.Windows.Controls.Primitives;
using Telerik.Windows.Controls.RibbonBar;
using Telerik.Windows.Controls.Sparklines;
using Telerik.Windows.Controls.TimeBar;
using Telerik.Windows.Controls.Timeline;
using Telerik.Windows.Controls.TransitionEffects;
using Telerik.Windows.Controls.TreeListView;
using Telerik.Windows.Controls.TreeMap;
using Telerik.Windows.Controls.TreeView;
using Telerik.Windows.Data;
using Telerik.Windows.DragDrop;
using Telerik.Windows.DragDrop.Behaviors;
using Telerik.Windows.Shapes;


namespace Microsoft.Support.Workflow.Authoring.Views {
    
    
    /// <summary>
    /// OpenWorkflowFromServerView
    /// </summary>
    public partial class OpenWorkflowFromServerView : System.Windows.Window, System.Windows.Markup.IComponentConnector, System.Windows.Markup.IStyleConnector {
        
        
        #line 46 "..\..\..\..\Views\OpenWorkflowFromServerView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button SearchButton;
        
        #line default
        #line hidden
        
        
        #line 51 "..\..\..\..\Views\OpenWorkflowFromServerView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox NameOption;
        
        #line default
        #line hidden
        
        
        #line 52 "..\..\..\..\Views\OpenWorkflowFromServerView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox DescriptionOption;
        
        #line default
        #line hidden
        
        
        #line 53 "..\..\..\..\Views\OpenWorkflowFromServerView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox TagsOption;
        
        #line default
        #line hidden
        
        
        #line 54 "..\..\..\..\Views\OpenWorkflowFromServerView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox TypeOption;
        
        #line default
        #line hidden
        
        
        #line 55 "..\..\..\..\Views\OpenWorkflowFromServerView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox VersionOption;
        
        #line default
        #line hidden
        
        
        #line 56 "..\..\..\..\Views\OpenWorkflowFromServerView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox CreatedByOption;
        
        #line default
        #line hidden
        
        
        #line 60 "..\..\..\..\Views\OpenWorkflowFromServerView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox LatestVersion;
        
        #line default
        #line hidden
        
        
        #line 62 "..\..\..\..\Views\OpenWorkflowFromServerView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox cbShouldDownloadDependencies;
        
        #line default
        #line hidden
        
        
        #line 77 "..\..\..\..\Views\OpenWorkflowFromServerView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button OpenButton;
        
        #line default
        #line hidden
        
        
        #line 95 "..\..\..\..\Views\OpenWorkflowFromServerView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DataGrid WorkflowsGrid;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/Microsoft.Support.Workflow.Foundry;component/views/openworkflowfromserverview.xa" +
                    "ml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\Views\OpenWorkflowFromServerView.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal System.Delegate _CreateDelegate(System.Type delegateType, string handler) {
            return System.Delegate.CreateDelegate(delegateType, this, handler);
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            
            #line 30 "..\..\..\..\Views\OpenWorkflowFromServerView.xaml"
            ((System.Windows.Controls.TextBox)(target)).KeyDown += new System.Windows.Input.KeyEventHandler(this.TextBox_KeyDown);
            
            #line default
            #line hidden
            return;
            case 2:
            this.SearchButton = ((System.Windows.Controls.Button)(target));
            return;
            case 3:
            this.NameOption = ((System.Windows.Controls.CheckBox)(target));
            return;
            case 4:
            this.DescriptionOption = ((System.Windows.Controls.CheckBox)(target));
            return;
            case 5:
            this.TagsOption = ((System.Windows.Controls.CheckBox)(target));
            return;
            case 6:
            this.TypeOption = ((System.Windows.Controls.CheckBox)(target));
            return;
            case 7:
            this.VersionOption = ((System.Windows.Controls.CheckBox)(target));
            return;
            case 8:
            this.CreatedByOption = ((System.Windows.Controls.CheckBox)(target));
            return;
            case 9:
            this.LatestVersion = ((System.Windows.Controls.CheckBox)(target));
            return;
            case 10:
            this.cbShouldDownloadDependencies = ((System.Windows.Controls.CheckBox)(target));
            return;
            case 11:
            this.OpenButton = ((System.Windows.Controls.Button)(target));
            
            #line 77 "..\..\..\..\Views\OpenWorkflowFromServerView.xaml"
            this.OpenButton.Click += new System.Windows.RoutedEventHandler(this.OpenButton_Click);
            
            #line default
            #line hidden
            return;
            case 12:
            this.WorkflowsGrid = ((System.Windows.Controls.DataGrid)(target));
            
            #line 94 "..\..\..\..\Views\OpenWorkflowFromServerView.xaml"
            this.WorkflowsGrid.Sorting += new System.Windows.Controls.DataGridSortingEventHandler(this.WorkflowsGridSorting);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        void System.Windows.Markup.IStyleConnector.Connect(int connectionId, object target) {
            System.Windows.EventSetter eventSetter;
            switch (connectionId)
            {
            case 13:
            eventSetter = new System.Windows.EventSetter();
            eventSetter.Event = System.Windows.FrameworkElement.LoadedEvent;
            
            #line 103 "..\..\..\..\Views\OpenWorkflowFromServerView.xaml"
            eventSetter.Handler = new System.Windows.RoutedEventHandler(this.WorkflowsGridRowLoaded);
            
            #line default
            #line hidden
            ((System.Windows.Style)(target)).Setters.Add(eventSetter);
            break;
            }
        }
    }
}

