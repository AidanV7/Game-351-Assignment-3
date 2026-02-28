using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 60f;
    public float lifeTime = 5f;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = transform.forward * speed;

        Destroy(gameObject, lifeTime);
    }

    void OnTriggerEnter(Collider other)
    {
        Animator anim = other.GetComponentInParent<Animator>();

        if (anim != null)
        {
            anim.SetTrigger("Die");

            Rigidbody rb = anim.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = true; // Stop physics
                rb.useGravity = false;
            }

            Collider col = anim.GetComponent<Collider>();
            if (col != null)
            {
                col.enabled = false; // Stop collision pushing
            }
        }

        Destroy(gameObject);
    }
}