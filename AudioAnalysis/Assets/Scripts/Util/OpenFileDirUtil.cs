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
    /*
    public static void FindFolrdDir(GetDirCallBack cb) {

        if (LocalDialog.GetSFN) { }
    }*/
}

