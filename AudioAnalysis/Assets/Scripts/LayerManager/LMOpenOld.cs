using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;

public class LMOpenOld : LayerManagerBase
{
    //---------------公有成员-----------------
    public Button BtnSelectOutPutPath;
    public Text TextOutPutPath;

    public Button BtnBack;
    public Button BtnOk;

    //---------------生命周期方法--------------
    // Use this for initialization
    void Start()
    {
        BtnSelectOutPutPath.onClick.AddListener(onBtnClickSelectOutPutPath);
        BtnBack.onClick.AddListener(onBtnClickBack);
        BtnOk.onClick.AddListener(onBtnClickOk);
    }
    //--------------私有方法-------------------
    private void setOutPutPath(OpenFileName openFileName)
    {
        TextOutPutPath.text = openFileName.file;
    }

    //---------------UI响应事件----------------
    private void onBtnClickSelectOutPutPath()
    {
        OpenFileDirUtil.SelectDir(setOutPutPath, "请选择输出工程路径");
    }
    public void onBtnClickBack()
    {
        CacheCanvasManager.StartModule(CanvasManager.ModuleType.E_START);
    }
    public void onBtnClickOk()
    {
        if (TextOutPutPath.text != null)
        {
            /*
             * 读取源数据配置文件
             */
            string strJson = FileUtil.LoadFile(TextOutPutPath.text, LMOpenNew.FILE_NAME_SOURCE_CONFIG);
            SourceDataConfig s = JsonUtility.FromJson<SourceDataConfig>(strJson);

            /*
             * 检查工程文件是否都完整
             */
            if (!File.Exists(s.StrAudioSourcePath)) {
                return;
            }
            if (!File.Exists(s.StrExcelPath)) {
                return;
            }

            /*
             * 装载任务的配置文件
             */
            if (!TaskManager.Instance.InitByConfig(TextOutPutPath.text)) {
                return;
            }

            /*
             * 装载配置文件到AudioPart Manager
             */
            if (!AudioEditManagercs.Instance.initWithOld(TextOutPutPath.text,s.StrAudioSourcePath)) {
                AudioEditManagercs.Instance.Dispose();
                return;
            }

            /*
             * 跳转界面到编辑界面
             */
            CacheCanvasManager.StartModule(
                CanvasManager.ModuleType.E_EDITOR,
                new ModuleParamToEditor(TextOutPutPath.text)
                );
        }
    }

    //---------------重写方法------------------
    public override void Dispose()
    {
        TextOutPutPath.text = "";
    }

}
