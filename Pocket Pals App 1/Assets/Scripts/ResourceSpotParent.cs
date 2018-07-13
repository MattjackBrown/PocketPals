using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceSpotParent : MonoBehaviour
{
    private Animator anim;

    public bool Used = false;

    public Vector2 spawnLoc;

    public int maxItemFind = 2;

    public ResourceSpotSave SaveData { set; get; }

    private void Start()
    {
        gameObject.transform.Rotate(new Vector3(0, Random.Range(0, 360), 0));
        anim = GetComponent<Animator>();
    }

    public void Hide()
    {

        transform.GetChild(0).gameObject.SetActive(true);
        gameObject.SetActive(false);
    }

    public void Show(Vector3 pos, Vector2 latlon)
    {
        Used = false;
        transform.position = pos;
        gameObject.SetActive(true);
        spawnLoc = latlon;
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

    public void OldUsed()
    {
        Used = true;
        transform.GetChild(0).gameObject.SetActive(false);
    }

    public void Finished()
    {
        anim.SetBool("Clicked", false);
        transform.GetChild(0).gameObject.SetActive(false);

        LocalDataManager.Instance.AddItem(AssetManager.Instance.GetRandomItem(maxItemFind));

        CameraController.Instance.MapZoomOutInit();

        ResourceSpotManager.Instance.AddNewSaveData(this);
    }
}

