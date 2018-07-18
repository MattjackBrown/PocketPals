using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupHandler : MonoBehaviour
{
    public static PopupHandler Instance { set; get; }
    private Queue<Sprite> Popups = new Queue<Sprite>();

    public List<PopupData> activePopups = new List<PopupData>();
    public List<Image> poolImage = new List<Image>();

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
            PopupData i = activePopups[j];
            i.Alpha += Time.deltaTime * speed;
            i.img.color= new Color(1,1,1,1-i.Alpha);
            i.img.transform.position = Vector2.Lerp(startPos, endPos, i.Alpha);
            if (i.Alpha >= 1.0) AddToPool(i);
        }
    }

    private void AddToPool(PopupData pd)
    {
        activePopups.Remove(pd);
        poolImage.Add(pd.img);
        pd.img.color = new Color(1, 1, 1, 1);
        pd.img.transform.position = startPos;
        pd.img.gameObject.SetActive(false);
    }

    private void AddToActive(Image i)
    {
        poolImage.Remove(i);
        i.transform.position = startPos;
        activePopups.Add(new PopupData(i));
        i.gameObject.SetActive(true);
    }

    public void AddPopup(Sprite spr)
    {
        Popups.Enqueue(spr);
    }

    private IEnumerator SpawnPopup()
    {
        while (true)
        {
            if (poolImage.Count >= 1 && Popups.Count >= 1)
            {
                Sprite sp = Popups.Dequeue();
                poolImage[0].sprite = sp;
                AddToActive(poolImage[0]);
            }
            yield return new WaitForSeconds(popupDelay);
        }
    }
}
public class PopupData
{
    public float Alpha = 0.0f;
    public Image img;
    public PopupData(Image i)
    {
        img = i;
    }
}
