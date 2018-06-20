using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class CellTask : TableViewCell<TaskTransform> {
    //--------------公有成员-------------------
    public Text TextAudioID = null;
    public Text TextAudioTask = null;
    public Text TextAudioExcel = null;

    public GameObject GFinishBG = null;
    public GameObject GUnFinishBG = null;

    public Button BtnPlay = null;
    public Button BtnEdit = null;
    public Button BtnMarkFinish = null;
    public Button BtnMarkUnFinish = null;

    //-------------私有成员--------------------
    private TaskTransform data_ = null;

    //--------------生命周期方法---------------
	void Start () {
        BtnPlay.onClick.AddListener(onBtnClickPlay);
        BtnEdit.onClick.AddListener(onBtnClickEdit);
        BtnMarkFinish.onClick.AddListener(onBtnClickMarkFinish);
        BtnMarkUnFinish.onClick.AddListener(onBtnClickMarkUnFinish);
    }

    //------------重写方法---------------------
    public override void UpdateContent(TaskTransform itemData)
    {
        /*
         * 注销事件
         */
        if (data_ != null) {
            data_.OnIsFinishChange -= onIsFinishChange;
        }

        TextAudioID.text = itemData.StrAudioId;
        TextAudioTask.text = itemData.StrTaskWord;
        TextAudioExcel.text = ""+itemData.ExcelCol;
        GFinishBG.SetActive(itemData.IsFinish);
        GUnFinishBG.SetActive(!itemData.IsFinish);

        /*
         * 注册事件
         */
        data_ = itemData;
        data_.OnIsFinishChange+= onIsFinishChange;
    }

    public override void Dispose()
    {
        if (data_ != null)
        {
            data_.OnIsFinishChange -= onIsFinishChange;
        }
    }


    //-------------私有方法-------------------
    private void onIsFinishChange(bool isFinish) {
        GFinishBG.SetActive(isFinish);
        GUnFinishBG.SetActive(!isFinish);
    }

    //--------------UI相应事件-----------------
    public void onBtnClickPlay() { }
    public void onBtnClickEdit() { }

    public void onBtnClickMarkFinish() {
        CacheManager.GetComponent<LMEditor>().MarkFinish(data_);
    }
    public void onBtnClickMarkUnFinish() {
        CacheManager.GetComponent<LMEditor>().MarkUnFinish(data_);
    }
}
