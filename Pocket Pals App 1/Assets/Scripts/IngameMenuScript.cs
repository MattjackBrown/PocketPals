using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngameMenuScript : MonoBehaviour {

    public List<GameObject> SubMenus;

    public TouchHandler touchHandler;

    private void OnDisable()
    {
        touchHandler.MenuControls();
    }

    void OnEnable()
    {
        foreach (GameObject obj in SubMenus)
        {
            obj.SetActive(false);
        }
        touchHandler.MapControls();
    }
}
