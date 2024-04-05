using System.Collections;
using System.Linq.Expressions;

namespace Huffman
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        enum TranslateState
        { 
            TextToCode,
            CodeToText
        }
        TranslateState currentState = TranslateState.TextToCode;

        public Dictionary<string, (int f, string c)> frequencyG;
        Node Tree;
        bool TreeIsExist = false;

        public Dictionary<string, (int, string)> getFriquency(string text)
        {
            if (text == null)
                return null;

            Dictionary<string, (int, string)> freq = new();

            foreach (char c in text)
            {
                string cr = c.ToString();
                if (!freq.ContainsKey(cr.ToString()))
                    freq.Add(cr, (0, "1"));
                freq[cr] = (freq[cr].Item1 + 1, freq[cr].Item2);
            }

            return freq;
        }

        public void AddCode(Node root, string parentCode)
        {
            var left = root.children.left;
            var right = root.children.right;

            if (!root.isParent)
                return;

            if (!left.isParent)
            {
                left.code = parentCode+'0';
                frequencyG[left.symbol] = (left.frequency, left.code);
            }
            else
                AddCode(left, parentCode+'0');

            if (!right.isParent)
            {
                right.code = parentCode+'1';
                frequencyG[right.symbol] = (right.frequency, right.code);
            }
            else
                AddCode(right, parentCode+'1');
        }

        public Node buildTree(Dictionary<string, (int, string)> dict)
        {
            List<Node> nodes = dict.Select(c => new Node(c.Key, c.Value.Item1)).ToList();

            while (nodes.Count != 1)
            {
                Node newRoot = new(nodes[0], nodes[1]);
                nodes[0].SetParent(newRoot);
                nodes[1].SetParent(newRoot);
                try
                {
                    nodes.Insert(nodes.IndexOf(nodes.First(n => n.frequency >= newRoot.frequency)), newRoot);
                }
                catch
                {
                    nodes.Add(newRoot);
                }
                dict.Add(newRoot.symbol, (newRoot.frequency, null));
                nodes.RemoveRange(0, 2);
            }

            AddCode(nodes.First(), null);
            return nodes.First();
        }

        private void buttonMakeTree_Clicked(object sender, EventArgs e)
        {
            string text = BaseInputEditor.Text;
            frequencyG = getFriquency(text);
            Tree = buildTree(frequencyG);
            TreeIsExist = true;

            BindingContext = this;
            TreeView.ItemsSource = frequencyG;
        }

        private string TextToCode(string text)
        {
            string result = string.Empty;
            foreach (var c in text)
            {
                if (!frequencyG.ContainsKey(c.ToString()))
                    continue;
                result += frequencyG[c.ToString()].c;
            }
            return result;
        }
        
        private string CodeToText(string text)
        {
            string result = string.Empty;
            Node current = Tree;
            foreach (char c in text)
            {
                if (c != '0' && c != '1')
                {
                    DisplayAlert("Ошибка!", "Присутствует неверный символ", "Ок");
                    return result;
                }
                current = c == '0' ? current.children.left : current.children.right;
                if (!current.isParent)
                {
                    result += current.symbol;
                    current = Tree;
                }
            }
            return result;
        }

        private void ButtonChange_Clicked(object sender, EventArgs e)
        {
            if (currentState != TranslateState.TextToCode)
            {
                currentState = TranslateState.TextToCode;
                FromButton.Text = "Текст";
                ToButton.Text = "Код";
            }
            else
            {
                currentState = TranslateState.CodeToText;
                FromButton.Text = "Код";
                ToButton.Text = "Текст";
            }
        }

        private void TranslateButton_Clicked(object sender, EventArgs e)
        {
            string inputText = InputEditor.Text;
            string outputText = string.Empty;
            switch (currentState)
            {
                case TranslateState.TextToCode:
                    { 
                        outputText = TextToCode(inputText);
                        break; 
                    }
                case TranslateState.CodeToText: 
                    {
                        outputText = CodeToText(inputText);
                        break; 
                    }
            }
            OutputEditor.Text = outputText;
        }

        private void BaseInputEditor_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (e.NewTextValue == string.Empty)
            {
                buttonMakeTree.IsEnabled = false;
                buttonMakeTree.BackgroundColor = Colors.DimGray;
            }
            else
            {
                buttonMakeTree.IsEnabled = true;
                buttonMakeTree.BackgroundColor = Colors.Green;
            }
        }

        private void InputEditor_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (e.NewTextValue == string.Empty)
            {
                TranslateButton.IsEnabled = false;
                TranslateButton.BackgroundColor = Colors.DimGray;
            }
            else
            {
                TranslateButton.IsEnabled = true;
                TranslateButton.BackgroundColor = Colors.Green;
            }
        }
    }
}
