using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using Firebase.Auth;
using UnityEngine;
using UnityEngine.UI;

public class ServerDataManager : MonoBehaviour {

    public static ServerDataManager Instance { get; set; }

    private bool FirebaseInitialised = false;

    DatabaseReference mDatabaseRef;

    FirebaseAuth auth;

    FirebaseUser newUser;

    GameData gameData;

    public Text ErrorText;

    public GameObject WelcomeScreen;

    // Use this for initialization
    void Start ()
    {
        Instance = this;

        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            var dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                // Set a flag here indiciating that Firebase is ready to use by your
                // application.

                FirebaseInitialised = true;
            }
            else
            {
                UnityEngine.Debug.Log(System.String.Format(
                  "Could not resolve all Firebase dependencies: {0}", dependencyStatus));

                ErrorText.text = System.String.Format("Could not resolve all Firebase dependencies: {0}", dependencyStatus);
                // Firebase Unity SDK is not safe to use here.
            }
        });
    }

    void Awake()
    {
        FirebaseInitialised = true;

        if (FirebaseInitialised)
        {

            if (FirebaseApp.DefaultInstance.Options.DatabaseUrl != null) FirebaseApp.DefaultInstance.SetEditorDatabaseUrl(FirebaseApp.DefaultInstance.Options.DatabaseUrl);

            mDatabaseRef = FirebaseDatabase.DefaultInstance.RootReference;

            auth = FirebaseAuth.DefaultInstance;

            auth.SignOut();
            if (auth.CurrentUser != null)
            {
                auth.CurrentUser.DeleteAsync();
            }
            ErrorText.text = "Ready To login";

            auth.StateChanged += AuthStateChanged;
            AuthStateChanged(this, null);

            gameData = new GameData();
        }
    }

    public void WriteNewUser(GameData gd)
    {
        string json = gd.GetJson();

        Debug.Log(json);

        mDatabaseRef.Child("Users").Child(gd.ID).SetRawJsonValueAsync(json);

    }

    public void WriteExsistingUser(GameData gd)
    {
        string json = gd.GetJson();

        Debug.Log(json);

        mDatabaseRef.Child("Users").Child(gd.Username).SetValueAsync(json);

    }

    public void WritePocketPals(GameData gd)
    {
        
    }

    public void CreateUser(string email, string password,Text failedText)
    {

        auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWith(task => {
            if (task.IsCanceled)
            {
                failedText.text = "Create User Failed";
                return;
            }
            if (task.IsFaulted)
            {
                failedText.text = "Bad email or password";
                return;
            }
            newUser = task.Result;
            Debug.LogFormat("User signed in successfully: {0} ({1})",
                newUser.DisplayName, newUser.UserId);
        });
    }

    public void SignIn(string email, string password, Text failedText)
    {
        auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWith(task => {
            if (task.IsCanceled)
            {
                failedText.text = "Login Failed";
                return;
            }
            if (task.IsFaulted)
            {
                failedText.text = "Wrong username or password";
                return;
            }
           newUser = task.Result;
            Debug.LogFormat("User signed in successfully: {0} ({1})",
                newUser.DisplayName, newUser.UserId);
        });
    }

    void AuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        if (auth.CurrentUser != newUser)
        {
            bool signedIn = newUser != auth.CurrentUser && auth.CurrentUser != null;
            if (!signedIn && newUser != null)
            {
                Debug.Log("Signed out " + newUser.UserId);
            }
            newUser = auth.CurrentUser;
            if (signedIn)
            {
                Debug.Log("Signed in " + newUser.UserId);
                gameData.Username = newUser.DisplayName ?? "";
                WelcomeScreen.SetActive(true);
            }
        }
    }

    void OnDestroy()
    {
        auth.SignOut();
        newUser.DeleteAsync();
        auth.StateChanged -= AuthStateChanged;
        auth = null;
    }
}


