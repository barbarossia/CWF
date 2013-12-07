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

using Microsoft.Support.Workflow.Context.HelperCalsses;
using System.Diagnostics;
using System.Activities;
namespace Microsoft.Support.Workflow.Activity
{
    // Interaction logic for DisplayMenuDesigner.xaml
    public partial class DisplayMenuDesigner
    {
        ExpressionTextBox ExpBox4menuText = new ExpressionTextBox();
        List<TextBox> menuList;
        string menuText ;
        string IsBranchSelected = "IsBranchSelected";
        InArgument<bool> ArgIsChecked;
        InArgument<string> ArgMenuText; 
        //ArgIsChecked = new InArgument<bool> { Expression = false };
        //ArgMenuText = new InArgument<string> { Expression = menuText }; 
        public bool HasUserInteraction { get; set; }

        public DisplayMenuDesigner()
        {

            InitializeComponent();
            menuList = new List<TextBox>();
            menuList.Add(Title);
            menuList.Add(Q1);
            menuList.Add(Q2);
            foreach (TextBox item in menuList)
            {
                setLostFocusHandler(item);

            }
               ArgIsChecked  = new InArgument<bool> { Expression = false };
               ArgMenuText = new InArgument<string> { Expression = "" };


            //bind twoway to ModelItem's IsMultipleSelection prop.
            Binding bnd = new Binding();
            bnd.Mode = BindingMode.TwoWay;
            bnd.Converter = new ArgumentToExpressionConverter();
            bnd.ConverterParameter = "In";

            //bnd.Source= chkMulti;
            //bnd.Path = new PropertyPath(chkMulti.IsChecked);

            this.Loaded+=new RoutedEventHandler(DisplayMenuDesigner_Loaded);
            

        }

        void DisplayMenuDesigner_Loaded(object sender, EventArgs e)
        {
            
            if(!(this.ModelItem.Properties["IsMultipleSelection"].ComputedValue == null))
                chkMulti.IsChecked = Convert.ToBoolean(((InArgument<bool>)this.ModelItem.Properties["IsMultipleSelection"].ComputedValue).Expression.ToString());

            if (!(this.ModelItem.Properties[IsBranchSelected].ComputedValue == null))
            chkBranch.IsChecked = Convert.ToBoolean(((InArgument<bool>)this.ModelItem.Properties[IsBranchSelected].ComputedValue).Expression.ToString());

            if (!(this.ModelItem.Properties["menuTxt"].ComputedValue == null))
                fillMenuTxtBoxex(((InArgument<string>)this.ModelItem.Properties["menuTxt"].ComputedValue).Expression.ToString());

            //Helper.Log("DisplayMenuDesigner_Loaded-" + ((InArgument<string>)this.ModelItem.Properties["menuTxt"].ComputedValue).Expression, true);
        
        }


        // given Menu-string add textboxes to the ItemsMenu.
        private void fillMenuTxtBoxex(string str)
        {


            //validaate the string.
            
            //instatiate QnA types
            Microsoft.Support.Workflow.Context.HelperClasses.MenuClasses.QnA qna = new Context.HelperClasses.MenuClasses.QnA(str);

            // fill existing text boxes
            Title.Text = qna.title.Substring(6);
            Q1.Text = qna._Q[0].Substring(2);
            Q2.Text = qna._Q[1].Substring(2);

            //Helper.Log("fillMenuTxtBoxex-" + qna._Q.Count.ToString(), true);

            // generate other textboxes dynamically.
            if (qna._Q.Count > 2)
            {
                for (int i=2; i<qna._Q.Count-1; i++)
                {
                    TextBox txtBx = new TextBox();
                    setLostFocusHandler(txtBx);
                    txtBx.Text = qna._Q[i].Substring(2);
                    menuItemsStackPanel.Children.Add(txtBx);
                }
            }
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
           int curChar = 1;
            
            str ="Title-"+ menuL[0].Text+";";
            
            foreach (TextBox item in menuL)
	        {
                if(item != menuL[0])
                {
                        if(item == menuL[menuL.Count-1])
                            str = str+ Convert.ToString( curChar)+"-"+ item.Text ;
                        else
                            str = str + Convert.ToString(curChar) + "-" + item.Text + ",";

                        curChar = curChar + 1;
                    
                }
	        }
           // str = str + ";";

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
            menuItemsStackPanel.Children.Insert(menuItemsStackPanel.Children.Count , txtbx);

            //Assign LostFocus Handle to each TextBox
            setLostFocusHandler(txtbx);

            //MessageBox.Show(menuText);
            //Helper.Log("AddNewTextBox-" + ((InArgument<string>)this.ModelItem.Properties["menuTxt"].ComputedValue).Expression, true);


            #region TestCode
            //string str = "";
            //foreach (TextBox item in menuList)
            //{
            //    str = str + item.Text;

            //}
            //Q1.Text = Q1.Text + str;

            #endregion
        }


        private void setLostFocusHandler( TextBox txtBox)
        {
            //foreach (TextBox item in menuList)
            //{
            //    //item.Text = "inSetfocus";
                //if(string.IsNullOrEmpty(item.Text))
                 txtBox.LostFocus += new RoutedEventHandler(item_LostFocus);
                 //Helper.Log("addSetFocusHandler-" + ((InArgument<string>)this.ModelItem.Properties["menuTxt"].ComputedValue).Expression, true);


            //}

        }
        /// <summary>
        /// A SIngle-LostFocus event handler to all in TextBoxes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void item_LostFocus(object sender, RoutedEventArgs e)
        {
            //get textboxes in stackpanel and create menustring. 
            menuText = constructMenuText(getMenuTextBoxList(menuItemsStackPanel.Children));
            
            //menuText = constructMenuText(menuList);
            
            //assing string to ModelItem 'menuText' Property
            this.ModelItem.Properties["menuTxt"].SetValue(new InArgument<string>() { Expression=menuText});
            //Helper.Log("LostFocusEvent-" + ((InArgument<string>) this.ModelItem.Properties["menuTxt"].ComputedValue).Expression, true);
            //Helper.Log("LostFocusEvent-stackpanel" + constructMenuText(getMenuTextBoxList(menuItemsStackPanel.Children)), true);

        }


        private void chkMulti_Checked(object sender, RoutedEventArgs e)
        {
            this.ModelItem.Properties["IsMultipleSelection"].SetValue(new InArgument<bool>() { Expression = chkMulti.IsChecked });
            //Helper.Log("Checked-" + ((InArgument<bool>)this.ModelItem.Properties["IsMultipleSelection"].ComputedValue).Expression, true);
        }
        private void chkMulti_Unchecked(object sender, RoutedEventArgs e)
        {
            this.ModelItem.Properties["IsMultipleSelection"].SetValue(new InArgument<bool>() { Expression = chkMulti.IsChecked });
            //Helper.Log("Unchecked-" + ((InArgument<bool>)this.ModelItem.Properties["IsMultipleSelection"].ComputedValue).Expression, true);
        }


        private void refreshButton_Click(object sender, RoutedEventArgs e)
        {
            //Helper.Log("refreshButton_Click-" + ((InArgument<string>)this.ModelItem.Properties["menuTxt"].ComputedValue).Expression, true);
            if (menuItemsStackPanel.Children.Count > 2)
                menuItemsStackPanel.Children.RemoveRange(2, menuItemsStackPanel.Children.Count - 2);

            //Helper.Log("refreshButton_Click-Count" + menuItemsStackPanel.Children.Count.ToString(), true);
            fillMenuTxtBoxex(((InArgument<string>)this.ModelItem.Properties["menuTxt"].ComputedValue).Expression.ToString());

        }

        //return List of MenuTextBox in StackPanel
        private List<TextBox> getMenuTextBoxList(UIElementCollection UIColl)
        {
            List<TextBox> lst = new List<TextBox>();
            lst.Add(Title);
            foreach (UIElement item in UIColl)
	        {
                //check if 'item' is of TextBox type
                
                //add to list
                lst.Add(item as TextBox );
	        }
            return lst;
        }

        private void chkBranch_Checked(object sender, RoutedEventArgs e)
        {
            this.ModelItem.Properties[IsBranchSelected].SetValue(new InArgument<bool>() { Expression = chkBranch.IsChecked });
        }

        private void chkBranch_Unchecked(object sender, RoutedEventArgs e)
        {
            this.ModelItem.Properties[IsBranchSelected].SetValue(new InArgument<bool>() { Expression = chkBranch.IsChecked });
        }


    }
}
