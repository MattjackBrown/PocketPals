using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngameMenuScript : MonoBehaviour {

	public Animator anim;

    public List<GameObject> SubMenus;

    public TouchHandler touchHandler;

	UIAnimationManager animManager;

	void Start () {
		animManager = GetComponentInParent<UIAnimationManager> ();
	}

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

		//animManager.OpenUI (anim);
    }

	public void CloseAnimation () {

		animManager.CloseUI (anim);

	}
}
