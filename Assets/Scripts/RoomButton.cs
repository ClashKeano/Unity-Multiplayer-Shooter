using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Realtime;

public class RoomButton : MonoBehaviour
{
    public TMP_Text roomButtonText;
    private RoomInfo roomInfo;
    
    public void setButtonInfo(RoomInfo inputInfo)
    {
        roomInfo = inputInfo;
        roomButtonText.text = roomInfo.Name; // Set button text to inputted RoomInfo
    }

    public void openRoom()
    {
        MultiplayerLauncher.instance.joinRoom(roomInfo);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
