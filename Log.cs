using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

/// <summary>
/// Summary description for Log
/// </summary>
public class Log
{

    public string CurDir = AppDomain.CurrentDomain.BaseDirectory;

    public void Write(string str, string lvl = "INFO ")
    {
        if (!Directory.Exists(this.CurDir + "/log/"))
        {
            Directory.CreateDirectory(this.CurDir + "/log/");
        }
        DateTime now = DateTime.Now;
        string path = this.CurDir + "/log/" + now.ToString("yyyy-MM-dd") + ".log";
        if (!File.Exists(path))
        {
            this.ClearLogs();
        }
        this.FileAdd(path, "[" + lvl + " " + now.ToString("yyyy-MM-dd HH:mm:ss.f") + "]  LOG " + str + "\r\n");
    }

    public void Write(string[] str, string lvl = "INFO ")
    {
        foreach (var item in str)
        {
            this.Write(item, lvl);
        }
    }

    private void FileAdd(string Path, string Str)
    {
        StreamWriter sw = File.AppendText(Path);
        sw.Write(Str);
        sw.Flush();
        sw.Close();
        sw.Dispose();
    }

    private void ClearLogs()
    {
        if (!Directory.Exists(this.CurDir + "/log/")) return;
        List<FileInfo> delFiles = new List<FileInfo>();
        DateTime now = DateTime.Now;
        DirectoryInfo Folder = new DirectoryInfo(this.CurDir + "/log/");
        foreach (FileInfo file in Folder.GetFiles())
        {
            try
            {
                DateTime dateTime = DateTime.ParseExact(file.Name.Substring(0, file.Name.Length - ".log".Length), "yyyy-MM-dd", CultureInfo.InvariantCulture);
                if (dateTime.AddDays(31 * 6) < now)
                {
                    delFiles.Add(file);
                }
            }
            catch (Exception) { }
        }
        foreach (FileInfo file in delFiles)
        {
            file.Delete();
        }
    }

    public Log(string curDir)
    {
        CurDir = curDir;
    }

    public Log() { }
}
