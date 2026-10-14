using System.Collections.Generic;
using UnityEngine;
public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }
    [SerializeField] 
    private AudioSource SoundFXObject;
    [SerializeField] 
    private AudioClip[] audioClips;
    [SerializeField] private int poolSize = 32;
    [SerializeField] private float minTimeBetweenSameClip = 0.05f;
    [SerializeField] private int maxSimultaneousSameClip = 4;


    private AudioSource[] pool;
    private int nextIndex;
    private Dictionary<string, AudioClip> clipLookup;
    private Dictionary<AudioClip, float> lastPlayTime = new();
    private Dictionary<AudioClip, int> activeCount = new();

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        clipLookup = new Dictionary<string, AudioClip>();
        foreach (var clip in audioClips) if (clip != null) clipLookup[clip.name] = clip;

        pool = new AudioSource[poolSize];
        for (int i = 0; i < poolSize; i++)
        {
            pool[i] = Instantiate(SoundFXObject, transform);
            pool[i].playOnAwake = false;
        }
    }

    public void PlaySoundFXClip(AudioClip clip, Transform spawnTransform, float volume, float? pitch = null)
    {
        Vector3 pos = spawnTransform != null ? spawnTransform.position : transform.position;
        Play(clip, pos, volume, pitch ?? 1f);
    }

    public void PlaySoundFXClipRandomPitch(AudioClip clip, Transform spawnTransform, float volume)
    {
        Vector3 pos = spawnTransform != null ? spawnTransform.position : transform.position;
        Play(clip, pos, volume, Random.Range(0.8f, 1.2f));
    }

    private void Play(AudioClip clip, Vector3 position, float volume, float pitch)
    {
        if (clip == null) return;

        if (lastPlayTime.TryGetValue(clip, out float last) && Time.unscaledTime - last < minTimeBetweenSameClip) return;

        activeCount.TryGetValue(clip, out int count);
        if (count >= maxSimultaneousSameClip) return;

        lastPlayTime[clip] = Time.unscaledTime;

        AudioSource source = pool[nextIndex];
        nextIndex = (nextIndex + 1) % pool.Length;

        source.Stop();
        source.transform.position = position;
        source.clip = clip;
        source.volume = volume;
        source.pitch = pitch;
        source.Play();

        activeCount[clip] = count + 1;
        StartCoroutine(ReleaseAfter(clip, clip.length / Mathf.Abs(pitch)));
    }

    private System.Collections.IEnumerator ReleaseAfter(AudioClip clip, float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        if (activeCount.TryGetValue(clip, out int c)) activeCount[clip] = Mathf.Max(0, c - 1);
    }

    public float GetClipLength(AudioClip clip) => clip.length;

    public AudioClip GetAudioClip(string clipToPlay)
    {
        if (clipLookup.TryGetValue(clipToPlay, out var clip)) return clip;
        Debug.LogWarning($"Audio clip '{clipToPlay}' not found");
        return null;
    }

    public void ButtonPlaySFX(string audioClip)
    {
        PlaySoundFXClip(GetAudioClip(audioClip), transform, 0.2f);
    }
}