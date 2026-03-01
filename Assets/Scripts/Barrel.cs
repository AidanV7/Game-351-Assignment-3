using UnityEngine;

public class Barrel : MonoBehaviour
{
    public GameObject explosionPrefab;   // particle effect
    public GameObject brokenBarrelPrefab; // debris
    public float explosionDuration = 2f;  // how long the effect lasts

    void OnCollisionEnter(Collision collision)
    {
        // Check if hit by a bullet
        if(collision.gameObject.CompareTag("Bullet"))
        {
            Explode();
        }
    }

    void Explode()
    {
        // Spawn explosion effect
        GameObject explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        Destroy(explosion, explosionDuration);

        // Spawn broken barrel debris
        Instantiate(brokenBarrelPrefab, transform.position, transform.rotation);

        // Destroy the barrel
        Destroy(gameObject);
    }
    
}