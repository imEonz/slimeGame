using UnityEngine;

public class PlaySlimeSFX : MonoBehaviour
{
    public void playSFX()
    {
        SoundManager.Instance.PlaySoundFXClip(SoundManager.Instance.GetAudioClip("Splat"), this.transform,0.1f);
    }
}
