using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 60f;
    public float lifeTime = 5f;

    public GameObject explosionPrefab;
    public GameObject debrisPrefab;

    public float force = 500f;
    public float radius = 5f;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = transform.forward * speed;
        Destroy(gameObject, lifeTime);
    }

    void OnTriggerEnter(Collider other)
    {
        Bandit b = other.GetComponentInParent<Bandit>();
        if (b != null) b.Die();

        if (other.CompareTag("Barrel"))
            StartCoroutine(ExplodeBarrel(other.gameObject));

        Destroy(gameObject);
    }

    IEnumerator ExplodeBarrel(GameObject barrel)
    {
        GameObject expl = Instantiate(explosionPrefab, barrel.transform.position, Quaternion.identity);
        SoundManager.Instance.PlayExplosion(barrel.transform.position);
        Instantiate(debrisPrefab, barrel.transform.position, barrel.transform.rotation);

        Collider[] hits = Physics.OverlapSphere(barrel.transform.position, radius);
        foreach (Collider h in hits)
        {
            Rigidbody hrb = h.GetComponent<Rigidbody>();
            if (hrb) hrb.AddExplosionForce(force, barrel.transform.position, radius);
        }

        ParticleSystem ps = expl.GetComponent<ParticleSystem>();
        if (ps != null)
            yield return new WaitForSeconds(ps.main.duration);

        Destroy(barrel);
        Destroy(expl);
    }
}