namespace ImageChat
{
    /// <summary>
    /// チャットのテキスト
    /// </summary>
    public class ChatText
    {
        /// <summary>
        /// 色
        /// </summary>
        public string Color { get; set; }
        /// <summary>
        /// テキスト内容
        /// </summary>
        public string Text { get; set; }
        /// <summary>
        /// 神からの通信かどうか
        /// </summary>
        public bool IsGod { get; set; }
    }
}