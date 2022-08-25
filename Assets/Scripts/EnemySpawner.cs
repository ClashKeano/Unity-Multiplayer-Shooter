using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{

    public GameObject enemy;
    public int enemyCount = 0;
    public int totalEnemies = 20;


    // Start is called before the first frame update

    void Start()
    {
        StartCoroutine(enemyDrop());

    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator enemyDrop()
    {
        while (enemyCount < 2000)
        {
            Instantiate(enemy, new Vector3(Random.Range(-40, 40), 4, 0), Quaternion.identity);
            Instantiate(enemy, new Vector3(0, 4, Random.Range(-40, 40)), Quaternion.identity);
            yield return new WaitForSeconds(4f);
            enemyCount++;
            
            
            EnemyAI.instance.zombieSounds[Random.Range(0, EnemyAI.instance.zombieSounds.Length)].Play();
        }

        

    }
}
    