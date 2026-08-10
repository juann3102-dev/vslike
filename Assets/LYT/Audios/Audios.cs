using UnityEngine;
using UnityEngine.Audio;

public class Audios : MonoBehaviour
{
    private AudioSource audioSource;
    [Header("Audio Clips")]
    [SerializeField] AudioClip ShotSound;
    [SerializeField] AudioClip ReloadSound;

    public void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayShot()
    {
        // clip 재생 중 다른 효과음이 들어와도 끊기지 않고 겹쳐서 출력됨
        audioSource.PlayOneShot(ShotSound);
    }

    // Update is called once per frame
    public void PlayReload()
    {
        audioSource.PlayOneShot(ReloadSound);
    }
}
