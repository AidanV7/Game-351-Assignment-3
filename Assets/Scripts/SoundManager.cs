using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("Ambient")]
    public AudioClip ambientClip;
    [Range(0,1)] public float ambientVol = 0.3f;

    [Header("Music")]
    public AudioClip defaultTrack, suspenseTrack, fightTrack;
    [Range(0,1)] public float musicVol = 0.5f;

    [Header("FX")]
    public AudioClip gunshotClip, explosionClip;
    public AudioClip banditDeathMale, banditDeathFemale;
    public AudioClip[] banditTaunts;
    public AudioClip[] walkClips;
    [Range(0,1)] public float walkVol = 0.5f;

    private AudioSource ambientSource;
    private AudioSource musicSource;

    void Awake()
    {
        if (!Instance) { Instance = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); return; }

        if (ambientClip)
        {
            ambientSource = gameObject.AddComponent<AudioSource>();
            ambientSource.clip = ambientClip;
            ambientSource.loop = true;
            ambientSource.volume = ambientVol;
            ambientSource.spatialBlend = 0f;
            ambientSource.Play();
        }

        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.loop = true;
        musicSource.volume = musicVol;
        PlayMusic(defaultTrack);
    }

    public void PlayMusic(AudioClip clip)
    {
        if (!clip || musicSource.clip == clip) return;
        musicSource.Stop();
        musicSource.clip = clip;
        musicSource.Play();
    }

    public void PlayDefaultMusic() => PlayMusic(defaultTrack);
    public void PlaySuspenseMusic() => PlayMusic(suspenseTrack);
    public void PlayFightMusic() => PlayMusic(fightTrack);

    public void PlaySound(AudioClip clip, Vector3 pos, float vol = 1f)
    {
        if (clip) AudioSource.PlayClipAtPoint(clip, pos, vol);
    }

    public void PlayGunshot(Vector3 pos) { PlaySound(gunshotClip, pos); PlayFightMusic(); }
    public void PlayExplosion(Vector3 pos) { PlaySound(explosionClip, pos); PlayFightMusic(); }
    public void PlayWalking(Vector3 pos)
    {
        if (walkClips.Length == 0) return;
        int i = Random.Range(0, walkClips.Length);
        PlaySound(walkClips[i], pos, walkVol);
    }

    public void PlayBanditDeath(Vector3 pos, bool female) => PlaySound(female ? banditDeathFemale : banditDeathMale, pos);
    public void PlayBanditTaunt(Vector3 pos)
    {
        if (banditTaunts.Length == 0) return;
        PlaySound(banditTaunts[Random.Range(0, banditTaunts.Length)], pos);
    }
}