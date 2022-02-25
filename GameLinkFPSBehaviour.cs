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
    public int hoverBotsKilled;
    public int turretsKilled;
}

public class GameLinkFPSBehaviour : MonoBehaviour
{
    public String GAMELINK_EXTENSION_ID = "";

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

    private SDK.PatchList PatchList;
    private float PatchListSendTime = 0.2f;
    private float PatchListSendTimer = 0.0f;
    private void SetupGameLink()
    {
        GameLink = new SDK(GAMELINK_EXTENSION_ID);
        PatchList = new SDK.PatchList();
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
            if (!Player) return;

            System.Random R = new System.Random();
            if (Purchase.SKU == "spawn-turret")
            {
                GameObject Turret = Instantiate(TurretPrefab, RandomSpawnPositionNearby(R.Next(6, 18), R.Next(-5, 5)), Quaternion.identity);
                Turret.GetComponent<Health>().MaxHealth += 200;
            }
            else if(Purchase.SKU == "spawn-hoverbot")
            {
                GameObject Hoverbot = Instantiate(HoverbotPrefab, RandomSpawnPositionNearby(R.Next(6, 18), R.Next(-5, 5)), Quaternion.identity);
                Hoverbot.GetComponent<Health>().MaxHealth += 50;
            }
        };

        DatastreamCB = (Data) =>
        {
            foreach(DatastreamUpdate.Event Event in Data.Events)
            {
                GameDatastreamEvent GameEvent = JsonUtility.FromJson<GameDatastreamEvent>(Event.Json);
                System.Random R = new System.Random();

                if (GameEvent.spawnMonsterType == "hoverbot")
                {
                    GameObject Hoverbot = Instantiate(HoverbotPrefab, RandomSpawnPositionNearby(R.Next(6, 18), R.Next(-5, 5)), Quaternion.identity);
                }
                if (GameEvent.spawnMonsterType == "turret")
                {
                    GameObject Hoverbot = Instantiate(TurretPrefab, RandomSpawnPositionNearby(R.Next(6, 18), R.Next(-5, 5)), Quaternion.identity);
                }


                if (GameEvent.spawnPickupType == "healthpack")
                {
                    Instantiate(HealthpackPrefab, RandomSpawnPositionNearby(R.Next(4, 8), R.Next(-5, 5)), Quaternion.identity);
                }
                else if (GameEvent.spawnPickupType == "shotgun")
                {
                    Instantiate(ShotgunPrefab, RandomSpawnPositionNearby(R.Next(4, 8), R.Next(-5, 5)), Quaternion.identity);
                }
                else if (GameEvent.spawnPickupType == "jetpack")
                {
                    Instantiate(JetpackPrefab, RandomSpawnPositionNearby(R.Next(4, 8), R.Next(-5, 5)), Quaternion.identity);
                }
                else if (GameEvent.spawnPickupType == "launcher")
                {
                    Instantiate(LauncherPrefab, RandomSpawnPositionNearby(R.Next(4, 8), R.Next(-5, 5)), Quaternion.identity);
                }
            }
        };

        PollCB = (Poll) =>
        {
            TotalVotesCountText.GetComponent<Text>().text = "Total Votes: " + Poll.Sum;
        };

        GameLink.OnTransaction(TransactionCB);
        GameLink.OnDatastream(DatastreamCB);
        GameLink.OnPollUpdate(PollCB);
    }
    public void OnClickAuthWithPIN()
    {
        GameLink.AuthenticateWithPIN(PINInput.text, AuthCB);
    }

    private Vector3 RandomSpawnPositionNearby(float FrontDistance, float Variation)
    {
        if (!Player) return new Vector3(0, 0, 0);
        Vector3 SpawnPos = Player.transform.position + Player.transform.forward * FrontDistance;
        SpawnPos.z += Variation;
        return SpawnPos;
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
        GameLink.SendBroadcast("start_poll", "{\"poll_duration\":\"" + PollDuration + "\"}");
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
            int Winner = Poll.GetWinnerIndex();
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
        State.hoverBotsKilled = 0;
        State.turretsKilled = 0;
        GameLink.SetState(SDK.STATE_TARGET_CHANNEL, JsonUtility.ToJson(State));
    }

    private void OnEnemyKilled(EnemyKillEvent Evt)
    {
        if (Evt.Enemy.name.ToLower().Contains("hoverbot"))
        {
            State.hoverBotsKilled++;
        }
        else if (Evt.Enemy.name.ToLower().Contains("turret"))
        {
            State.turretsKilled++;
        }

        PatchList.UpdateStateWithInteger("add", "/" + nameof(State.hoverBotsKilled), State.hoverBotsKilled);
        PatchList.UpdateStateWithInteger("add", "/" + nameof(State.turretsKilled), State.turretsKilled);
    }    

    private void OnPlayerKilled(PlayerDeathEvent Evt)
    {
        ResetGravity();
        CleanupPoll();
        ClearState();
        VoteResultMessageText.GetComponent<Text>().text = "";
        GameLink.SendBroadcast("game_over", "{}");
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

    private void HandleGameTimers()
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

    private void HandlePatchListSend()
    {
        PatchListSendTimer -= Time.deltaTime;
        if (PatchListSendTimer <= 0)
        {
            if (!PatchList.Empty())
            {
                GameLink.UpdateStateWithPatchList(SDK.STATE_TARGET_CHANNEL, PatchList);
                PatchList.Clear();
            }
            PatchListSendTimer = PatchListSendTime;
        }
    }    

    void Start()
    {
        SetupFindPrivateObjects();
        SetupGameLink();
        SetupGameLinkCallbacks();
        EventManager.AddListener<EnemyKillEvent>(OnEnemyKilled);
        EventManager.AddListener<PlayerDeathEvent>(OnPlayerKilled);
        SceneManager.sceneLoaded += OnSceneLoaded;

        DontDestroyOnLoad(this.gameObject);
    }

    public void Update()
    {
        Transport.Update(GameLink);
        HandleGameTimers();
        HandlePatchListSend();
    }

    private void OnApplicationQuit()
    {
        PrefabUtility.UnloadPrefabContents(TurretPrefab);
        PrefabUtility.UnloadPrefabContents(HoverbotPrefab);
        PrefabUtility.UnloadPrefabContents(HealthpackPrefab);
        PrefabUtility.UnloadPrefabContents(ShotgunPrefab);
        PrefabUtility.UnloadPrefabContents(JetpackPrefab);
        PrefabUtility.UnloadPrefabContents(LauncherPrefab);

        PatchList.FreeMemory();
        ClearState();
        Transport.Stop();
    }

}
