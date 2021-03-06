﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LMProcess : LayerManagerBase
{
    //----------------公有成员---------------------
    public Button BtnBack;
    public Slider SliderProcess;
    public RectTransform RTContent;
    public Text TextConsoleOutPut;

    //----------------私有成员---------------------
    private ModuleParamToProcess cacheParam_;

    //----------------生命周期方法-----------------
    // Use this for initialization
    void Start()
    {
        BtnBack.onClick.AddListener(onBtnBack);
    }

    //---------------UI事件------------------------
    private void onBtnBack()
    {
        AudioEditManagercs.Instance.CancleProcess();
        AudioEditManagercs.Instance.Dispose();
        CacheCanvasManager.StartModule(CanvasManager.ModuleType.E_OPENNEW);
    }

    //--------------私有方法-----------------------
    private void onProcessChange(float process)
    {
        SliderProcess.value = process;
    }
    private void onProcessFinish()
    {
        /*
         * 跳转界面到编辑界面
         */
        CacheCanvasManager.StartModule(
            CanvasManager.ModuleType.E_EDITOR,
            new ModuleParamToEditor(cacheParam_.StrOutPutPath)
            );
    }

    //---------------重写方法----------------------
    public override void StartWithParam(ModuleParamBase param)
    {
        ModuleParamToProcess p = (ModuleParamToProcess)param;
        AudioEditManagercs.Instance.initWithNew(p.StrAudioPath, p.StrOutPutPath, onProcessChange, onProcessFinish);
        cacheParam_ = p;
        SliderProcess.value = 1;
    }

    public override void Dispose()
    {
        SliderProcess.value = 0;
    }

}
/*
  Audio-> AudioPartList 1 
  Baidu-> AudioPartList 2
  AudioPartList-> local json file save config 
               -> audioPart save as pcm part
               -> orignal audio path

  Singleton AudioEditManager;
 */
