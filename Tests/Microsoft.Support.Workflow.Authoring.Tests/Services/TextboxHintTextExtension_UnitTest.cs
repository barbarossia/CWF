using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Support.Workflow.Authoring.Services;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using Microsoft.DynamicImplementations;

namespace Microsoft.Support.Workflow.Authoring.Tests.Services
{
    [TestClass]
    public class TextboxHintTextExtension_UnitTest
    {

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        [TestMethod]
        [TestCategory("Unit")]
        [Owner("v-kason")]
        public void TestTextboxHintTextExtension()
        {
            TextBox tb = new TextBox();
            TextboxHintTextExtension thte = new TextboxHintTextExtension(tb, "hint");
            string value = thte.GetValue(TextboxHintTextExtension.TextProperty) as string;
            Assert.AreEqual(tb.Text, value);
        }

        /// <summary>
        ///A test for OnRender
        ///</summary>
        [TestMethod()]
        [TestCategory("Unit")]
        [Owner("v-kason")]
        [DeploymentItem("Microsoft.Support.Workflow.Authoring.exe")]
        public void OnRenderTest()
        {
            TextBox tb = new TextBox();
            
            TextboxHintTextExtension thte = new TextboxHintTextExtension(tb,"hint");
            
            PrivateObject param0 = new PrivateObject(thte);
            
            TextboxHintTextExtension_Accessor target = new TextboxHintTextExtension_Accessor(param0);
            
            DrawingVisual dv = new DrawingVisual();
            
            using (DrawingContext drawingContext = dv.RenderOpen())
            {
                try
                {
                    tb.Text = "test";
                    target.OnRender(drawingContext);
                    Assert.IsFalse(target.isVisible);

                    tb.Text = "";
                    tb.RenderSize = new Size(10, 10);
                    target.OnRender(drawingContext);
                    
                }
                catch (Exception e)//drawingContext.DrawText() operation is invalid for test project
                {
                    Assert.IsTrue(target.isVisible);
                    Assert.IsTrue(e is System.InvalidProgramException);
                }
            }
        }

        /// <summary>
        ///A test for OnTextChanged
        ///</summary>
        [TestMethod()]
        [TestCategory("Unit")]
        [Owner("v-kason")]
        [DeploymentItem("Microsoft.Support.Workflow.Authoring.exe")]
        public void OnTextChangedTest()
        {
                TextBox sender = new TextBox();
                sender.Text = "test";

                DependencyPropertyChangedEventArgs e = 
                    new DependencyPropertyChangedEventArgs(TextboxHintTextExtension_Accessor.TextProperty, "new", "old"); 
                
                TextboxHintTextExtension_Accessor.OnTextChanged(sender, e);
                Assert.AreEqual(sender.Text, "test");

                e = new DependencyPropertyChangedEventArgs(TextboxHintTextExtension_Accessor.TextProperty, "same", "same");

                sender.Text = "";
                TextboxHintTextExtension_Accessor.OnTextChanged(sender, e);
                Assert.AreEqual(sender.Text, "");
        }

        /// <summary>
        ///A test for TextboxContentChanged
        ///</summary>
        [TestMethod()]
        [TestCategory("Unit")]
        [Owner("v-kason")]
        [DeploymentItem("Microsoft.Support.Workflow.Authoring.exe")]
        public void TextboxContentChangedTest()
        {
            TextBox tb = new TextBox();
            using (var args = new Implementation<TextboxHintTextExtension>())
            {
                bool isChanged = false;
                tb.Text = "";
                args.Register(inst => inst.InvalidateVisual()).Execute(() => { isChanged = true; });

                TextboxHintTextExtension thte = args.Instance;

                PrivateObject param0 = new PrivateObject(thte); ;
                TextboxHintTextExtension_Accessor target = new TextboxHintTextExtension_Accessor(param0); 
                object sender = tb; 
                RoutedEventArgs e = new RoutedEventArgs();
                target.TextboxContentChanged(sender, e);
                Assert.IsTrue(isChanged);

                tb.Text = "NotNUll";
                target.TextboxContentChanged(sender, e);
                Assert.IsTrue(isChanged);
            }
        }

        /// <summary>
        ///A test for TextboxGotFocus
        ///</summary>
        [TestMethod()]
        [TestCategory("Unit")]
        [Owner("v-kason")]
        [DeploymentItem("Microsoft.Support.Workflow.Authoring.exe")]
        public void TextboxGotFocusTest()
        {
            TextBox tb = new TextBox();
            using (var args = new Implementation<TextboxHintTextExtension>())
            {
                bool isChanged = false;
                tb.Text = "";
                args.Register(inst => inst.InvalidateVisual()).Execute(() => { isChanged = true; });

                TextboxHintTextExtension thte = args.Instance;

                PrivateObject param0 = new PrivateObject(thte); ;
                TextboxHintTextExtension_Accessor target = new TextboxHintTextExtension_Accessor(param0);
                object sender = tb;
                RoutedEventArgs e = new RoutedEventArgs();
                target.isVisible = true;
                target.TextboxGotFocus(sender, e);
                Assert.AreEqual(isChanged,target.isVisible);
            }
        }

        /// <summary>
        ///A test for TextboxLostFocus
        ///</summary>
        [TestMethod()]
        [TestCategory("Unit")]
        [Owner("v-kason")]
        [DeploymentItem("Microsoft.Support.Workflow.Authoring.exe")]
        public void TextboxLostFocusTest()
        {
            TextBox tb = new TextBox();
            using (var args = new Implementation<TextboxHintTextExtension>())
            {
                bool isChanged = false;
                tb.Text = "";
                args.Register(inst => inst.InvalidateVisual()).Execute(() => { isChanged = true; });

                TextboxHintTextExtension thte = args.Instance;

                PrivateObject param0 = new PrivateObject(thte); ;
                TextboxHintTextExtension_Accessor target = new TextboxHintTextExtension_Accessor(param0);
                object sender = tb;
                RoutedEventArgs e = new RoutedEventArgs();
                target.isVisible = true;
                tb.Text = "NotNUll";
                target.TextboxLostFocus(sender, e);
                Assert.IsFalse(isChanged);

                target.isVisible = false;
                tb.Text = "";
                target.TextboxLostFocus(sender, e);
                Assert.IsTrue(isChanged);
            }
        }

    }
}
