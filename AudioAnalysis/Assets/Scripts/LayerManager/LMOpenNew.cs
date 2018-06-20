using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

[Serializable]
public class SourceDataConfig {
    public string StrAudioSourcePath;
    public string StrExcelPath;
}

public class LMOpenNew : LayerManagerBase
{

    //------------共有成员--------------
    public Button BtnSelectExcel;
    public Button BtnSelectAudio;
    public Button BtnSelectOutPutPath;
    public Text TextExcelPath;
    public Text TextAudioPath;
    public Text TextOutPutPath;

    public Button BtnBack;
    public Button BtnOk;

    //-------------常量-----------------
    public const string FILE_NAME_SOURCE_CONFIG = "sourceConfig";

    //-------------生命周期方法---------
    // Use this for initialization
    void Start()
    {
        BtnSelectExcel.onClick.AddListener(onBtnClickSelectExcel);
        BtnSelectAudio.onClick.AddListener(onBtnClickSelectAudio);
        BtnSelectOutPutPath.onClick.AddListener(onBtnClickSelectOutPutPath);

        BtnOk.onClick.AddListener(onBtnClickOk);
        BtnBack.onClick.AddListener(onBtnClickBack);
    }
    //-------------私有方法--------------
    private void setExcelPath(OpenFileName openFileName)
    {
        string path = openFileName.file;
        Debug.Log("fileName:" + FileUtil.GetFileName(path));
        Debug.Log("lastName:" + FileUtil.GetFileLastName(path));
        if (FileUtil.GetFileLastName(path) == "xlsx")
        {
            TextExcelPath.text = openFileName.file;
        }
    }
    private void setAudioPath(OpenFileName openFileName)
    {
        string path = openFileName.file;
        if (FileUtil.GetFileLastName(path) == "pcm" || FileUtil.GetFileLastName(path) == "pc")
        {
            TextAudioPath.text = openFileName.file;
        }
    }
    private void setOutPutPath(OpenFileName openFileName)
    {
        TextOutPutPath.text = openFileName.file;
    }

    //-------------UI事件----------------
    public void onBtnClickSelectExcel()
    {
        OpenFileDirUtil.FindFileDir(setExcelPath,"请选择台本Excel表格");
    }
    public void onBtnClickSelectAudio()
    {
        OpenFileDirUtil.FindFileDir(setAudioPath,"请选择音频文件");
    }
    public void onBtnClickSelectOutPutPath()
    {
        OpenFileDirUtil.SelectDir(setOutPutPath,"请选择输出工程路径");
    }

    public void onBtnClickBack()
    {
        CacheCanvasManager.StartModule(CanvasManager.ModuleType.E_START);
    }
    public void onBtnClickOk()
    {
        if (TextAudioPath.text != "" && TextExcelPath.text != "" && TextOutPutPath.text != "")
        {

            /*
            * 初始化任务
            */
            if (!TaskManager.Instance.InitByExcel(TextExcelPath.text,TextOutPutPath.text)) {
                Debug.Log("excel == null");
                return;
            }

            /*
             * 创建源数据保存文件到导出路径
             */
            SourceDataConfig s = new SourceDataConfig();
            s.StrAudioSourcePath = TextAudioPath.text;
            s.StrExcelPath = TextExcelPath.text;
            string strJson = JsonUtility.ToJson(s);
            FileUtil.CreateFile(TextOutPutPath.text,FILE_NAME_SOURCE_CONFIG,strJson);

           /*
            * 跳转到翻译界面
            */
           ModuleParamToProcess mpp = new ModuleParamToProcess(
                TextExcelPath.text,
                TextAudioPath.text,
                TextOutPutPath.text
                );
            CacheCanvasManager.StartModule(CanvasManager.ModuleType.E_PROCESS, mpp);
        }
    }

    //------------重写方法---------------
    public override void Dispose()
    {
        TextExcelPath.text = "";
        TextAudioPath.text = "";
        TextOutPutPath.text = "";
    }
}
