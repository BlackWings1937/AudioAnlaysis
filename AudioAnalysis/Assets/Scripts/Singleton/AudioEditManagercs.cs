using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using UnityEngine;
using Baidu.Aip.Speech;
using Baidu;
using System.Text.RegularExpressions;
using System.Collections;
using UnityEngine.UI;

[Serializable]
public class AudioPart
{
    public long startIndex;
    public long endIndex;
    public string[] textArr;
    public string[] textPingYinArr;
    public string strText;
    public string audioPath;
    public bool mark = false;
    /*
     * 临时是否被标记使用的成员
     */
    private bool isUse = false;
    public event CallBackBool OnValueChangeIsUse;
    public bool IsUse
    {
        get
        {
            return isUse;
        }

        set
        {
            isUse = value;
            if (OnValueChangeIsUse!= null) {
                OnValueChangeIsUse.Invoke(isUse);
            }
        }
    }

    /*
     * 被其他方法所引用的成员
     */
    public bool isRef = false;

   

    public void transformCb(string word)
    {
        /*
         * 筛选出中文
         */
        Regex reg = new Regex("[\u4e00-\u9fa5]+");
        string dealText = "";
        foreach (Match v in reg.Matches(word))
            dealText = dealText + v;
        char[] arr = dealText.ToCharArray();
        string[] aimTextArr = new string[arr.Length];
        string[] aimTextPingYingArr = new string[arr.Length];
        for (int z = 0; z < arr.Length; z++) aimTextArr[z] = arr[z].ToString();
        for (int z = 0; z < arr.Length; z++) aimTextPingYingArr[z] = PinYinHelper.ConvertToAllSpell(arr[z].ToString());
        textArr = aimTextArr;
        textPingYinArr = aimTextPingYingArr;
        strText = word;

        AudioEditManagercs.Instance.TransformAudioPartToWordCallBack();
    }
}

[Serializable]
public class AudioPartConfig
{
    public List<AudioPart> ListAudioParts;
}

[Serializable]
public class VoiceToWordParam
{
    public string format;
    public int rate;
    public int channel;
    public string token;
    public string cuid;
    public int len;
    public string speech;
}



public class AudioEditManagercs : MonoBehaviour, IDispose
{
    //---------------私有成员------------------------
    /*
     * 目前操作的音频总缓冲
     */
    private byte[] buffer_;

    /*
     * 目前的音频片段配置数据
     */
    private List<AudioPart> listAudioParts_;

    /*
     * 音频切割顺序坐标
     */
    private static int AudioIndex = 0;

    /*
     * 路径
     */
    private string strAudioPath_ = "";
    private string strOutPutPath_ = "";
    private ProcessCb processCb_ = null;
    private CallBackZero finishCb_ = null;
    private float nowProcess_ = 0;
    private int callBackTime = 0;
    private Coroutine corCut = null;
    private Coroutine corTrans = null;
    private bool isProcessing_ = false;


    //--------------常量-----------------------------
    private int SAMPLE_RATE = 16000;
    private int LOWER_VOICE = 10000;
    private string FLODER_NAME_FIRST_CUT = "firstCutTemp";
    private string AUDIO_PART_CONFIG_FILE_NAME = "AudioPartConfig";

    private string grant_Type = "client_credentials";
    private string client_ID = "这里输入百度的appkey，自己到官网申请填入这里";  //百度appkey
    private string client_Secret = "这里输入百度secretkey，自己到官网申请填写";  //百度Secret Key

    //-----------------------------------------------
    private static AudioEditManagercs instance_;
    public void Awake()
    {
        instance_ = this;
    }

    public static AudioEditManagercs Instance
    {
        get
        {
            // if (instance_ == null) { instance_ = new AudioEditManagercs(); }
            return instance_;
        }
    }

    private float NowProcess
    {
        get
        {
            return nowProcess_;
        }

        set
        {
            nowProcess_ = value;
            if (processCb_ != null)
            {
                processCb_(nowProcess_);
            }
        }
    }

    private void saveAudioPartConfig()
    {
        /*
         * 保存音频片段配置信息
         */
        AudioPartConfig audioPart = new AudioPartConfig();
        audioPart.ListAudioParts = listAudioParts_;
        string strJson = JsonUtility.ToJson(audioPart);
        FileUtil.CreateFile(strOutPutPath_, AUDIO_PART_CONFIG_FILE_NAME, strJson);
    }
    private void readAudioPartConfig()
    {
        string strJson = FileUtil.LoadFile(strOutPutPath_, AUDIO_PART_CONFIG_FILE_NAME);
        AudioPartConfig config = JsonUtility.FromJson<AudioPartConfig>(strJson);
        listAudioParts_ = config.ListAudioParts;
    }

    /*
     * 检查片段音频数组中的片段文件是否都存在
     */
    private bool checkAudioParts()
    {
        for (int i = 0; i < listAudioParts_.Count; ++i)
        {
            if (!File.Exists(listAudioParts_[i].audioPath))
            {
                return false;
            }
        }
        return true;
    }

    void cutAudioByDotOneTime(Byte[] audio, long startDotOneTime, long EndDotOneTime, List<AudioPart> audioListPart)
    {
        long startIndex = startDotOneTime * SAMPLE_RATE / 10 * 2;
        long endIndex = EndDotOneTime * SAMPLE_RATE / 10 * 2;
        byte[] newAudio = new byte[endIndex - startIndex];
        long index = 0;
        for (long i = startIndex; i <= endIndex; ++i)
        {
            if (index < newAudio.Length)
            {
                newAudio[index] = audio[i];
            }
            index++;
        }
        /*
         * 写入为音频文件到本地
         */

        string savePath = strOutPutPath_ + "\\" + FLODER_NAME_FIRST_CUT + "\\" + AudioIndex + ".pc";
        Debug.Log("savePath" + savePath);
        FileStream fs = new FileStream(savePath, FileMode.Create);
        fs.Write(newAudio, 0, newAudio.Length);
        fs.Close();
        AudioIndex++;

        AudioPart p = new AudioPart();
        p.audioPath = savePath;
        p.startIndex = startIndex;
        p.endIndex = endIndex;
        audioListPart.Add(p);
    }

    private IEnumerator cutAudio(List<AudioPart> listAudioParts, byte[] buffer, int cutTime, CallBackZero cb = null)
    {
        short[] intBuffer = new short[buffer.Length / 2];
        int index = 0;
        byte[] arr = new byte[2];
        float batchTime = 100000.0f;
        for (long i = 0; i < buffer.Length - 1; ++i)
        {
            if (i % 2 == 1)
            {

                arr[0] = buffer[i - 1];
                arr[1] = buffer[i];
                intBuffer[index] = BitConverter.ToInt16(arr, 0);
                index++;
            }
            /*
             * 更新进度
             */
            if (i % (int)batchTime == 0)
            {
                NowProcess += batchTime / (float)(buffer.Length - 1) * 0.2f;
                yield return new WaitForEndOfFrame();
            }
        }
        NowProcess = 0.2f;

        index = 0;
        long dotOneIndex = 0;
        long value = 0;
        long[] testDotOneTimeValue = new long[buffer.Length / (2 * SAMPLE_RATE / 10)];
        for (long i = 0; i < intBuffer.Length; ++i)
        {
            value += Math.Abs(intBuffer[i]);
            index++;
            if (index >= SAMPLE_RATE / 10)
            {
                index = 0;
                testDotOneTimeValue[dotOneIndex] = value;
                value = 0;
                dotOneIndex++;
            }

            if (i % (int)batchTime == 0)
            {
                NowProcess += batchTime / (float)(intBuffer.Length) * 0.2f;
                yield return new WaitForEndOfFrame();
            }
        }
        NowProcess = 0.4f;

        /*
         * 裁剪音频
         */
        bool isOnRecorder = false;
        int lowerTime = 0;
        long markStartIndex = 0;
        long markEndIndex = 0;
        for (long i = 0; i < testDotOneTimeValue.Length; ++i)
        {
            if (isOnRecorder == false)
            {
                if (testDotOneTimeValue[i] > LOWER_VOICE)
                {
                    markStartIndex = i;
                    isOnRecorder = true;
                }
            }
            else
            {
                if (testDotOneTimeValue[i] < LOWER_VOICE)
                {
                    lowerTime++;
                    if (lowerTime >= cutTime || i >= testDotOneTimeValue.Length - 1)
                    {
                        /*打断*/
                        isOnRecorder = false;
                        lowerTime = 0;
                        markEndIndex = i;
                        cutAudioByDotOneTime(buffer, markStartIndex, markEndIndex, listAudioParts);
                    }
                }
                else
                {
                    /*
                     * 出现回升就打断
                     */
                    lowerTime = 0;
                }
            }
            if (i % (int)batchTime == 0)
            {
                NowProcess += batchTime / (float)(testDotOneTimeValue.Length) * 0.2f;
                yield return new WaitForEndOfFrame();
            }
        }
        NowProcess = 0.5f;

        if (cb != null)
        {
            cb();
        }
    }


    //-------------私有方法------------------
    private void initBuffer(string audioPath)
    {
        FileStream stream = new FileInfo(audioPath).OpenRead();
        byte[] buffer = new byte[stream.Length];
        stream.Read(buffer, 0, Convert.ToInt32(stream.Length));
        buffer_ = buffer;
        stream.Close();
    }
    private void initListAudioPart(byte[] buffer, CallBackZero cb = null)
    {
        corCut = StartCoroutine(cutAudio(listAudioParts_, buffer, 4, cb));
    }
    private void transformAudioPartToWord(AudioPart audioPart)
    {
        var data = File.ReadAllBytes(audioPart.audioPath);
        BaiduManager.Instance.BytesToWord(data, audioPart.transformCb);
    }

    public void TransformAudioPartToWordCallBack()
    {
        if (isProcessing_)
        {
            callBackTime++;
            NowProcess = 0.5f + (((float)callBackTime) / (float)listAudioParts_.Count) / 2;
            if (callBackTime >= listAudioParts_.Count)
            {
                NowProcess = 1;
                saveAudioPartConfig();
                if (finishCb_ != null)
                {
                    finishCb_();
                }
                isProcessing_ = false;
            }
        }
    }

    IEnumerator transformAudioPartsToWord()
    {
        for (int i = 0; i < listAudioParts_.Count; ++i)
        {
            transformAudioPartToWord(listAudioParts_[i]);
            yield return new WaitForSeconds(0.1f);
        }
        yield return true;
    }

    private void initFolder()
    {
        FileUtil.TryCreateFolder(strOutPutPath_ + "\\" + FLODER_NAME_FIRST_CUT);
    }

    private void initListAudioPart(string jsonPart)
    {

    }

    /*
     * 首次拆分音频结束后的回调
     */
    private void onInitListAudioPartCallBack()
    {
        corCut = null;
        corTrans = StartCoroutine(transformAudioPartsToWord());
    }

    //-------------对外接口------------------

    public void initWithNew(
        string strAudioPath,
        string outPutPath,
        ProcessCb cb = null,
        CallBackZero finishCb = null)
    {

        /*
         * 如果文件夹已经存在则删除
         */
        if (Directory.Exists(strOutPutPath_ + "\\" + FLODER_NAME_FIRST_CUT))
        {
            Directory.Delete(strOutPutPath_ + "\\" + FLODER_NAME_FIRST_CUT, true);
        }

        /*
         * 初始化
         */
        strAudioPath_ = strAudioPath;
        strOutPutPath_ = outPutPath;
        processCb_ = cb;
        finishCb_ = finishCb;
        listAudioParts_ = new List<AudioPart>();
        initFolder();
        initBuffer(strAudioPath);

        /*
         * 大规模工作集合
         */
        isProcessing_ = true;
        initListAudioPart(buffer_, onInitListAudioPartCallBack);
    }

    public void CancleProcess()
    {
        /*
         * 终止进程携程
         */
        isProcessing_ = false;
        if (corCut != null)
            StopCoroutine(corCut);
        if (corTrans != null)
            StopCoroutine(corTrans);

        /*
         * 删除项目文件
         */
        Directory.Delete(strOutPutPath_ + "\\" + FLODER_NAME_FIRST_CUT, true);
    }

    public bool initWithOld(string outPutPath, string audioPath)
    {
        strOutPutPath_ = outPutPath;
        if (File.Exists(strOutPutPath_ + "\\" + AUDIO_PART_CONFIG_FILE_NAME))
        {
            readAudioPartConfig();
            if (checkAudioParts())
            {
                initBuffer(audioPath);
                return true;
            }
            else
            {
                return false;
            }
        }
        return false;
    }

    public List<AudioPart> GetAudioPartsList()
    {
        return listAudioParts_;
    }

    public int GetIndexByAudioPart(AudioPart ap)
    {
        for (int i = 0; i < listAudioParts_.Count; ++i)
        {
            if (ap.audioPath == listAudioParts_[i].audioPath) { return i; }
        }
        return -1;
    }
    public AudioPart GetAudioPartByIndex(int index)
    {
        if (index < 0 || index >= listAudioParts_.Count) { return null; }
        return listAudioParts_[index];
    }
    public AudioPart GetAudioPartByAudioPart(AudioPart ap) {
        for (int i = 0; i < listAudioParts_.Count; ++i)
        {
            if (ap.audioPath == listAudioParts_[i].audioPath) {
                return listAudioParts_[i];
            }
        }
        return null;
    }

    public void Dispose()
    {
        corCut = null;
        corTrans = null;
        nowProcess_ = 0;
        finishCb_ = null;
        processCb_ = null;
        callBackTime = 0;
        AudioIndex = 0;
        buffer_ = null;
        listAudioParts_.Clear();
    }
}
