using UnityEngine;
using System.Collections;

public class FootstepPlayer : MonoBehaviour
{
    public AudioSource Source;
    public AudioClip[] footstepSounds;

    public void PlayFootstep()
    {
        if (footstepSounds.Length == 0) return;

        int randomIndex = Random.Range(0, footstepSounds.Length);
        Source.PlayOneShot(footstepSounds[randomIndex]);
    }

}