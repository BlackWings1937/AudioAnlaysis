using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class EditorResultLayer : MonoBehaviour,IDispose {

    //-------------公有成员----------------------
    public Button BtnBack;
    public TableViewTaskResult MyTableViewTaskResult;

    //--------------私有成员---------------------
    private TaskTransform t_;
    private LMEditor cacheLMEditor_;

    public LMEditor CacheLMEditor
    {
        get
        {
            if (cacheLMEditor_ == null) { cacheLMEditor_ = this.transform.parent.GetComponent<LMEditor>(); }
            return cacheLMEditor_;
        }
    }

    //-------------生命周期方法------------------
    // Use this for initialization
    void Start () {
        BtnBack.onClick.AddListener(onBtnClickBack);
	}

    //---------------UI事件----------------------
    private void onBtnClickBack() {
        CacheLMEditor.CloseEditTaskView();
    }

    //--------------对外接口--------------------
    public void SetData(TaskTransform t) {
        t_ = t;
        t_.ListTaskResult.Sort(
            delegate(TaskResult a,TaskResult b) {
                if (a.MarchRate > b.MarchRate)
                {
                    return -1;
                }
                else { return 1; }
        });
        MyTableViewTaskResult.SetData(t_.ListTaskResult);
    }

    public void Dispose()
    {

    }
}
