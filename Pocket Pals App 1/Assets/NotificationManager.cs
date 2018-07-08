using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NotificationManager : MonoBehaviour
{
    static public NotificationManager Instance { set; get; }

    public GameObject NotificationMenu;

    private Queue<Notification> Notifications = new Queue<Notification>();

    public Text Header;

    public Text NotificaitonMessage;

	// Use this for initialization
	void Start ()
    {
        Instance = this;
	}

    public void ErrorNotification(string m)
    {
        AddNotificationToQueue(new Notification("Uh oh Error", m));
    }

    public void CongratsNotification(string m)
    {
        AddNotificationToQueue(new Notification("Congrats!", m));
    }

    public void LoginNotification(string m)
    {
        AddNotificationToQueue(new Notification("Login Message:", m));
    }

    public void LoginFailedNotification(string m)
    {
        AddNotificationToQueue(new Notification("Login Failed:", m));
    }

    public void CreateUserErrorNotification(string m)
    {
        AddNotificationToQueue(new Notification("Create User Failed:", m));
    }

    public void LogoutNotification(string m)
    {
        AddNotificationToQueue(new Notification("Signed Out", m));
    }

    public void NotificationDismissed()
    {
        if (Notifications.Count < 1) NotificationMenu.SetActive(false);
        else
        {
            Notification n = Notifications.Dequeue();
            ApplyNotification(n);
        }
    }

    //--------- Internal stuff--------\\

    private void ApplyNotification(Notification n)
    {
        Header.text = n.header;
        NotificaitonMessage.text = n.message;
    }

    private void AddNotificationToQueue(Notification n)
    {
        if (NotificationMenu.activeSelf == false)
        {
            NotificationMenu.SetActive(true);
            ApplyNotification(n);
        }
        else
        {
            Notifications.Enqueue(n);
        }
    }

}

public class Notification
{
    public string header, message;
    public Notification(string h, string m)
    {
        header = h;
        message = m;
    }
}
