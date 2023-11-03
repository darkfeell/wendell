using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyCombat : MonoBehaviour
{
    [Header("Atributes")]
    public float totalHealth;
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
        float distance = Vector3.Distance(player.position, transform.position);

        if(distance <= lookRadius)
        {
            meshAgent.isStopped = false;
            if (!attacking)
            {
                meshAgent.SetDestination(player.position);
                e_anim.SetBool("Walk Forward", true);
                walking = true;
            }
            
            if(distance <= meshAgent.stoppingDistance)
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
            meshAgent.isStopped = true;
            e_anim.SetBool("Walk Forward", false);
            walking = false;
            attacking = false;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, lookRadius);
    }

    IEnumerator Attack()
    {
        if (!waitFor)
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
        
    }

    void GetPlayer()
    {
        foreach(Collider coll in Physics.OverlapSphere((transform.position + transform.forward * colliderRadius), colliderRadius))
        {
            if (coll.gameObject.CompareTag("Player"))
            {

            }
        }
    }
}
