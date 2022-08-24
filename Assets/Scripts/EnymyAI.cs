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
    public float damage = 10f;

    bool isProvoked = false;
    // Start is called before the first frame update
    void Start()
    {
        nav = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        if (target != null)
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
        nav.SetDestination(target.position);

    }
    void AttackTarget()
    {
        if (target == null) return;
        target.GetComponent<SinglePlayerController>().DealDamage(damage);
        Debug.Log("attacking " + target.name);
        
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 1, 0, 0.75f);
        Gizmos.DrawWireSphere(transform.position, chaseRange);
    }
}
