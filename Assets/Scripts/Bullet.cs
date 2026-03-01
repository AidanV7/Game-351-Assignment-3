using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 60f;
    public float lifeTime = 5f;

    public GameObject explosionPrefab; // particle system prefab
    public GameObject debrisPrefab;    // broken barrel prefab

    public float explosionForce = 500f;
    public float explosionRadius = 5f;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = transform.forward * speed;
        Destroy(gameObject, lifeTime);
    }

    void OnTriggerEnter(Collider other)
    {
        Bandit bandit = other.GetComponentInParent<Bandit>();
        if (bandit != null)
        {
            bandit.Die();
        }

        // Barrel logic
        if (other.CompareTag("Barrel"))
        {
            StartCoroutine(ExplodeBarrel(other.gameObject));
        }

        Destroy(gameObject);
    }

    IEnumerator ExplodeBarrel(GameObject barrel)
    {
        // Spawn explosion effect immediately
        GameObject explosion = Instantiate(explosionPrefab, barrel.transform.position, Quaternion.identity);

        // Play explosion sound
        SoundManager.Instance.PlayExplosion(barrel.transform.position);

        // Spawn debris immediately
        Instantiate(debrisPrefab, barrel.transform.position, barrel.transform.rotation);

        // Apply physics explosion to nearby rigidbodies
        Collider[] colliders = Physics.OverlapSphere(barrel.transform.position, explosionRadius);
        foreach (Collider hit in colliders)
        {
            Rigidbody hitRb = hit.GetComponent<Rigidbody>();
            if (hitRb != null)
                hitRb.AddExplosionForce(explosionForce, barrel.transform.position, explosionRadius);
        }

        // Optional: wait until the particle system finishes
        ParticleSystem ps = explosion.GetComponent<ParticleSystem>();
        if (ps != null)
            yield return new WaitForSeconds(ps.main.duration);

        // Destroy barrel and explosion effect
        Destroy(barrel);
        Destroy(explosion);
    }
}