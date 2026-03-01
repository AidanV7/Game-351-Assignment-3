using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float impulseForce  = 5000.0f;    
    public float impulseTorque = 3000.0f;

    [Header("Footstep Settings")]
    public float footstepInterval = 0.5f;  
    private float footstepTimer = 0f;

    [Header("Kick Settings")]
    public float kickForce = 10f;            
    public float kickRange = 2f;             
    public Transform kickPoint;              
    public LayerMask kickableLayer;          

    [Header("Shooting")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float fireRate = 1f;

    public GameObject hero;

    private Animator animController;
    private Rigidbody rigidBody;

    private float nextFireTime = 0f;

    void Start()
    {
        animController = hero.GetComponent<Animator>();
        rigidBody      = GetComponent<Rigidbody>();
    }

    void Update()
    {
        HandleMovement();
        HandleCrouch();
        HandleKick();
        HandleShooting();
    }

    void HandleMovement()
    {
        Vector3 input = new Vector3(0, Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        bool isWalking = input.magnitude > 0.001f && !animController.GetBool("Crouch");

        if (isWalking)
        {
            rigidBody.AddRelativeTorque(new Vector3(0, input.y * impulseTorque * Time.deltaTime, 0));
            rigidBody.AddRelativeForce(new Vector3(0, 0, input.z * impulseForce * Time.deltaTime));

            animController.SetBool("Walk", true);

            // Footstep sound
            footstepTimer -= Time.deltaTime;
            if (footstepTimer <= 0f)
            {
                SoundManager.Instance.PlayWalking(transform.position);
                footstepTimer = footstepInterval;
            }
        }
        else
        {
            animController.SetBool("Walk", false);
            footstepTimer = 0f; // reset timer
        }
    }

    void HandleCrouch()
    {
        animController.SetBool("Crouch", Input.GetKey(KeyCode.C));
    }

    void HandleKick()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            int randKick = Random.Range(0, 3);
            animController.SetInteger("KickType", randKick);
            animController.SetTrigger("Kick");
        }
    }

    void PerformKick()
    {
        Vector3 kickCenter = transform.position + transform.forward * kickRange;
        Collider[] hits = Physics.OverlapSphere(kickCenter, kickRange, kickableLayer);

        foreach (Collider hit in hits)
        {
            Rigidbody rb = hit.attachedRigidbody;
            if (rb != null && !rb.isKinematic)
            {
                Vector3 forceDir = (hit.transform.position - transform.position).normalized;
                forceDir.y = 0.3f;
                rb.AddForce(forceDir * kickForce, ForceMode.Impulse);
            }
        }
    }

    void HandleShooting()
    {
        if (Input.GetKeyDown(KeyCode.F) && Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + fireRate;
            animController.SetTrigger("Shoot");

            // Instantiate bullet
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

            // Play gunshot sound
            SoundManager.Instance.PlayGunshot(firePoint.position);
        }
    }

    // Optional: visualize kick range
    void OnDrawGizmosSelected()
    {
        if (kickPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(kickPoint.position, kickRange);
        }
    }
}