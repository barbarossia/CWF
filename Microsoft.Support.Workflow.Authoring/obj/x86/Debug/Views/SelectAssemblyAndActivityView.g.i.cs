﻿#pragma checksum "..\..\..\..\Views\SelectAssemblyAndActivityView.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "D2714A53BBE87F0A299B803C0E884B43"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18046
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Microsoft.Support.Workflow.Authoring.Common.Converters;
using Microsoft.Support.Workflow.Authoring.Views;
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


namespace Microsoft.Support.Workflow.Authoring.Views {
    
    
    /// <summary>
    /// SelectAssemblyAndActivityView
    /// </summary>
    public partial class SelectAssemblyAndActivityView : System.Windows.Window, System.Windows.Markup.IComponentConnector, System.Windows.Markup.IStyleConnector {
        
        
        #line 2 "..\..\..\..\Views\SelectAssemblyAndActivityView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Microsoft.Support.Workflow.Authoring.Views.SelectAssemblyAndActivityView SelectAssemblyWindow;
        
        #line default
        #line hidden
        
        
        #line 37 "..\..\..\..\Views\SelectAssemblyAndActivityView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DataGrid Assemblies;
        
        #line default
        #line hidden
        
        
        #line 136 "..\..\..\..\Views\SelectAssemblyAndActivityView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Microsoft.Support.Workflow.Authoring.Views.ActivityItemView activityItemView;
        
        #line default
        #line hidden
        
        
        #line 153 "..\..\..\..\Views\SelectAssemblyAndActivityView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DataGrid Activities;
        
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
            System.Uri resourceLocater = new System.Uri("/Microsoft.Support.Workflow.Foundry;component/views/selectassemblyandactivityview" +
                    ".xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\Views\SelectAssemblyAndActivityView.xaml"
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
            this.SelectAssemblyWindow = ((Microsoft.Support.Workflow.Authoring.Views.SelectAssemblyAndActivityView)(target));
            return;
            case 2:
            this.Assemblies = ((System.Windows.Controls.DataGrid)(target));
            return;
            case 5:
            this.activityItemView = ((Microsoft.Support.Workflow.Authoring.Views.ActivityItemView)(target));
            return;
            case 6:
            this.Activities = ((System.Windows.Controls.DataGrid)(target));
            
            #line 149 "..\..\..\..\Views\SelectAssemblyAndActivityView.xaml"
            this.Activities.SelectedCellsChanged += new System.Windows.Controls.SelectedCellsChangedEventHandler(this.DataGrid_SelectedCellChanged);
            
            #line default
            #line hidden
            return;
            case 7:
            
            #line 188 "..\..\..\..\Views\SelectAssemblyAndActivityView.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.OkButton_Click);
            
            #line default
            #line hidden
            return;
            case 8:
            
            #line 192 "..\..\..\..\Views\SelectAssemblyAndActivityView.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.CancelButton_Click);
            
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
            case 3:
            eventSetter = new System.Windows.EventSetter();
            eventSetter.Event = System.Windows.Controls.DataGridRow.SelectedEvent;
            
            #line 41 "..\..\..\..\Views\SelectAssemblyAndActivityView.xaml"
            eventSetter.Handler = new System.Windows.RoutedEventHandler(this.DataGridRow_OnSelected);
            
            #line default
            #line hidden
            ((System.Windows.Style)(target)).Setters.Add(eventSetter);
            break;
            case 4:
            
            #line 110 "..\..\..\..\Views\SelectAssemblyAndActivityView.xaml"
            ((System.Windows.Documents.Hyperlink)(target)).Click += new System.Windows.RoutedEventHandler(this.GetDetailHyperlink_Click);
            
            #line default
            #line hidden
            break;
            }
        }
    }
}

