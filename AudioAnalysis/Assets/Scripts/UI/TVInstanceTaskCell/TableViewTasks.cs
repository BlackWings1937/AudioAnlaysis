using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class TableViewTasks : TableViewController<TaskTransform>
{
    public void SetData(List<TaskTransform> data)
    {
        if (tableData.Count <= 0)
        {
            tableData = data;
            UpdateContents();
        }
        else
        {
            RefreshCell();
        }
    }
    protected override float CellHeightAtIndex(int index)
    {
        return 300;
    }

    protected override float CellWidthAtIndex(int index)
    {
        return 250;
    }
}
