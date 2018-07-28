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
    public UIAnimationScript Stars;
    public GameObject expBack;
    public Color expCol;
    public Color backColor;
    public Image cBack;

    public float popupDelay = 1.0f;
    public float speed = 2.0f;
    public float countSpeed = 1.0f;

    private Vector2 rStartPos;
    private Vector2 rEndPos;
    public GameObject rEndObj;
    public GameObject rStartObj;

    private Vector2 cStartPos;
    private Vector2 cEndPos;
    public GameObject cStartObj;
    public GameObject cEndObj;


    public bool debug = false;

	// Use this for initialization
	void Start ()
    {
        Instance = this;
        rStartPos = rStartObj.transform.position;
        rEndPos = rEndObj.transform.position;

        cStartPos = cStartObj.transform.position;
        cEndPos = cEndObj.transform.position;

        StartCoroutine(SpawnPopup());
	}

    // Update is called once per frame
    private void FixedUpdate()
    {
		if (activePopups.Count < 1) {
			cBack.gameObject.SetActive (false);
			return;
		}
		bool hasExp = false;
        for(int j = 0; j< activePopups.Count; j++)
        {
            PopupData pd = activePopups[j];
            pd.Alpha += Time.deltaTime * speed;
            if (pd.ID == 0)
            {
                ItemDrop id = (ItemDrop)pd;
                id.img.color = new Color(1, 1, 1, 1 - pd.Alpha);
                id.img.transform.position = Vector2.Lerp(rStartPos, rEndPos, pd.Alpha);
            }
            else if (pd.ID == 1 )
            {
                hasExp = true;
                ProcessExpDrop(pd);
            }


            if (pd.Alpha >= 1.0) AddToPool(pd);
        }
		if (hasExp)
            cBack.gameObject.SetActive(true);
        else
            cBack.gameObject.SetActive(false);

    }

    public void ProcessExpDrop(PopupData pd)
    {
        ExpDrop ed = (ExpDrop)pd;
        cBack.color = new Color(0, 0, 0, (1 - ed.Alpha) * 0.75f);
        if (ed.countAlpha >= 1)
        {
            if (ed.PlayStars)
            {
                // Play video here
                Stars.Play();
                ed.PlayStars = false;
            }

            Color c = new Color(expCol.r, expCol.g, expCol.b, 1);
            c.a = 1 - ed.Alpha;
            ed.text.color = c;
            ed.text.transform.position = Vector2.Lerp(cStartPos, cEndPos, pd.Alpha);

        }
        else
        {
            cBack.color = new Color(0, 0, 0, (ed.countAlpha) * 0.75f);
            ed.countAlpha += Time.deltaTime * countSpeed;
            ed.Alpha = 0.0f;
            ed.SetText();
        }
    }

    private void AddToPool(PopupData pd)
    {
        activePopups.Remove(pd);
        if (pd.ID == 0)
        {
            ItemDrop id = (ItemDrop)pd;
            poolImage.Add(id.img);
            id.img.color = new Color(1, 1, 1, 1);
            id.img.transform.position = rStartPos;
            id.img.gameObject.SetActive(false);
        }
        else if(pd.ID == 1)
        {
            ExpDrop ed = (ExpDrop)pd;
            poolText.Add(ed.text);
            ed.text.transform.position = cStartPos;
            ed.text.gameObject.SetActive(false);
        }
    }

    public void AddPopup(Sprite spr)
    {
        Popups.Enqueue(new ItemDrop(spr));
    }

    public void AddPopup(int exp)
    {
        Popups.Enqueue(new ExpDrop(exp));
    }

    private IEnumerator SpawnPopup()
    {
        while (true)
        {
            if(debug)AddPopup(Random.Range(0, 100000));
            if ( Popups.Count >= 1)
            {
                PopupData pd = Popups.Dequeue();
                if (pd.ID == 0 && poolImage.Count >= 1)
                {
                    ItemDrop id = (ItemDrop)pd;
                    id.img = poolImage[0];
                    id.img.sprite = id.spr;
                    poolImage.Remove(id.img);
                    id.img.transform.position = rStartPos;
                    id.img.gameObject.SetActive(true);
                    activePopups.Add(pd);
                    SoundEffectHandler.Instance.PlaySound("pop");
                }
                else if (pd.ID == 1 && poolText.Count >=1 )
				{
                    ExpDrop ed = (ExpDrop)pd;
                    ed.text = poolText[0];
                    ed.text.color =  new Color(expCol.r, expCol.g, expCol.b, 1);
                    ed.SetText();
                    poolText.Remove(ed.text);
                    ed.text.transform.position = cStartPos;
                    ed.text.gameObject.SetActive(true);
                    activePopups.Add(pd);
                    SoundEffectHandler.Instance.PlaySound("pop");
                }
                else
                {
                    Popups.Enqueue(pd);
                }
               
            }
            yield return new WaitForSeconds(popupDelay);
        }
    }
}

