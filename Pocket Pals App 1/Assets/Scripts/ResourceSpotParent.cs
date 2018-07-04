using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceSpotParent : MonoBehaviour
{

    public Vector2 IngameLocation { set; get; }
    public Vector2 LatLonLocation { set; get; }

    private Animator anim;

    private void Start()
    {
        gameObject.transform.Rotate(new Vector3(0,Random.Range(0,360), 0));
        anim = GetComponent<Animator>();
    }

    public void Clicked()
    {
        anim.SetBool("Clicked", true);
        //To DO: Zoom in to the game object

    }
    public void Finished()
    {
        anim.SetBool("Clicked", false);
        transform.GetChild(0).gameObject.SetActive(false);
        //To DO: Zoom back out
    }
}
