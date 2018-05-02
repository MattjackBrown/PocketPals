using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapRotate : MonoBehaviour
{

    private bool right = false;
    private bool left = false;
    public float rotationSpeed = 200;
    public Button rightButton;
    public Button leftButton;
    public GPS map;

    void Update()
    {
        if (right)
        {
            this.transform.Rotate(-Vector3.up * rotationSpeed * Time.deltaTime);
        }
        else if (left)
        {
            this.transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
        }
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
