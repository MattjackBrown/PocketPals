using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngameMenuScript : MonoBehaviour {

    public List<GameObject> SubMenus;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnEnable()
    {
        foreach (GameObject obj in SubMenus)
        {
            obj.SetActive(false);
        }
    }
}
