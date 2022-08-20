using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class UIController : MonoBehaviour
{
    public static UIController instance;

    public TMP_Text overheatedMessage;
    public Slider weaponHeatSlider;
    public GameObject deathMessage;
    public TMP_Text deathText;
    public Slider healthBar;
    public TMP_Text kills, deaths;
    public bool UIHiddenStatus = false;
    public int waitTime;

    public GameObject Leaderboard;
    public Leaderboard topPlayer;

    public TMP_Text timer;

    public GameObject EndMatch;
    public GameObject optionsScreen;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
        if(Input.GetKeyUp(KeyCode.Escape))
        {
            options();
        }

        if(optionsScreen.activeInHierarchy && Cursor.lockState != CursorLockMode.None)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            // If cursor is not unlocked when options screen is active: unlock it
        }

    }

    public void options()
    { 
        if(!optionsScreen.activeInHierarchy)
        {
            optionsScreen.SetActive(true);
  
        }else
        {
            optionsScreen.SetActive(false);
        }

    }

    public void returnToMainMenu()
    {
        PhotonNetwork.AutomaticallySyncScene = false;
        PhotonNetwork.LeaveRoom();
    }

    public void quitGame()
    {
        Application.Quit();
    }
}
