using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class MatchManager : MonoBehaviour
{

    public static MatchManager isntance;

    private void Awake()
    {
        isntance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
      if(!PhotonNetwork.IsConnected)
        {
            SceneManager.LoadScene(1); // Load Main mene if disconnected from network
            Cursor.lockState = CursorLockMode.None;
        }
     
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
