using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;
using UnityEngine.SceneManagement;



public class MultiplayerLauncher : MonoBehaviourPunCallbacks
{

    public static MultiplayerLauncher instance;

    private void Awake()
    {
        instance = this;
    }

    public GameObject loadingScreen;
    public TMP_Text loadingText;
    public GameObject invalidRoomName, invalidPlayerName;
    public GameObject menuButtons;
    public GameObject createRoomButton;
    public GameObject createRoomScreen;
    public GameObject returnButton;
    public TMP_InputField createRoomInput;
    public GameObject roomScreen;
    public TMP_Text roomName, playerName;
    public GameObject errorScreen;
    public TMP_Text errorText;
    public GameObject roomFinderScreen;
    public RoomButton originalRoomButton;
    private List<RoomButton> roomButtons = new List<RoomButton>();
    private List<TMP_Text>playerNameList = new List<TMP_Text>();
    public GameObject playerNameScreen;
    public TMP_InputField playerNameInput;
    public static bool isNameSet;
    public string currentLevel;
    public GameObject startGameButton;

    static class Constants
    {
        public const int maxPlayerNameLength = 15;
        public const int maxRoomNameLength = 15;
    }

    // Start is called before the first frame update
    void Start()
    {
        closeMenus(); // Close any open menus on game start
        loadingScreen.SetActive(true);
        loadingText.text = "Connecting to Network...";

        PhotonNetwork.ConnectUsingSettings();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    
    void closeMenus()
    {
        loadingScreen.SetActive(false);
        menuButtons.SetActive(false);
        createRoomButton.SetActive(false);
        createRoomScreen.SetActive(false);
        roomScreen.SetActive(false);
        errorScreen.SetActive(false);
        roomFinderScreen.SetActive(false);
        playerNameScreen.SetActive(false);
    }

    public override void OnConnectedToMaster()
    {

        PhotonNetwork.JoinLobby();
        loadingText.text = "Joining Lobby...";

        PhotonNetwork.AutomaticallySyncScene = true;

        
    }

    public override void OnJoinedLobby()
    {
        closeMenus();
        menuButtons.SetActive(true);
        PhotonNetwork.NickName = "Player " + Random.Range(0, 1000).ToString();

        if (!isNameSet)
        {
            closeMenus();
            playerNameScreen.SetActive(true);

            if (PlayerPrefs.HasKey("playerName")) // Check if player name has been already saved to player prefs
            {
                playerNameInput.text = PlayerPrefs.GetString("playerName"); // Assigns previously set player name
            }
        }


    }

    public void SetName()
    {
        if (!string.IsNullOrEmpty(playerNameInput.text) && playerNameInput.text.Length <= Constants.maxPlayerNameLength)
        {
            PhotonNetwork.NickName = playerNameInput.text;

            PlayerPrefs.SetString("playerName", playerNameInput.text); // Stores player name between sessions

            closeMenus();
            menuButtons.SetActive(true);

            isNameSet = true;
        }
        else
        {
            invalidPlayerName.SetActive(true);
        }
    }

    public void loadCreateRoomScreen()
    {
        closeMenus();
        createRoomScreen.SetActive(true);
        createRoomButton.SetActive(true);
        returnButton.SetActive(true);

    }

    public void createRoom()
    {

        if (!string.IsNullOrEmpty(createRoomInput.text) &&  createRoomInput.text.Length <= Constants.maxRoomNameLength)  // Ensure player enter some input for room name and set a cap for the length of the room name
        {
            RoomOptions roomOptions = new RoomOptions();
            roomOptions.MaxPlayers = 6; // Limit number of players in a room
            PhotonNetwork.CreateRoom(createRoomInput.text, roomOptions); // Create new room with input name and above options

            closeMenus();
            loadingText.text = "Creating Room (" + createRoomInput.text +") ...";
            loadingScreen.SetActive(true);
        }
        else
        {
            invalidRoomName.SetActive(true);
        }
       
    }

    public override void OnJoinedRoom()
    {
        closeMenus();
        roomScreen.SetActive(true);
        roomName.text = "Room Name: " + PhotonNetwork.CurrentRoom.Name;

        listPlayers();

        if (PhotonNetwork.IsMasterClient)
        {
            startGameButton.SetActive(true);    // Only show start game button for master player
        }
        else
        {
            startGameButton.SetActive(false);
        }


    }

    private void listPlayers()
    {
        foreach (TMP_Text player in playerNameList)
        {
            Destroy(player.gameObject);
        }

        playerNameList.Clear();

        Player[] players = PhotonNetwork.PlayerList;

        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            updatePlayerList(PhotonNetwork.PlayerList[i]);
        }
    }

    public void updatePlayerList(Player player)
    {
        TMP_Text newPlayerName = Instantiate(playerName, playerName.transform.parent);
        newPlayerName.text = player.NickName;
        newPlayerName.gameObject.SetActive(true);
        playerNameList.Add(newPlayerName);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        updatePlayerList(newPlayer);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        listPlayers();
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        errorScreen.SetActive(true);
        errorText.text = "Failed to create room: " + message;
    }

    public void leaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        closeMenus();
        loadingText.text = "Leaving Room... ";
        loadingScreen.SetActive(true);
    }

   
    public void returnToMultiplayerMenu()
    {
        closeMenus();
        menuButtons.SetActive(true);
        
    }

    public IEnumerator addDelay(int i)
    {
        yield return new WaitForSeconds(i);
    }

    public void returnToMainMenu(string sceneName)
    {

        SceneManager.LoadScene(sceneName);
        PhotonNetwork.Disconnect();
        loadingText.text = "Disconnecting from Network...";
        loadingScreen.SetActive(true);
        StartCoroutine(addDelay(5));

    }

    public void openRoomFinder()
    {
        closeMenus();
        roomFinderScreen.SetActive(true);
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach(RoomButton roomButton in roomButtons)
        {
            Destroy(roomButton.gameObject); // Delete room buttons
        }

        roomButtons.Clear(); // Clear the previous information from the list

        originalRoomButton.gameObject.SetActive(false);

        for (int i = 0; i < roomList.Count; i++) // Loops through all available rooms
        {
            if (roomList[i].PlayerCount != roomList[i].MaxPlayers) // Only display rooms that are not full
            {
                RoomButton newRoomButton = Instantiate(originalRoomButton, originalRoomButton.transform.parent);
                newRoomButton.setButtonInfo(roomList[i]);
                newRoomButton.gameObject.SetActive(true);

                roomButtons.Add(newRoomButton);
            }

        }
    }

    public void joinRoom(RoomInfo inputRoomInfo)
    {
        PhotonNetwork.JoinRoom(inputRoomInfo.Name);
        closeMenus();
        loadingText.text = loadingText.text = "Joining Room (" + inputRoomInfo.Name + ") ...";
        loadingScreen.SetActive(true);
    }

    public void StartGame()
    {
        PhotonNetwork.LoadLevel(currentLevel);
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            startGameButton.SetActive(true);    // Only show start game button for master player
        }
        else
        {
            startGameButton.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
