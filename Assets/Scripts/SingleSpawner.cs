using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleSpawner : MonoBehaviour
{
    public static SingleSpawner instance;

    public Transform[] spawnPoints;

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

            SpawnPlayer();



    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SpawnPlayer()
    {
        Transform spawnPoint = spawn();
        player = Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);

    }

    public Transform spawn()
    {
        return spawnPoints[Random.Range(0, spawnPoints.Length)];
    }

    }
