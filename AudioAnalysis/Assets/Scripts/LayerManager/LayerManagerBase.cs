using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerManagerBase : MonoBehaviour {
    //------------私有成员---------------
    private CanvasManager cacheCanvasManager_;
    //------------属性-------------------
    public CanvasManager CacheCanvasManager {
        get {
            if (cacheCanvasManager_ == null) {
                cacheCanvasManager_ = transform.parent.GetComponent<CanvasManager>();
            }
            return cacheCanvasManager_;
        }
    }
}
