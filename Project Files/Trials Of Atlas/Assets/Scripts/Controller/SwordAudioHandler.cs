using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordAudioHandler : MonoBehaviour
{
    public AudioSource swordAudio;
    public float basePitchWidth;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void PlaySwordSound(float pitchWidth)
    {
        swordAudio.Play();
        swordAudio.pitch = 1 + Random.Range(-basePitchWidth / 2, basePitchWidth / 2) + pitchWidth;
    }
}
