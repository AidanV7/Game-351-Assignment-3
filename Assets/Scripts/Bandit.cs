using System.Collections;
using UnityEngine;

public class Bandit : MonoBehaviour
{
    public bool isFemale = false;
    public float minTaunt = 2f, maxTaunt = 6f;

    private Coroutine tauntRoutine;
    private bool alive = true;

    void Start()
    {
        tauntRoutine = StartCoroutine(TauntLoop());
    }

    IEnumerator TauntLoop()
    {
        while (alive)
        {
            yield return new WaitForSeconds(Random.Range(minTaunt, maxTaunt));
            if (alive && SoundManager.Instance)
                SoundManager.Instance.PlayBanditTaunt(transform.position);
        }
    }

    public void Die()
    {
        alive = false;
        if (tauntRoutine != null) StopCoroutine(tauntRoutine);

        if (SoundManager.Instance) SoundManager.Instance.PlayBanditDeath(transform.position, isFemale);

        Collider col = GetComponent<Collider>();
        if (col) col.enabled = false;

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb) { rb.isKinematic = true; rb.useGravity = false; }

        Animator anim = GetComponent<Animator>();
        if (anim) anim.SetTrigger("Die");
    }
}