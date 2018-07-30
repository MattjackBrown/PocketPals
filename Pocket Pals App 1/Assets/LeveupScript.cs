using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeveupScript : MonoBehaviour
{
    public GameObject popupLayer, LevelupLayer, mainLayer;

    public Transform sPos, ePos;

    public Text levelText;

    public float countSpeed = 3.0f;
    public float moveSpeed = 2.0f;
    

    public Image backGround;
    public Color bColor;

    public UIAnimationScript[] starbursts;
    private bool DoStarBurst = true; 

    public bool PlayAnim = false;

    private float countAlpha = 0.0f;
    private float moveAlpha = 0.0f;
    private int targlevel = 0;


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        if (PlayAnim)
        {
            if (countAlpha < 1)
            {

                
                countAlpha += Time.deltaTime * countSpeed;
                bColor.a = countAlpha * 0.75f;
                backGround.color = bColor;
                levelText.text = ((int)(targlevel * countAlpha)).ToString();
            }

            else if (moveAlpha < 1)
            {
                if (DoStarBurst)
                {
                    foreach (UIAnimationScript anim in starbursts)
                    {
                        anim.Play();
                    }
                    DoStarBurst = false;
                }

                moveAlpha += Time.deltaTime * moveSpeed;
                bColor.a = (1-moveAlpha) * 0.75f;
                backGround.color = bColor;
                LevelupLayer.transform.position = Vector3.Lerp(sPos.position, ePos.position, moveAlpha);
            }
            else
            {
                PlayAnim = false;
                mainLayer.SetActive(false);
            }
        }
	}

    public IEnumerator TryLevelup(int level)
    {
        while (PopupHandler.Instance.hasEXP || PlayAnim)
        {
            Debug.Log("tryin");
            yield return new WaitForSeconds(0.2f);
        }
        targlevel = level;
        Debug.Log("lup");
        countAlpha = 0;
        moveAlpha = 0;
        LevelupLayer.transform.position = sPos.position;
        mainLayer.SetActive(true);
        LevelupLayer.SetActive(true);
        PlayAnim = true;
        DoStarBurst = true;
    }
}

