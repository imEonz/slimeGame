using UnityEngine;
public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }
    [SerializeField] 
    private AudioSource SoundFXObject;
    [SerializeField] 
    private AudioClip[] audioClips;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void PlaySoundFXClip(AudioClip audioClip, Transform spawnTransform, float volume, float? pitch = null)
    {
        if(audioClip == null) return;
        AudioSource audioSource = Instantiate(SoundFXObject, spawnTransform.transform);
    
        audioSource.clip = audioClip;

        audioSource.volume = volume;

        if(pitch != null) audioSource.pitch = (float)pitch;

        audioSource.Play();

        float clipLength = audioSource.clip.length;

        Destroy(audioSource.gameObject, clipLength);
    }
    public void PlaySoundFXClipRandomPitch(AudioClip audioClip, Transform spawnTransform, float volume)
    {
        if(audioClip == null) return;
        AudioSource audioSource = Instantiate(SoundFXObject, spawnTransform.transform);
    
        audioSource.clip = audioClip;

        audioSource.volume = volume;

        audioSource.pitch = Random.Range(0.8f,1.2f);;

        audioSource.Play();

        float clipLength = audioSource.clip.length;

        Destroy(audioSource.gameObject, clipLength);
    }
    public float GetClipLength(AudioClip clip)
    {
        return clip.length;
    }

    public AudioClip GetAudioClip(string clipToPlay)
    {
        foreach (AudioClip clip in audioClips)
        {
            if(clip.name == clipToPlay)
            {
                return clip;
            }
        }
        Debug.LogWarning("Didn't find shit");
        return null;
    }
}