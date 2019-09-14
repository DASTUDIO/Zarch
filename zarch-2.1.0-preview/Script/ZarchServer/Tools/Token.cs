using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;

namespace  Z.Tools
{
    public class Token
    {
        /// <summary>
        /// 随机生成一个不重复的Token字符串
        /// </summary>
        /// <returns>生成的token字符串.</returns>
        /// <param name="tokenLength">Token长度.</param>
        public static string GenToken(int tokenLength)
        {
            StringBuilder builder = new StringBuilder();

            Random r = new Random();

            for (int i = 0; i < tokenLength; i++)
            {
                builder.Append(Elements[r.Next(Elements.Length)]);
            }

            string result = builder.ToString();

            // 需要解决 递归深度的控制 如果长度过短出现列举穷尽 怎么办
            // 要么限制最短长度 并积极清除不用的Token
            // 要么有一个穷尽检测 也就是那个_tokens的内容数量等于长度的极限（如果全部定长）
            // 动态长度 是不是好一点
            if (_tokens.Keys.ToList().Contains(result))
                return GenToken(tokenLength);

            //_tokens.Add(result, null);

            return result;

        }

        /// <summary>
        /// 每个token保存自定义object
        /// </summary>
        /// <param name="token">Token.</param>
        public  TcpConnectedPeer this[string token]
        {
            get
            {
                if (_tokens.Keys.Contains(token))
                    return _tokens[token];
                else
                    return null;
            }

            set
            {
                if (value != null)
                    _tokens[token] = value;
                else
                    if(_tokens.Keys.Contains(token))
                        _tokens.Remove(token);
            }
        }

        /// <summary>
        /// 清空保存的所有token数据object和重复检测也被重置
        /// </summary>
        public void Clear()
        {
            _tokens.Clear();
        }

        static char[] Elements = { 'a','b','c','d','e','f','g','h','i','j','k','l','m','n','o','p','q','r','s','t','u','v','w','x','y','z',
                                   'A','B','C','D','E','F','G','H','I','J','K','L','M','N','O','P','Q','R','S','T','U','V','W','X','Y','Z',
                                   '0','1','2','3','4','5','6','7','8','9'};

        static Dictionary<string,  TcpConnectedPeer> _tokens = new Dictionary<string,  TcpConnectedPeer>();

        Token() { }

        static Token instance;

        public static Token Instance
        {
            get
            {
                if (instance == null)
                    instance = new Token();

                return instance;
            }
        }

    }
}
