using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnymyAI : MonoBehaviour
{

    [SerializeField] Transform target;
    public float chaseRange = 10f;
    float distanceToTarget = Mathf.Infinity;
    NavMeshAgent nav;
    public float damage = 5f;

    bool isProvoked = false;

    AIHealth health;
    // Start is called before the first frame update
    void Start()
    {
        nav = GetComponent<NavMeshAgent>();
        health = GetComponent<AIHealth>();
    }

    // Update is called once per frame
    void Update()
    {

        if (target != null)
        {
            if (health.IsDead())
            {
                enabled = false;
                nav.enabled = false;
            }
            else
            {
                distanceToTarget = Vector3.Distance(target.position, transform.position);
                if (isProvoked)
                {
                    EngageTarget();
                }
                else if (distanceToTarget <= chaseRange)
                {
                    isProvoked = true;
                    //nav.SetDestination(target.position);
                }
            }
        }
        
    }

    private void EngageTarget()
    {
        if(distanceToTarget >= nav.stoppingDistance)
        {
            ChaseTarget();
        }
        if(distanceToTarget <= nav.stoppingDistance)
        {
            AttackTarget();
        }
    }

    void ChaseTarget()
    {
        GetComponent<Animator>().SetBool("Attack", false);
        GetComponent<Animator>().SetTrigger("move");
        nav.SetDestination(target.position);


    }
    void AttackTarget()
    {
        if (target == null) return;
        GetComponent<Animator>().SetBool("Attack", true);
        
    }

    void animateAttack()
    {
        if (target == null) return;
        //target.GetComponent<SinglePlayerController>().DealDamage(damage);
        Debug.Log("attacking " + target.name);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 1, 0, 0.75f);
        Gizmos.DrawWireSphere(transform.position, chaseRange);
    }
}
