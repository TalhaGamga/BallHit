using UnityEngine;
using System.Collections;

namespace DinoFracture.Internal
{
    /// <summary>
    /// This component is automatically added to temporary sound game objects
    /// created by the PlaySoundOnFracture component.  It is not intended to
    /// be added by the user.
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class DestroyOnAudioFinish : MonoBehaviour
    {
        private AudioSource _source;

        private void Start()
        {
            _source = GetComponent<AudioSource>();
            _source.Play();
        }

        private void Update()
        {
            if (!_source.isPlaying)
            {
                Destroy(gameObject);
            }
        }
    }
}
