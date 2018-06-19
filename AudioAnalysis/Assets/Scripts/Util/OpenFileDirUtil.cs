using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.InteropServices;
using WavePlayer;
using System.IO;
using System;
public delegate void GetDirCallBack(OpenFileName openFileName);
public class OpenFileDirUtil
{
    public static void FindFileDir(GetDirCallBack cb) {
        OpenFileName openFileName = new OpenFileName();
        openFileName.structSize = Marshal.SizeOf(openFileName);
        openFileName.filter = "";// "Excel文件(*.xlsx)\0*.xlsx";
        openFileName.file = new string(new char[256]);
        openFileName.maxFile = openFileName.file.Length;
        openFileName.fileTitle = new string(new char[64]);
        openFileName.maxFileTitle = openFileName.fileTitle.Length;
        openFileName.initialDir = Application.streamingAssetsPath.Replace('/', '\\');//默认路径
        openFileName.title = "窗口标题";
        openFileName.flags = 0x00080000 | 0x00001000 | 0x00000800 | 0x00000008;

        if (LocalDialog.GetOpenFileName(openFileName)) {
            cb(openFileName);
        }
    }
    public static void SelectDir(GetDirCallBack cb)
    {
        OpenDialogDir ofn2 = new OpenDialogDir();
        ofn2.pszDisplayName = new string(new char[2000]); ;     // 存放目录路径缓冲区    
        ofn2.lpszTitle = "Open Project";// 标题    
        //ofn2.ulFlags = BIF_NEWDIALOGSTYLE | BIF_EDITBOX; // 新的样式,带编辑框    
        IntPtr pidlPtr = LocalDialog.SHBrowseForFolder(ofn2);
        char[] charArray = new char[2000];
        for (int i = 0; i < 2000; i++)
            charArray[i] = '\0';
        LocalDialog.SHGetPathFromIDList(pidlPtr, charArray);
        string fullDirPath = new String(charArray);
        fullDirPath = fullDirPath.Trim('\0');
        OpenFileName openFileName = new OpenFileName();
        openFileName.file = fullDirPath;
        cb(openFileName);
    }

    

}

