using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LMStart : LayerManagerBase {

    public Button BtnOpenNew;
    public Button BtnOpenOld;

	// Use this for initialization
	void Start () {
        BtnOpenNew.onClick.AddListener(onBtnClickOpenNew);
        BtnOpenOld.onClick.AddListener(onBtnClickOpenOld);
	}

    //--------------------------UI事件-------------------------
    private void onBtnClickOpenNew() { CacheCanvasManager.StartModule(CanvasManager.ModuleType.E_OPENNEW); }
    private void onBtnClickOpenOld() { CacheCanvasManager.StartModule(CanvasManager.ModuleType.E_OPENOLD); }
}
