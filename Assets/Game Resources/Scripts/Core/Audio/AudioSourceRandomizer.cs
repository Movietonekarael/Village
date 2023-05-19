using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameCore.Audio
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioSourceRandomizer : MonoBehaviour
    {
        private AudioSource _audioSource;
        [SerializeField] private SoundEffectType _soundType;
        public SoundEffectType SoundType
        {
            get
            {
                return _soundType;
            }
        }

        [SerializeField] private AudioClip[] _audioClips;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        public void Play()
        {
            var randomNumber = Random.Range(0, _audioClips.Length);
            _audioSource.clip = _audioClips[randomNumber];
            _audioSource.Play();
        }
    }
}

