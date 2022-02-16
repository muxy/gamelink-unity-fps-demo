using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using UnityEngine.SceneManagement;
using Unity.FPS;
using Unity.FPS.Game;
using Unity.FPS.Gameplay;
using UnityEngine.UI;
using UnityEditor;

using MuxyGameLink;

public struct GameDatastreamEvent
{
    public string spawnMonsterType;
    public string spawnPickupType;
}

public struct GameChannelState
{
    public int enemiesKilled;
    public int healthpacksPickedUp;
}

public class GameLinkFPSBehaviour : MonoBehaviour
{
    public String GAMELINK_CLIENT_ID = "";

    private GameObject TurretPrefab;
    private GameObject HoverbotPrefab;
    private GameObject HealthpackPrefab;
    private GameObject ShotgunPrefab;
    private GameObject JetpackPrefab;
    private GameObject LauncherPrefab;

    private String GAMELINK_PLAYERPREF_RT = "MuxyGameLinkRefreshToken";
    private InputField PINInput;
    private GameObject TotalVotesCountText;
    private GameObject VoteResultMessageText;
    private GameObject PollTimerText;
    private GameObject GameLinkLoginUI;
    private GameObject GameLinkPollUI;
    private GameObject Player;

    private float GravityModeDuration = 15;
    private float GravityModeTimer = 0;
    private string GravityModeType = "";

    private float PollDuration = 25;
    private float PollTimer = 0;
    private bool PollIsRunning = false;

    private GameChannelState State;

    private SDK GameLink;
    private WebsocketTransport Transport;
    private SDK.AuthenticationCallback AuthCB;
    private SDK.TransactionCallback TransactionCB;
    private SDK.DatastreamCallback DatastreamCB;
    private SDK.PollUpdateResponseCallback PollCB;

    private void SetupGameLink()
    {
        GameLink = new SDK(GAMELINK_CLIENT_ID);

        Transport = new WebsocketTransport(true);
        Transport.OpenAndRunInStage(GameLink, Stage.Sandbox);
    }

    private void SetupFindPrivateObjects()
    {
        TurretPrefab = PrefabUtility.LoadPrefabContents("Assets/FPS/Prefabs/Enemies/Enemy_Turret.prefab");
        HoverbotPrefab = PrefabUtility.LoadPrefabContents("Assets/FPS/Prefabs/Enemies/Enemy_Hoverbot.prefab");
        HealthpackPrefab = PrefabUtility.LoadPrefabContents("Assets/FPS/Prefabs/Pickups/Pickup_Health.prefab");
        ShotgunPrefab = PrefabUtility.LoadPrefabContents("Assets/FPS/Prefabs/Pickups/Pickup_Shotgun.prefab");
        JetpackPrefab = PrefabUtility.LoadPrefabContents("Assets/FPS/Prefabs/Pickups/Pickup_Jetpack.prefab");
        LauncherPrefab = PrefabUtility.LoadPrefabContents("Assets/FPS/Prefabs/Pickups/Pickup_Launcher.prefab");


        PINInput = GameObject.Find("GameLinkAuthInput").GetComponent<InputField>();
        GameLinkLoginUI = GameObject.Find("GameLinkLoginUI");
        GameLinkPollUI = GameObject.Find("GameLinkPollUI");
        TotalVotesCountText = GameObject.Find("GameLinkTotalVotes");
        VoteResultMessageText = GameObject.Find("GameLinkPollResultMessage");
        PollTimerText = GameObject.Find("GameLinkPollTimer"); 
        GameLinkPollUI.SetActive(false);
        TotalVotesCountText.SetActive(false);
        PollTimerText.SetActive(false);
    }

    private void SetupGameLinkCallbacks()
    {
        GameLink.OnDebugMessage((message) =>
        {
            Debug.Log(String.Format("MuxyGameLink: {0}", message));
        });

        AuthCB = (AuthResp) =>
        {
            Error Err = AuthResp.GetFirstError();
            if (Err == null)
            {
                // Successful auth, store refresh token so we can auth automatically next time
                if (GameLink.IsAuthenticated())
                {
                    String RefreshToken = GameLink.User?.RefreshToken;
                    if (RefreshToken != null)
                    {
                        if (PlayerPrefs.GetString(GAMELINK_PLAYERPREF_RT, "") == "") PlayerPrefs.SetString(GAMELINK_PLAYERPREF_RT, RefreshToken);
                    }

                    // Hide login UI
                    GameLinkLoginUI.SetActive(false);
                    GameLinkPollUI.SetActive(true);
                    GameLink.SubscribeToAllPurchases();
                    GameLink.SubscribeToDatastream();
                }
            }
            else
            {
                // Failed auth
                PINInput.text = "";
                PINInput.placeholder.GetComponent<Text>().text = "Authentication failed... invalid PIN!";
                PlayerPrefs.SetString(GAMELINK_PLAYERPREF_RT, "");
            }
        };

        TransactionCB = (Purchase) =>
        {
            if (Purchase.SKU == "spawn-turret" && Player)
            {
                Vector3 SpawnPos = Player.transform.position + Player.transform.forward * 10;
                Instantiate(TurretPrefab, SpawnPos, Quaternion.identity);
            }
        };

        DatastreamCB = (Data) =>
        {
            if (!Player) Debug.Log("GameLink no player found! Got datastream!");
            
            foreach(DatastreamUpdate.Event Event in Data.Events)
            {
                GameDatastreamEvent GameEvent = JsonUtility.FromJson<GameDatastreamEvent>(Event.Json);
                System.Random R = new System.Random();

                if (GameEvent.spawnMonsterType == "hoverbot")
                {
                    Vector3 SpawnPos = Player.transform.position + Player.transform.forward * R.Next(6, 18);
                    SpawnPos.z += R.Next(-5, 5);
                    GameObject Hoverbot = Instantiate(HoverbotPrefab, SpawnPos, Quaternion.identity);
                }
                if (GameEvent.spawnMonsterType == "turret")
                {
                    Vector3 SpawnPos = Player.transform.position + Player.transform.forward * R.Next(6, 18);
                    SpawnPos.z += R.Next(-5, 5);
                    GameObject Hoverbot = Instantiate(TurretPrefab, SpawnPos, Quaternion.identity);
                }


                if (GameEvent.spawnPickupType == "healthpack")
                {
                    Vector3 SpawnPos = Player.transform.position + Player.transform.forward * R.Next(4, 8);
                    SpawnPos.z += R.Next(-5, 5);
                    Instantiate(HealthpackPrefab, SpawnPos, Quaternion.identity);
                }
                else if (GameEvent.spawnPickupType == "shotgun")
                {
                    Vector3 SpawnPos = Player.transform.position + Player.transform.forward * R.Next(4, 8);
                    SpawnPos.z += R.Next(-5, 5);
                    Instantiate(ShotgunPrefab, SpawnPos, Quaternion.identity);
                }
                else if (GameEvent.spawnPickupType == "jetpack")
                {
                    Vector3 SpawnPos = Player.transform.position + Player.transform.forward * R.Next(4, 8);
                    SpawnPos.z += R.Next(-5, 5);
                    Instantiate(JetpackPrefab, SpawnPos, Quaternion.identity);
                }
                else if (GameEvent.spawnPickupType == "launcher")
                {
                    Vector3 SpawnPos = Player.transform.position + Player.transform.forward * R.Next(4, 8);
                    SpawnPos.z += R.Next(-5, 5);
                    Instantiate(LauncherPrefab, SpawnPos, Quaternion.identity);
                }
            }
        };

        PollCB = (Poll) =>
        {
            TotalVotesCountText.GetComponent<Text>().text = "Total Votes: " + GetPollSum(Poll.Results);
        };

        GameLink.OnTransaction(TransactionCB);
        GameLink.OnDatastream(DatastreamCB);
        GameLink.OnPollUpdate(PollCB);
    }
    public void OnClickAuthWithPIN()
    {
        GameLink.AuthenticateWithPIN(PINInput.text, AuthCB);
    }

    // why is poll.sum broken for 1 vote at 0 index? thats why we have this
    private int GetPollSum(List<int> Results)
    {
        int Sum = 0;
        for (int i = 0; i < Results.Count; i++)
        {
            Sum += Results[i];
        }
        return Sum;
    }
    private int GetPollWinnerIndex(List<int> Results)
    {
        int Winner = 0;
        int Index = -1;
        for (int i = 0; i < Results.Count; i++)
        {
            if (Results[i] > Winner)
            {
                Winner = Results[i];
                Index = i;
            }
        }

        return Index;
    }

    private void CleanupPoll()
    {
        PollIsRunning = false;
        GameLink.SendBroadcast("stop_poll", "{}");
        GameLink.DeletePoll("gravityMode");
    }

    public void OnClickStartPoll()
    {
        TotalVotesCountText.SetActive(true);
        PollTimerText.SetActive(true);
        List<string> PollOptions = new List<string> { "Low", "High" };
        GameLink.CreatePoll("gravityMode", "Vote for the gravity mode!", PollOptions);
        GameLink.SubscribeToPoll("gravityMode");
        GameLink.SendBroadcast("start_poll", "{}");
        TotalVotesCountText.GetComponent<Text>().text = "Total Votes: 0";
        PollTimer = PollDuration;
        PollIsRunning = true;
    }

    public void OnClickStopPoll()
    {
        TotalVotesCountText.SetActive(false);
        PollTimerText.SetActive(false);

        GameLink.GetPoll("gravityMode", (Poll) =>
        {
            PlayerCharacterController Controller = Player.GetComponent<PlayerCharacterController>();
            int Winner = GetPollWinnerIndex(Poll.Results);
            if (Winner != -1) // As long as there is a winner set the timer
            {
                GravityModeTimer = GravityModeDuration;
            }
            if (Winner == 0) // Low
            {
                GravityModeType = "Low";
                Controller.JumpForce = 20;
            }
            else if (Winner == 1) // High
            {
                GravityModeType = "High";
                Controller.JumpForce = 4;
            }
        });

        CleanupPoll();
    }
    private void ClearState()
    {
        State.enemiesKilled = 0;
        State.healthpacksPickedUp = 0;
        UpdateState();
    }
    private void UpdateState()
    {
        string json = JsonUtility.ToJson(State);
        GameLink.SetState(SDK.STATE_TARGET_CHANNEL, json);
        GameLink.SendBroadcast("channel_state_update", "{}");
    }
    private void OnEnemyKilled(EnemyKillEvent Evt)
    {
        State.enemiesKilled++;
        UpdateState();
    }    

    private void OnPlayerKilled(PlayerDeathEvent Evt)
    {
        ResetGravity();
        CleanupPoll();
        ClearState();
        VoteResultMessageText.GetComponent<Text>().text = "";
        GameLink.SendBroadcast("game_over", "{}");
    }

    private void OnPickup(PickupEvent Evt)
    {
        if (Evt.Pickup.GetComponent<HealthPickup>() && Player.GetComponent<Health>().CurrentHealth < Player.GetComponent<Health>().MaxHealth)
        {
            State.healthpacksPickedUp++;
            UpdateState();
        }

    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MainScene")
        {
            ClearState();
            Player = GameObject.Find("Player");
        }
    }

    private void ResetGravity()
    {
        GravityModeTimer = 0;
        Player.GetComponent<PlayerCharacterController>().JumpForce = 9;
        VoteResultMessageText.GetComponent<Text>().text = "";
    }

    private void HandleTimers()
    {
        if (!Player) return;

        if (GravityModeTimer > 0)
        {
            GravityModeTimer -= Time.deltaTime;
            VoteResultMessageText.GetComponent<Text>().text = GravityModeType + " gravity mode (" + (int)GravityModeTimer + ")";
        }
        else
        {
            ResetGravity();
        }

        if (PollIsRunning)
        {
            Time.timeScale = 1f;
            if (PollTimer > 0)
            {
                PollTimer -= Time.deltaTime;
            }
            else
            {
                PollTimer = 0;
                OnClickStopPoll();
                PollIsRunning = false;
            }
            PollTimerText.GetComponent<Text>().text = "Poll Timer: " + (int)PollTimer;
        }
    }

    void Start()
    {
        SetupFindPrivateObjects();
        SetupGameLink();
        SetupGameLinkCallbacks();
        EventManager.AddListener<EnemyKillEvent>(OnEnemyKilled);
        EventManager.AddListener<PlayerDeathEvent>(OnPlayerKilled);
        EventManager.AddListener<PickupEvent>(OnPickup);
        SceneManager.sceneLoaded += OnSceneLoaded;

        DontDestroyOnLoad(this.gameObject);
    }

    public void Update()
    {
        Transport.Update(GameLink);
        HandleTimers();
    }

    private void OnApplicationQuit()
    {
        ClearState();
        Transport.Stop();
    }

}
