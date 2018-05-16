using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapRotate : MonoBehaviour
{
    public float rotationSpeed = 200;
    public GPS map;

    private bool right = false;
    private bool left = false;
    
    void Update()
    {
        //Make sure we're trying to rotate a valid map, if not return
        if (!map.isActiveAndEnabled) return;
/*
        if (right)
        {
            this.transform.Rotate(-Vector3.up * rotationSpeed * Time.deltaTime);
        }
        else if (left)
        {
            this.transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
        }
*/        
    }

    public void StartRight()
    {
        right = true;
    }
    public void StopRight()
    {
        right = false;
    }

    public void StartLeft()
    {
        left = true;
    }

    public void StopLeft()
    {
        left = false;
    }
}
