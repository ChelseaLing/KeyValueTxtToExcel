using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class Filehandle
{
    private static List<string> tempList = new List<string>();
    private static string ExtensionName = null;
    private static string FileName = null;

    public static string currentWriteFile = string.Empty;
    public static List<string> hostLanFileList = new List<string>();

    //配置文件内根据Key修改对应Value
    public static void ChangeConfig(string key, string newValue)
    {
        string langPath = Application.streamingAssetsPath + "/" + "Config.txt";
        if (!File.Exists(langPath))
            return;
        string source = File.ReadAllText(langPath);
        string oldValue = findValueByKey(source, key);
        if (!string.IsNullOrEmpty(oldValue))
        {
            source = source.Replace(oldValue, key + "=" + newValue + '\n');
            System.IO.File.WriteAllText(langPath, source);
        }
    }

    private static string findValueByKey(string source, string key)
    {
        string content = source.Trim().Replace("\r\n", "\n");
        string[] config = content.Split('\n');
        int length = config.Length;
        for (int i = 0; i < length; i++)
        {
            if (config[i].IndexOf(key) > -1)
                return config[i];
        }
        return null;
    }

    //查找文件夹内的文件，并返回文件路径列表
    public static List<string> GetFileList(string sourcePath, List<string> filterType = null, List<string> filterfiles = null)
    {
        tempList.Clear();
        if (!Directory.Exists(sourcePath))
            return null;
        getFileList(sourcePath, filterType, filterfiles);
        return tempList;
    }

    private static void getFileList(string sourcePath, List<string> filterType = null, List<string> filterfiles = null)
    {
        foreach (var file in Directory.GetFiles(sourcePath))
        {
            ExtensionName = Path.GetExtension(file).Replace(".", "");
            FileName = Path.GetFileName(file);
            if (filterType != null && filterType.IndexOf(ExtensionName) > -1)
                continue;
            if (filterfiles != null && filterfiles.IndexOf(FileName) > -1)
                continue;
            tempList.Add(file);
        }
        foreach (var dir in Directory.GetDirectories(sourcePath))
        {
            getFileList(dir, filterType, filterfiles);
        }
    }
}