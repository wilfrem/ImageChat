using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageChat
{
    public class Room
    {
        /// <summary>
        /// リンク先ホスト名
        /// </summary>
        public string Host { get; private set; }
        /// <summary>
        /// 画像ファイル
        /// </summary>
        public string ImageUrl { get; private set; }
        /// <summary>
        /// チャット一覧
        /// </summary>
        public Queue<ChatText> Texts { get; private set; }
        public Room(string host, string imageUrl)
        {
            Host = host;
            ImageUrl = imageUrl;
            Texts = new Queue<ChatText>();
        }
    }
}
