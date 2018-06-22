using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class TableViewAudios : TableViewController<AudioPart>
{

    public void SetData(List<AudioPart> data)
    {
        tableData = data;
        UpdateContents();
    }
    protected override float CellWidthAtIndex(int index)
    {
        return 250;
    }
}
