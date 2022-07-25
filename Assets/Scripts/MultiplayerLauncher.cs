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
    public GameObject invalidRoomName;
    public GameObject menuButtons;
    public GameObject createRoomButton;
    public GameObject createRoomScreen;
    public TMP_InputField createRoomInput;
    public GameObject roomScreen;
    public TMP_Text roomName;
    public GameObject errorScreen;
    public TMP_Text errorText;

    // Start is called before the first frame update
    void Start()
    {
        closeMenus(); // Close any open menus on game start
        loadingScreen.SetActive(true);
        loadingText.text = "Connecting to Network...";

        PhotonNetwork.ConnectUsingSettings();
    }

    void closeMenus()
    {
        loadingScreen.SetActive(false);
        menuButtons.SetActive(false);
        createRoomButton.SetActive(false);
        createRoomScreen.SetActive(false);
        roomScreen.SetActive(false);
        errorScreen.SetActive(false);
    }

    public override void OnConnectedToMaster()
    {

        PhotonNetwork.JoinLobby();
        loadingText.text = "Joining Lobby...";
    }

    public override void OnJoinedLobby()
    {
        closeMenus();
        menuButtons.SetActive(true);
    }

    public void loadCreateRoomScreen()
    {
        closeMenus();
        createRoomScreen.SetActive(true);
        createRoomButton.SetActive(true);
    }

    public void createRoom()
    {

        if (!string.IsNullOrEmpty(createRoomInput.text) &&  createRoomInput.text.Length <= 15)  // Ensure player enter some input for room name and set a cap for the length of the room name
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

    // Update is called once per frame
    void Update()
    {
        
    }
}
