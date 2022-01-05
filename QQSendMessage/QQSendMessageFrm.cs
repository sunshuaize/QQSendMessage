using Furion.TaskScheduler;
using Microsoft.Win32;
using QQSendMessage;
using System.Data;
using System.Diagnostics;
using System.Runtime.InteropServices;
using WinFormsApp1.Controls;

namespace WinFormsApp1
{
    public partial class QQSendMessageFrm : Form
    {
        public QQSendMessageFrm()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
        }

        //找窗体
        [System.Runtime.InteropServices.DllImport("user32.dll", EntryPoint = "FindWindow")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        //把窗体置于最前
        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        //拖动窗体
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        public static extern bool SendMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);

        public const int WM_SYSCOMMAND = 0x0112;
        public const int SC_MOVE = 0xF010;
        public const int HTCAPTION = 0x0002;

        private void button1_Click(object sender, EventArgs e)
        {
            Button button = (Button)sender; 


            try
            {
                button1.Enabled = false;
                button2.Enabled = false;
                button3.Enabled = false;
                button5.Enabled = false;
                button6.Enabled = false;
                button8.Enabled = false;

                List<SendMessage> list = new List<SendMessage>();

                foreach (Control item in this.flowLayoutPanel1.Controls)
                {
                    if (item is SendInfoControl)
                    {
                        SendInfoControl sendInfo = (SendInfoControl)item;

                        SendMessage send = new SendMessage();

                        if (sendInfo.SendCount != 0)
                        {
                            send.Count = sendInfo.SendCount;
                            send.Name = sendInfo.UserName;
                            send.Message = sendInfo.Message;
                            send.QQCode = sendInfo.QQCode;
                            list.Add(send);
                        }
                    }
                }
                HttpClient client = new HttpClient();

                client.GetAsync("http://wpa.qq.com/msgrd?v=3&uin=1242365540&site=&menu=yes");

                if (!string.IsNullOrWhiteSpace(cronTxt.Text))
                {
                    if (list.Count > 1)
                    {
                        // 每隔 1s 执行
                        SpareTime.Do(cronTxt.Text, (timer, count) =>
                        {
                            listBox1.Invoke(new Action(() =>
                            {
                                AddItem("现在时间：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                                AddItem($"一共执行了：{count} 次");
                                sendMessage(list);
                            }));
                        }, workerName: "qq自动发消息", cronFormat: CronFormat.IncludeSeconds);
                    }
                    else
                    {
                        AddItem("没有任务可以执行！！！");
                    }
                }
                else
                {
                    sendMessage(list);

                    button1.Enabled = true;
                    button2.Enabled = true;
                    button3.Enabled = true;
                    button5.Enabled = true;
                    button6.Enabled = true;
                    button8.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                AddItem($@"异常:{ex.Message}");
            }
        }

        //发送消息
        public void sendMessage(List<SendMessage> sendMessages)
        {
            foreach (var item in sendMessages)
            {
                string aioName = item.Name;  //AIO名
                string info = item.Message;            //要发送的消息

                IntPtr k = FindWindow(null, aioName);   //查找窗口
                if (k.ToString() != "0")
                {
                    SetForegroundWindow(k);             //把窗体置于最前
                    for (int i = 1; i <= item.Count; i++)
                    {
                        SendKeys.SendWait(info);
                        SendKeys.Send("{ENTER}");
                        Thread.Sleep(200);
                    }
                }
                else
                {
                    AddItem($"木有找到{item.Name}聊天窗口,请先打开消息框");
                }
            }
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, WM_SYSCOMMAND, SC_MOVE + HTCAPTION, 0);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SendInfoControl sendInfoControl = new SendInfoControl();

            this.flowLayoutPanel1.Controls.Add(sendInfoControl);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = $"{PathHelper.GetDir()}";//注意这里写路径时要用c:\\而不是c:\
            openFileDialog.Filter = "表格文件|*.xlsx";
            openFileDialog.RestoreDirectory = true;
            openFileDialog.FilterIndex = 1;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                var excelpath = openFileDialog.FileName;

                DataTable table = MiniExcelHelper.ReadExcel(excelpath, 0);

                List<SendMessage> list = new List<SendMessage>();

                foreach (DataRow row in table.Rows)
                {
                    SendMessage send = new SendMessage();
                    send.Count = int.Parse(row["次数"].ToString());
                    send.Name = row["好友名称或群名"].ToString();
                    send.Message = row["消息"].ToString();
                    send.QQCode = int.Parse(row["QQ号"].ToString());
                    list.Add(send);
                    AddItem($" {send.Name} 连接成功！！！");
                }
                this.flowLayoutPanel1.Controls.Clear();

                for (int i = 0; i < list.Count; i++)
                {
                    SendInfoControl sendInfo = new SendInfoControl();
                    sendInfo.SendCount = list[i].Count;
                    sendInfo.UserName = list[i].Name;
                    sendInfo.Message = list[i].Message;
                    sendInfo.QQCode = list[i].QQCode;

                    this.flowLayoutPanel1.Controls.Add(sendInfo);
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            AddItem("欢迎使用QQ群发助手！！！！");
            AddItem("本软件只支持发送当前桌面上打开的对话框");
        }

        private void button5_Click(object sender, EventArgs e)
        {
            this.flowLayoutPanel1.Controls.Clear();
            QQWindow.Clear();
            QQCodeList.Clear();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();

            AddItem("欢迎使用QQ群发助手！！！！");
            AddItem("本软件只支持发送当前桌面上打开的对话框");
        }

        private bool scroll = false;

        private void AddItem(string text)
        {
            if (this.listBox1.TopIndex == this.listBox1.Items.Count - (int)(this.listBox1.Height / this.listBox1.ItemHeight))
                scroll = true;
            if (this.listBox1.InvokeRequired)
            {
                this.Invoke(new Action(() =>
                {
                    listBox1.Items.Add(text);
                }));
            }
            else
            {
                this.listBox1.Items.Add(text);
            }
            if (scroll)
                this.listBox1.TopIndex = this.listBox1.Items.Count - (int)(this.listBox1.Height / this.listBox1.ItemHeight);
        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            button1.Enabled = true;
            button2.Enabled = true;
            button3.Enabled = true;
            button5.Enabled = true;
            button6.Enabled = true;
            button8.Enabled = true;
            var worker = SpareTime.GetWorker("qq自动发消息");

            if (worker != null && worker.Status == SpareTimeStatus.Running)
            {
                SpareTime.Cancel("qq自动发消息");
                AddItem("停止发送消息");
            }
        }

        private void button1_MouseEnter(object sender, EventArgs e)
        {
        }

        private void button1_MouseLeave(object sender, EventArgs e)
        {
        }

        private void button7_Click(object sender, EventArgs e)
        {
            CronHelper cronHelper = new CronHelper();
            cronHelper.ShowDialog();
        }
        List<string> QQCodeList = new List<string>();
        List<string> QQWindow = new List<string>();
        private void button8_Click(object sender, EventArgs e)
        {
            #region 获取QQ号
            foreach (Control item in this.flowLayoutPanel1.Controls)
            {
                if (item is SendInfoControl)
                {
                    SendInfoControl sendInfo = (SendInfoControl)item;
                    if (sendInfo.SendCount != 0)
                    {
                        QQCodeList.Add(sendInfo.QQCode.ToString());
                        QQWindow.Add(sendInfo.UserName);
                    }
                }
            }

            //string userTemphtmlPath = $@"{PathHelper.GetDir()}\UserTemp.html";

            //string htmlContent = File.ReadAllText(userTemphtmlPath);

            //foreach (var qq in QQCodeList)
            //{
            //    htmlContent = htmlContent.Replace("号码", qq);
            //    string html = $@"{PathHelper.GetDir()}\QQHtml\{qq}.html";
            //    if (File.Exists(html))
            //    {
            //        File.Delete(html);
            //    }
            //    File.WriteAllText(html, htmlContent);
            //}

            //var files = new DirectoryInfo($@"{PathHelper.GetDir()}\QQHtml").GetFiles();
            ////通过注册表获取默认浏览器
            //RegistryKey key = Registry.ClassesRoot.OpenSubKey(@"http\shell\open\command\");
            //String s = key.GetValue("").ToString();
            //String browserpath = null;
            //if (s.StartsWith("\""))
            //{
            //    browserpath = s.Substring(1, s.IndexOf('\"', 1) - 1);
            //}
            //else
            //{
            //    browserpath = s.Substring(0, s.IndexOf(" "));
            //}
            //foreach (var file in files)
            //{
            //    //System.Diagnostics.Process.Start(browserpath, file.FullName);
            //    System.Diagnostics.Process.Start(browserpath, @"C:\Users\11572\Desktop\youbage.lnk");
            //}
            #endregion

            Process myProcess = new Process();//创建进程对象
            myProcess.StartInfo.FileName = "cmd.exe";//设置打开cmd命令窗口
            myProcess.StartInfo.UseShellExecute = false;//不使用操作系统shell启动进程的值
            myProcess.StartInfo.RedirectStandardInput = true;//设置可以从标准输入流读取值
            myProcess.StartInfo.RedirectStandardOutput = true;//设置可以向标准输出流写入值
            myProcess.StartInfo.RedirectStandardError = true;//设置可以显示输入输出流中出现的错误
            myProcess.StartInfo.CreateNoWindow = true;//设置在新窗口中启动进程
            myProcess.Start();//启动进程

            string LinksDir = $@"{new DirectoryInfo(Directory.GetCurrentDirectory()).Parent.Parent.Parent.FullName}\Links";
            var files= new DirectoryInfo(LinksDir).GetFiles("*.lnk");

            foreach (var file in files)
            {
                if (QQWindow.Contains(Path.GetFileNameWithoutExtension(file.Name)))
                {
                    myProcess.StandardInput.WriteLine(file.FullName);//传入要执行的命令
                }
            }
        }


    }
}