namespace WinFormsApp1.Controls
{
    public partial class SendInfoControl : UserControl
    {
        public SendInfoControl()
        {
            InitializeComponent();
        }

        private string userName;

        public string UserName
        {
            get { return this.name.Text; }
            set
            {
                userName = value;
                this.name.Text = value;
            }
        }

        private int sendCount;

        public int SendCount
        {
            get { return String.IsNullOrWhiteSpace(this.count.Text)?0: int.Parse(this.count.Text); }
            set
            {
                sendCount = value;
                this.count.Text = value.ToString();
            }
        }

        private string message;

        public string Message
        {
            get { return this.msg.Text; }
            set
            {
                message = value;
                this.msg.Text = value;
            }
        }

        private int qqCode;

        public int QQCode
        {
            get { return qqCode; }
            set { qqCode = value; }
        }

    }
}