using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
public class FileUtil
{

    // 读取本地配置文件的方法
    // 参数:
    // path:文件路径
    // name:文件名称
    public static string LoadFile(string path, string name)
    {
        //文件流
        StreamReader sr = null;
        try
        {
            sr = File.OpenText(path + "//" + name);
        }
        catch (Exception e)
        {
            // 没找到文件
            return "";
        }
        string line;
        string allLine = "";
        while ((line = sr.ReadLine()) != null)
        {
            allLine = allLine + line;
        }
        sr.Close();
        sr.Dispose();
        return allLine;
    }

    // 创建（覆盖如果存在）本地配置文件的方法
    // 参数:
    // path:写入目标路径
    // name:文件名称
    // info:需要写入的文本
    public static void CreateFile(string path, string name, string info)
    {
        //文件流
        StreamWriter sw;
        FileInfo t = new FileInfo(path + "//" + name);
        sw = t.CreateText();
        //以行的形式写入信息
        sw.WriteLine(info);
        sw.Close();
        sw.Dispose();
    }

    /*
     * 尝试创建文件夹
     * 参数:
     * path:文件夹路径
     */
    public static void TryCreateFolder(string path)
    {
        if (!Directory.Exists(path))
        {
            DirectoryInfo di = Directory.CreateDirectory(path);
        }
    }

    /*
     * 获取文件后缀名
     * 参数:
     * path:文件路径
     */
    public static string GetFileLastName(string path) {
        string aLastName = path.Substring(path.LastIndexOf(".") + 1, (path.Length - path.LastIndexOf(".") - 1));
        return aLastName;
    }

    /*
     * 获取文件名
     * 参数:
     * path:文件路径
     */
    public static string GetFileName(string path) {
        string aFirstName = path.Substring(path.LastIndexOf("\\") + 1, (path.LastIndexOf(".") - path.LastIndexOf("\\") - 1));
        return aFirstName;
    }


}
