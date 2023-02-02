using UnityEngine;
using System.Collections;

namespace DinoFracture
{
    /// <summary>
    /// An object with this component will play the audio source when fractured.
    /// </summary>
    [RequireComponent(typeof (AudioSource))]
    public class PlaySoundOnFracture : MonoBehaviour
    {
        private void OnFracture(OnFractureEventArgs args)
        {
            if (args.OriginalObject.gameObject == gameObject)
            {
                // We most likely will be disabled next frame.  When that
                // happens, the audio source on this object will not play.
                // To work around this, we create a temporary object and
                // copy all the sound properties to it.

                GameObject go = new GameObject("FractureTempAudioSource");

                AudioSource ourSrc = GetComponent<AudioSource>();
                AudioSource newSrc = go.AddComponent<AudioSource>();
                newSrc.bypassEffects = ourSrc.bypassEffects;
                newSrc.bypassListenerEffects = ourSrc.bypassListenerEffects;
                newSrc.bypassReverbZones = ourSrc.bypassReverbZones;
                newSrc.clip = ourSrc.clip;
                newSrc.dopplerLevel = ourSrc.dopplerLevel;
                newSrc.ignoreListenerPause = ourSrc.ignoreListenerPause;
                newSrc.ignoreListenerVolume = ourSrc.ignoreListenerVolume;
                newSrc.loop = ourSrc.loop;
                newSrc.maxDistance = ourSrc.maxDistance;
                newSrc.mute = ourSrc.mute;
                newSrc.mute = ourSrc.mute;
                newSrc.minDistance = ourSrc.minDistance;
                newSrc.mute = ourSrc.mute;
                newSrc.panStereo = ourSrc.panStereo;
                newSrc.spatialBlend = ourSrc.spatialBlend;
                newSrc.pitch = ourSrc.pitch;
                newSrc.playOnAwake = ourSrc.playOnAwake;
                newSrc.priority = ourSrc.priority;
                newSrc.rolloffMode = ourSrc.rolloffMode;
                newSrc.spread = ourSrc.spread;
                newSrc.time = ourSrc.time;
                newSrc.timeSamples = ourSrc.timeSamples;
                newSrc.velocityUpdateMode = ourSrc.velocityUpdateMode;
                newSrc.volume = ourSrc.volume;

                go.AddComponent<Internal.DestroyOnAudioFinish>();
            }
        }
    }
}