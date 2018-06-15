using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.InteropServices;
using WavePlayer;
using System.IO;
using System;
public class CanvasManager : MonoBehaviour {

    //---------------公有成员---------------------
    public Button BtnOpenFile;
    public Button BtnPlay;

    //---------------私有成员---------------------
    public WavePlayer.WavePlayer wavePlayer_;

    //---------------生命周期方法-----------------
	// Use this for initialization
	void Start () {
        BtnOpenFile.onClick.AddListener(onBtnClickOpenFile);
        BtnPlay.onClick.AddListener(onBtnClickPlay);

        wavePlayer_ = new WavePlayer.WavePlayer();
	}
	
	// Update is called once per frame
	void Update () {

    }
    //---------------UI事件------------------------
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
    }
    private void onBtnClickPlay() {
        wavePlayer_.Play();
    }
}
