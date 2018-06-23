using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using Excel;
using System.Data;

[Serializable]
public class TaskResult
{
    public List<AudioPart> ListAudioParts = new List<AudioPart>();
    public float MarchRate;
    public int StartMarchIndex;
    public int StopMarchIndex;
    public TaskTransform task;
}

[Serializable]
public class TaskResultsGroup {
    public List<TaskResult> ListResults;
}

[Serializable]
public class TaskTransform
{
    
    public TaskResult TopResult = null;
    public List<TaskResult> ListTaskResult = new List<TaskResult>();
    public string PathOfListTaskResult;
    public string StrAudioId;
    public string StrTaskWord;
    public string[] arrStrPingYing;
    public int ExcelCol;
    public event CallBackBool OnIsFinishChange;
    public bool isFinish = false;

    /*
     * 常数
     */
    public const string FILE_NAME_PRE = "TaskTransform:";
    public const string FOLDER_NAME_RESULTS_CONFIG = "ResultConfig";

    /*
     * 保存可行方案到本地
     */
    public void SaveResultToLocalByOutPutPath(string outputPath) {
        TaskResultsGroup g = new TaskResultsGroup();
        g.ListResults = ListTaskResult;
        string strJson = JsonUtility.ToJson(g);
        FileUtil.CreateFile(
            outputPath+"\\"+ FOLDER_NAME_RESULTS_CONFIG
            , FILE_NAME_PRE+ StrTaskWord,
            strJson);
    }

    /*
     * 读取所有方案到内存
     */
    public void ReadResultToMemory(string outputPath) {
        string strJson = FileUtil.LoadFile(outputPath + "\\" + FOLDER_NAME_RESULTS_CONFIG, FILE_NAME_PRE + StrTaskWord);
        TaskResultsGroup g = JsonUtility.FromJson<TaskResultsGroup>(strJson);
        ListTaskResult = g.ListResults;
    }

    public bool IsFinish
    {
        get
        {
            return isFinish;
        }

        set
        {
            isFinish = value;
            if (OnIsFinishChange != null)
            {
                OnIsFinishChange.Invoke(isFinish);
            }
        }
    }
}

[Serializable]
public class TasksConfig
{
    public List<TaskTransform> ListTasks;
}

public class TaskManager : MonoBehaviour, IDisposable
{

    private static TaskManager instance_;

    public static TaskManager Instance
    {
        get
        {
            return instance_;
        }
    }
    //-----------------私有成员----------------
    private List<TaskTransform> listTasks_ = new List<TaskTransform>();
    private string strOutPutPath_ = "";
    //------------------生命周期方法-----------
    public void Start()
    {
        instance_ = this;
    }
    //------------------常量-------------------
    public const string FILE_NAME_TASKS_CONFIG = "tasksConfig";

    //------------------私有方法----------------

    //------------------对外接口----------------

    public bool InitByExcel(string path, string outputPath)
    {
        strOutPutPath_ = outputPath;
        //if (File.Exists(path))
        // {
        FileStream stream = File.Open(path, FileMode.Open, FileAccess.Read);
        IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
        var result = excelReader.AsDataSet();

        int index = 0;
        int y = 0;
        //int columns = result.Tables[0].Columns.Count;//获取列数
        int rows = result.Tables[0].Rows.Count;//获取行数
        while (true)
        {
            
            y = index + 1;
            if (rows <= y)
            {
                break;
            }
            string tempword = result.Tables[0].Rows[y][2].ToString();
            string tempAudio = result.Tables[0].Rows[y][0].ToString();
            Debug.Log("tempword" + tempword);
            TaskTransform t = new TaskTransform();
            t.ExcelCol = y;
            t.StrAudioId = tempAudio;
            t.StrTaskWord = tempword;
            t.arrStrPingYing = StringUtil.GetPingYingArrayFromChinaString(StringUtil.GetChinaWordFromString(t.StrTaskWord));
            listTasks_.Add(t);
            index++;
        }

        TasksConfig tc = new TasksConfig();
        tc.ListTasks = listTasks_;
        string strJson = JsonUtility.ToJson(tc);
        FileUtil.CreateFile(outputPath, FILE_NAME_TASKS_CONFIG, strJson);

        stream.Close();
        return true;
        // } //else { return false; }
    }
    public bool InitByConfig(string outputPath)
    {
        strOutPutPath_ = outputPath;
        if (File.Exists(outputPath + "\\" + FILE_NAME_TASKS_CONFIG))
        {
            string strJson = FileUtil.LoadFile(outputPath, FILE_NAME_TASKS_CONFIG);
            TasksConfig tc = JsonUtility.FromJson<TasksConfig>(strJson);
            listTasks_ = tc.ListTasks;
            return true;
        }
        else
        {
            return false;
        }
    }

    public void SaveTasksConfig() {
        TasksConfig tc = new TasksConfig();
        tc.ListTasks = listTasks_;
        string strJson = JsonUtility.ToJson(tc);
        FileUtil.CreateFile(strOutPutPath_, FILE_NAME_TASKS_CONFIG, strJson);
    }

    public List<TaskTransform> GetTasks()
    {
        return listTasks_;
    }
    //------------------实现接口-----------------
    public void Dispose()
    {
        strOutPutPath_ = "";
        listTasks_.Clear();
    }


}
