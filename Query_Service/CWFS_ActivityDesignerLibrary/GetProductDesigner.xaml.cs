using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.Activities.Presentation.View;
using System.Activities.Presentation.Converters;
namespace Microsoft.Support.Workflow.Activity.Design
{
    // Interaction logic for GetProductDesigner.xaml
    public partial class GetProductDesigner
    {

        ExpressionTextBox ExpBox4menuText = new ExpressionTextBox();
        List<TextBox> menuList;
        string menuText;
        
        public GetProductDesigner()
        {
            InitializeComponent();
            menuList = new List<TextBox>();

            menuList.Add(Title);
           // menuList.Add(Q1);
           // menuList.Add(Q2);
            setLostFocusHandler();

            Binding bnd = new Binding();

            bnd.Path = new PropertyPath("ModelItem.menuTxt");
            bnd.Mode = BindingMode.TwoWay;
            bnd.Converter = new ArgumentToExpressionConverter();
            bnd.ConverterParameter = "In";

            ExpBox4menuText.SetBinding(ExpressionTextBox.ExpressionProperty, bnd);
            ExpBox4menuText.ExpressionType = typeof(string);
            //ExpBox4menuText.OwnerActivity=
            //ExpBox4menuText.Expression.Properties["ExpressionText"].ComputedValue = "DefaultText";
               
            
        }

        /// <summary>
        /// add A-,B- and so on to FinalText string.
        /// Ex: A-Question1,B-Question2,C-Question3...
        /// </summary>
        /// <param name="menuL"></param>
        /// <returns></returns>
        private string constructMenuText(List<TextBox> menuL)
        {
           string str;
           int curChar = 65;
            
            str ="Title-"+ menuL[0].Text+";";
            
            foreach (TextBox item in menuL)
	        {
                if(item != menuL[0])
                {
                str = str+ char.ConvertFromUtf32(curChar)+"-"+ item.Text + ",";
                curChar = curChar + 1;
                }
	        }
            str = str + ";";

            return str;
        }

        /// <summary>
        /// Adds a new TextBox adds item_LostFocus handler TextBox-elements in the List<name="menuList"/>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addButton_Click(object sender, RoutedEventArgs e)
        {

            ////add new TextBox
            TextBox txtbx = new TextBox();
            menuList.Add(txtbx);
            menuItemsStackPanel.Children.Insert(menuItemsStackPanel.Children.Count-1,txtbx);

            //Assign LostFocus Handle to each TextBox
            setLostFocusHandler();
            MessageBox.Show(menuText);

            ////Refresh Grid.
            ////menuList.Clear();  

            #region TestCode
            //string str = "";
            //foreach (TextBox item in menuList)
            //{
            //    str = str + item.Text;

            //}
            //Q1.Text = Q1.Text + str;

            #endregion
        }


        private void setLostFocusHandler()
        {
            foreach (TextBox item in menuList)
            {
                //item.Text = "inSetfocus";
                if(string.IsNullOrEmpty(item.Text))
                item.LostFocus += new RoutedEventHandler(item_LostFocus);
            }

        }

        /// <summary>
        /// A SIngle-LostFocus event handler to all in TextBoxes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void item_LostFocus(object sender, RoutedEventArgs e)
        {
            menuText = constructMenuText(menuList);
            //MessageBox.Show(menuText);
            //TestBlock.Text = menuText;
            //ExpBox4menuText.Content = str;

        }

        private void Title_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("Present a list of filter options?");
        }

        private void Title_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Title_Drop(object sender, DragEventArgs e)
        {
            
        }
    }
}
