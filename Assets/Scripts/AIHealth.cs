using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIHealth : MonoBehaviour
{
    public float Health = 100f;
    public bool isDead = false;

    public bool IsDead()
    {
        return isDead;
    }


    // Start is called before the first frame update
    public void TakeDamage(float damage)
    {
        Health -= damage;
        if(Health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;
        GetComponent<Animator>().SetTrigger("die");
        Destroy(gameObject, 3f);
    }

    }

