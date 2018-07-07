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

    public void Clicked()
    {
        Used = true;

        anim.SetBool("Clicked", true);

        LocalDataManager.Instance.AddItem(new ItemData("Berries", 1));
    }
    public void Finished()
    {
        anim.SetBool("Clicked", false);
        transform.GetChild(0).gameObject.SetActive(false);

        //To DO: Zoom back out
		CameraController.Instance.MapZoomOutInit();
    }
}
