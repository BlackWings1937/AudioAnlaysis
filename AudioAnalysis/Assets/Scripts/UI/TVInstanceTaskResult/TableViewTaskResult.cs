using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class TableViewTaskResult : TableViewController<TaskResult>
{
    public void SetData(List<TaskResult> data)
    {
        tableData = data;
        UpdateContents();
    }
    protected override float CellWidthAtIndex(int index)
    {
        return 250;
    }
}
