
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class CreateUserScript : MonoBehaviour
{
    public Text Email;
    public Text Password;
    public Text Retype;

    public Text Error;

    public void TryCreateUser()
    {

        if (Password.text == Retype.text)
        {
			SaveDefaults (Email.text, Password.text);

            ServerDataManager.Instance.CreateUser(Email.text, Password.text, Error);
        }
        else
        {
            Error.text = "Passwords Dont Match";
        }
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
}
