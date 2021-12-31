using System.Data;
using System.Xml.Linq;

namespace QQSendMessage
{
    public partial class CronHelper : Form
    {
        public CronHelper()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void CronHelper_Load(object sender, EventArgs e)
        {
           

            var paths = AppContext.BaseDirectory.Split("\\", StringSplitOptions.RemoveEmptyEntries).ToList();
            paths.RemoveRange(paths.Count - 3, 3);

            string path = $@"{string.Join("\\", paths)}\CronHelper.xml";

            DataTable data = MiniExcelHelper.CreateTable("表达式","描述");


            if (File.Exists(path))
            {
                XElement xElement = XElement.Load(path);

                foreach (var item in xElement.Elements("item"))
                {
                    DataRow dataRow = data.NewRow();

                    dataRow["表达式"] = item.Element("value").Value;
                    dataRow["描述"] = item.Element("description").Value;

                    data.Rows.Add(dataRow);
                }
            }
           this.dataGridView1.DataSource = data;




        }

        private void dataGridView1_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
           
            Clipboard.SetDataObject(dataGridView1[e.ColumnIndex, e.RowIndex].Value);
        }
    }
}