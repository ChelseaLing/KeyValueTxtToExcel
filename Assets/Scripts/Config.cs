using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

public class Config
{
    public static string defalutDirPath = string.Empty;
    public static List<string> smartFilterFileType = new List<string>();
    public static List<string> smartFilterFileName = new List<string>();
    public static List<string> multilingualKey = new List<string>();
    public static Dictionary<string, string> multilingual = new Dictionary<string, string>();

    private static Regex languageReg = new Regex("^(language)[0-9]+$");

    public static bool initConfig()
    {
        defalutDirPath = string.Empty;
        smartFilterFileType.Clear();
        smartFilterFileName.Clear();
        multilingual.Clear();
        multilingualKey.Clear();
        string langPath = Application.streamingAssetsPath + "/" + "Config.txt";
        if (!File.Exists(langPath))
            return false;
        string content = File.ReadAllText(langPath).Trim();
        if (content.Length <= 0)
            return false;
        analysisConfig(content);
        return true;
    }

    private static void analysisConfig(string content)
    {
        content = content.Trim().Replace("\r\n", "\n");
        string[] config = content.Split('\n');
        string line, tKey, tValue;
        int length = config.Length;
        string[] singleWord;
        for (int i = 0; i < length; i++)
        {
            line = config[i];
            line = line.Replace("\\n", "\n");
            if (line.Trim().Length > 0)
            {
                if (line.IndexOf("#") == 0)
                    continue;
                singleWord = line.Split('=');
                if (singleWord.Length < 2)
                    continue;
                tKey = singleWord[0].Trim();
                tValue = singleWord[1].Trim();
                //默认路径
                if (tKey == "defalutDirPath")
                {
                    defalutDirPath = tValue;
                    continue;
                }
                //多语言
                if (languageReg.IsMatch(tKey))
                {
                    if (!multilingual.ContainsKey(tKey))
                    {
                        multilingualKey.Add(tKey.Replace("language", ""));
                        multilingual.Add(tKey, tValue);
                    }
                }
                //过滤的文件类型
                if (tKey == "smartFilterFileType")
                {
                    setFilterFileType(tValue);
                    continue;
                }
                //过滤的文件名字
                if (tKey == "smartFilterFileName")
                {
                    setFilterFileName(tValue);
                    continue;
                }
            }
        }
    }

    private static void setFilterFileType(string fileTypes)
    {
        fileTypes = fileTypes.Replace("，", ",");
        string[] fileTypeList = fileTypes.Split(',');
        int count = fileTypeList.Length;
        for (int i = 0; i < count; i++)
        {
            smartFilterFileType.Add(fileTypeList[i].Trim());
        }
    }

    private static void setFilterFileName(string filenames)
    {
        filenames = filenames.Replace("，", ",");
        string[] fileNameList = filenames.Split(',');
        int count = fileNameList.Length;
        for (int i = 0; i < count; i++)
        {
            smartFilterFileName.Add(fileNameList[i].Trim());
        }
    }
}