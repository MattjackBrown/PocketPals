using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using Firebase.Auth;
using UnityEngine;
using UnityEngine.UI;

public class ServerDataManager : MonoBehaviour {

    public static ServerDataManager Instance { get; set; }

    private bool FirebaseInitialised = false;

    //Database reference used to call up the server and the local copy that firebase provides
    DatabaseReference mDatabaseRef;

    //Reference to the fire base authentication instance
    FirebaseAuth auth;

    //Rerfernce to the user provided after the user has successfully logged in
    FirebaseUser newUser;

    //Probs going to vanish soon
    GameData gameData;

    //Text to print any error messages
    public Text ErrorText;

    //This will be the first screen to come up after a successful log in.
    public GameObject WelcomeScreen;

    // Use this for initialization
    void Start ()
    {
        Instance = this;

        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            var dependencyStatus = task.Result;

            //for some reason this always fails on phone builds. Yet if I just assume it worked everything just works :) 

            //TO DO: Work out why this never successds
            if (dependencyStatus == DependencyStatus.Available)
            {
                // Set a flag here indiciating that Firebase is ready to use by your
                // application.

                UnityEngine.Debug.Log(System.String.Format(
                    "Could not resolve all Firebase dependencies: {0}", dependencyStatus));

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
            //Check to see if its the App has a reference to the database setup
            if (FirebaseApp.DefaultInstance.Options.DatabaseUrl != null) FirebaseApp.DefaultInstance.SetEditorDatabaseUrl(FirebaseApp.DefaultInstance.Options.DatabaseUrl);
            
            //Assign references.
            mDatabaseRef = FirebaseDatabase.DefaultInstance.RootReference;
            auth = FirebaseAuth.DefaultInstance;

            //Sometimes firebase will leave a user logged in from a previous session this will stop that.
            auth.SignOut();

            //Make sure there is nothing left over from the last session
            if (auth.CurrentUser != null)
            {
                auth.CurrentUser.DeleteAsync();
            }
  
            ErrorText.text = "Ready To login";

            //Assign a listner to check for a state change to the authentication
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
        //TO DO write pocketpals to server.      
    }

    public void CreateUser(string email, string password,Text failedText)
    {

        //Create a new user with the given user name and password.
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
            Debug.LogFormat("User signed in successfully: {0} ({1})", newUser.DisplayName, newUser.UserId);
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


