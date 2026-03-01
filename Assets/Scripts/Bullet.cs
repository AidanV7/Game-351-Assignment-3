using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 60f;
    public float lifeTime = 5f;
    
    public GameObject explosionPrefab; // assign the particle system
    public GameObject debrisPrefab;    // assign Broken Barrel

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
            // Trigger death animation
            anim.SetTrigger("Die");

            // Play death sound if Bandit component exists
            Bandit bandit = other.GetComponentInParent<Bandit>();
            if (bandit != null)
            {
                SoundManager.Instance.PlayBanditDeath(anim.transform.position, bandit.isFemale);
            }

            // Stop physics and collisions
            Rigidbody rb = anim.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = true;
                rb.useGravity = false;
            }

            Collider col = anim.GetComponent<Collider>();
            if (col != null)
            {
                col.enabled = false;
            }
        }

        Destroy(gameObject);
    }

    IEnumerator ExplodeBarrel(GameObject barrel)
    {
        // Spawn explosion effect
        GameObject explosion = Instantiate(explosionPrefab, barrel.transform.position, Quaternion.identity);

        ParticleSystem ps = explosion.GetComponent<ParticleSystem>();
        float waitTime = ps != null ? ps.main.duration : 1f;

        // Play explosion sound
        SoundManager.Instance.PlayExplosion(barrel.transform.position);

        yield return new WaitForSeconds(waitTime);

        // Spawn debris
        Instantiate(debrisPrefab, barrel.transform.position, barrel.transform.rotation);

        Destroy(barrel);
        Destroy(explosion);
    }
}