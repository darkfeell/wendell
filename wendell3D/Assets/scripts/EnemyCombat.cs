using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyCombat : MonoBehaviour
{
    [Header("Atributes")]
    public float totalHealth = 100f;
    public float attackDamage;
    public float moveSpeed;
    public float lookRadius;
    public float colliderRadius;
    

    [Header("Components")]
    private Animator e_anim;
    private CapsuleCollider capColl;
    private NavMeshAgent meshAgent;

    [Header("Others")]
    private Transform player;
    private bool walking;
    private bool attacking;
    private bool waitFor;
    private bool takeHit;
    public bool playerIsDead;

    [Header("Waypoints")]
    public List<Transform> waypoints = new List<Transform>();
    public int currentWaypointIndex;
    public float pathDistance;
    // Start is called before the first frame update
    void Start()
    {
        e_anim = GetComponent<Animator>();
        capColl = GetComponent<CapsuleCollider>();
        meshAgent = GetComponent<NavMeshAgent>();

        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        if(totalHealth > 0)
        {
            float distance = Vector3.Distance(player.position, transform.position);

            if (distance <= lookRadius)
            {
                meshAgent.isStopped = false;
                if (!attacking)
                {
                    meshAgent.SetDestination(player.position);
                    e_anim.SetBool("Walk Forward", true);
                    walking = true;
                }

                if (distance <= meshAgent.stoppingDistance)
                {
                    StartCoroutine("Attack");
                }
                else
                {
                    attacking = false;
                }
            }
            else
            {
                //meshAgent.isStopped = true;
                e_anim.SetBool("Walk Forward", false);
                walking = false;
                attacking = false;
                MoveToWaypoint();
            }
        }
        
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, lookRadius);
    }

    void MoveToWaypoint()
    {
        if(waypoints.Count > 0)
        {
            float distance = Vector3.Distance(waypoints[currentWaypointIndex].position, transform.position);
            meshAgent.destination = waypoints[currentWaypointIndex].position;
            if(distance <= pathDistance)
            {
                currentWaypointIndex = Random.Range(0, waypoints.Count);
            }
        }
    }

    IEnumerator Attack()
    {
        if (!waitFor && !takeHit && !playerIsDead)
        {
            waitFor = true;
            attacking = true;
            walking = false;
            e_anim.SetBool("Walk Forward", false);
            e_anim.SetBool("Bite Attack", true);
            yield return new WaitForSeconds(1.2f);

            GetPlayer();


           // yield return new WaitForSeconds(1f);
            waitFor = false;
        }

        if (playerIsDead)
        {
            e_anim.SetBool("Walk Forward", false);
            e_anim.SetBool("Bite Attack", false);
            walking = false;
            attacking = false;
            meshAgent.isStopped = true;

        }
        
    }

    void GetPlayer()
    {
        foreach(Collider coll in Physics.OverlapSphere((transform.position + transform.forward * colliderRadius), colliderRadius))
        {
            if (coll.gameObject.CompareTag("Player"))
            {
                coll.gameObject.GetComponent<player>().GetHit();
                playerIsDead = coll.gameObject.GetComponent<player>().isDead;
            }
        }
    }

    public void GetHit() //aqui tinha um float damage dentro dos parenteses
    {
        totalHealth =- attackDamage;
        if(totalHealth > 0)
        {
            StopCoroutine("Attack");
            e_anim.SetTrigger("Take Damage");
            takeHit = true;
            StartCoroutine("RecoveryFromHit");
        }
        else
        {
            e_anim.SetTrigger("Die");
        }
    } 

    IEnumerator RecoveryFromHit()
    {
        yield return new WaitForSeconds(1f);
        e_anim.SetBool("Walk Forward", false);
        e_anim.SetBool("Bite Attack", true);
        takeHit = false;
        waitFor = false;
    }
}
