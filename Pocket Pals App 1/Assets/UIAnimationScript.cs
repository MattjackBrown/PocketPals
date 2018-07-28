using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIAnimationScript : MonoBehaviour {

    public Sprite[] images;
    public Image image;
    public float FPS = 10;
    private int iter = 0;
    private bool playAnim = false;

    public void Start()
    {
        gameObject.SetActive(false);
    }

    public  void Play()
    {
        gameObject.SetActive(true);
        StartCoroutine(CycleCoroutine());
    }
    IEnumerator CycleCoroutine()
    {
        //destroy all game objects
        for (int i = 0; i < images.Length; i++)
        {
            image.sprite = images[i];
            yield return new WaitForSeconds(1/FPS);
        }
        gameObject.SetActive(false);
    }
}
