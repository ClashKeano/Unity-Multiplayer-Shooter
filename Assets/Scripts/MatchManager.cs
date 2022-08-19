using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class MatchManager : MonoBehaviourPunCallbacks, IOnEventCallback
{

    public static MatchManager instance;

    private void Awake()
    {
        instance = this;
    }

    public List<PlayerData> allPlayerData = new List<PlayerData>();
    private int localPlayerIndex;

    private List<Leaderboard> leaderboardPlayer = new List<Leaderboard>();

    public enum EventCodes : byte
    {
        NewPlayer,
        ListPlayers,
        UpdatePlayerData,
        NextMatch,
        TimerSync
    }

    public enum GameState
    {
        Waiting,
        Playing,
        Ending
    }

    public int killsToWin = 3;
    public GameState state = GameState.Waiting;
    public float waitAfterEnding = 5f;

    public bool perpetual;

    public float matchLength = 300;
    private float currentMatchTime;

    private float sendTimer;

    // Start is called before the first frame update
    void Start()
    {
      if(!PhotonNetwork.IsConnected)
        {
            SceneManager.LoadScene(1); // Load Main mene if disconnected from network
            Cursor.lockState = CursorLockMode.None; // Unlock cursor when sent back to menus
        }
        else
        {
            SendNewPlayerEvent(PhotonNetwork.NickName); // Send new player event when connected to network

            state = GameState.Playing;
            setupTimer();
            if(!PhotonNetwork.IsMasterClient)
            {
                UIController.instance.timer.gameObject.SetActive(false);
            }
        }
     
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Tab) && state != GameState.Ending)
        {
            if(UIController.instance.Leaderboard.activeInHierarchy)
            {
                UIController.instance.Leaderboard.SetActive(false);
            }
            else
            {
                ShowLeaderboard();
            }
        }
        if (PhotonNetwork.IsMasterClient)
        {
            if (currentMatchTime > 0f && state == GameState.Playing)
            {
                currentMatchTime -= Time.deltaTime;

                if (currentMatchTime <= 0f)
                {
                    currentMatchTime = 0f;
                    state = GameState.Ending;


                     SendListPlayersEvent();
                     StateCheck();

                    
                }
                updateTimer();

                sendTimer -= Time.deltaTime;
                if(sendTimer <= 0)
                {
                    sendTimer += 1f;

                    timerSend();
                }
            }
        }
    }

    public void OnEvent(EventData photonEvent)
    {
        // Sent photon event data to relevant function

        if(photonEvent.Code < 200) // Codes 200 and over reserved for Photon systems
        {
            EventCodes newEvent = (EventCodes)photonEvent.Code; // Convert code received from Photon to one of our event codes

            object[] eventData = (object[])photonEvent.CustomData; // Convert custom data received from event into an array of objects (allows us to use this data)

           

            switch(newEvent)
            {
                case EventCodes.NewPlayer:

                    ReceiveNewPlayerEvent(eventData);

                    break;
                case EventCodes.ListPlayers:

                    ReceiveListPlayersEvent(eventData);

                    break;
                case EventCodes.UpdatePlayerData:

                    ReceiveUpdatePlayerDataEvent(eventData);

                    break;
                case EventCodes.NextMatch:
                    NextMatchReceive();
                    break;
                case EventCodes.TimerSync:
                    timerReceive(eventData);
                    break;
            }
        }
    }

    public override void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this); // When event callback happens, this will add it to the list
    }

    public override void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this); // When event callback happens, this will remove it to from the network list
    }

    public void SendNewPlayerEvent(string playerName)
    {
        object[] player = new object[4];
        player[0] = playerName; // Player username
        player[1] = PhotonNetwork.LocalPlayer.ActorNumber; // Player actor number on photon network
        player[2] = 0;  // Kills at start of match
        player[3] = 0;  // Deaths at start of match

        PhotonNetwork.RaiseEvent(
            (byte)EventCodes.NewPlayer, player, new RaiseEventOptions { Receivers = ReceiverGroup.MasterClient }, new SendOptions { Reliability = true }); // Send out new player event with relevant data only to master client



       
    }
    public void ReceiveNewPlayerEvent(object[] dataReceived)
    {
        PlayerData player = new PlayerData((string)dataReceived[0], (int)dataReceived[1], (int)dataReceived[2], (int)dataReceived[3]);

        allPlayerData.Add(player); // Add new player to all players list

        SendListPlayersEvent(); // Update player list whenever new plauer event occurs
    }

    public void SendListPlayersEvent()
    {
        object[] playersPackage = new object[allPlayerData.Count + 1];

        playersPackage[0] = state;

        for (int i = 0; i < allPlayerData.Count; i++)
        {
            object[] player = new object[4];

            player[0] = allPlayerData[i].name;
            player[1] = allPlayerData[i].number;
            player[2] = allPlayerData[i].kills;
            player[3] = allPlayerData[i].deaths;

            playersPackage[i + 1] = player; // Add player data to object array
        }

        PhotonNetwork.RaiseEvent(
            (byte)EventCodes.ListPlayers, playersPackage, new RaiseEventOptions { Receivers = ReceiverGroup.All }, new SendOptions { Reliability = true }); // Send out updated player list to all players
    }
    public void ReceiveListPlayersEvent(object[] dataReceived)
    {
        allPlayerData.Clear(); // Remove all current player data

        state = (GameState)dataReceived[0];

        for (int i = 1; i < dataReceived.Length; i++)
        {
            object[] player = (object[])dataReceived[i]; // Store received data in player object array

            PlayerData newPlayer = new PlayerData((string)player[0], (int)player[1], (int)player[2], (int)player[3]); // Create new player data using information stored in object array

            allPlayerData.Add(newPlayer); // Add new player to all players list

            if(PhotonNetwork.LocalPlayer.ActorNumber == newPlayer.number)
            {
                localPlayerIndex = i - 1; // If the player just added is the local player, index will be stored here
            }

        }

        StateCheck();

    }
    public void SendUpdatePlayerDataEvent(int sendingPlayerNumber, int statToUpdate)
    {
        object[] dataPackage = new object[] {sendingPlayerNumber, statToUpdate};

        PhotonNetwork.RaiseEvent(
           (byte)EventCodes.UpdatePlayerData, dataPackage, new RaiseEventOptions { Receivers = ReceiverGroup.All }, new SendOptions { Reliability = true });


}
    public void ReceiveUpdatePlayerDataEvent(object[] dataReceived)
    {
        int number = (int)dataReceived[0];
        int stat = (int)dataReceived[1];

        for(int i = 0; i < allPlayerData.Count; i++)
        {
            if (allPlayerData[i].number == number)
            { 
                if(stat == 0) // If stat to update is kills
                {
                    allPlayerData[i].kills++;

                    Debug.Log("Player " + allPlayerData[i].name + " : Kills = " + allPlayerData[i].kills);
                }
                else if(stat == 1) // If stat to update is deaths
                {
                    allPlayerData[i].deaths++;

                    Debug.Log("Player " + allPlayerData[i].name + " : Deaths = " + allPlayerData[i].deaths);
                }
                
                if (i == localPlayerIndex)
                {
                    updateStatsLabels();
                }

                if(UIController.instance.Leaderboard.activeInHierarchy)
                {
                    ShowLeaderboard();
                }

                break; // Break as the correct player has been found
            }
        }

        ScoreCheck();
    }

    public void updateStatsLabels()
    {
        UIController.instance.kills.text = "Kills: " + allPlayerData[localPlayerIndex].kills; // Update UI to show local player kills

        UIController.instance.deaths.text = "Deaths: " + allPlayerData[localPlayerIndex].deaths; // Update UI to show local player stats
    }

    void ShowLeaderboard()
    {
        UIController.instance.Leaderboard.SetActive(true);
        
        foreach(Leaderboard lp in leaderboardPlayer)
        {
            Destroy(lp.gameObject);
        }
        leaderboardPlayer.Clear();
        UIController.instance.topPlayer.gameObject.SetActive(false);
        List<PlayerData> sorted = sortPlayer(allPlayerData);
        foreach(PlayerData player in sorted)
        {
            Leaderboard newPlayerDisplay = Instantiate(UIController.instance.topPlayer, UIController.instance.topPlayer.transform.parent);

            newPlayerDisplay.PlayerStat(player.name, player.kills, player.deaths);

            newPlayerDisplay.gameObject.SetActive(true);

            leaderboardPlayer.Add(newPlayerDisplay);
        }
    }

    private List<PlayerData> sortPlayer(List<PlayerData> players)
    {
        List<PlayerData> sorted = new List<PlayerData>();

        while(sorted.Count < players.Count)
        {
            int highest = -1;
            PlayerData selection = players[0];
            foreach(PlayerData player in players)
            {
                if (!sorted.Contains(player))
                {
                    if (player.kills > highest)
                    {
                        selection = player;
                        highest = player.kills;
                    }
                }
            }

            sorted.Add(selection);
        }

        return sorted;
    }
    //check if the player quit the game load back to the main menu
    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
        SceneManager.LoadScene(1);
    }
    //will end the match if get enough kills
    void ScoreCheck()
    {
        bool winner = false;
        foreach(PlayerData player in allPlayerData)
        {
            if(player.kills >= killsToWin && killsToWin > 0)
            {
                winner = true;
                break;
            }
        }
        if(winner)
        {
            if(PhotonNetwork.IsMasterClient && state != GameState.Ending)
            {
                state = GameState.Ending;
                SendListPlayersEvent();
            }

        }
    }

    void StateCheck()
    {
        if(state == GameState.Ending)
        {
            EndMatch();
        }
    }

    void EndMatch()
    {
        state = GameState.Ending;

        if(PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.DestroyAll();
        }

        UIController.instance.EndMatch.SetActive(true);
        ShowLeaderboard();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        StartCoroutine(EndCo());
    }

    //wait for some time until return to main menu
    private IEnumerator EndCo()
    {
        yield return new WaitForSeconds(waitAfterEnding);
        if (!perpetual)
        {
            PhotonNetwork.AutomaticallySyncScene = false;
            PhotonNetwork.LeaveRoom();
        }
        else
        {
            if(PhotonNetwork.IsMasterClient)
            {
                NextMatchSend();
            }
        }
    }

    public void NextMatchSend()
    {
        PhotonNetwork.RaiseEvent((byte)EventCodes.NextMatch, null, new RaiseEventOptions { Receivers = ReceiverGroup.All }, new SendOptions { Reliability = true });

    }

    public void NextMatchReceive()
    {
        state = GameState.Playing;
        UIController.instance.EndMatch.SetActive(false);
        UIController.instance.Leaderboard.SetActive(false);

        foreach(PlayerData player in allPlayerData)
        {
            player.kills = 0;
            player.deaths = 0;
        }

        updateStatsLabels();

        SpawnManager.instance.SpawnPlayer();

        setupTimer();
    }

    public void setupTimer()
    {
        if(matchLength > 0)
        {
            currentMatchTime = matchLength;
            updateTimer();
        }
    }

    public void updateTimer()
    {
        var timeToDisplay = System.TimeSpan.FromSeconds(currentMatchTime);

        UIController.instance.timer.text = timeToDisplay.Minutes.ToString("00") + ":" + timeToDisplay.Seconds.ToString("00");
    }

    public void timerSend()
    {
        object[] package = new object[] { (int)currentMatchTime, state };

        PhotonNetwork.RaiseEvent(
            (byte)EventCodes.TimerSync,
            package,
            new RaiseEventOptions { Receivers = ReceiverGroup.All },
            new SendOptions { Reliability = true }
            );
    }
    public void timerReceive(object[] dataReceived)
    {
        currentMatchTime = (int)dataReceived[0];
        state = (GameState)dataReceived[1];

        updateTimer();

        UIController.instance.timer.gameObject.SetActive(true);
    }
}

[System.Serializable] // Allow class to show up in Unity inspector
public class PlayerData
{
    public string name;
    public int number, kills, deaths;

    public PlayerData(string nameInput, int numberInput, int killsInput, int deathsInput)
    {
        this.name = nameInput;
        this.number = numberInput;
        this.kills = killsInput;
        this.deaths = deathsInput;
    }
}