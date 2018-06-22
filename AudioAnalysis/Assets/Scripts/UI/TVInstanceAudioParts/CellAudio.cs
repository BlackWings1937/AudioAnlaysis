using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class CellAudio : TableViewCell<AudioPart>
{
    //---------------公有成员----------------------
    public Text TextAudioTransformText;
    public Button BtnPlay;
    public Button BtnCut;
    public Toggle ToggleIsUse;
    //---------------私有成员----------------------
    private AudioPart data_;
    private EditorResultLayer cacheEditResultLayer_;

    public EditorResultLayer CacheEditResultLayer
    {
        get
        {
            if (cacheEditResultLayer_ == null) { cacheEditResultLayer_ = transform.parent.parent.parent.GetComponent<EditorResultLayer>(); }
            return cacheEditResultLayer_;
        }
    }

    //---------------生命周期方法------------------
    // Use this for initialization
    void Start () {
        BtnPlay.onClick.AddListener(onBtnClickPlay);
        BtnCut.onClick.AddListener(onBtnClickCut);
        ToggleIsUse.onValueChanged.AddListener(onToggleValueChangeIsUse);
        
    }
    //---------------重写方法----------------------
    public override void Dispose()
    {
    }

    public override void UpdateContent(AudioPart itemData)
    {
        /*
         * 注销事件
         */
        if (data_!= null) {
            data_.OnValueChangeIsUse -= OnValueChangeIsUse;
        }
        TextAudioTransformText.text = itemData.strText;
        data_ = itemData;

        /*
         * 初始化是否选中状态
         */
        OnValueChangeIsUse(itemData.IsUse);

        /*
         * 注册事件
         */
        if (data_!= null) {
            data_.OnValueChangeIsUse += OnValueChangeIsUse;
        }
    }
    //---------------私有方法----------------------
    private void OnValueChangeIsUse(bool isUse) {
        /*
         * 触发更变事件
         */
        ToggleIsUse.isOn = isUse;
    }

    //---------------UI事件------------------------
    private void onBtnClickPlay() {
        string[] paths = new string[1];
        paths[0] = data_.audioPath;
        PCMPlayer.Instance.PlayerLocalFiles(paths);
    }
    private void onBtnClickCut() {
        /*
         * 打开图形化的此audioPart的波形文件
         */

    }

    private void onToggleValueChangeIsUse(bool value) {
        if (data_!=null) {
            data_.IsUse = value;
        }
    }
}
