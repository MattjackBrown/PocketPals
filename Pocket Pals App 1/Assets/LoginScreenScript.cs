using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

[System.Serializable]
public class DefaultLogin
{
	public string name, password;

	public DefaultLogin(string newName, string newPassword)
	{
		name = newName;
		password = newPassword;
	}
}
	
public class LoginScreenScript : MonoBehaviour
{
	public static LoginScreenScript Instance { get; set; }

    // The UI fields
    public InputField email, password;
    public Text error;

	// Default values
	public string DebugUserName, DebugPassword;

	void Start () {

		//Load any default login values
		LoadDefaults ();

		Instance = this;
	}

    public void TryLogin()
    {

        ServerDataManager.Instance.SignIn(email.text, password.text, error);

		// Save attempted data to use aas a default for next time
		// Empty string check
		if (email.text.Length > 0 && password.text.Length > 0) {

			// Actually prob best to do this only if login successful
			SaveDefaults (email.text, password.text);
		}
    }

	public void DebugQuickLogin ()
    {
        email.GetComponentInParent<InputField>().text = DebugUserName;
        password.GetComponentInParent<InputField>().text = DebugPassword;
        Debug.Log(email.text);
		ServerDataManager.Instance.SignIn(email.text, password.text, error);
	}

	public void SaveDefaults(string name, string password)
	{
		string destination = Application.persistentDataPath + "/save.dat";
		FileStream file;

		if (File.Exists(destination))
			file = File.OpenWrite(destination);
		else
			file = File.Create(destination);

		DefaultLogin data = new DefaultLogin (name, password);
		BinaryFormatter bf = new BinaryFormatter();
		bf.Serialize(file, data);
		file.Close();
	}

	public void LoadDefaults()
	{
		string destination = Application.persistentDataPath + "/save.dat";
		FileStream file;
		if (File.Exists(destination))
			file = File.OpenRead(destination);
		else {
			Debug.Log("File not found");
			return;
		}
        
		BinaryFormatter bf = new BinaryFormatter();
		DefaultLogin data = (DefaultLogin) bf.Deserialize(file);
		file.Close();

		email.text = data.name;
		password.text = data.password;
	}
}
