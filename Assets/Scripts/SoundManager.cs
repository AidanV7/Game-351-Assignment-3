using System.Collections;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("Ambient Sound")]
    public AudioClip ambientClip;
    [Range(0f, 1f)] public float ambientVolume = 0.3f;

    [Header("Dynamic Music Tracks")]
    public AudioClip defaultTrack;
    public AudioClip suspenseTrack;
    public AudioClip fightTrack;
    [Range(0f, 1f)] public float musicVolume = 0.5f;

    [Header("Gun & Explosions")]
    public AudioClip gunshotClip;
    public AudioClip explosionClip;

    [Header("Bandit Death Sounds")]
    public AudioClip banditDeathMaleClip;
    public AudioClip banditDeathFemaleClip;

    [Header("Bandit Taunts")]
    public AudioClip[] banditTaunts;

    [Header("Footsteps (Walking)")]
    public AudioClip[] walkingClips; // assign 2–4 clips only
    [Range(0f, 1f)] public float walkingVolume = 0.5f;

    private AudioSource ambientSource;
    private AudioSource musicSource;

    private void Awake()
    {
        // Singleton setup
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Ambient music setup
        if (ambientClip != null)
        {
            ambientSource = gameObject.AddComponent<AudioSource>();
            ambientSource.clip = ambientClip;
            ambientSource.loop = true;
            ambientSource.volume = ambientVolume;
            ambientSource.spatialBlend = 0f; // non-3D
            ambientSource.Play();
        }

        // Music source setup
        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.loop = true;
        musicSource.volume = musicVolume;
        PlayMusic(defaultTrack);
    }

    // --- Dynamic Music ---
    public void PlayMusic(AudioClip clip)
    {
        if (clip == null || musicSource.clip == clip) return;

        musicSource.Stop();
        musicSource.clip = clip;
        musicSource.Play();
    }

    public void PlayDefaultMusic() => PlayMusic(defaultTrack);
    public void PlaySuspenseMusic() => PlayMusic(suspenseTrack);
    public void PlayFightMusic() => PlayMusic(fightTrack);

    // --- SFX ---
    public void PlaySound(AudioClip clip, Vector3 position, float volume = 1f)
    {
        if (clip != null)
            AudioSource.PlayClipAtPoint(clip, position, volume);
    }

    public void PlayGunshot(Vector3 position)
    {
        PlaySound(gunshotClip, position);
        PlayFightMusic(); // trigger fight music when shooting
    }

    public void PlayExplosion(Vector3 position)
    {
        PlaySound(explosionClip, position);
        PlayFightMusic(); // optional: trigger fight music
    }

    public void PlayWalking(Vector3 position)
    {
        if (walkingClips.Length == 0) return;
        int index = Random.Range(0, walkingClips.Length);
        PlaySound(walkingClips[index], position, walkingVolume);
    }

    public void PlayBanditDeath(Vector3 position, bool isFemale)
    {
        PlaySound(isFemale ? banditDeathFemaleClip : banditDeathMaleClip, position);
    }

    public void PlayBanditTaunt(Vector3 position)
    {
        if (banditTaunts.Length == 0) return;
        int index = Random.Range(0, banditTaunts.Length);
        PlaySound(banditTaunts[index], position);
    }
}