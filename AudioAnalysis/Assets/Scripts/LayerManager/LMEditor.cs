using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LMEditor : LayerManagerBase
{

    //--------------公有成员-----------------------
    public Button BtnBack;
    public Button BtnPublish;
    public Button BtnStartMatch;


    public Slider SliderTableViewTask;
    public TableViewTasks MyTableViewTasks;
    public GameObject GTaskProcess;
    public GameObject PrefabTaskProcessPart;
    public List<GameObject> ListTaskProcessPart = new List<GameObject>();
    public EditorResultLayer EditResultLayer;
    //--------------私有成员----------------------
    private List<TaskTransform> listTasks_ = null;

    //--------------生命周期方法-------------------
    void Start()
    {
        BtnStartMatch.onClick.AddListener(onBtnClickProcessMarching);
        BtnBack.onClick.AddListener(onBtnClickBack);
        BtnPublish.onClick.AddListener(onBtnClickPublish);
        SliderTableViewTask.onValueChanged.AddListener(onValueChangeSliderTableViewTask);
    }

    //-------------重写方法------------------------
    public override void StartWithParam(ModuleParamBase param)
    {
        /*
         * 初始化ui 信息
         */
        List<TaskTransform> listTasks = TaskManager.Instance.GetTasks();
        listTasks_ = listTasks;
        /*
         * 初始化列表
         */
        MyTableViewTasks.SetData(listTasks_);

        /*
         * 初始化事件回调
         */
        MyTableViewTasks.OnValueRateChange += onValueChangeTableViewTask;

        /*
         * 初始化进度条
         */
        float width = GameObjectUtil.GetWidth(GTaskProcess);
        float length = width / (float)listTasks.Count;
        for (int i = 0; i < listTasks.Count; ++i)
        {
            GameObject part = GameObject.Instantiate(PrefabTaskProcessPart);
            GameObjectUtil.SetWidth(part, length);
            if (listTasks[i].IsFinish)
            {
                GameObjectUtil.SetColor(part, Color.blue);
            }
            else
            {
                GameObjectUtil.SetColor(part, Color.red);
            }
            GameObjectUtil.SetX(part, i * length);
            GameObjectUtil.SetParentToChild(GTaskProcess, part, false);
            ListTaskProcessPart.Add(part);
        }
    }

    public override void Dispose()
    {
        /*
         * 重置所有ui 界面
         */
        listTasks_ = null;
        MyTableViewTasks.OnValueRateChange -= onValueChangeTableViewTask;
        ListTaskProcessPart.Clear();
    }

    //---------------UI事件------------------------

    private void onBtnClickBack()
    {
        TaskManager.Instance.SaveTasksConfig();
        TaskManager.Instance.Dispose();
        AudioEditManagercs.Instance.Dispose();
        CacheCanvasManager.StartModule(CanvasManager.ModuleType.E_START);
    }
    private void onBtnClickPublish()
    {

    }
    private void onBtnClickProcessMarching()
    {
        processMarching();
    }
    private void onValueChangeSliderTableViewTask(float value)
    {
        MyTableViewTasks.MoveTableViewContentToRate(value);
    }
    private void onValueChangeTableViewTask(float rate)
    {
        SliderTableViewTask.value = rate;
    }
    //-------------私有方法------------------------

    /*
     * 生成比较结果的进程
     */
    private void processMarching()
    {
        List<TaskTransform> listTasks = TaskManager.Instance.GetTasks();
        List<AudioPart> listAudioParts = AudioEditManagercs.Instance.GetAudioPartsList();
        /*
         * 生成第一次比较结果
         */
        TaskTransform t = null;
        AudioPart a = null;
        TaskResult tr = null;
        float matchRateThanTask = 0;
        float matchRateThanAudioPart = 0;

        float LOWER_MAIN_BODY_MARCH_RATE = 0.5f;
        int startMarchIndex = 0;
        int stopMarchIndex = 0;
        for (int i = 0; i < listTasks.Count; ++i)
        {
            t = listTasks[i];
            t.ListTaskResult.Clear();
            for (int z = 0; z < listAudioParts.Count; ++z)
            {
                a = listAudioParts[z];
                matchRateThanTask = ArrayUtil<string>.CaculateRegularSimilar(t.arrStrPingYing, a.textPingYinArr) ; //StringUtil.CaculateStringSimilar(t.StrTaskWord, a.strText);// 解决复杂度
                matchRateThanAudioPart = ArrayUtil<string>.CaculateRegularSimilar(a.textPingYinArr, t.arrStrPingYing);//StringUtil.CaculateStringSimilar(a.strText, t.StrTaskWord);
                if (matchRateThanTask >= LOWER_MAIN_BODY_MARCH_RATE)
                {

                    tr = new TaskResult();
                    tr.MarchRate = (matchRateThanTask + matchRateThanAudioPart) / 2.0f;
                    tr.ListAudioParts.Add(a);
                    t.ListTaskResult.Add(tr);
  
                    if (matchRateThanAudioPart > matchRateThanTask)
                    {
                        tr = new TaskResult();
                        tr.MarchRate = (matchRateThanTask + matchRateThanAudioPart) / 2.0f - 0.1f;
                        int index = AudioEditManagercs.Instance.GetIndexByAudioPart(a);
                        if (index != -1) {
                            for (int x = 0; x < 3; x++)
                            {
                                AudioPart apNow = AudioEditManagercs.Instance.GetAudioPartByIndex(index - 1 + x);
                                tr.ListAudioParts.Add(apNow);
                            }
                            t.ListTaskResult.Add(tr);
                        }
                    }
                }
            }
        }


        TaskManager.Instance.SaveTasksConfig();
    }

    /*
     * 主体相似度比较方法
     */



    private void setAudioPartFinishOrNotOnSlider(int index, bool isFinish)
    {
        if (index < 0 || index >= ListTaskProcessPart.Count) return;
        GameObject g = ListTaskProcessPart[index];
        if (isFinish)
        {
            GameObjectUtil.SetColor(g, Color.blue);
        }
        else
        {
            GameObjectUtil.SetColor(g, Color.red);
        }
    }
    private int findTaskAtInidex(TaskTransform task)
    {
        for (int i = 0; i < listTasks_.Count; ++i)
        {
            if (listTasks_[i] == task)
            {
                return i;
            }
        }
        return -1;
    }

    //---------------对外接口---------------------
    /*
     * 标记任务完成
     */
    public void MarkFinish(TaskTransform task)
    {
        int index = findTaskAtInidex(task);
        if (index != -1)
        {
            task.IsFinish = true;
            setAudioPartFinishOrNotOnSlider(index, task.IsFinish);
        }
    }

    /*
     * 标记任务未玩成
     */
    public void MarkUnFinish(TaskTransform task)
    {
        int index = findTaskAtInidex(task);
        if (index != -1)
        {
            task.IsFinish = false;
            setAudioPartFinishOrNotOnSlider(index, task.IsFinish);
        }
    }

    /*
     * 打开任务编辑层
     */
    public void OpenEditTaskView(TaskTransform task) {
        EditResultLayer.SetData(task);
        EditResultLayer.gameObject.SetActive(true);
    }

    /*
     * 关闭任务编辑成
     */
    public void CloseEditTaskView() {
        EditResultLayer.Dispose();
        EditResultLayer.gameObject.SetActive(false);
    }

}
