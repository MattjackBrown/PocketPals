using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using Firebase.Auth;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ServerDataManager : MonoBehaviour {

    public static ServerDataManager Instance { get; set; }

    private bool FirebaseInitialised = false;

    //Database reference used to call up the server and the local copy that firebase provides
    DatabaseReference mDatabaseRef;

    //Reference to the fire base authentication instance
    FirebaseAuth auth;

    //Rerfernce to the user provided after the user has successfully logged in
    FirebaseUser newUser;

    //Text to print any error messages
    public Text ErrorText;

    //This will be the first screen to come up after a successful log in.
    public GameObject WelcomeScreen;

    // Use this for initialization
    void Start ()
    {
        Instance = this;

		DontDestroyOnLoad (this.gameObject);

		// Store this instance reference as a static variable in the Global variables class
		GlobalVariables.serverDataManager = this;


        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            var dependencyStatus = task.Result;

            //for some reason this always fails on phone builds. Yet if I just assume it worked everything just works :) 

            //TO DO: Work out why this never successds
            if (dependencyStatus == DependencyStatus.Available)
            {
                // Set a flag here indiciating that Firebase is ready to use by your
                // application.

                FirebaseInitialised = true;
            }
            else
            {

                FirebaseInitialised = true;

                UnityEngine.Debug.Log(System.String.Format(
                  "Could not resolve all Firebase dependencies: {0}", dependencyStatus));

                ErrorText.text = System.String.Format("Could not resolve all Firebase dependencies: {0}", dependencyStatus);
                // Firebase Unity SDK is not safe to use here.
            }
        });
    }

    void Awake()
	{
		// Don't destroy on load does not stop new instances from being instantiated on scene load. This will check and delete
		if (FindObjectsOfType(typeof(LocalDataManager)).Length > 1)
		{
			Debug.Log("Dirty Singleton management. Deleting new instance.");
			DestroyImmediate(gameObject);
		}

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
        }
    }

    public void WriteNewUser(GameData gd)
    {
        string json = gd.GetJson();

        Debug.Log(json);

        //write the new user to the database
        mDatabaseRef.Child("Users").Child(gd.ID).SetRawJsonValueAsync(json);

        //write the user inventory to the databse.
        WriteInventory(gd);
    }

    public void WriteExsistingUser(GameData gd)
    {
        string json = gd.GetJson();

        Debug.Log(json);

        mDatabaseRef.Child("Users").Child(gd.ID).SetRawJsonValueAsync(json);
    }

    public void WriteInventory(GameData gd)
    {
        foreach (PocketPalData ppd in gd.Inventory.GetMyPocketPals())
        {
            WritePocketPal(gd, ppd);
        }
    }

    public void UpdateDistace(GameData gd)
    {
        mDatabaseRef.Child("Users").Child(gd.ID).Child("DistanceTravelled").SetValueAsync(gd.DistanceTravelled);
    }

    public void WritePocketPal(GameData gd, PocketPalData ppd)
    {
        string json = JsonUtility.ToJson(ppd);
        mDatabaseRef.Child("Inventories").Child(gd.inventoryID).Child(ppd.ID.ToString()).SetRawJsonValueAsync(json);
    }

    public void UpdatePlayerName(GameData gd)
    {
        mDatabaseRef.Child("Users").Child(gd.ID).Child("Username").SetValueAsync(gd.Username);
    }

    public void UpdatePlayerExp(GameData gd)
    {
        mDatabaseRef.Child("Users").Child(gd.ID).Child("EXP").SetValueAsync(gd.EXP);
    }

    public void GetPlayerData(GameData gd)
    {

        int iter = 0;

        mDatabaseRef.Child("Users").Child(gd.ID).GetValueAsync().ContinueWith(task => {
            if (task.IsFaulted)
            {
                    Debug.Log("Failed Getting user from databse Writing new user");
                    WriteNewUser(LocalDataManager.Instance.GetData());
            }
            else if (task.IsCompleted)
            {
                try
                {
                    DataSnapshot snapshot = task.Result;
                    
                    //Read the Data received from the server.
                    foreach (DataSnapshot obj in snapshot.Children)
                    {
                        iter++;
                        switch (obj.Key.ToLower())
                        {
                            case "distancetravelled":
                                gd.DistanceTravelled = Convert.ToSingle(obj.Value);
                                break;

                            case "exp":
                                gd.EXP = Convert.ToSingle(obj.Value);
                                break;

                            case "usernmae":
                                gd.Username = (string)obj.Value;
                                break;

                            case "id":
                                gd.ID = (string)obj.Value;
                                break;

                            case "inventoryid":
                                gd.inventoryID = (string)obj.Value;
                                break;
                        }
                    }
                }
                catch(Exception ex)
                {
                    Debug.Log(ex);
                }
            }
            //If iter a new user Write it to the server... This is a bad fix but the only one 
            //that seems to work consistently
            if (iter < 2)
            {
                WriteNewUser(gd);
            }
            GetInventory(gd);
        });
    }

    public void GetInventory(GameData gd)
    {
        Debug.Log(gd.inventoryID);
        //Start a task that will populate the players inventory with their ppals.
        mDatabaseRef.Child("Inventories").Child(gd.inventoryID).GetValueAsync().ContinueWith(task => {
            if (task.IsFaulted)
            {
                Debug.Log("Failed Getting user from databse Writing new user");
                WriteNewUser(gd);
            }
            else if (task.IsCompleted)
            {
                try
                {
                    DataSnapshot snapshot = task.Result;
                    foreach (DataSnapshot obj in snapshot.Children)
                    {
                        PocketPalData ppd = new PocketPalData();

                        foreach (DataSnapshot child in obj.Children)
                        {
                            switch (child.Key.ToLower())
                            {
                                case "exp":
                                    ppd.EXP = Convert.ToSingle(child.Value);
                                    break;
                                case "id":
                                    ppd.ID = Convert.ToInt32(child.Value);
                                    break;
                                case "agressiveness":
                                    ppd.agressiveness = Convert.ToSingle(child.Value);
                                    break;
                                case "weight":
                                    ppd.weight = Convert.ToSingle(child.Value);
                                    break;
                                case "name":
                                    ppd.name = Convert.ToString(child.Value);
                                    break;
                                case "size":
                                    ppd.size = Convert.ToSingle(child.Value);
                                    break;
                                case "numbercaught":
                                    ppd.numberCaught = Convert.ToInt32(child.Value);
                                    break;
                                case "baserarity":
                                    ppd.baseRarity = Convert.ToInt32(child.Value);
                                    break;
                            }

                        }
                        gd.Inventory.GetMyPocketPals().Add(ppd);
                    }
                }
                catch (Exception ex)
                {
                    Debug.Log(ex);
                }
                WelcomeScreen.SetActive(true);
            }
        });
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

			GlobalVariables.hasLoggedIn = true;
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

			GlobalVariables.hasLoggedIn = true;
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
                ErrorText.text = "Logging in....";
                LocalDataManager.Instance.GetData().ID = newUser.UserId ?? "";
                LocalDataManager.Instance.GetData().Username = newUser.DisplayName ?? "";

                GetPlayerData(LocalDataManager.Instance.GetData());

            }
        }
    }
}


