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

    //---------------生命周期方法-------------------
	// Use this for initialization
	void Start () {
		
	}
    //--------------重写方法-----------------------
    public override void Dispose() {

    }

    public override void UpdateContent(TaskResult itemData) {
        TextResultId.text = ""+ itemData.ListAudioParts[0].strText;
        TextMatchRate.text = ""+ itemData.MarchRate;
    }

    //--------------私有方法------------------------
    private void onBtnClickPlay() { }
    private void onBtnClickEdit() { }
    private void onBtnClickSave() { }

}
