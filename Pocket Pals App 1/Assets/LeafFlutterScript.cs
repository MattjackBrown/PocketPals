using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeafFlutterScript : MonoBehaviour
{
    public Animator anim;
    private void Start()
    {
    }

    public void PlayAnimAtLocation(Vector3 pos)
    {
        gameObject.SetActive(true);
        gameObject.transform.position = pos;
        if (GPS.Insatance != null)
        {
            gameObject.transform.rotation = Quaternion.LookRotation(GPS.Insatance.girl.transform.position - pos);
        }
        anim.Play("LeafFlutter");
    }


    public void Hide()
    {
        AnimationFactoryScript.Instance.RemoveLeafFlutter(this);
        gameObject.SetActive(false);
    }
}
