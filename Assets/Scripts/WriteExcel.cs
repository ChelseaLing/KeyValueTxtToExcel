using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;
using OfficeOpenXml;
using System.Text;

public class WriteExcel
{
    private static int HorizontalIndex = 0;
    private static int Vertical = 0;
    private static List<string> lanFileList = new List<string>();
    private static Dictionary<string, int> lanFileContentDic = new Dictionary<string, int>();
    private static int currentFileIndex = 0;

    public static void InitExportExcel()
    {
        lanFileList.Clear();
        lanFileContentDic.Clear();
        currentFileIndex = 0;
        Vertical = 0;
        HorizontalIndex = 0;
        lanFileList = Filehandle.GetFileList(Config.defalutDirPath + "/" + Config.multilingualKey[0], Config.smartFilterFileType, Config.smartFilterFileName);
        setExportOutputDir();
    }

    private static void setExportOutputDir()
    {
        string outputDir = string.Empty;
        SaveFileDialog sfd = new SaveFileDialog();
        sfd.InitialDirectory = "C:\\";
        sfd.Title = "Excel文件导出";
        sfd.Filter = "Excel-2007(*.xlsx)|*.xlsx|Excel-2003(*.xls)|*.xls|所有文件(*.*)|*.*";
        if (sfd.ShowDialog() == DialogResult.OK)
        {
            outputDir = sfd.FileName;
            setExcel(outputDir);
        }
        else
        {
            sfd.Dispose();
            return;
        }
    }

    private static void setExcel(string outputDir)
    {
        if (string.IsNullOrEmpty(outputDir))
            return;
        FileInfo newFile = new FileInfo(outputDir);
        if (newFile.Exists)
        {
            newFile.Delete();
            newFile = new FileInfo(outputDir);
        }
        using (ExcelPackage package = new ExcelPackage(newFile))
        {
            ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Sheet1");
            setExcelTitle(worksheet);
            setExcelContent(worksheet);
            package.Save();
        }
    }

    private static void setExcelTitle(ExcelWorksheet worksheet)
    {
        worksheet.Cells[1, 1].Value = "ID";
        worksheet.Cells[2, 1].Value = "LanKey";
        int lanCount = Config.multilingual.Count;
        HorizontalIndex = 1;
        Vertical = 2;
        foreach (string key in Config.multilingual.Keys)
        {
            HorizontalIndex = HorizontalIndex + 1;
            worksheet.Cells[1, HorizontalIndex].Value = Config.multilingual[key];
            worksheet.Cells[1, HorizontalIndex].AutoFitColumns();
            worksheet.Cells[Vertical, HorizontalIndex].Value = System.Convert.ToInt32(key.Replace("language", ""));
        }
    }

    private static void setExcelContent(ExcelWorksheet worksheet)
    {
        HorizontalIndex = 1;
        Vertical += 2;
        lanFileContentDic.Clear();
        int count = Config.multilingualKey.Count;
        worksheet.Cells[LingzeTool.ConvertToTitle(1) + Vertical].Value = "#FileName:" + Path.GetFileName(lanFileList[currentFileIndex]);
        string fileUrl;
        for (int i = 0; i < count; i++)
        {
            fileUrl = lanFileList[currentFileIndex];
            fileUrl = fileUrl.Replace('/', '\\');
            fileUrl = fileUrl.Replace("\\" + Config.multilingualKey[0] + "\\", "\\" + Config.multilingualKey[i] + "\\");
            SetFileToExcel(worksheet, Config.multilingualKey[i], fileUrl);
        }
        currentFileIndex++;
        if (currentFileIndex >= lanFileList.Count)
            return;
        setExcelContent(worksheet);
    }

    public static void SetFileToExcel(ExcelWorksheet worksheet, string currentLan, string fileUrl)
    {
        if (!File.Exists(fileUrl))
            return;
        string content = File.ReadAllText(fileUrl, Encoding.Default);
        if (content.Trim().Length > 0)
        {
            content = content.Trim().Replace("\r\n", "\n");
            string[] config = content.Split('\n');
            string line, tKey;
            int length = config.Length;
            string[] singleWord;
            for (int i = 0; i < length; i++)
            {
                line = config[i];
                line = line.Replace("\\n", "\n");
                if (line.Trim().Length > 0)
                {
                    singleWord = line.Split('=');
                    tKey = singleWord[0];
                    if (currentLan == Config.multilingualKey[0])
                    {
                        if (!lanFileContentDic.ContainsKey(singleWord[0]))
                        {
                            Vertical++;
                            lanFileContentDic.Add(singleWord[0], Vertical);
                            worksheet.Cells[LingzeTool.ConvertToTitle(1) + Vertical].Value = System.Convert.ToInt32(singleWord[0]);
                            worksheet.Cells[LingzeTool.ConvertToTitle(2) + Vertical].Value = singleWord[1];
                            worksheet.Cells[LingzeTool.ConvertToTitle(2) + Vertical].AutoFitColumns();
                        }
                    }
                    else
                    {
                        int holIndex = Config.multilingualKey.IndexOf(currentLan);
                        int verIndex = 0;
                        if (holIndex > -1 && lanFileContentDic.TryGetValue(singleWord[0], out verIndex))
                        {
                            worksheet.Cells[LingzeTool.ConvertToTitle(holIndex + 2) + verIndex].Value = singleWord[1];
                        }
                    }
                }
            }
        }
    }
}