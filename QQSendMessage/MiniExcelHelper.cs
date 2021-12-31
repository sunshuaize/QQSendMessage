using MiniExcelLibs;
using System.Data;

namespace QQSendMessage
{
    public class MiniExcelHelper
    {
        public static DataTable ReadExcel(string path, string sheetName)
        {
            var table = MiniExcel.QueryAsDataTable(path, useHeaderRow: true, sheetName: sheetName);

            return table;
        }

        public static DataTable ReadExcel(string path, int sheetName)
        {
            DataTable dataTable = null;
            try
            {
                var sheets = MiniExcel.GetSheetNames(path);

                dataTable = MiniExcel.QueryAsDataTable(path, useHeaderRow: true, sheetName: sheets[sheetName]);
            }
            catch (Exception ex)
            {
                return null;
            }
            return dataTable;
        }

        public static bool WriterExcel(DataTable table, string path, string sheetName)
        {
            if (File.Exists(path))
            {
                var sheets = MiniExcel.GetSheetNames(path);

                Dictionary<string, object> she = new Dictionary<string, object>();
                foreach (var sheet in sheets)
                {
                    if (sheet == sheetName)
                    {
                        she.Add(sheet, table);
                    }
                    else
                    {
                        var rows = MiniExcel.Query(path, useHeaderRow: true, sheetName: sheet);
                        she.Add(sheet, rows);
                    }
                }
                string savePath = $@"{Path.GetDirectoryName(path)}\{DateTime.Now.ToString("yyyyMMdd")}_{Path.GetFileName(path)}";
                MiniExcel.SaveAs(savePath, she);
                File.Delete(path);
                File.Move(savePath, path);
            }
            else
            {
                MiniExcel.SaveAs(path, value: table, sheetName: sheetName);
            }
            return default;
        }

        /// <summary>
        /// 创建有列名的表格
        /// </summary>
        /// <param name="column">列名</param>
        /// <returns></returns>
        public static DataTable CreateTable(params string[] column)
        {
            DataTable table = new DataTable();
            DataColumn dataColumn = null;
            for (int i = 0; i < column.Length; i++)
            {
                dataColumn = new DataColumn(column[i]);
                table.Columns.Add(dataColumn);
            }
            return table;
        }
    }
}