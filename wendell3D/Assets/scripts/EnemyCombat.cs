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

    [Header("Components")]
    private Animator e_anim;
    private CapsuleCollider capColl;
    private NavMeshAgent meshAgent;

    [Header("Components")]
    private Transform player;
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

        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, lookRadius);
    }
}
