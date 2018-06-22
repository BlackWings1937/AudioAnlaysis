using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class EditorResultLayer : MonoBehaviour, IDispose
{

    //-------------公有成员----------------------
    public Button BtnBack;
    public Button BtnCreateResult;
    public TableViewTaskResult MyTableViewTaskResult;
    public TableViewAudios MyTableViewAudios;

    //--------------私有成员---------------------
    private TaskTransform t_;
    private LMEditor cacheLMEditor_;
    private TaskResult tr_;

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
    void Start()
    {
        BtnBack.onClick.AddListener(onBtnClickBack);
        BtnCreateResult.onClick.AddListener(onBtnClickCreateResult);
    }

    //---------------UI事件----------------------
    private void onBtnClickBack()
    {
        CacheLMEditor.CloseEditTaskView();
    }
    private void onBtnClickCreateResult()
    {
        TaskResult tr = new TaskResult();
        t_.ListTaskResult.Add(tr);
        MyTableViewTaskResult.SetData(t_.ListTaskResult);
    }
    //--------------对外接口--------------------
    /*
     * 设置结果
     */
    public void SetData(TaskTransform t)
    {
        t_ = t;
        t_.SortTaskResults();
        MyTableViewTaskResult.SetData(t_.ListTaskResult);
        MyTableViewAudios.SetData(AudioEditManagercs.Instance.GetAudioPartsList());
    }

    /*
     * 编辑具体的一个结果
     */
    public void LocalToResultAudioPart(TaskResult tr)
    {
        if (tr == tr_) return;
        /*
         * 取消先前方案所有标记的语音
         */
        if (tr_ != null)
        {
            SaveResult();
        }

        MyTableViewAudios.gameObject.SetActive(true);
        if (tr == null) return;
        /*
         * 标记所有使用的语音片段
         */
        if (tr.ListAudioParts.Count > 0)
        {
            for (int i = 0; i < tr.ListAudioParts.Count; ++i)
            {
                AudioPart nowAp = AudioEditManagercs.Instance.GetAudioPartByAudioPart(
                    tr.ListAudioParts[i]
                    );
                nowAp.IsUse = true;
            }
            /*
             * 滚动到定位点
             */
            AudioPart ap = tr.ListAudioParts[0];
            int index = AudioEditManagercs.Instance.GetIndexByAudioPart(ap);
            MyTableViewAudios.MoveTableViewContentToIndex(index);
        }
        Debug.Log("audioCutNum:" + tr.ListAudioParts.Count);
        tr_ = tr;
    }

    /*
     * 添加使用标记
     */
    public void AddAudioPartToResult(AudioPart ap) {

    }

    /*
     * 取消使用标记
     */
    public void SubAudioPartToResult(AudioPart ap) {

    }

    /*
     * 保存方案
     */
    public void SaveResult() {
        List<AudioPart> listOfResult = new List<AudioPart>();
        /*
         * 清空临时状态
         */
        List<AudioPart> listAll = AudioEditManagercs.Instance.GetAudioPartsList();
        for (int i = 0;i<listAll.Count;++i) {
            if (listAll[i].IsUse) {
                listOfResult.Add(listAll[i]);
            }
            listAll[i].IsUse = false;
        }
        if(tr_!= null)
        tr_.ListAudioParts = listOfResult;
        MyTableViewAudios.gameObject.SetActive(false);
        tr_ = null;
    }


    public void Dispose()
    {
        tr_ = null;
    }
}
