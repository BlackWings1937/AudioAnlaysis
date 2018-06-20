using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.InteropServices;
using WavePlayer;
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
    // private readonly OneThreadSynchronizationContext contex = new OneThreadSynchronizationContext();
    //private string testC = "temp";
    //---------------生命周期方法-----------------
    // Use this for initialization
    private AudioPart ap_ = null;
    private List<AudioPart> lap_ = new List<AudioPart>();
    void Start()
    {
        //SynchronizationContext.SetSynchronizationContext(this.contex);
        StartModule(ModuleType.E_START);
        //AudioPart ap = new AudioPart();
        //lap_.Add(ap);
        //StartCoroutine(count(lap_[0].transformCb));
    }

    IEnumerator count(CallBack cb)
    {
        int i = 0;
        while (true)
        {
            //Debug.Log("process"+ i);
            i++;
            if (cb != null)
            {
                cb("str");
            }
            else
            {
                Debug.Log("cb == null");
            }
            yield return new WaitForSeconds(0.5f);
        }
        Debug.Log("count result");
    }

    IEnumerator count2()
    {
        for (int i = 10; i >= 0; i--)
        {
            Debug.Log("count2:i:" + i);
            yield return new WaitForSeconds(0.2f);
        }
        Debug.Log("count2:finish");
        yield return true;
    }

    // Update is called once per frame
    void Update()
    {

        //this.contex.Update();
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
    private void onBtnClickOpenFile() {
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

        if (LocalDialog.GetSaveFileName(openFileName))
        {
            Debug.Log(openFileName.file);
            Debug.Log(openFileName.fileTitle);
            byte[] buffer = File.ReadAllBytes(openFileName.file);
            AudioClip audio = AudioClip.Create("test", buffer.Length/2, 1, 16000, false);
            audio.SetData(TransformByteToFloat(buffer),0);
            this.gameObject.GetComponent<AudioSource>().clip = audio;
            this.gameObject.GetComponent<AudioSource>().Play();
        }
    }

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
        if (GUILayout.Button("stop count"))
            lap_.Clear();
    }*/
}
