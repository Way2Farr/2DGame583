using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffectPlayer : MonoBehaviour
{
    // Start is called before the first frame update


    public AudioSource src;
    public AudioClip sound1, sound2;

public void grappleSound() {
    src.clip = sound1;
    src.Play();
}

public void jumpSound() {
    src.clip = sound2;
    src.Play();

    }
}
