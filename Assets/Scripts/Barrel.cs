using UnityEngine;

public class Barrel : MonoBehaviour
{
    public GameObject explosionPrefab;   
    public GameObject brokenBarrelPrefab;
    public float explosionDuration = 2f; 

    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Bullet"))
        {
            Explode();
        }
    }

    void Explode()
    {
        GameObject explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        Destroy(explosion, explosionDuration);

        Instantiate(brokenBarrelPrefab, transform.position, transform.rotation);

        Destroy(gameObject);
    }
    
}