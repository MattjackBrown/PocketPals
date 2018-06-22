using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginScreenScript : MonoBehaviour
{


    public Text email;

    public Text password;

    public Text error;

	public string DebugUserName, DebugPassword;

    public void TryLogin()
    {
        ServerDataManager.Instance.SignIn(email.text, password.text, error);
    }

	public void DebugQuickLogin ()
    {
        email.GetComponentInParent<InputField>().text = DebugUserName;
        password.GetComponentInParent<InputField>().text = DebugPassword;
        Debug.Log(email.text);
		ServerDataManager.Instance.SignIn(email.text, password.text, error);
	}
}
