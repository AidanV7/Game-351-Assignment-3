using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float impulseForce = 5000f;
    public float impulseTorque = 3000f;

    [Header("Footsteps")]
    public float footstepInterval = 0.5f;
    private float footTimer = 0f;

    [Header("Kick")]
    public float kickForce = 10f;
    public float kickRange = 2f;
    public Transform kickPoint;
    public LayerMask kickables;

    [Header("Shooting")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float fireRate = 1f;

    [Header("First Person")]
    public Camera playerCamera;      // assign your FPS camera here
    public GameObject hero;          // your full-body mesh/rig
    public LayerMask bodyLayer;      // assign a layer like "PlayerBody"

    private Animator anim;
    private Rigidbody rb;
    private float nextFire = 0f;

    void Start()
    {
        anim = hero.GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();

        // Hide body from first-person camera
        if (playerCamera)
        {
            playerCamera.cullingMask &= ~(1 << bodyLayer);
        }
    }

    void Update()
    {
        Move();
        Crouch();
        Kick();
        Shoot();
    }

    void Move()
    {
        Vector3 input = new Vector3(0, Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        bool walking = input.magnitude > 0.001f && !anim.GetBool("Crouch");

        if (walking)
        {
            rb.AddRelativeTorque(0, input.y * impulseTorque * Time.deltaTime, 0);
            rb.AddRelativeForce(0, 0, input.z * impulseForce * Time.deltaTime);
            anim.SetBool("Walk", true);

            footTimer -= Time.deltaTime;
            if (footTimer <= 0f)
            {
                SoundManager.Instance.PlayWalking(transform.position);
                footTimer = footstepInterval;
            }
        }
        else
        {
            anim.SetBool("Walk", false);
            footTimer = 0f;
        }
    }

    void Crouch()
    {
        anim.SetBool("Crouch", Input.GetKey(KeyCode.C));
    }

    void Kick()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            anim.SetInteger("KickType", Random.Range(0, 3));
            anim.SetTrigger("Kick");
        }
    }

    void PerformKick()
    {
        Vector3 center = transform.position + transform.forward * kickRange;
        Collider[] hits = Physics.OverlapSphere(center, kickRange, kickables);

        foreach (Collider hit in hits)
        {
            Rigidbody hitRb = hit.attachedRigidbody;
            if (hitRb != null && !hitRb.isKinematic)
            {
                Vector3 dir = (hit.transform.position - transform.position).normalized;
                dir.y = 0.3f;
                hitRb.AddForce(dir * kickForce, ForceMode.Impulse);
            }
        }
    }

    void Shoot()
    {
        if (Input.GetKeyDown(KeyCode.F) && Time.time >= nextFire)
        {
            nextFire = Time.time + fireRate;
            anim.SetTrigger("Shoot");
            Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            SoundManager.Instance.PlayGunshot(firePoint.position);
        }
    }

    void OnDrawGizmosSelected()
    {
        if (kickPoint)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(kickPoint.position, kickRange);
        }
    }
}