using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceSpotParent : MonoBehaviour
{
    private Animator anim;

    public bool Used = false;

    private void Start()
    {
        gameObject.transform.Rotate(new Vector3(0,Random.Range(0,360), 0));
        anim = GetComponent<Animator>();
    }



	public void Init() {
        // Called from the camera controller the moment it has fully zoomed in

		// Temp. Anything else?
		Clicked();
	}

    public bool IsUsed()
    {
        if (Used)
        {
            return true;
        }
        else
        {
            Used = true;
            return false;
        }
    }

    public void Clicked()
    {

        anim.SetBool("Clicked", true);
    }
    public void Finished()
    {
        anim.SetBool("Clicked", false);
        transform.GetChild(0).gameObject.SetActive(false);

        LocalDataManager.Instance.AddItem(AssetManager.Instance.GetRandomItem());

        //To DO: Zoom back out
        CameraController.Instance.MapZoomOutInit();
    }
}
