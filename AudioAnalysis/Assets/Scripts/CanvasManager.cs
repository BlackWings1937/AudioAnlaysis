using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.InteropServices;
using System.IO;
using System;
using System.Threading;

using LitJson;

public class ModuleParamBase { }

public class ModuleParamToProcess : ModuleParamBase
{
    public string StrExcelPath;
    public string StrAudioPath;
    public string StrOutPutPath;

    public ModuleParamToProcess(string strExcelPath, string strAudioPath, string strOutPutPath)
    {
        StrExcelPath = strExcelPath;
        StrAudioPath = strAudioPath;
        StrOutPutPath = strOutPutPath;
    }
}

public class ModuleParamToEditor : ModuleParamBase
{
    public string StrOutPutPath;

    public ModuleParamToEditor(string strOutPutPath)
    {
        StrOutPutPath = strOutPutPath;
    }
}

public class CanvasManager : MonoBehaviour
{
    //界面枚举
    public enum ModuleType { E_START, E_OPENNEW, E_OPENOLD, E_PROCESS, E_EDITOR, E_EDITORRESULT }
    //---------------公有成员---------------------
    public Button BtnOpenFile;
    public Button BtnPlay;

    public GameObject LayerStart;
    public GameObject LayerOpenOld;
    public GameObject LayerOpenNew;
    public GameObject LayerProcess;
    public GameObject LayerEditor;
    public GameObject LayerEditorResult;

    //---------------私有成员---------------------
    public LayerManagerBase activeLm_ = null;
    //---------------生命周期方法-----------------
    // Use this for initialization
    private AudioPart ap_ = null;
    private List<AudioPart> lap_ = new List<AudioPart>();
    void Start()
    {
        StartModule(ModuleType.E_START);
    }
    //---------------对外接口----------------------
    public void StartModule(ModuleType type, ModuleParamBase param = null)
    {
        LayerManagerBase baseLm = null;
        switch (type)
        {
            case ModuleType.E_START:
                baseLm = LayerStart.GetComponent<LayerManagerBase>();
                break;
            case ModuleType.E_OPENNEW:
                baseLm = LayerOpenNew.GetComponent<LayerManagerBase>();
                break;
            case ModuleType.E_OPENOLD:
                baseLm = LayerOpenOld.GetComponent<LayerManagerBase>();
                break;
            case ModuleType.E_PROCESS:
                baseLm = LayerProcess.GetComponent<LayerManagerBase>();
                break;
            case ModuleType.E_EDITOR:
                baseLm = LayerEditor.GetComponent<LayerManagerBase>();
                break;
            case ModuleType.E_EDITORRESULT:
                baseLm = LayerEditorResult.GetComponent<LayerManagerBase>();
                break;
        }
        if (activeLm_ != null) { activeLm_.Dispose(); activeLm_.gameObject.SetActive(false); }
        if (baseLm != null) { baseLm.StartWithParam(param); baseLm.gameObject.SetActive(true); activeLm_ = baseLm; }
    }

    //---------------UI事件------------------------
    /*
    private float[] TransformByteToFloat(byte [] buffer) {
        float[] testInt = new float[buffer.Length / 2];
        int index = 0;
        byte[] arr = new byte[2];
        for (long i = 0; i < buffer.Length - 1; ++i)
        {
            if (i % 2 == 1)
            {
                arr[0] = buffer[i - 1];
                arr[1] = buffer[i];
                short num = BitConverter.ToInt16(arr, 0);
                float sample = ((float)num) / 32767.0f;
                testInt[index] = sample;
                index++;
            }
        }
        return testInt;
    }*/
    /*
    private void OnGUI()
    {
        string[] paths = {
            "C:\\Users\\DELL\\Desktop\\OutPutPath\\firstCutTemp\\1.pc",
            "C:\\Users\\DELL\\Desktop\\OutPutPath\\firstCutTemp\\2.pc",
            "C:\\Users\\DELL\\Desktop\\OutPutPath\\firstCutTemp\\3.pc"
        };
        if (GUILayout.Button("play")) {
            PCMPlayer.Instance.PlayerLocalFiles(paths);
        }
    }*/
}
