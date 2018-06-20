using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LMEditor : LayerManagerBase
{

    //--------------公有成员-----------------------
    public Button BtnBack;
    public Button BtnPublish;


    public Slider SliderTableViewTask;
    public TableViewTasks MyTableViewTasks;
    public GameObject GTaskProcess;
    public GameObject PrefabTaskProcessPart;
    public List<GameObject> ListTaskProcessPart = new List<GameObject>();
    //--------------私有成员----------------------
    private List<TaskTransform> listTasks_ = null;

    //--------------生命周期方法-------------------
    void Start()
    {
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
        for (int i = 0; i < listTasks.Count; ++i) {
            GameObject part = GameObject.Instantiate(PrefabTaskProcessPart);
            GameObjectUtil.SetWidth(part, length);
            if (listTasks[i].IsFinish) {
                GameObjectUtil.SetColor(part, Color.blue);
            }
            else {
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
    private void onValueChangeSliderTableViewTask(float value)
    {
        MyTableViewTasks.MoveTableViewContentToRate(value);
    }
    private void onValueChangeTableViewTask(float rate)
    {
        SliderTableViewTask.value = rate;
    }
    //-------------私有方法------------------------
    private void setAudioPartFinishOrNotOnSlider(int index, bool isFinish) {
        if (index < 0 || index >= ListTaskProcessPart.Count) return;
        GameObject g = ListTaskProcessPart[index];
        if (isFinish) {
            GameObjectUtil.SetColor(g, Color.blue);
        }
        else {
            GameObjectUtil.SetColor(g, Color.red);
        }
    }
    private int findTaskAtInidex(TaskTransform task) {
        for (int i= 0;i<listTasks_.Count;++i) {
            if (listTasks_[i] == task) {
                return i;
            }
        }
        return -1;
    }

    //---------------对外接口---------------------
    /*
     * 标记任务完成
     */
    public void MarkFinish(TaskTransform task) {
        int index = findTaskAtInidex(task);
        if (index != -1) {
            task.IsFinish = true;
            setAudioPartFinishOrNotOnSlider(index, task.IsFinish);
        }
    }

    /*
     * 标记任务未玩成
     */
    public void MarkUnFinish(TaskTransform task) {
        int index = findTaskAtInidex(task);
        if (index != -1)
        {
            task.IsFinish = false;
            setAudioPartFinishOrNotOnSlider(index, task.IsFinish);
        }
    }

}
