using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossfadeMusic : MonoBehaviour
{
    public float musicVolume;
    //We create an array with 2 audio sources that we will swap between for transitions
    public static AudioSource[] aud = new AudioSource[2];

    //We will use this boolean to determine which audio source is the current one
    bool activeMusicSource;
    //We will store the transition as a Coroutine so that we have the ability to stop it halfway if necessary
    IEnumerator musicTransition;

    void Awake()
    {
        //Create the AudioSource components that we will be using
        aud[0] = gameObject.AddComponent<AudioSource>();
        aud[1] = gameObject.AddComponent<AudioSource>();

        musicVolume = 0.7f;
    }

    public void PlaySimultaneously(AudioClip clip)
    {
        int nextSource = !activeMusicSource ? 0 : 1;
        int currentSource = activeMusicSource ? 0 : 1;

        //If the clip is already being played on the current audio source, we will end now and prevent the audio play
        if (clip == aud[currentSource].clip)
            return;

        aud[nextSource].clip = clip;
        aud[nextSource].Play();
    }

    //use this method to start a new soundtrack, with a reference to the AudioClip that you want to use
    //    such as:        newSoundtrack((AudioClip)Resources.Load("Audio/soundtracks/track01"));
    public void newSoundtrack(AudioClip clip)
    {
        //This ?: operator is short hand for an if/else statement, eg.
        //
        //      if (activeMusicSource) {
        //          nextSource = 1;
        //      } else {
        //           nextSource = 0;
        //      }

        int nextSource = !activeMusicSource ? 0 : 1;
        int currentSource = activeMusicSource ? 0 : 1;

        //If the clip is already being played on the current audio source, we will end now and prevent the transition
        if (clip == aud[currentSource].clip)
            return;

        //If a transition is already happening, we stop it here to prevent our new Coroutine from competing
        if (musicTransition != null)
            StopCoroutine(musicTransition);

        aud[nextSource].clip = clip;
        aud[nextSource].Play();

        musicTransition = transition(20); //20 is the equivalent to 2 seconds (More than 3 seconds begins to overlap for a bit too long)
        StartCoroutine(musicTransition);
    }

    //  'transitionDuration' is how many tenths of a second it will take, eg, 10 would be equal to 1 second
    IEnumerator transition(int transitionDuration)
    {

        for (int i = 0; i < transitionDuration + 1; i++)
        {
            aud[0].volume = activeMusicSource ? (transitionDuration - i) * (1f / transitionDuration) : (0 + i) * (1f / transitionDuration);
            aud[1].volume = !activeMusicSource ? (transitionDuration - i) * (1f / transitionDuration) : (0 + i) * (1f / transitionDuration);

            //  Here I have a global variable to control maximum volume.
            //  options.musicVolume is a float that ranges from 0f - 1.0f
            //------------------------------------------------------------//
            aud[0].volume *= musicVolume;
            aud[1].volume *= musicVolume;
            //------------------------------------------------------------//

            yield return new WaitForSecondsRealtime(0.1f);
            //use realtime otherwise if you pause the game you could pause the transition half way
        }

        //finish by stopping the audio clip on the now silent audio source
        aud[activeMusicSource ? 0 : 1].Stop();

        activeMusicSource = !activeMusicSource;
        musicTransition = null;
    }

    private void Update()
    {
        //loop music if it stops playing
        int nextSource = !activeMusicSource ? 0 : 1;
        int currentSource = activeMusicSource ? 0 : 1;
        if (!aud[currentSource].isPlaying && !aud[nextSource].isPlaying)
        {
            aud[currentSource].Play();
        }
    }

}
