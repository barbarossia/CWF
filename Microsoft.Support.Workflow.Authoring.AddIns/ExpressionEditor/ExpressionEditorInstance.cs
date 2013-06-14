using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities.Presentation.View;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows;
using System.Collections.ObjectModel;

namespace Microsoft.Support.Workflow.Authoring.ExpressionEditor
{
    /// <summary>
    /// Expression editor instance
    /// </summary>
    public class ExpressionEditorInstance : IExpressionEditorInstance
    {
        private static readonly Key[] commitKeys = new Key[] {
            Key.Space,
            Key.OemMinus,
            Key.OemPlus,
            Key.OemOpenBrackets,
            Key.Oem6, // ]
            Key.Oem5, // \
            Key.Oem1, // ;
            Key.OemQuotes,
            Key.OemComma,
            Key.OemPeriod,
            Key.OemQuestion,
            Key.Divide,
            Key.Multiply,
            Key.Subtract,
            Key.Add,
            Key.Decimal,
        };
        private static readonly Key[] commitKeysWithShift = new Key[] {
            Key.D1,
            Key.D2,
            Key.D3,
            Key.D5,
            Key.D6,
            Key.D7,
            Key.D8,
            Key.D9,
        }.Union(commitKeys).ToArray();
        private TextBox editorTextBox;
        private TreeNode currentMethodNode;
        private int currentMethodInfoIndex;
        private IntellisensePopup popupControl = null;
        private ToolTip methodToolTip
        {
            get
            {
                return editorTextBox.ToolTip as ToolTip;
            }
            set
            {
                editorTextBox.ToolTip = value;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public ExpressionEditorInstance()
        {
            editorTextBox = new TextBox();
            editorTextBox.TextChanged += EditorTextChanged;
            editorTextBox.KeyDown += EditorKeyDown;
            editorTextBox.PreviewKeyDown += EditorKeyPress;
            editorTextBox.LostKeyboardFocus += EditorLostFocus;
        }

        internal TreeNode IntellisenseNodeList { get; set; }
        internal string HighlightWords { get; set; }
        internal Type ExpressionType { get; set; }
        internal Guid Guid { get; set; }

        /// <summary>
        /// Event handler on lost focus
        /// </summary>
        public event EventHandler LostAggregateFocus;
        /// <summary>
        /// Event handler on closing
        /// </summary>
        public event EventHandler Closing;
        /// <summary>
        /// Event handler on got focus
        /// </summary>
        public event EventHandler GotAggregateFocus;
        /// <summary>
        /// Event handler on text changed
        /// </summary>
        public event EventHandler TextChanged;

        public bool AcceptsReturn
        {
            get { return editorTextBox.AcceptsReturn; }
            set { editorTextBox.AcceptsReturn = value; }
        }

        public bool AcceptsTab
        {
            get { return editorTextBox.AcceptsTab; }
            set { editorTextBox.AcceptsTab = value; }
        }

        public bool CanCompleteWord()
        {
            return true;
        }

        public bool CanCopy()
        {
            return true;
        }

        public bool CanCut()
        {
            return true;
        }

        public bool CanDecreaseFilterLevel()
        {
            return false;
        }

        public bool CanGlobalIntellisense()
        {
            return false;
        }

        public bool CanIncreaseFilterLevel()
        {
            return false;
        }

        public bool CanParameterInfo()
        {
            return false;
        }

        public bool CanPaste()
        {
            return true;
        }

        public bool CanQuickInfo()
        {
            return false;
        }

        public bool CanRedo()
        {
            return editorTextBox.CanRedo;
        }

        public bool CanUndo()
        {
            return editorTextBox.CanUndo;
        }

        public void ClearSelection()
        {
        }

        public void Close()
        {
            if (Closing != null)
                Closing(this, new EventArgs());
        }

        public bool CompleteWord()
        {
            return true;
        }

        public bool Copy()
        {
            return true;
        }

        public bool Cut()
        {
            return true;
        }

        public bool DecreaseFilterLevel()
        {
            return false;
        }

        public void Focus()
        {
            editorTextBox.Focus();
            editorTextBox.SelectionStart = Text.Length;

            if (GotAggregateFocus != null)
                GotAggregateFocus(this, new EventArgs());
        }

        public string GetCommittedText()
        {
            return editorTextBox.Text;
        }

        public bool GlobalIntellisense()
        {
            return false;
        }

        public bool HasAggregateFocus
        {
            get { return true; }
        }

        public ScrollBarVisibility HorizontalScrollBarVisibility
        {
            get { return editorTextBox.HorizontalScrollBarVisibility; }
            set { editorTextBox.HorizontalScrollBarVisibility = value; }
        }

        public Control HostControl
        {
            get { return editorTextBox; }
        }

        public bool IncreaseFilterLevel()
        {
            return false;
        }

        public int MaxLines
        {
            get { return editorTextBox.MaxLines; }
            set { editorTextBox.MaxLines = value; }
        }

        public int MinLines
        {
            get { return editorTextBox.MinLines; }
            set { editorTextBox.MinLines = value; }
        }

        public bool ParameterInfo()
        {
            return false;
        }

        public bool Paste()
        {
            return true;
        }

        public bool QuickInfo()
        {
            return false;
        }

        public bool Redo()
        {
            return true;
        }

        public string Text
        {
            get { return editorTextBox.Text; }
            set { editorTextBox.Text = value; }
        }

        public bool Undo()
        {
            return true;
        }

        public ScrollBarVisibility VerticalScrollBarVisibility
        {
            get { return editorTextBox.VerticalScrollBarVisibility; }
            set { editorTextBox.VerticalScrollBarVisibility = value; }
        }

        public void EditorTextChanged(object sender, TextChangedEventArgs e)
        {
            if (popupControl != null && popupControl.IsOpen)
            {
                UpdatePopup();
            }
            else
            {
                if (methodToolTip != null && methodToolTip.IsOpen)
                {
                    string searchText = GetSearchText();
                    if (!searchText.Contains("("))
                        CloseMethodToolTip();
                }
            }

            if (TextChanged != null)
                TextChanged(this, e);
        }

        private void CloseMethodToolTip()
        {
            if (methodToolTip != null)
            {
                if (methodToolTip.IsOpen)
                {
                    methodToolTip.IsOpen = false;
                    methodToolTip = null;
                }
            }
            currentMethodNode = null;
        }

        private void UpdatePopup()
        {
            string searchText = GetSearchText();
            List<TreeNode> searchList = FindMatches(IntellisenseNodeList, searchText);
            if (string.IsNullOrEmpty(searchText) || searchList == null || searchList.Count == 0)
            {
                popupControl.IsOpen = false;
            }
            else
            {
                popupControl.ViewModel.TreeNodes = new ObservableCollection<TreeNode>(searchList);
                popupControl.ViewModel.SelectedItem = FindBestMatch(SplitByDot(searchText).Last(), searchList);
            }
        }

        private TreeNode FindBestMatch(string text, List<TreeNode> nodes)
        {
            return nodes.FirstOrDefault(n => n.Name.StartsWith(text, StringComparison.OrdinalIgnoreCase)) ??
                nodes.FirstOrDefault(n => n.IsMatch(text));
        }

        public void EditorKeyDown(object sender, KeyEventArgs e)
        {
            if ((!AcceptsReturn && e.Key == Key.Enter && Keyboard.Modifiers == ModifierKeys.None) ||
            (!AcceptsTab && e.Key == Key.Tab && Keyboard.Modifiers == ModifierKeys.None))
            {
                e.Handled = true;

                var request = new TraversalRequest(FocusNavigationDirection.Next);
                var focusedElement = GetFocusedElement();
                if (focusedElement != null)
                {
                    focusedElement.MoveFocus(request);
                }
            }
        }

        public void EditorKeyPress(object sender, KeyEventArgs e)
        {
            if ((popupControl == null) || (!popupControl.IsOpen))
            {
                bool isFirstLetter = GetSearchText().Length == 0
                    && !Keyboard.Modifiers.HasFlag(ModifierKeys.Control)
                    && ((e.Key == Key.OemMinus && Keyboard.Modifiers.HasFlag(ModifierKeys.Shift))// "_"
                        || (e.Key >= Key.A && e.Key <= Key.Z));
                bool shouldPopup = (Keyboard.Modifiers.HasFlag(ModifierKeys.Control)
                    && e.Key == Key.Space) || (e.Key == Key.Back && !string.IsNullOrEmpty(editorTextBox.Text)) || isFirstLetter || IsDotPressed(e);

                if (shouldPopup && !IsInString(Text, editorTextBox.SelectionStart))
                    InitializePopup(IntellisenseNodeList.Nodes);
            }
            else // popup control is open
            {
                OnKeyDownInPopup(e);
                if (commitKeys.Contains(e.Key) ||
                    (commitKeysWithShift.Contains(e.Key) && Keyboard.Modifiers.HasFlag(ModifierKeys.Shift)))
                {
                    if (IsDotPressed(e))
                        InitializePopup(IntellisenseNodeList.Nodes);
                    e.Handled = false;
                }
            }

            if (e.Key == Key.D9 && Keyboard.Modifiers == ModifierKeys.Shift) // when key "(" press
            {
                string inputText = editorTextBox.Text;
                string searchWord = SplitBySpace(inputText)[1];
                TreeNode node = SearchMethodNode(searchWord);
                if (node != null)
                    ShowMethodInfo(node);
            }

            if (e.Key == Key.D0 && Keyboard.Modifiers == ModifierKeys.Shift) // when key ")" press
                CloseMethodToolTip();

            if (methodToolTip != null && methodToolTip.IsOpen
                && currentMethodNode != null && currentMethodNode.HasOverrideMethods)
            {
                if (e.Key == Key.Up) //navigate among override methods by up key
                {
                    methodToolTip.Content = currentMethodNode.GetMethodDescriptionAt(--currentMethodInfoIndex);
                }
                else if (e.Key == Key.Down) //navigate among override methods by down key
                {
                    methodToolTip.Content = currentMethodNode.GetMethodDescriptionAt(++currentMethodInfoIndex);
                }
            }
        }

        private bool IsDotPressed(KeyEventArgs e)
        {
            return (e.Key == Key.OemPeriod) || (e.Key == Key.Decimal);
        }

        private bool IsInString(string text, int position)
        {
            int quotesCount = text.Substring(0, position).Count(c => c == '"');
            return (quotesCount % 2) != 0;
        }

        private void ShowMethodInfo(TreeNode node)
        {
            CloseMethodToolTip();

            methodToolTip = new ToolTip
            {
                Content = node.Description,
                Placement = PlacementMode.Right,
                PlacementTarget = editorTextBox
            };
            methodToolTip.IsOpen = true;
            currentMethodNode = node;
            currentMethodInfoIndex = 0;
        }

        private TreeNode SearchMethodNode(string inputText)
        {
            if (string.IsNullOrEmpty(inputText))
                return null;

            string prefixText = SplitByDot(inputText)[0];
            var prefixMatches = PrefixMatch(IntellisenseNodeList, prefixText);
            if (prefixMatches == null)
                return null;
            var existNode = from node in prefixMatches
                            where node.GetFullPath() == inputText && node.ItemType == TreeNodeType.Method
                            select node;
            return existNode.SingleOrDefault();
        }

        private void OnKeyDownInPopup(KeyEventArgs e)
        {
            if (e.Key == Key.Up)
            {
                popupControl.ViewModel.SelectedIndex--;
                e.Handled = true;
            }
            else if (e.Key == Key.Down)
            {
                popupControl.ViewModel.SelectedIndex++;
                e.Handled = true;
            }
            else if (e.Key == Key.Home)
            {
                popupControl.ViewModel.SelectedIndex = 0;
                e.Handled = true;
            }
            else if (e.Key == Key.End)
            {
                popupControl.ViewModel.SelectedIndex = popupControl.ViewModel.ItemsCount - 1;
                e.Handled = true;
            }
            else if (e.Key == Key.Escape)
            {
                popupControl.IsOpen = false;
                e.Handled = true;
            }
            else if ((e.Key == Key.Enter) || (e.Key == Key.Tab)
                || commitKeys.Contains(e.Key)
                || (commitKeysWithShift.Contains(e.Key) && Keyboard.Modifiers.HasFlag(ModifierKeys.Shift)))
            {
                TreeNode node = popupControl.ViewModel.SelectedItem;
                if (node != null)
                    CommitIntellisenseItem(node);
                e.Handled = true;
            }
        }

        public void EditorLostFocus(object sender, EventArgs e)
        {
            var popupItem = this.GetFocusedElement() as ListBoxItem;
            if (popupItem == null)
            {
                if ((popupControl != null) && (popupControl.IsOpen))
                {
                    UninitializePopup();
                }
                CloseMethodToolTip();
                if (LostAggregateFocus != null)
                    LostAggregateFocus(sender, e);
            }
        }

        public void ListKeyDown(object sender, KeyEventArgs e)
        {
            OnKeyDownInPopup(e);
            if (e.Handled)
                Focus();
        }

        public void ListItemDoubleClick(object sender, EventArgs e)
        {
            ListBoxItem item = sender as ListBoxItem;
            if (item == null)
                return;

            TreeNode node = item.DataContext as TreeNode;
            if (node == null)
                return;

            Focus();
            CommitIntellisenseItem(node);
        }

        private void CommitIntellisenseItem(TreeNode selectedNodes)
        {
            string prefix = SplitByDot(SplitBySpace(Text).Last()).First();
            string inputText = string.Format("{0}{1}{2}{3}",
                SplitBySpace(Text).First(), prefix, string.IsNullOrEmpty(prefix) ? string.Empty : ".", selectedNodes.Name);
            editorTextBox.Text = inputText;
            editorTextBox.SelectionStart = Text.Length;
            editorTextBox.UpdateLayout();
            UninitializePopup();
        }

        private string GetSearchText()
        {
            return SplitBySpace(Text).Last();
        }

        private string[] SplitBySpace(string inputText)
        {
            int spacePos = inputText.LastIndexOf(" ");
            return new string[] {
                inputText.Substring(0, spacePos + 1),
                inputText.Substring(spacePos + 1, inputText.Length - spacePos - 1)
            };
        }

        private string[] SplitByDot(string inputText)
        {
            int dotPos = inputText.LastIndexOf(".");
            return new string[] {
                dotPos >= 0 ? inputText.Substring(0, dotPos) : string.Empty,
                inputText.Substring(dotPos + 1, inputText.Length - dotPos - 1)
            };
        }

        private UIElement GetFocusedElement()
        {
            return Keyboard.FocusedElement as UIElement;
        }

        private void InitializePopup(List<TreeNode> source)
        {
            popupControl = new IntellisensePopup();
            popupControl.ViewModel.TreeNodes = new ObservableCollection<TreeNode>(source);
            popupControl.PlacementTarget = editorTextBox;
            popupControl.Placement = PlacementMode.Bottom;

            popupControl.ListBoxKeyDown += ListKeyDown;
            popupControl.ListBoxItemDoubleClick += ListItemDoubleClick;

            popupControl.IsOpen = true;
        }

        private void UninitializePopup()
        {
            popupControl.ListBoxKeyDown -= ListKeyDown;
            popupControl.ListBoxItemDoubleClick -= ListItemDoubleClick;

            if (popupControl.IsOpen)
                popupControl.IsOpen = false;

            popupControl = null;
        }

        private List<TreeNode> FindMatches(TreeNode targetNodes, string namePath)
        {
            if (string.IsNullOrEmpty(namePath))
                return targetNodes.Nodes;

            string[] inputs = SplitByDot(namePath);
            string prefix = inputs[0];
            string searchWord = inputs[1];

            List<TreeNode> children = PrefixMatch(targetNodes, prefix);
            if (string.IsNullOrEmpty(searchWord))
                return children;
            else
                return LastWordMatch(children, searchWord);
        }

        private List<TreeNode> PrefixMatch(TreeNode targetNodes, string namePath)
        {
            if (namePath.Contains('(') && namePath.Contains(')'))
            {
                namePath = namePath.Substring(0, namePath.IndexOf('('));
            }
            return GetChildren(string.IsNullOrEmpty(namePath) ? targetNodes : ExpressionEditorHelper.SearchNodes(targetNodes, namePath));
        }

        private List<TreeNode> LastWordMatch(List<TreeNode> nodes, string lastWord)
        {
            if (nodes == null)
                return null;

            return nodes.Where(n => n.IsMatch(lastWord)).ToList();
        }

        private List<TreeNode> GetChildren(TreeNode node)
        {
            if (node == null)
            {
                return null;
            }

            if (node.Nodes.Any())
            {
                return node.Nodes;
            }

            List<TreeNode> nodes = new List<TreeNode>();
            if (node.SystemType != null)
            {
                TreeNode n = ExpressionEditorHelper.SearchNodes(IntellisenseNodeList, node.SystemType.FullName);
                if (n != null)
                {
                    nodes = n.Nodes;
                }
            }
            return nodes;
        }
    }
}
