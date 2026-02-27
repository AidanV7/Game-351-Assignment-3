using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 50f;
    public float lifeTime = 5f;

    private void Start()
    {
        Destroy(gameObject, lifeTime); // auto-destroy after a while
    }

    private void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check for barrels
        if (other.CompareTag("Barrel"))
        {
            // Play explosion particle system
            ParticleSystem explosion = other.GetComponentInChildren<ParticleSystem>();
            if (explosion != null)
            {
                explosion.Play();
            }

            // Replace barrel with debris after short delay
            StartCoroutine(ReplaceWithDebris(other.gameObject));
        }

        // Check for bandits
        if (other.CompareTag("Bandit"))
        {
            Animator banditAnim = other.GetComponent<Animator>();
            if (banditAnim != null)
            {
                banditAnim.SetTrigger("Die"); // Make sure your bandit Animator has a "Die" trigger
            }

            // Optional: disable collider so it doesn't interfere
            Collider banditCollider = other.GetComponent<Collider>();
            if (banditCollider != null)
                banditCollider.enabled = false;

            // Destroy bandit after animation
            Destroy(other.gameObject, 2f); // adjust to match animation length
        }

        Destroy(gameObject); // destroy bullet on impact
    }

    private IEnumerator ReplaceWithDebris(GameObject barrel)
    {
        // Hide barrel mesh
        MeshRenderer renderer = barrel.GetComponent<MeshRenderer>();
        if (renderer != null)
            renderer.enabled = false;

        Rigidbody rb = barrel.GetComponent<Rigidbody>();
        if (rb != null)
            rb.isKinematic = true;

        yield return new WaitForSeconds(0.5f); // wait for explosion effect

        // Spawn debris prefab (make sure it's assigned in inspector or set a reference here)
        GameObject debrisPrefab = Resources.Load<GameObject>("BarrelDebris"); // example path
        if (debrisPrefab != null)
        {
            Instantiate(debrisPrefab, barrel.transform.position, barrel.transform.rotation);
        }

        Destroy(barrel);
    }
}