using UnityEngine;

public class Sound : MonoBehaviour
{
    private AudioSource _audio;

    public AudioClip ClickSound;

    private void Start()
    {
        _audio = GetComponent<AudioSource>();
    }

    public void OnClickSoundButton()
    {
        _audio.PlayOneShot(ClickSound);
    }
}
