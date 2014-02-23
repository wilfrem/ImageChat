namespace ImageChat
{
    /// <summary>
    /// チャットへのエコーデータ
    /// </summary>
    class ChatEchoData 
    {
        /// <summary>
        /// エコーするJSONテキスト
        /// </summary>
        public string Text { get; private set; }
        /// <summary>
        /// メッセージタイプ
        /// </summary>
        public int MessaageType { get; private set; }
        /// <summary>
        /// 送信終了かどうか
        /// </summary>
        public bool EndOfMessage { get; private set; }
        public ChatEchoData(string text, int messageType, bool endOfMessage)
        {
            Text = text;
            MessaageType = messageType;
            EndOfMessage = endOfMessage;
        }
    }
}