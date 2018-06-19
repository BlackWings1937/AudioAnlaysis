using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
public delegate void CallBack(string word);

public class BaiduManager : MonoBehaviour {

    public static string audioToString;
    private string grant_Type = "client_credentials";
    private string client_ID = "rDveYNZ6SY4IY48VPbKYPR4H";  //百度appkey
    private string client_Secret = "m3Moj6DN8C3TffFiSkA4Gn3X3G5od5gM";  //百度Secret Key
    private string token = "";                           //access_token
    private string cuid = "11";        //用户标识
    private string format = "pcm";                  //语音格式
    private int rate = 16000;                        //采样率
    private int channel = 1;                        //声道数
    private string speech;                          //语音数据，进行base64编码
    private int len;                                //原始语音长度
    private string getTokenAPIPath = "https://openapi.baidu.com/oauth/2.0/token?";
    private string baiduAPI = "http://vop.baidu.com/server_api";

    private IEnumerator GetToken(string url)
    {
        WWWForm getTForm = new WWWForm();
        getTForm.AddField("grant_type", grant_Type);
        getTForm.AddField("client_id", client_ID);
        getTForm.AddField("client_secret", client_Secret);

        WWW getTW = new WWW(url, getTForm);
        yield return getTW;
        if (getTW.isDone)
        {
            if (getTW.error == null)
            {
                token = JsonMapper.ToObject(getTW.text)["access_token"].ToString();
                Debug.Log("获取百度用户令牌 初始化完成");
            }
            else
                Debug.Log("error:" + getTW.error);
        }
    }

    private IEnumerator GetAudioString(string url, CallBack cb)
    {
        JsonWriter jw = new JsonWriter();
        jw.WriteObjectStart();
        jw.WritePropertyName("format");
        jw.Write(format);
        jw.WritePropertyName("rate");
        jw.Write(rate);
        jw.WritePropertyName("channel");
        jw.Write(channel);
        jw.WritePropertyName("token");
        jw.Write(token);
        jw.WritePropertyName("cuid");
        jw.Write(cuid);
        jw.WritePropertyName("len");
        jw.Write(len);
        jw.WritePropertyName("speech");
        jw.Write(speech);
        jw.WriteObjectEnd();
        WWWForm w = new WWWForm();
        WWW getASW = new WWW(url, Encoding.Default.GetBytes(jw.ToString()));
        yield return getASW;
        if (getASW.isDone)
        {
            if (getASW.error == null)
            {
                JsonData getASWJson = JsonMapper.ToObject(getASW.text);
                if (getASWJson["err_msg"].ToString() == "success.")
                {
                    audioToString = getASWJson["result"][0].ToString();
                    if (audioToString.Substring(audioToString.Length - 1) == "，")
                        audioToString = audioToString.Substring(0, audioToString.Length - 1);
                }
            }
            else
            {
                //Debug.LogError(getASW.error);
                audioToString = "";
                Debug.Log("error:" + getASW.error);
            }
            Debug.Log("此次语音文字为：" + audioToString);
            if (cb != null)
            {
                cb(audioToString);
            }
        }
    }

    public string BytesToWord(byte [] buffer ,CallBack cb) {
        len = buffer.Length;
        speech = Convert.ToBase64String(buffer);
        StartCoroutine(GetAudioString(baiduAPI, cb));
        return "";
    }

    // Use this for initialization
    void Start () {
        StartCoroutine(GetToken(getTokenAPIPath));
        instance_ = this;

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private static BaiduManager instance_;

    public static BaiduManager Instance
    {
        get
        {
            return instance_;
        }

    }

    /*private void OnGUI()
    {
        if (GUILayout.Button("Start")) {
            FileStream stream = new FileInfo("C:\\Users\\DELL\\Desktop\\OutPutPath\\firstCutTemp\\10.pc").OpenRead();
            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer, 0, Convert.ToInt32(stream.Length));
            BytesToWord(buffer);
            stream.Close();
        }
    }*/
}
