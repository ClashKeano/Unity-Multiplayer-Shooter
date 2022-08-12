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

    public enum EventCodes : byte
    {
        NewPlayer,
        ListPlayers,
        UpdatePlayerData,
    }

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
        }
     
    }

    // Update is called once per frame
    void Update()
    {
        
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
        object[] playersPackage = new object[allPlayerData.Count];

        for (int i = 0; i < allPlayerData.Count; i++)
        {
            object[] player = new object[4];

            player[0] = allPlayerData[i].name;
            player[1] = allPlayerData[i].number;
            player[2] = allPlayerData[i].kills;
            player[3] = allPlayerData[i].deaths;

            playersPackage[i] = player; // Add player data to object array
        }

        PhotonNetwork.RaiseEvent(
            (byte)EventCodes.ListPlayers, playersPackage, new RaiseEventOptions { Receivers = ReceiverGroup.All }, new SendOptions { Reliability = true }); // Send out updated player list to all players
    }
    public void ReceiveListPlayersEvent(object[] dataReceived)
    {
        allPlayerData.Clear(); // Remove all current player data

        for (int i = 0; i < dataReceived.Length; i++)
        {
            object[] player = (object[])dataReceived[i]; // Store received data in player object array

            PlayerData newPlayer = new PlayerData((string)player[0], (int)player[1], (int)player[2], (int)player[3]); // Create new player data using information stored in object array

            allPlayerData.Add(newPlayer); // Add new player to all players list

            if(PhotonNetwork.LocalPlayer.ActorNumber == newPlayer.number)
            {
                localPlayerIndex = i; // If the player just added is the local player, index will be stored here
            }

        }

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

                break; // Break as the correct player has been found
            }
        }
    }

    public void updateStatsLabels()
    {
        UIController.instance.kills.text = "Kills: " + allPlayerData[localPlayerIndex].kills; // Update UI to show local player kills

        UIController.instance.deaths.text = "Deaths: " + allPlayerData[localPlayerIndex].deaths; // Update UI to show local player stats
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