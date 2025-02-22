using UnityEngine;


namespace GameCore
{
    namespace Audio
    {
        public class AudioSourceRandomizerReference : MonoBehaviour
        {
            [SerializeField] private AudioSourceRandomizer[] _audioSourceRandomizers;

            public void Play(SoundEffectType playingSoundEffectType)
            {
                foreach (var randomizer in _audioSourceRandomizers)
                {
                    if (randomizer.SoundType == playingSoundEffectType)
                    {
                        randomizer.Play();
                        break;
                    }
                }
            }
        }
    }
}