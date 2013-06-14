﻿#pragma checksum "..\..\..\..\Views\WorkflowEditorView.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "58668B81A754933BEB113918ADA77425"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18046
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Microsoft.Support.Workflow.Authoring.AddIns.Converters;
using Microsoft.Support.Workflow.Authoring.Behaviors;
using Microsoft.Support.Workflow.Authoring.Views;
using System;
using System.Activities.Presentation.View;
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


namespace Microsoft.Support.Workflow.Authoring.AddIns.Views {
    
    
    /// <summary>
    /// WorkflowEditorView
    /// </summary>
    public partial class WorkflowEditorView : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector, System.Windows.Markup.IStyleConnector {
        
        
        #line 14 "..\..\..\..\Views\WorkflowEditorView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Microsoft.Support.Workflow.Authoring.AddIns.Views.WorkflowEditorView workflowEditorView;
        
        #line default
        #line hidden
        
        
        #line 37 "..\..\..\..\Views\WorkflowEditorView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Border heightConstraint;
        
        #line default
        #line hidden
        
        
        #line 51 "..\..\..\..\Views\WorkflowEditorView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ContentPresenter designer;
        
        #line default
        #line hidden
        
        
        #line 68 "..\..\..\..\Views\WorkflowEditorView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DataGrid tasksTable;
        
        #line default
        #line hidden
        
        
        #line 104 "..\..\..\..\Views\WorkflowEditorView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox XamlCodeEditor;
        
        #line default
        #line hidden
        
        
        #line 110 "..\..\..\..\Views\WorkflowEditorView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ScrollViewer stepList;
        
        #line default
        #line hidden
        
        
        #line 145 "..\..\..\..\Views\WorkflowEditorView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListBox errorList;
        
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
            System.Uri resourceLocater = new System.Uri("/Microsoft.Support.Workflow.Authoring.AddIns;component/views/workfloweditorview.x" +
                    "aml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\Views\WorkflowEditorView.xaml"
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
            this.workflowEditorView = ((Microsoft.Support.Workflow.Authoring.AddIns.Views.WorkflowEditorView)(target));
            return;
            case 2:
            this.heightConstraint = ((System.Windows.Controls.Border)(target));
            return;
            case 4:
            this.designer = ((System.Windows.Controls.ContentPresenter)(target));
            return;
            case 5:
            this.tasksTable = ((System.Windows.Controls.DataGrid)(target));
            return;
            case 6:
            this.XamlCodeEditor = ((System.Windows.Controls.TextBox)(target));
            
            #line 107 "..\..\..\..\Views\WorkflowEditorView.xaml"
            this.XamlCodeEditor.LostFocus += new System.Windows.RoutedEventHandler(this.XamlCodeEditor_MightHaveBeenEdited);
            
            #line default
            #line hidden
            return;
            case 7:
            this.stepList = ((System.Windows.Controls.ScrollViewer)(target));
            return;
            case 9:
            this.errorList = ((System.Windows.Controls.ListBox)(target));
            return;
            case 10:
            
            #line 179 "..\..\..\..\Views\WorkflowEditorView.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.TasksButton_Click);
            
            #line default
            #line hidden
            return;
            case 11:
            
            #line 190 "..\..\..\..\Views\WorkflowEditorView.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.StepsButton_Click);
            
            #line default
            #line hidden
            return;
            case 12:
            
            #line 201 "..\..\..\..\Views\WorkflowEditorView.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.ExportButton_Click);
            
            #line default
            #line hidden
            return;
            case 13:
            
            #line 212 "..\..\..\..\Views\WorkflowEditorView.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.ShowXamlButton_Click);
            
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
            eventSetter.Event = System.Windows.UIElement.DragEnterEvent;
            
            #line 43 "..\..\..\..\Views\WorkflowEditorView.xaml"
            eventSetter.Handler = new System.Windows.DragEventHandler(this.ExpressionTextBox_DragEnter);
            
            #line default
            #line hidden
            ((System.Windows.Style)(target)).Setters.Add(eventSetter);
            eventSetter = new System.Windows.EventSetter();
            eventSetter.Event = System.Windows.UIElement.PreviewDropEvent;
            
            #line 45 "..\..\..\..\Views\WorkflowEditorView.xaml"
            eventSetter.Handler = new System.Windows.DragEventHandler(this.ExpressionTextBox_PreviewDrop);
            
            #line default
            #line hidden
            ((System.Windows.Style)(target)).Setters.Add(eventSetter);
            eventSetter = new System.Windows.EventSetter();
            eventSetter.Event = System.Windows.UIElement.DragLeaveEvent;
            
            #line 47 "..\..\..\..\Views\WorkflowEditorView.xaml"
            eventSetter.Handler = new System.Windows.DragEventHandler(this.ExpressionTextBox_DragLeave);
            
            #line default
            #line hidden
            ((System.Windows.Style)(target)).Setters.Add(eventSetter);
            break;
            case 8:
            
            #line 137 "..\..\..\..\Views\WorkflowEditorView.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.FocusCurrentStep);
            
            #line default
            #line hidden
            break;
            }
        }
    }
}

