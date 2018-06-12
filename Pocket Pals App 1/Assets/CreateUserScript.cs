
using UnityEngine;
using UnityEngine.UI;

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
            ServerDataManager.Instance.CreateUser(Email.text, Password.text, Error);
        }
        else
        {
            Error.text = "Passwords Dont Match";
        }
    }
}
