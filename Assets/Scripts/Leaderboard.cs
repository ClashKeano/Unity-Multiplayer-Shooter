using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Leaderboard : MonoBehaviour
{
    public TMP_Text playerName, Kills, Deaths;

    public void PlayerStat(string name, int kills, int deaths)
    {
        playerName.text = name;
        Kills.text = kills.ToString();
        Deaths.text = deaths.ToString();
    }
}
