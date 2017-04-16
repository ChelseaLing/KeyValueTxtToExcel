using System.IO;
using OfficeOpenXml;

//解析Excel文件，还原成对应文本
public class AnalysisExcel
{
    private static int currentColumn = 1; //行
    private static int currentRow = 1;//列

    private static string currentLan = string.Empty;
    private static string lastFileUrl = string.Empty;
    private static string lastFileContent = string.Empty;

    public static void parseExcel(string excelUrl, string saveDir)
    {
        FileStream stream = File.OpenRead(excelUrl);
        using (stream)
        {
            ExcelPackage package = new ExcelPackage(stream);
            ExcelWorksheet sheet = package.Workbook.Worksheets[1];
            if(sheet==null)
            {
                MainPage.Ins.setTips("[ERROR]Excel文件解析异常");
                return;
            }
            currentColumn = 2; //1 = ID,所以从第二列开始遍历
            int lastColumn = sheet.Dimension.End.Column;
            while (currentColumn <= lastColumn)
            {
                currentLan = sheet.Cells[LingzeTool.ConvertToTitle(currentColumn) + 2].Value.ToString();
                createDirAndFile(saveDir, sheet);
                currentColumn++;
            }
        }
    }

    private static void createDirAndFile(string saveDir, ExcelWorksheet sheet)
    {
        saveDir = saveDir + "/" + currentLan;
        if (!Directory.Exists(saveDir))
        {
            Directory.CreateDirectory(saveDir);
        }
        currentRow = 3; // 1=ID,2=LanKey,所以从第三列开始遍历
        object key = string.Empty;
        object lanValue = string.Empty;
        int lastLie = sheet.Dimension.End.Row;
        lastFileUrl = string.Empty;
        lastFileContent = string.Empty;
        //从上到下，碰到FileName，则证明开始是一个文件，这时候就要记录
        while (currentRow <= lastLie)
        {
            key = sheet.Cells[LingzeTool.ConvertToTitle(1) + currentRow].Value;
            if (key != null)
            {
                if (key.ToString().IndexOf("#FileName:") > -1)
                {
                    if (!string.IsNullOrEmpty(lastFileUrl) && !string.IsNullOrEmpty(lastFileContent))
                    {
                        writeTxt(lastFileUrl, lastFileContent);
                    }
                    lastFileUrl = saveDir +"/"+ key.ToString().Replace("#FileName:", "/");
                    lastFileContent = null;
                }
                else
                {
                    lanValue = sheet.Cells[LingzeTool.ConvertToTitle(currentColumn) + currentRow].Value;
                    if (lanValue == null)
                        lastFileContent += key.ToString() + "=\r\n";
                    else
                        lastFileContent += key.ToString() + "=" + lanValue.ToString() + "\r\n";
                }
            }
            currentRow++;
            if (currentRow > lastLie)
            {
                if (!string.IsNullOrEmpty(lastFileUrl) && !string.IsNullOrEmpty(lastFileContent))
                {
                    writeTxt(lastFileUrl, lastFileContent);
                    lastFileUrl = null;
                    lastFileContent = null;
                }
            }
        }
    }

    private static void writeTxt(string fileurl, string contnt)
    {
        try
        {
            FileStream fs = new FileStream(fileurl, FileMode.OpenOrCreate);
            byte[] data = System.Text.Encoding.UTF8.GetBytes(contnt);
            fs.Write(data, 0, data.Length);
            fs.Flush();
            fs.Close();
        }
        catch (System.Exception)
        {
            throw;
        }
    }
}