using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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

    //-------------生命周期方法---------
    // Use this for initialization
    void Start()
    {
        BtnSelectExcel.onClick.AddListener(onBtnClickSelectExcel);
        BtnSelectAudio.onClick.AddListener(onBtnClickSelectAudio);
        BtnSelectOutPutPath.onClick.AddListener(onBtnClickSelectOutPutPath);
    }
    //-------------私有方法--------------
    private void setExcelPath(OpenFileName openFileName)
    {
        TextExcelPath.text = openFileName.file;
    }
    private void setAudioPath(OpenFileName openFileName)
    {
        TextAudioPath.text = openFileName.file;
    }
    private void setOutPutPath(OpenFileName openFileName)
    {
        TextAudioPath.text = openFileName.file;
    }

    //-------------UI事件----------------
    public void onBtnClickSelectExcel()
    {
        OpenFileDirUtil.FindFileDir(setExcelPath);
    }
    public void onBtnClickSelectAudio()
    {
        OpenFileDirUtil.FindFileDir(setAudioPath);
    }
    public void onBtnClickSelectOutPutPath()
    {
        OpenFileDirUtil.FindFileDir(setOutPutPath);
    }

    public void onBtnClickBack()
    {
        CacheCanvasManager.StartModule(CanvasManager.ModuleType.E_START);
    }
    public void onBtnClickOk()
    {
        ModuleParamToProcess mpp = new ModuleParamToProcess(
            TextExcelPath.text,
            TextAudioPath.text,
            TextOutPutPath.text
            );
        CacheCanvasManager.StartModule(CanvasManager.ModuleType.E_PROCESS, mpp);
    }

    //------------重写方法---------------
    public override void Dispose()
    {
        TextExcelPath.text = "";
        TextAudioPath.text = "";
        TextOutPutPath.text = "";
    }
}
