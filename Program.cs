using Spire.Pdf;
using System;
using System.IO;
using System.Net;
using System.Web;

namespace webprotocol
{
    internal class Program
    {
        private readonly static Log log = new Log();

        static void Main(string[] args)
        {
            string url = args[0];

            bool preview = false;
            string tempFile;
            string fileType = "word";

            url = url.Replace("webprotocol://https-", "https://")
                .Replace("webprotocol://http-", "http://");
            var uri = new Uri(url);
            var query = HttpUtility.ParseQueryString(uri.Query);

            var previewStr = query["_preview"];
            if (previewStr == "1")
            {
                preview = true;
            }

            var base64Str = query["base64"];
            if (base64Str != null && base64Str != "")
            {
                var buffer = Convert.FromBase64String(base64Str);
                tempFile = Path.GetTempFileName();
                FileStream fs = new FileStream(tempFile, FileMode.Create);
                fs.Write(buffer, 0, buffer.Length);
                fs.Flush();
                fs.Close();
            }
            else
            {
              tempFile = Downloadfile(url);
            }

            var fileTypeStr = query["_type"];
            if (fileTypeStr.Trim() != "")
            {
                fileType = fileTypeStr;
            }

            // string tempFile = "C:\\Users\\15126\\Desktop\\a.docx";
            if (fileType == "word")
            {
                PrintWord(tempFile, preview);
            }
            else if (fileType == "excel")
            {
                PrintExcel(tempFile, preview);
            }
            else if (fileType == "pdf")
            {
                PrintPdf(tempFile, preview);
            }
        }

        private static string Downloadfile(string url)
        {
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
            WebClient client = new WebClient();
            string tempFile = Path.GetTempFileName();
            client.DownloadFile(url, tempFile);
            return tempFile;
        }

        private static void PrintWord(string fileName, bool preview)
        {
            try
            {
                var word = new Microsoft.Office.Interop.Word.Application { Visible = false }; // 启动Word进程
                var doc = word.Documents.Open(fileName, ReadOnly: true, Visible: true); // 打开待打印的文档
                doc.PrintOut(); // 打印
                doc.Close(SaveChanges: false); // 关闭文档
                // word.Quit(SaveChanges: false); // 退出Word进程
                System.Runtime.InteropServices.Marshal.ReleaseComObject(word);
            }
            catch(Exception ex)
            {
                log.Write(ex.Message + "\n" + ex.StackTrace);
            }
        }

        private static void PrintExcel(string fileName, bool preview)
        {
            Microsoft.Office.Interop.Excel.Application xApp = new Microsoft.Office.Interop.Excel.Application
            {
                Visible = false
            };
            Microsoft.Office.Interop.Excel.Workbook xBook = xApp.Workbooks._Open(fileName);
            Microsoft.Office.Interop.Excel.Worksheet xSheet = null;
            try
            {
                xApp.Run("Workbook_Open");
            }
            catch (Exception)
            {

            }
            try
            {
                xSheet = (Microsoft.Office.Interop.Excel.Worksheet)xBook.ActiveSheet;
                xSheet.PrintOut(1, 1, 1, preview);
            }
            finally
            {
                xBook.Close(false);
                xApp.Quit();
                System.Runtime.InteropServices.Marshal.ReleaseComObject(xApp);
                xSheet = null;
                xBook = null;
                xApp = null;
                GC.Collect();
            }
        }

        private static void PrintPdf(string fileName, bool preview)
        {
            PdfDocument doc = new PdfDocument();
            doc.LoadFromFile(fileName);
            doc.Print();
        }

    }
}
