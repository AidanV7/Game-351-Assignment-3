using System.Collections;
using UnityEngine;

public class Bandit : MonoBehaviour
{
    public bool isFemale = false; // Set in Inspector

    [Header("Taunt Settings")]
    public float minTauntDelay = 2f;
    public float maxTauntDelay = 6f;

    private Coroutine tauntCoroutine;
    private bool isAlive = true;

    void Start()
    {
        // Start taunting while alive
        tauntCoroutine = StartCoroutine(TauntRoutine());
    }

    IEnumerator TauntRoutine()
    {
        while (isAlive)
        {
            float delay = Random.Range(minTauntDelay, maxTauntDelay);
            yield return new WaitForSeconds(delay);

            if (isAlive && SoundManager.Instance != null)
            {
                SoundManager.Instance.PlayBanditTaunt(transform.position);
            }
        }
    }

    // Called by Bullet when bandit dies
    public void Die()
    {
        isAlive = false;

        // Stop taunting immediately
        if (tauntCoroutine != null)
            StopCoroutine(tauntCoroutine);

        // Play death sound
        if (SoundManager.Instance != null)
            SoundManager.Instance.PlayBanditDeath(transform.position, isFemale);

        // Disable collider & rigidbody
        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        // Trigger death animation
        Animator anim = GetComponent<Animator>();
        if (anim != null)
            anim.SetTrigger("Die");
    }
}