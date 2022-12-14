using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BlockAudioHandler : MonoBehaviour
{
    private AudioSource aSource;

    [SerializeField] private AudioClip[] placeClips;

    [SerializeField,Range(0,2)] private float breakPitch = 1.6f;
    [SerializeField,Range(0,2)] private float placePitch = 1f;

    public void Initialize()
    {
        aSource = GetComponent<AudioSource>();
    }

    public void PlayDestroySound()
    {
        aSource.pitch = Random.Range(breakPitch - .1f, breakPitch + .1f);
        PlaySound(GetRandomClip());
    }

    public void PlayPlaceSound()
    {
        aSource.pitch = placePitch;
        PlaySound( GetRandomClip() );
    }

    private AudioClip GetRandomClip() => placeClips[Random.Range(0, placeClips.Length)];

    private void PlaySound(AudioClip clip)
    {
        aSource.clip = clip;
        aSource.Play();
    }
}
