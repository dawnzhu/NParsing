using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace DotNet.Standard.Common.Text
{
    /// <summary>
    /// Aho-Corasick算法实现
    /// </summary>
    public class Searching
    {
        private readonly string[] _keywords; // 所有关键词
        private static string _wildcardRegex; //通配符正则表达式
        private Node _root; // 根节点

        public Searching(string[] keywords, string wildcardRegex)
        {
            _keywords = keywords;
            _wildcardRegex = wildcardRegex;
            Initialize();
        }

        /// <summary>
        /// 根据关键词来初始化所有节点
        /// </summary>
        private void Initialize()
        {
            _root = new Node(' ', null, false);

            // 添加模式
            foreach (string k in _keywords)
            {
                if(k.Length == 0)
                    continue;
                if (k[0] == '*' || k[0] == '?' ||
                    k[k.Length - 1] == '*' || k[k.Length - 1] == '?')
                    continue;
                
                Node n = _root;
                for(var i = 0; i<k.Length; i++)
                {
                    var c = k[i];
                    Node temp = null;
                    foreach (Node tnode in n.Transitions)
                    {
                        if (tnode.Char == c)
                        {
                            temp = tnode; break;
                        }
                    }

                    if (temp == null)
                    {
                        temp = new Node(c, n, i == 0);
                        n.AddTransition(temp);
                    }
                    n = temp;
                }
                n.AddResult(k);
            }
            
            // 第一层失败指向根节点
            var nodes = new List<Node>();
            foreach (Node node in _root.Transitions)
            {
                // 失败指向root
                node.Failure = _root;
                nodes.AddRange(node.Transitions);
            }
            // 其它节点 BFS
            while (nodes.Count != 0)
            {
                var newNodes = new List<Node>();
                foreach (Node nd in nodes)
                {
                    Node r = nd.Parent.Failure;
                    char c = nd.Char;

                    while (r != null && !r.ContainsTransition(c))
                    {
                        r = r.Failure;
                    }

                    if (r == null)
                    {
                        // 失败指向root
                        nd.Failure = _root;
                    }
                    else
                    {
                        nd.Failure = r.GetTransition(c);
                        nd.IsStart = nd.Failure.IsStart;
                        foreach (string result in nd.Failure.Results)
                        {
                            nd.AddResult(result);
                        }
                    }

                    newNodes.AddRange(nd.Transitions);
                }
                nodes = newNodes;
            }
            // 根节点的失败指向自己
            _root.Failure = _root;
        }

        /// <summary>
        /// 找出所有出现过的关键词
        /// </summary>
        /// <param name="text">原文</param>
        /// <param name="separator">返回关键词分隔符</param>
        /// <returns>原文中所有出现过关键词的组合串</returns>
        public string Find(string text, char separator)
        {
            var sb = new StringBuilder();
            var retsb = new StringBuilder();
            Node current = _root;
            var iList = new List<int>();//*号匹配长度
            var lList = new List<int>();//开始当前位置
            for (var m = 0; m < text.Length; m++)
            {
                var t = text[m];
                //将全角转成半角
                if (t == 12288)
                    t = (char)32;
                if (t > 65280 && t < 65375)
                    t= (char)(t - 65248);

                Node trans;
                do
                {
                    trans = current.GetTransition(t);
                    if (current == _root || (trans != null && trans.IsStart))
                    {
                        iList.Add(0);
                        lList.Add(sb.Length);
                        break;
                    }

                    if (trans == null)
                    {
                        current = current.Failure;
                    }
                } while (trans == null);

                if (trans != null)
                {
                    if (trans.Char == '*')
                    {
                        for (int i = 0; i < iList.Count; i++)
                        {
                            iList[i]++;
                        }
                    }
                    else
                    {
                        current = trans;
                    }
                }
                // 处理字符
                if (current.Results.Count > 0)
                {
                    foreach (var first in current.Results)
                    {
                        int c = 0; //*号个数
                        foreach (var b in first)
                        {
                            if (b == '*') c++;
                        }
                        int j = 0;
                        bool bb = false;
                        for (int k = iList.Count - 1; k > -1; k--)
                        {
                            var i = iList[k];
                            if (sb.Length - (first.Length - c + i) + 1 == lList[k])
                            {
                                j = k;
                                bb = true;
                                break;
                            }
                        }
                        if (!bb)
                            continue;
                        for (int k = iList.Count - 1; k > j; k--)
                        {
                            iList.RemoveAt(k);
                            lList.RemoveAt(k);
                        }
                        if (iList.Count == 0)
                            break;
                        if (retsb.Length != 0)
                            retsb.Append(separator);
                        var ii = iList[iList.Count - 1];
                        retsb.Append(sb.ToString().Substring(sb.Length - (first.Length - c + ii) + 1, (first.Length - c + ii) - 1));
                        retsb.Append(t);
                        if (current.Transitions.Count == 0)
                        {
                            iList.RemoveAt(iList.Count - 1);
                            lList.RemoveAt(lList.Count - 1);
                        }
                    }
                }
                sb.Append(text[m]);
            }
            return retsb.ToString();
        }

        /// <summary>
        /// 过虑关键词
        /// </summary>
        /// <param name="text">原文</param>
        /// <returns>将关键词替换成*的文本</returns>
        public string Filter(string text)
        {
            var sb = new StringBuilder();
            Node current = _root;
            var iList = new List<int>();
            var lList = new List<int>();
            for (var m = 0; m < text.Length; m++)
            {
                var t = text[m];
                //将全角转成半角
                if (t == 12288)
                    t = (char)32;
                if (t > 65280 && t < 65375)
                    t = (char)(t - 65248);

                Node trans;
                do
                {
                    trans = current.GetTransition(t);
                    if (current == _root || (trans != null && trans.IsStart))
                    {
                        iList.Add(0);
                        lList.Add(sb.Length );
                        break;
                    }
                    if (trans == null)
                    {
                        current = current.Failure;
                    }
                } while (trans == null);

                if (trans != null)
                {
                    if (trans.Char == '*')
                    {
                        for (int i = 0; i < iList.Count; i++)
                        {
                            iList[i]++;
                        }
                    }
                    else
                    {
                        current = trans;
                    }
                }
                // 处理字符
                if (current.Results.Count > 0)
                {
                    foreach (var first in current.Results)
                    {
                        int c = 0; //*号个数
                        foreach (var b in first)
                        {
                            if (b == '*') c++;
                        }
                        int j = 0;
                        bool bb = false;
                        for (int k = iList.Count - 1; k > -1; k--)
                        {
                            var i = iList[k];
                            if (sb.Length - (first.Length - c + i) + 1 == lList[k])
                            {
                                j = k;
                                bb = true;
                                break;
                            }
                        }
                        if(!bb)
                            continue;
                        for (int k = iList.Count-1; k > j; k--)
                        {
                            iList.RemoveAt(k);
                            lList.RemoveAt(k);
                        }
                        if(iList.Count == 0)
                            break;
                        var ii = iList[iList.Count - 1];
                        sb.Remove(sb.Length - (first.Length - c + ii) + 1, (first.Length - c + ii) - 1); // 把匹配到的替换为**
                        sb.Append(new string('*', first.Length - c + ii));
                        if (current.Transitions.Count == 0)
                        {
                            iList.RemoveAt(iList.Count - 1);
                            lList.RemoveAt(lList.Count - 1);
                        }
                    }
                }
                else
                {
                    sb.Append(text[m]);
                }
            }
            return sb.ToString();
        }

        #region Nested type: Node

        /// <summary>
        /// 构造节点
        /// </summary>
        private class Node
        {
            /// <summary>
            /// 子键值集合
            /// </summary>
            private readonly Dictionary<char, Node> _transDict;

            public Node(char c, Node parent, bool isStart)
            {
                IsStart = isStart;
                Char = c;
                Parent = parent;
                Transitions = new List<Node>();
                Results = new List<string>();
                _transDict = new Dictionary<char, Node>();
            }

            public bool IsStart { get; set; }
            /// <summary>
            /// 当前键值key值
            /// </summary>
            public char Char { get; private set; }

            /// <summary>
            /// 父亲节点
            /// </summary>
            public Node Parent { get; private set; }

            /// <summary>
            /// 空节点
            /// </summary>
            public Node Failure { get; set; }

            public List<Node> Transitions { get; private set; }

            public List<string> Results { get; private set; }

            public void AddResult(string result)
            {
                if (!Results.Contains(result))
                {
                    Results.Add(result);
                }
            }

            public void AddTransition(Node node)
            {
                _transDict.Add(node.Char, node);
                //Transitions = _transDict.Values.ToList();
                Transitions = new List<Node>();
                foreach (var node1 in _transDict.Values)
                {
                    Transitions.Add(node1);
                }
            }

            public Node GetTransition(char c)
            {
                var objAlphaPatt = new Regex(_wildcardRegex);
                Node node;
                if (_transDict.TryGetValue(c, out node))
                {
                    return node;
                }
                if (_transDict.TryGetValue('?', out node) && objAlphaPatt.IsMatch(c.ToString(CultureInfo.InvariantCulture)))
                {
                    return node;
                }
                if (_transDict.TryGetValue('*', out node))
                {
                    Node node2;
                    if (node._transDict.TryGetValue(c, out node2))
                        return node2;
                    if (objAlphaPatt.IsMatch(c.ToString(CultureInfo.InvariantCulture)))
                        return node;
                }
                return null;
            }

            public bool ContainsTransition(char c)
            {
                return GetTransition(c) != null;
            }
        }

        #endregion
    }
}