using UnityEngine;

public class BuildingMusicTrigger : MonoBehaviour
{
    public AudioClip buildingMusic;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && buildingMusic)
        {
            SoundManager.Instance.PlayMusic(buildingMusic);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SoundManager.Instance.PlayDefaultMusic();
        }
    }
}