using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class PCMPlayer : MonoBehaviour
{
    private static PCMPlayer instance_;

    public static PCMPlayer Instance
    {
        get
        {
            return instance_;
        }
    }

    public AudioSource CacheAudioSource
    {
        get
        {
            if (cacheAudioSource_ == null) {
                cacheAudioSource_ = this.GetComponent<AudioSource>();
            }
            return cacheAudioSource_;
        }
    }
    private AudioSource cacheAudioSource_;
    //----------------------常量----------------------------------
    public const int SAMPLE_NUM = 16000;

    //----------------------生命周期方法--------------------------

    public void Start()
    {
        instance_ = this;
    }

    //---------------------对外接口-------------------------------
    /*
     * 播放本地pcm文件
     */
    public void PlayerLocalFiles(string[] pathArr)
    {
        List<byte[]> allBytes = new List<byte[]>();
        long length = 0;
        for (int i = 0;i<pathArr.Length;++i) {
            FileStream stream = new FileInfo(pathArr[i]).OpenRead();
            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer, 0, Convert.ToInt32(stream.Length));
            allBytes.Add(buffer);
            length += buffer.Length;
            stream.Close();
        }
        byte[] allBuffer = new byte[length];
        int index = 0;
        for (int i = 0;i<allBytes.Count;++i) {
            System.Buffer.BlockCopy(allBytes[i],0,allBuffer, index, allBytes[i].Length);
            index += allBytes[i].Length;
        }
        PlayerByteBuffer(allBuffer);
    }

    public void PlayerByteBuffer(byte[] buffer) {
        PlayerFloatBuffer(TransformByteToFloat(buffer),buffer.Length/SAMPLE_NUM);
    }

    /*
     * 播放pcm buffer
     */
    public void PlayerFloatBuffer(float[] buffer,int second)
    {
        AudioClip ac = AudioClip.Create("test", SAMPLE_NUM*second, 1, SAMPLE_NUM,false);
        ac.SetData(buffer,0);
        CacheAudioSource.clip = ac;
        CacheAudioSource.Play();
    }

    private float[] TransformByteToFloat(byte[] buffer)
    {
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
}
