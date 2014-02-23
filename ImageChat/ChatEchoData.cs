namespace ImageChat
{
    /// <summary>
    /// �`���b�g�ւ̃G�R�[�f�[�^
    /// </summary>
    class ChatEchoData 
    {
        /// <summary>
        /// �G�R�[����JSON�e�L�X�g
        /// </summary>
        public string Text { get; private set; }
        /// <summary>
        /// ���b�Z�[�W�^�C�v
        /// </summary>
        public int MessaageType { get; private set; }
        /// <summary>
        /// ���M�I�����ǂ���
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