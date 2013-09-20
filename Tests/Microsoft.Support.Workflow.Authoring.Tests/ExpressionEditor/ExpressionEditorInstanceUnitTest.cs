using System;
using System.Activities;
using System.Activities.Presentation;
using System.Activities.Presentation.Model;
using System.Activities.Presentation.Services;
using System.Activities.Presentation.View;
using System.Activities.Statements;
using System.Activities.XamlIntegration;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Xaml;
using AuthoringToolTests.Services;
using Microsoft.DynamicImplementations;
using Microsoft.Support.Workflow.Authoring.ExpressionEditor;
using Microsoft.Support.Workflow.Authoring.Models;
using Microsoft.Support.Workflow.Authoring.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Support.Workflow.Authoring.Tests.ExpressionEditor
{
    [TestClass]
    public class ExpressionEditorInstanceUnitTest
    {
        [WorkItem(321709)]
        [TestMethod]
        [Description("Check properties in ExpressionEditorInstance.")]
        [Owner("v-ery")]
        [TestCategory("Unit-NoDif")]
        public void ExpressionEditor_TestProperties()
        {
            ExpressionEditorInstance instance;
            ExpressionEditorService service = new ExpressionEditorService();
            service.IntellisenseNode = new TreeNode();
            EditingContext ec = new EditingContext();
            ModelTreeManager mtm = new ModelTreeManager(ec);

            Variable<string> argument = new Variable<string>("arg1");
            mtm.Load(argument);
            instance = service.CreateExpressionEditor(
                null, null, new List<ModelItem> { mtm.Root }, "Text") as ExpressionEditorInstance;
            Assert.IsNotNull(instance);
            PrivateObject privateInstance = new PrivateObject(instance);
            TextBox tb = privateInstance.GetField("editorTextBox") as TextBox;

            bool closingEventInvoked = false;
            instance.Closing += new EventHandler((s, e) => { closingEventInvoked = true; });
            instance.Close();
            Assert.IsTrue(closingEventInvoked);

            bool gotFocusEventInvoked = false;
            instance.GotAggregateFocus += new EventHandler((s, e) => { gotFocusEventInvoked = true; });
            instance.Focus();
            Assert.IsTrue(gotFocusEventInvoked);

            bool lostFocusEventInvoked = false;
            instance.LostAggregateFocus += new EventHandler((s, e) => { lostFocusEventInvoked = true; });
            instance.EditorLostFocus(tb, new EventArgs());
            Assert.IsTrue(lostFocusEventInvoked);

            bool textChangedEventInvoked = false;
            instance.TextChanged += new EventHandler((s, e) => { textChangedEventInvoked = true; });
            instance.Text = "tbtxt";
            Assert.IsTrue(textChangedEventInvoked);

            instance.AcceptsReturn = false;
            instance.AcceptsTab = false;
            instance.MinLines = 1;
            instance.MaxLines = 10;
            instance.HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;
            instance.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
            instance.ClearSelection();

            Assert.AreEqual(tb, instance.HostControl);
            Assert.AreEqual(tb.AcceptsReturn, instance.AcceptsReturn);
            Assert.AreEqual(tb.AcceptsTab, instance.AcceptsTab);
            Assert.AreEqual(tb.MinLines, instance.MinLines);
            Assert.AreEqual(tb.MaxLines, instance.MaxLines);
            Assert.AreEqual(tb.HorizontalScrollBarVisibility, instance.HorizontalScrollBarVisibility);
            Assert.AreEqual(tb.VerticalScrollBarVisibility, instance.VerticalScrollBarVisibility);
            Assert.AreEqual(tb.Text, instance.Text);
            Assert.AreEqual(tb.CanRedo, instance.CanRedo());
            Assert.AreEqual(tb.CanUndo, instance.CanUndo());
            Assert.AreEqual(tb.Text, instance.GetCommittedText());

            Assert.IsTrue(instance.HasAggregateFocus);
            Assert.IsTrue(instance.CanCompleteWord());
            Assert.IsTrue(instance.CanCopy());
            Assert.IsTrue(instance.CanCut());
            Assert.IsFalse(instance.CanDecreaseFilterLevel());
            Assert.IsFalse(instance.CanGlobalIntellisense());
            Assert.IsFalse(instance.CanIncreaseFilterLevel());
            Assert.IsFalse(instance.CanParameterInfo());
            Assert.IsTrue(instance.CanPaste());
            Assert.IsFalse(instance.CanQuickInfo());
            Assert.IsTrue(instance.CompleteWord());
            Assert.IsTrue(instance.Copy());
            Assert.IsTrue(instance.Cut());
            Assert.IsFalse(instance.DecreaseFilterLevel());
            Assert.IsFalse(instance.IncreaseFilterLevel());
            Assert.IsFalse(instance.GlobalIntellisense());
            Assert.IsFalse(instance.ParameterInfo());
            Assert.IsTrue(instance.Paste());
            Assert.IsFalse(instance.QuickInfo());
            Assert.IsTrue(instance.Redo());
            Assert.IsTrue(instance.Undo());
        }

        [WorkItem(322362)]
        [TestMethod]
        [Description("Check EditorKeyDown in ExpressionEditorInstance.")]
        [Owner("v-ery")]
        [TestCategory("Unit-Dif")]
        public void ExpressionEditor_TestEditorKeyDown()
        {
            using (var mock = new ImplementationOfType(typeof(Keyboard)))
            {
                TextBox tb = new TextBox();
                mock.Register(() => Keyboard.FocusedElement).Return(tb);

                ExpressionEditorInstance instance;
                ExpressionEditorService service = new ExpressionEditorService();
                service.IntellisenseNode = new TreeNode();
                EditingContext ec = new EditingContext();
                ModelTreeManager mtm = new ModelTreeManager(ec);
                KeyEventArgs e;

                Variable<string> argument = new Variable<string>("arg1");
                mtm.Load(argument);
                instance = service.CreateExpressionEditor(
                    null, null, new List<ModelItem> { mtm.Root }, string.Empty) as ExpressionEditorInstance;

                e = GetKeyDownEventArgs(Key.Enter);
                instance.EditorKeyDown(instance, e);
                Assert.IsTrue(e.Handled);

                e = GetKeyDownEventArgs(Key.Tab);
                instance.EditorKeyDown(instance, e);
                Assert.IsTrue(e.Handled);
            }
        }

        [WorkItem(322361)]
        [TestMethod]
        [Description("Check EditorKeyPress in ExpressionEditorInstance.")]
        [Owner("v-ery")]
        [TestCategory("Unit-Dif")]
        public void aaExpressionEditor_TestEditorKeyPress()
        {
            TreeNode node = null;
            using (new CachingIsolator())
            {
                AssemblyName[] names = Assembly.GetExecutingAssembly().GetReferencedAssemblies()
                    .Where(a => a.Name == "mscorlib" && a.Version.Major == 4).ToArray();
                ExpressionEditorHelper.GetReferencesFunc = () => { return names; };
                using (var assembly = new Implementation<Assembly>())
                {
                    using (var assemblyMock = new ImplementationOfType(typeof(Assembly)))
                    {
                        assemblyMock.Register(() => Assembly.GetExecutingAssembly()).Return(assembly.Instance);
                        AssemblyCompanyAttribute attrib = new AssemblyCompanyAttribute("TestCompany");
                        using (var attribMock = new ImplementationOfType(typeof(Attribute)))
                        {
                            attribMock.Register(() => Attribute.GetCustomAttribute(assembly.Instance, typeof(AssemblyCompanyAttribute), false))
                                .Return(attrib);

                            ExpressionEditorHelper.ClearIntellisenseList();
                            node = ExpressionEditorHelper.CreateIntellisenseList();
                        }
                    }
                }

                using (var keyboardMock = new ImplementationOfType(typeof(Keyboard)))
                {
                    ExpressionEditorInstance instance;
                    ExpressionEditorService service = new ExpressionEditorService();
                    service.IntellisenseNode = node;
                    instance = service.CreateExpressionEditor(
                        null, null, new List<ModelItem> { }, string.Empty) as ExpressionEditorInstance;
                    PrivateObject obj = new PrivateObject(instance);
                    TextBox tb = obj.GetField("editorTextBox") as TextBox;
                    IntellisensePopup popup;
                    KeyEventArgs e;
                    
                    e = GetKeyDownEventArgs(Key.S);
                    instance.EditorKeyPress(tb, e);
                    tb.Text = "s";
                    popup = obj.GetField("popupControl") as IntellisensePopup;
                    Assert.IsTrue(popup.IsOpen);
                    Assert.AreEqual("SByte", popup.ViewModel.SelectedItem.Name);

                    e = GetKeyDownEventArgs(Key.Y);
                    instance.EditorKeyPress(tb, e);
                    tb.Text = "sy";
                    popup = obj.GetField("popupControl") as IntellisensePopup;
                    Assert.IsTrue(popup.IsOpen);
                    Assert.AreEqual("System", popup.ViewModel.SelectedItem.Name);

                    e = GetKeyDownEventArgs(Key.Decimal);
                    instance.EditorKeyPress(tb, e);
                    Assert.AreEqual("System", tb.Text);
                    tb.Text = "System.";
                    popup = obj.GetField("popupControl") as IntellisensePopup;
                    Assert.IsTrue(popup.IsOpen);
                    Assert.AreEqual("AccessViolationException", popup.ViewModel.SelectedItem.Name);

                    e = GetKeyDownEventArgs(Key.Down);
                    instance.EditorKeyPress(tb, e);
                    Assert.IsTrue(e.Handled);
                    Assert.AreNotEqual("AccessViolationException", popup.ViewModel.SelectedItem.Name);

                    e = GetKeyDownEventArgs(Key.Up);
                    instance.EditorKeyPress(tb, e);
                    Assert.IsTrue(e.Handled);
                    Assert.AreEqual("AccessViolationException", popup.ViewModel.SelectedItem.Name);

                    e = GetKeyDownEventArgs(Key.End);
                    instance.EditorKeyPress(tb, e);
                    Assert.IsTrue(e.Handled);
                    Assert.AreNotEqual("AccessViolationException", popup.ViewModel.SelectedItem.Name);

                    e = GetKeyDownEventArgs(Key.Home);
                    instance.EditorKeyPress(tb, e);
                    Assert.IsTrue(e.Handled);
                    Assert.AreEqual("AccessViolationException", popup.ViewModel.SelectedItem.Name);

                    e = GetKeyDownEventArgs(Key.Escape);
                    instance.EditorKeyPress(tb, e);
                    Assert.IsTrue(e.Handled);
                    Assert.IsFalse(popup.IsOpen);

                    tb.Text = string.Empty;
                    keyboardMock.Register(() => Keyboard.Modifiers).Return(ModifierKeys.Shift);
                    e = GetKeyDownEventArgs(Key.D9);
                    instance.EditorKeyPress(tb, e);
                    popup = obj.GetField("popupControl") as IntellisensePopup;
                    Assert.IsFalse(popup.IsOpen);
                    keyboardMock.Register(() => Keyboard.Modifiers).Return(ModifierKeys.None);

                    tb.Text = "a";
                    e = GetKeyDownEventArgs(Key.Decimal);
                    instance.EditorKeyPress(tb, e);
                    popup = obj.GetField("popupControl") as IntellisensePopup;
                    Assert.IsTrue(popup.IsOpen);

                    instance.EditorLostFocus(tb, new EventArgs());
                    Assert.IsFalse(popup.IsOpen);

                    // test open method tooltip
                    keyboardMock.Register(() => Keyboard.Modifiers).Return(ModifierKeys.Shift);
                    tb.Text = "System.String.Substring";

                    e = GetKeyDownEventArgs(Key.D9);
                    instance.EditorKeyPress(tb, e);
                    tb.Text = "System.String.Substring(";
                    Assert.IsTrue(((ToolTip)tb.ToolTip).IsOpen);
                    popup = obj.GetField("popupControl") as IntellisensePopup;
                    Assert.IsNull(popup);
       
                    string content = ((ToolTip)tb.ToolTip).Content as string;
                    e = GetKeyDownEventArgs(Key.Up);
                    instance.EditorKeyPress(tb, e);
                    Assert.AreNotEqual(content, ((ToolTip)tb.ToolTip).Content);
                    e = GetKeyDownEventArgs(Key.Down);
                    instance.EditorKeyPress(tb, e);
                    Assert.AreEqual(content, ((ToolTip)tb.ToolTip).Content);

                    e = GetKeyDownEventArgs(Key.OemQuotes);
                    instance.EditorKeyPress(tb, e);
                    tb.Text = "System.String.Substring(\"";
                    Assert.IsTrue(((ToolTip)tb.ToolTip).IsOpen);
                    popup = obj.GetField("popupControl") as IntellisensePopup;
                    Assert.IsNull(popup);
                    instance.EditorKeyPress(tb, e);
                    tb.Text = "System.String.Substring(\"\"";
                    Assert.IsTrue(((ToolTip)tb.ToolTip).IsOpen);
                    popup = obj.GetField("popupControl") as IntellisensePopup;
                    Assert.IsNull(popup);

                    e = GetKeyDownEventArgs(Key.D0);
                    instance.EditorKeyPress(tb, e);
                    tb.Text = "System.String.Substring(\"\")";
                    Assert.IsNull(tb.ToolTip);
        
                    keyboardMock.Register(() => Keyboard.Modifiers).Return(ModifierKeys.None);

                    e = GetKeyDownEventArgs(Key.Decimal);
                    instance.EditorKeyPress(tb, e);
                    tb.Text += ".";
                    popup = obj.GetField("popupControl") as IntellisensePopup;
                    Assert.IsTrue(popup.IsOpen);
                    Assert.AreEqual("String", popup.ViewModel.SelectedItem.Parent.Name);

                    instance.ListKeyDown(null, GetKeyDownEventArgs(Key.End));
                    Assert.IsTrue(tb.IsFocused);
                }
            }
        }

        private static KeyEventArgs GetKeyDownEventArgs(Key key)
        {
            return new KeyEventArgs(Keyboard.PrimaryDevice, new HwndSource(
                0, 0, 0, 0, 0, string.Empty, IntPtr.Zero), 0, key)
            {
                RoutedEvent = UIElement.KeyDownEvent
            };
        }
    }
}
