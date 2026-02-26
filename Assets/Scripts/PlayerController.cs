using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float impulseForce  = 5000.0f;    // Reduce from 170000 for stability
    public float impulseTorque = 3000.0f;

    [Header("Kick Settings")]
    public float kickForce = 10f;            // Force applied to objects
    public float kickRange = 2f;             // Distance in front
    public Transform kickPoint;              // Empty GameObject in front of player
    public LayerMask kickableLayer;          // Layer for Tumbleweeds, Crates, etc.

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

        if (input.magnitude > 0.001f && !animController.GetBool("Crouch"))
        {
            rigidBody.AddRelativeTorque(new Vector3(0, input.y * impulseTorque * Time.deltaTime, 0));
            rigidBody.AddRelativeForce(new Vector3(0, 0, input.z * impulseForce * Time.deltaTime));

            animController.SetBool("Walk", true);
        }
        else
        {
            animController.SetBool("Walk", false);
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
            Debug.Log("SPACE PRESSED");

            int randKick = Random.Range(0, 3);
            animController.SetInteger("KickType", randKick);
            animController.SetTrigger("Kick");
        }
    }

    void PerformKick()
    {
        if (kickPoint == null) return;

        Collider[] hits = Physics.OverlapSphere(
            kickPoint.position,
            kickRange,
            kickableLayer
        );

        foreach (Collider hit in hits)
        {
            Rigidbody rb = hit.attachedRigidbody;

            if (rb != null && !rb.isKinematic)
            {
                Vector3 direction =
                    (hit.transform.position - transform.position).normalized;

                // Make sure object is in front of player
                if (Vector3.Dot(transform.forward, direction) > 0.5f)
                {
                    rb.AddForce(
                        direction * kickForce,
                        ForceMode.Impulse
                    );
                }
            }
        }
    }

    void HandleShooting()
    {
        if (Input.GetKeyDown(KeyCode.F) && Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + fireRate;

            animController.SetTrigger("Shoot");

            Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
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