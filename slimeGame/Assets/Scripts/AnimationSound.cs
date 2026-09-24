using Unity;
using UnityEngine;
public class AnimationSound : MonoBehaviour
{

    public void PlaySFX(AnimationEvent animEvent)
    {
        string clipName = animEvent.stringParameter;
        float volume = animEvent.floatParameter;
        AudioClip clip = animEvent.objectReferenceParameter as AudioClip;

        SoundManager.Instance.PlaySoundFXClip(SoundManager.Instance.GetAudioClip(clipName),this.transform,volume);
    }
}
