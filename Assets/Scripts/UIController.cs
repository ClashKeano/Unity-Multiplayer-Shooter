using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
        // Toggling of kills and deaths on screen
        // Works in game in editor
        // But doesn't work smoothly in built version

        /*
        if (UIHiddenStatus == false)
        {
            if(Input.GetKey(KeyCode.H) && waitTime <= 0)
            {
                kills.gameObject.SetActive(false);
                deaths.gameObject.SetActive(false);

                UIHiddenStatus = true;

                waitTime = 10;
            }
            waitTime--;
            // Hide kills and deaths statistics if player presses H key
        }
        if (UIHiddenStatus == true)
        {
            if (Input.GetKey(KeyCode.H) && waitTime <= 0)
            {
                kills.gameObject.SetActive(true);
                deaths.gameObject.SetActive(true);

                UIHiddenStatus = false;

                waitTime = 10;
            }
            waitTime--;
            // Hide kills and deaths statistics if player presses H key
        }*/


    }
}
