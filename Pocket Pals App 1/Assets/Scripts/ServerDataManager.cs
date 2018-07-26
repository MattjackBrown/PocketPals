﻿using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using Firebase.Auth;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ServerDataManager : MonoBehaviour
{

    public static ServerDataManager Instance { get; set; }

    public CharacterCustomisation charLoadout;

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
//    public GameObject WelcomeScreen;

    //Login screen ref
/*    
 	public GameObject LoginScreen;
	public GameObject initialGameUI;
*/
	public UIAnimationManager canvasParent;
	public LoadingScreenController loadingScreen;
    private bool ShouldUpdatePlayer = false;

    private bool createUser = false;

    // Use this for initialization
    void Start()
    {
        Instance = this;

        DontDestroyOnLoad(this.gameObject);

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

                FirebaseInitialised = false;

                UnityEngine.Debug.Log(System.String.Format(
                  "Could not resolve all Firebase dependencies: {0}", dependencyStatus));

                ErrorText.text = System.String.Format("Could not resolve all Firebase dependencies: {0}", dependencyStatus);
                // Firebase Unity SDK is not safe to use here.
            }
        });
        AuthStateChanged(this, null);
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

            if (auth.CurrentUser != null)auth.SignOut();

            //Assign a listner to check for a state change to the authentication
            auth.StateChanged += AuthStateChanged;

        }
    }

    //-------------- Player Stats stuff -----------\\
    public void WriteNewUser(GameData gd)
    {
        string json = gd.GetJson();

        Debug.Log(json);

        //write the new user to the database
        mDatabaseRef.Child("Users").Child(gd.ID).SetRawJsonValueAsync(json);

        //write the user inventory to the databse.

    }

    public void WriteExsistingUser(GameData gd)
    {
        string json = gd.GetJson();

        Debug.Log(json);

        mDatabaseRef.Child("Users").Child(gd.ID).SetRawJsonValueAsync(json);
    }

    public void UpdateDistace(GameData gd)
    {
        if (!ShouldUpdatePlayer) return;
        mDatabaseRef.Child("Users").Child(gd.ID).Child("DistanceTravelled").SetValueAsync(gd.DistanceTravelled);
    }

    public void UpdatePlayerName(GameData gd)
    {
        if (!ShouldUpdatePlayer) return;
        mDatabaseRef.Child("Users").Child(gd.ID).Child("Username").SetValueAsync(gd.Username);
    }

    public void UpdatePlayerExp(GameData gd)
    {
        if (!ShouldUpdatePlayer) return;
        mDatabaseRef.Child("Users").Child(gd.ID).Child("EXP").SetValueAsync(gd.EXP);
    }

    public void RefreshCoins(GameData gd)
    {
        if (!ShouldUpdatePlayer) return;
        mDatabaseRef.Child("Users").Child(gd.ID).Child("PocketCoins").GetValueAsync().ContinueWith(task => {
            if (task.IsFaulted)
            {
                // Handle the error...
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                try
                {
                    gd.PocketCoins = Convert.ToInt32(snapshot.Value);
                }
                catch (Exception ex)
                {
                    Debug.Log("Could not read PocketPal Coins writing now.");
                    WriteCoins(gd);
                }
                ShopHandler.Instance.RefreshCoins();
            }
        }); 
    }

    public void UpdateCharacterStyleData(CharacterStyleData csd)
    {
        string json = JsonUtility.ToJson(csd);
        Debug.Log("Updated");
        GameData gd = LocalDataManager.Instance.GetData();
        mDatabaseRef.Child("Users").Child(gd.ID).Child("Appearance").SetRawJsonValueAsync(json);
    }

    public void UpdateHasCostumePack(int i)
    {
        GameData gd = LocalDataManager.Instance.GetData();
        mDatabaseRef.Child("Users").Child(gd.ID).Child("HasCostumePack").SetValueAsync(i);
    }

    public void UpdateHasAR(int i)
    {
        GameData gd = LocalDataManager.Instance.GetData();
        mDatabaseRef.Child("Users").Child(gd.ID).Child("HasAR").SetValueAsync(i);
    }

    public void AddPocketCoins(GameData gd, int delta)
    {
        RefreshCoins(gd);
        gd.PocketCoins += delta;
        WriteCoins(gd);
    }

    public void WriteCoins(GameData gd)
    {
        if (!ShouldUpdatePlayer) return;
        ShopHandler.Instance.RefreshCoins();
        mDatabaseRef.Child("Users").Child(gd.ID).Child("PocketCoins").SetValueAsync(gd.PocketCoins);
    }

    private CharacterStyleData ProcessCharData(DataSnapshot data)
    {
        CharacterStyleData csd = new CharacterStyleData();
        foreach (DataSnapshot ds in data.Children)
        {
            switch (ds.Key.ToLower())
            {
                case "m_hairmeshid":
                    {
                        csd.m_HairMeshID = Convert.ToInt32(ds.Value);
                        break;
                    }
                case "m_hairmatid":
                    {
                        csd.m_HairMatID = Convert.ToInt32(ds.Value);
                        break;
                    }
                case "m_bagid":
                    {
                        csd.m_BagID = Convert.ToInt32(ds.Value);
                        break;
                    }
                case "m_shirtsid":
                    {
                        csd.m_ShirtID = Convert.ToInt32(ds.Value);
                        break;
                    }
                case "m_shortsid":
                    {
                        csd.m_ShortsID = Convert.ToInt32(ds.Value);
                        break;
                    }
                case "m_skinid":
                    {
                        csd.m_SkinID = Convert.ToInt32(ds.Value);
                        break;
                    }
                case "m_bootsid":
                    {
                        csd.m_BootsID = Convert.ToInt32(ds.Value);
                        break;
                    }
                case "m_poseid":
                    {
                        csd.m_PoseID = Convert.ToInt32(ds.Value);
                        break;
                    }
            }
        }

        return csd;
    }

    public void GetPlayerData(GameData gd)
    {

        CharacterStyleData csd = new CharacterStyleData();
        int iter = 0;

        if (ErrorText.text != null)
        {
            ErrorText.text = "Getting Player Data";
        }
        mDatabaseRef.Child("Users").Child(gd.ID).GetValueAsync().ContinueWith(task => {
            if (task.IsFaulted)
            {
                Debug.Log("Failed Getting user from databse");

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

                            case "pocketcoins":
                                gd.PocketCoins = Convert.ToInt32(obj.Value);
                                break;
                            case "appearance":
                                {
                                    csd = ProcessCharData(obj);
                                    break;
                                }
                            case "hascostumepack":
                                gd.HasCostumePack = Convert.ToInt32(obj.Value);
                                break;
                            case "hasar":
                                gd.HasAR = Convert.ToInt32(obj.Value);
                                break;
                        }
                    }
                    //Load up the character appear
                    if (LocalDataManager.Instance.HasCP())
                    {
                       charLoadout.customisationKitUnlocked = true;
                        charLoadout.LoadSavedLoadOut(csd);
                    }

                }

                catch (Exception ex)
                {
                    Debug.Log(ex);
                }
            }


            GetInventory(gd);
                            
            ShouldUpdatePlayer = true;
        });
    }



    //----------- Item Inventory Stuff -------------\\

    public void WriteItemInventory(GameData gd)
    {
        foreach (ItemData id in gd.ItemInv.GetItemDatas())
        {
            WriteItem(gd, id);
        }
    }

    public void GetItemInventory(GameData gd)
    {
        gd.ItemInv.Init();
        //Start a task that will populate the players inventory with their ppals.
        mDatabaseRef.Child("ItemInventories").Child(gd.ID).GetValueAsync().ContinueWith(task => {
            if (task.IsFaulted)
            {
                Debug.Log("Failed Getting user from databse Writing new user");
                WriteItemInventory(gd);
            }
            else if (task.IsCompleted)
            {
                try
                {
                    DataSnapshot snapshot = task.Result;
                    foreach (DataSnapshot obj in snapshot.Children)
                    {
                        if(obj.Key.ToLower() == "tracksandtrails")
                        {
                            //TO DO: GET TRACKS AND TRAILS
                        }

                        ItemData id = new ItemData();

                        foreach (DataSnapshot child in obj.Children)
                        {
                            switch (child.Key.ToLower())
                            {
                                case "numberowned":
                                    id.numberOwned = Convert.ToInt32(child.Value);
                                    break;
                                case "id":
                                    id.ID = Convert.ToInt32(child.Value);
                                    break;
                                case "name":
                                    id.name = Convert.ToString(child.Value);
                                    break;
                            }
                        }

                        gd.ItemInv.AddItem(id);
                    }
                }
                catch (Exception ex)
                {
                    Debug.Log(ex);
                }

                GlobalVariables.hasLoggedIn = true;
                loadingScreen.AllowToComplete();
            }

        });
    }

    public void WriteItem(GameData gd, ItemData id)
    {
        string json = JsonUtility.ToJson(id);
        mDatabaseRef.Child("ItemInventories").Child(gd.ID).Child(id.ID.ToString()).SetRawJsonValueAsync(json);
    }


    //------- Track Inventory stuff -----------\\

    public void WriteTracks(GameData gd)
    {
        foreach (TrackData td in gd.TrackInv.GetTracks())
        {
            WriteTrack(gd, td);
        }
    }

    public void WriteTrack(GameData gd, TrackData td)
    {
        string json = JsonUtility.ToJson(td);
        mDatabaseRef.Child("TracksAndTrails").Child(gd.ID).Child(td.uID.ToString()).SetRawJsonValueAsync(json);
    }

    public void GetTrackData(GameData gd)
    {
        mDatabaseRef.Child("TracksAndTrails").Child(gd.ID).GetValueAsync().ContinueWith(task => {
            if (task.IsFaulted)
            {
                Debug.Log("Failed Getting Tracks And Trails data from server");
                WriteTracks(gd);
            }
            else if (task.IsCompleted)
            {
                try
                {
                    DataSnapshot snapshot = task.Result;
                    foreach (DataSnapshot obj in snapshot.Children)
                    { 
                        TrackData td = new TrackData();

                        foreach (DataSnapshot child in obj.Children)
                        {
                            switch (child.Key.ToLower())
                            {
                                case "disttarget":
                                    td.distTarget = Convert.ToSingle(child.Value);
                                    break;
                                case "startdistance":
                                    td.startDistance = Convert.ToSingle(child.Value);
                                    break;
                                case "id":
                                    td.ID = Convert.ToInt32(child.Value);
                                    break;
                                case "uid":
                                    td.uID = (string)child.Value;
                                    break;
                            }
                        }

                        gd.TrackInv.TryAddTrack(td);
                    }
                }
                catch (Exception ex)
                {
                    Debug.Log(ex);
                }
                GetItemInventory(gd);
            }

        });
    }

    public void RemoveTrack(GameData gd, TrackData td)
    {
        mDatabaseRef.Child("TracksAndTrails").Child(gd.ID).Child(td.uID.ToString()).RemoveValueAsync();
    }

    //----------- PocketPalInventoryStuff ---------------\\

    public void GetInventory(GameData gd)
    {
        //Start a task that will populate the players inventory with their ppals.
        mDatabaseRef.Child("Inventories").Child(gd.ID).GetValueAsync().ContinueWith(task => {
            if (task.IsFaulted)
            {
                Debug.Log("Failed Getting user from databse Writing new user");
                WriteInventory(gd);
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
                                case "weight":
                                    ppd.weight = Convert.ToSingle(child.Value);
                                    break;
                                case "name":
                                    ppd.name = Convert.ToString(child.Value);
                                    break;
                                case "length":
                                    ppd.length = Convert.ToSingle(child.Value);
                                    break;
                                case "numbercaught":
                                    ppd.numberCaught = Convert.ToInt32(child.Value);
                                    break;
                                case "baserarity":
                                    ppd.baseRarity = Convert.ToInt32(child.Value);
                                    break;
                                case "hasrare":
                                    ppd.HasRare = Convert.ToInt32(child.Value);
                                    break;
                                case "haschampion":
                                    ppd.HasChampion = Convert.ToInt32(child.Value);
                                    break;
                                case "firstseen":
                                    ppd.FirstSeen = (string)child.Value;
                                    break;
                                case "lastseen":
                                    ppd.LastSeen = (string)child.Value;
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
                GetTrackData(gd);
            }
        });
    }

    public void WriteInventory(GameData gd)
    {
        foreach (PocketPalData ppd in gd.Inventory.GetMyPocketPals())
        {
            WritePocketPal(gd, ppd);
        }
    }

    public void WritePocketPal(GameData gd, PocketPalData ppd)
    {
        string json = JsonUtility.ToJson(ppd);
        mDatabaseRef.Child("Inventories").Child(gd.ID).Child(ppd.ID.ToString()).SetRawJsonValueAsync(json);
    }

    //---------------- Login Logout Statechanging stuff ------------------\\

    public void CreateUser(string email, string password, Text failedText, GameObject CreateUserScreen)
    {
        //used to stop firebase from auto signing in
        createUser = true;

        //Create a new user with the given user name and password.
        auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWith(task => {
            if (task.IsCanceled)
            {
                NotificationManager.Instance.CreateUserErrorNotification("Check Internet Connection");
                return;
            }
            if (task.IsFaulted)
            {
                NotificationManager.Instance.CreateUserErrorNotification("Email Address Is Already Registered Or It Is Invalid");
                return;
            }
            newUser = task.Result;
            UserCreated(CreateUserScreen);
        });
    }

    public void UserCreated(GameObject CreateUserScreen)
    {
        NotificationManager.Instance.LoginNotification("You Have Created An Account!");
        SendEmailVerification();
        CreateUserScript cus = CreateUserScreen.GetComponent<CreateUserScript>();
        LoginScreenScript lss = CreateUserScreen.GetComponent<CreateUserScript>().LoginScreen.GetComponent<LoginScreenScript>();
        lss.email.text = cus.Email.text;
        lss.password.text = cus.Password.text;
        CreateUserScreen.SetActive(false);
    }

    public void SignIn(string email, string password, Text failedText)
    {
        //to stop firebase to auto signin
        createUser = false;
        GlobalVariables.hasLoggedIn = false;

        auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWith(task => {
            if (task.IsCanceled)
            {
                NotificationManager.Instance.LoginFailedNotification("Check Internet Connection");
                return;
            }
            if (task.IsFaulted)
            {
                NotificationManager.Instance.LoginFailedNotification("Incorrect Username Or Password");
                return;
            }
            newUser = task.Result;
            Debug.LogFormat("User signed in successfully: {0} ({1})",
            newUser.DisplayName, newUser.UserId);

			canvasParent.CloseLogin(true);
        });
    }

    void AuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        if (createUser) return;
        newUser = auth.CurrentUser;
        if (newUser != null)
        {
            if (!newUser.IsEmailVerified)
            {
                NotificationManager.Instance.LoginFailedNotification("Please Verify Your Email Address Before Playing");
            }
            else
            {
                LocalDataManager.Instance.GetData().ID = newUser.UserId ?? "";
                LocalDataManager.Instance.GetData().Username = newUser.DisplayName ?? "";

                GetPlayerData(LocalDataManager.Instance.GetData());
                GPS.Insatance.UpdateMap();
            }
        }
        else
        {
            Debug.Log("Signed out ");
            ErrorText.text = "Signed out";
        }
    }

    void SendEmailVerification()
    {
        newUser.SendEmailVerificationAsync().ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                NotificationManager.Instance.ErrorNotification("Failed to send Verification Email");
                return;
            }
            if (task.IsFaulted)
            {
                NotificationManager.Instance.ErrorNotification("Failed to send Verification Email");
                return;
            }
            if (task.IsCompleted)
            {
                NotificationManager.Instance.LoginNotification("Verification Email has been sent");
            }
        });
        auth.SignOut();
    }

    public void TryLogOut()
    {
        NotificationManager.Instance.QuestionNotification("Are you sure you want to log out?", LogOut, null);
    }

    public void LogOut()
    {
        NotificationManager.Instance.LogoutNotification("Come Back Soon!!!");
        auth.SignOut();
        newUser = null;
        //LoginScreen.SetActive(true);
		canvasParent.GetComponent<Animator>().SetBool("logOut", true);
    }

    void OnDestroy()
    {
        auth.SignOut();
        auth.StateChanged -= AuthStateChanged;
        auth = null;
    }
}
