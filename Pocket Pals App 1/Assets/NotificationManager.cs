using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NotificationManager : MonoBehaviour
{

    static public NotificationManager Instance { set; get; }

    public GameObject NotificationMenu;
    private Queue<Notification> Notifications = new Queue<Notification>();

    private Notification LastNotification;

    public List<GameObject> Layers;

    //---- Message Objects-----------\\
    private const int mID = 0;
    public Text mHeader;
    public Text mMessage;
    public GameObject mLayer;

    //------Question Objects -------\\
    private const int qID = 1;
    public Text qQuestion;
    public GameObject qLayer;

    //---unRemovable Layer----\\
    private const int uID = 2;
    public GameObject uLayer;
    public Text uMessage;

    //------ Message Functions--------\\\
    public void ErrorNotification(string m)
    {
        AddNotificationToQueue(new Notification("Uh oh Error", m));
    }

    public void CustomHeaderNotification(string h, string m)
    {
        AddNotificationToQueue(new Notification(h, m));
    }

    public void CongratsNotification(string m)
    {
        AddNotificationToQueue(new Notification("Congratulations!", m));
    }

    public void LoginNotification(string m)
    {
        AddNotificationToQueue(new Notification("Login message:", m));
    }

    public void LoginFailedNotification(string m)
    {
        AddNotificationToQueue(new Notification("Login failed:", m));
    }

    public void CreateUserErrorNotification(string m)
    {
        AddNotificationToQueue(new Notification("Failed to create user:", m));
    }

    public void LogoutNotification(string m)
    {
        AddNotificationToQueue(new Notification("Signed out", m));
    }

    public void ItemFailedNotification(string m)
    {
        AddNotificationToQueue(new Notification("None left!!", m));
    }

	public void MiniGameFailedNotification(string m) {

		AddNotificationToQueue(new Notification("You ran out of time", m));
	}

	public void CharCustLockedNotification(string m) {

		AddNotificationToQueue(new Notification("Locked!", m));
	}

    public void ResourceSpotUsed(float timeLeft)
    {
        int mintues = Mathf.RoundToInt(timeLeft / 60);

        int seconds = Mathf.RoundToInt(timeLeft % 60); 
        AddNotificationToQueue(new Notification("Resource is not ready!", "You have: " + mintues +  " minutes and: " + seconds +  " seconds until it can be used again", 1));
    }

    public void InteractError(string m)
    {
        AddNotificationToQueue(new Notification("Failed To Interact!", m));
    }

    //--------- Question Functions -------\\

    public void QuestionNotification(string q, Notification.del yesFunc, Notification.del noFunc )
    {
        AddNotificationToQueue(new Notification(1, q, yesFunc, noFunc));
    }


    //--------- Unremovable Notification -----\\\\\
    public void UndissmissableNotification(string m)
    {
        AddNotificationToQueue(new Notification(uID, m));
    }
    //--------- Internal stuff--------\\

    public void ToggleLayer(int activeLayer)
    {
        for (int i = 0; i < Layers.Count; i++)
        {
            if (i == activeLayer) Layers[i].SetActive(true);
            else Layers[i].SetActive(false);
        }
    }

    public void NotificationDismissed(bool b)
    {
        if (LastNotification.type == qID) LastNotification.Respone(b);

        if (Notifications.Count < 1) NotificationMenu.SetActive(false);
        else
        {          
            while (Notifications.Count > 0)
            {
                Notification n = Notifications.Dequeue();
                if (n.id != uID)
                {
                    ApplyNotification(n);
                    return;
                }
            }
        }
    }

    private void ApplyNotification(Notification n)
    {

        switch (n.type)
        {
            case mID:
                {
                    ToggleLayer(mID);
                    mHeader.text = n.header;
                    mMessage.text = n.message;
                    break;
                }
            case qID:
                {
                    ToggleLayer(qID);
                    qQuestion.text = n.message;
                    break;
                }
            case uID:
                {
                    ToggleLayer(uID);
                    uMessage.text = n.message;
                    break;
                }
        }
        LastNotification = n;
        SoundEffectHandler.Instance.PlaySound("ping");
    }

    private void AddNotificationToQueue(Notification n)
    {

        //Check to stop spamming notifications of certain typees
        if (n.id != -1)
        {
            if (LastNotification != null && NotificationMenu.activeSelf &&  LastNotification.id == n.id) return;
            foreach (Notification not in Notifications)
            {
                if (not.id == n.id) return;
            }
        }

        if (!NotificationMenu.activeSelf)
        {
            NotificationMenu.SetActive(true);
            ApplyNotification(n);
        }
        else
        {
            if (LastNotification != null && LastNotification.id == uID)
            {
                ApplyNotification(n);
            }
            else
            {
                Notifications.Enqueue(n);
            }
        }
    }

    void Start()
    {
        Instance = this;
    }

}

public class Notification
{
    public int id = -1;
    public int type =0;
    public string header, message;
    public delegate void del();
    public del yesFunc;
    public del noFunc;

    public Notification(string h, string m)
    {
        header = h;
        message = m;
    }

    public Notification(int t, string m)
    {
        type = t;
        message = m;
    }

    public Notification(string h, string m, int i)
    {
        header = h;
        message = m;
        id = i;
    }

    public Notification(int t, string q, del pos, del neg)
    {
        type = t;
        message = q;
        yesFunc = pos;
        noFunc = neg;
    }

    public void Respone(bool r)
    {
        if (r)
        {
            if(yesFunc != null) yesFunc();
        }
        else
        {
            if (noFunc != null) noFunc();
        }
    }
}
