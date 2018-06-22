using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class CellTaskResult : TableViewCell<TaskResult>
{
    //---------------公有成员-----------------------

    public Button BtnPlay;
    public Button BtnEdit;
    public Button BtnSave;

    public Text TextResultId;
    public Text TextMatchRate;
    //---------------私有成员-----------------------
    private TaskResult data_;
    private EditorResultLayer cachemanager_;

    public EditorResultLayer Cachemanager
    {
        get
        {
            if (cachemanager_ == null) {
                cachemanager_ = transform.parent.parent.parent.GetComponent<EditorResultLayer>();
            }
            return cachemanager_;
        }
    }


    //---------------生命周期方法-------------------
    // Use this for initialization
    void Start () {
        BtnPlay.onClick.AddListener(onBtnClickPlay);
        BtnEdit.onClick.AddListener(onBtnClickEdit);
        BtnSave.onClick.AddListener(onBtnClickSave);
	}
    //--------------重写方法-----------------------
    public override void Dispose() {

    }

    public override void UpdateContent(TaskResult itemData) {
        if (itemData.ListAudioParts.Count>0) {
            TextResultId.text = "" + itemData.ListAudioParts[0].strText;
            TextMatchRate.text = "" + itemData.MarchRate;
        }
        data_ = itemData;
    }

    //--------------私有方法------------------------
    private void onBtnClickPlay() {
        TaskResult tr = data_;
        string[] paths = new string[tr.ListAudioParts.Count];
        for (int i = 0; i < paths.Length; ++i)
        {
            paths[i] = tr.ListAudioParts[i].audioPath;
        }
        PCMPlayer.Instance.PlayerLocalFiles(paths);
    }
    private void onBtnClickEdit() {
        if (Cachemanager !=null&&data_!=null) {
            Cachemanager.LocalToResultAudioPart(data_);
        }
    }
    private void onBtnClickSave() {
        Cachemanager.SaveResult();
        UpdateContent(data_);
    }

}
