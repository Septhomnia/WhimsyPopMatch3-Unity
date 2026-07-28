using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioSource[] destroyNoise;

    public void PlayRandomDestroyNoise()
    {
        if (destroyNoise == null || destroyNoise.Length == 0)
        {
            return;
        }

        int clipToPlay = Random.Range(0, destroyNoise.Length);

        if (destroyNoise[clipToPlay] != null)
        {
            destroyNoise[clipToPlay].Play();
        }
    }
}