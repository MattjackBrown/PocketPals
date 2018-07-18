using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupHandler : MonoBehaviour
{
    public static PopupHandler Instance { set; get; }
    private Queue<PopupData> Popups = new Queue<PopupData>();

    public List<PopupData> activePopups = new List<PopupData>();
    public List<Image> poolImage = new List<Image>();
    public List<Text> poolText = new List<Text>();

    public float popupDelay = 1.0f;
    public float speed = 2.0f;

    private Vector2 startPos;
    private Vector2 endPos;
    public GameObject endObj;
    public GameObject startObj;

	// Use this for initialization
	void Start ()
    {
        Instance = this;
        startPos = startObj.transform.position;
        endPos = endObj.transform.position;
        StartCoroutine(SpawnPopup());
	}

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (activePopups.Count < 1) return;
        for(int j = 0; j< activePopups.Count; j++)
        {
            PopupData pd = activePopups[j];
            pd.Alpha += Time.deltaTime * speed;
            if (pd.ID == 0)
            {
                pd.img.color = new Color(1, 1, 1, 1 - pd.Alpha);
                pd.img.transform.position = Vector2.Lerp(startPos, endPos, pd.Alpha);
            }
            else if (pd.ID == 1)
            {
                pd.text.color = new Color(0, 1,0, 1 - pd.Alpha);
                pd.text.transform.position = Vector2.Lerp(startPos, endPos, pd.Alpha); 
            }
            if (pd.Alpha >= 1.0) AddToPool(pd);
        }
    }

    private void AddToPool(PopupData pd)
    {
        activePopups.Remove(pd);
        if (pd.ID == 0)
        {
            poolImage.Add(pd.img);
            pd.img.color = new Color(1, 1, 1, 1);
            pd.img.transform.position = startPos;
            pd.img.gameObject.SetActive(false);
        }
        else if(pd.ID == 1)
        {
            poolText.Add(pd.text);
            pd.text.color = new Color(1, 1, 1, 1);
            pd.text.transform.position = startPos;
            pd.text.gameObject.SetActive(false);
        }
    }

    public void AddPopup(Sprite spr)
    {
        Popups.Enqueue(new PopupData(spr));
    }

    public void AddPopup(string str)
    {
        Popups.Enqueue(new PopupData(str));
    }

    private IEnumerator SpawnPopup()
    {
        while (true)
        {
            if (poolImage.Count >= 1 && Popups.Count >= 1)
            {
                PopupData pd = Popups.Dequeue();
                if (pd.ID == 0)
                {
                    pd.img = poolImage[0];
                    pd.img.sprite = pd.spr;
                    poolImage.Remove(pd.img);
                    pd.img.transform.position = startPos;
                    pd.img.gameObject.SetActive(true);
                    activePopups.Add(pd);
                }
                else if (pd.ID == 1 && poolText.Count >=1)
                {
                    pd.text = poolText[0];
                    pd.text.text = pd.str;
                    poolText.Remove(pd.text);
                    pd.text.transform.position = startPos;
                    pd.text.gameObject.SetActive(true);
                    activePopups.Add(pd);
                }
               
            }
            yield return new WaitForSeconds(popupDelay);
        }
    }
}
public class PopupData
{
    public int ID = 0;
    public float Alpha = 0.0f;

    public Image img;
    public Sprite spr;

    public  Text text;
    public string str;
    public PopupData(Sprite s)
    {
        spr = s;
        ID = 0;
    }
    public PopupData(string t)
    {
        str = t;
        ID = 1;
    }
}
