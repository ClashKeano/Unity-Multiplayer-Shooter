using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager instance;

    public Transform[] spawnPoints;
    public GameObject deathEffect;

    private void Awake()
    {
        instance = this;
    }

    public GameObject playerPrefab;
    private GameObject player;
    public float respawnTime = 2f;
    // Start is called before the first frame update
    void Start()
    {
        foreach (Transform spawnPoint in spawnPoints)
        {
            spawnPoint.gameObject.SetActive(false);
        }

        if(PhotonNetwork.IsConnected)
        {
            SpawnPlayer();
        }

        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnPlayer()
    {
        Transform spawnPoint = spawn();

        player = PhotonNetwork.Instantiate(playerPrefab.name, spawnPoint.position, spawnPoint.rotation);
    }

    public Transform spawn()
    {
        return spawnPoints[Random.Range(0, spawnPoints.Length)];
    }

    public void PlayerDeath(string damager)
    {

        UIController.instance.deathText.text = "You were killed by " + damager;


        if(player != null)
        {
            StartCoroutine(Die());
        }

    }

    public IEnumerator Die()
    {
        PhotonNetwork.Instantiate(deathEffect.name, player.transform.position, Quaternion.identity);
        PhotonNetwork.Destroy(player);
        UIController.instance.deathMessage.SetActive(true);

        yield return new WaitForSeconds(respawnTime);

        UIController.instance.deathMessage.SetActive(false);
        SpawnPlayer();
    }
}
