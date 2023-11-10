using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player : MonoBehaviour
{
    [Header("Floats")]
    public float damage = 20;
    public float speed;
    public float totalHealth;
    public float enemy_damage;
    
    
    public float smoothRotTime;
    private float turnSmoothVelocity;
    public float gravity;
    
    public float timeToAttack;
    public float colliderRadius;
    
    
    [Header("Booleans")]
    private bool waitFor;
    private bool takingHit;
    public bool isDead;
    [Header("Other")]
    private Animator anim;
    public Transform cam;
    public List<Transform> enemyList = new List<Transform>();
    Vector3 moveDirection;
    private CharacterController controller;
    public AudioSource attack;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
        cam = Camera.main.transform;
        attack = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isDead)
        {
            Move();
            GetMouseInput();
        }
        
    }


    private void Move()
    {
        if (controller.isGrounded)
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");

            Vector3 direction = new Vector3(horizontal, 0f, vertical);


            if (direction.magnitude > 0)
            {
                float angle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
                float smoothAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, angle, ref turnSmoothVelocity, smoothRotTime);
                transform.rotation = Quaternion.Euler(0f, smoothAngle, 0f);
                moveDirection = Quaternion.Euler(0f, angle, 0f) * Vector3.forward * speed;
                anim.SetInteger("transition", 2);
            }
            else
            {
                anim.SetInteger("transition", 1);
                moveDirection = Vector3.zero;
            }
        }
        moveDirection.y -= gravity * Time.deltaTime;
        controller.Move(moveDirection * Time.deltaTime);

    }

    void GetMouseInput()
    {
        if (controller.isGrounded)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (anim.GetBool("walking"))
                {
                    anim.SetBool("walking", false);
                    anim.SetInteger("transition", 1);
                }
                if(!anim.GetBool("walking"))
                {
                    StartCoroutine("Attack");
                }
                
            }
            
            
        }
    }

    IEnumerator Attack()
    {
        if (!waitFor && !takingHit)
        {
            waitFor = true;
            anim.SetBool("attacking", true);
            anim.SetInteger("transition", 3);
            attack.Play();
            yield return new WaitForSeconds(timeToAttack);
            GetEnemiesList();

            foreach (Transform enem in enemyList)
            {
                EnemyCombat enemy = enem.GetComponent<EnemyCombat>();

                if (enemy != null)
                {
                    enemy.GetHit();
                }
            }

            yield return new WaitForSeconds(1.5f);
            anim.SetInteger("transition", 1);
            waitFor = false;
        }
        
    }

    void GetEnemiesList()
    {
        enemyList.Clear();
        //foreach(Collider coll in Physics.OverlapSphere((transform.position + transform.forward * colliderRadius), colliderRadius))
        //{
            //if (coll.gameObject.CompareTag("Enemy"))
            //{
               // enemyList.Add(coll.transform);

           // }
        //}
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        //Gizmos.DrawWireSphere(transform.position * transform.forward, colliderRadius);
    }

    public void GetHit() //aqui tinha um float damage dentro dos parenteses
    {
        totalHealth -= enemy_damage;
        if (totalHealth > 0)
        {
            StopCoroutine("Attack");
            anim.SetInteger("transition", 4);
            takingHit = true;
            StartCoroutine("RecoveryFromHit");
        }
        else
        {
           isDead = true;
           anim.SetTrigger("die");
        }
    }

    IEnumerator RecoveryFromHit()
    {
        yield return new WaitForSeconds(1f);

        anim.SetInteger("transition", 1);
        takingHit = false;
        anim.SetBool("attacking", false);
        
    }
}
