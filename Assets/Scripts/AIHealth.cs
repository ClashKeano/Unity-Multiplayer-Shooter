using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIHealth : MonoBehaviour
{
    public float Health = 100f;

    // Start is called before the first frame update
    public void TakeDamage(float damage)
    {
        Health -= damage;
        if(Health <= 0)
        {
            Destroy(gameObject);
        }
    }
}
