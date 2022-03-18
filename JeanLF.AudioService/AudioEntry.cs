using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using JeanLF.AudioService.Filters;
using UnityEngine;

namespace JeanLF.AudioService
{
    [System.Serializable]
    public struct AudioEntry
    {
#if UNITY_EDITOR
        public static readonly string FilterPropertyName = nameof(_filters);
#endif
        
        public AudioEntry(float volume = 1f)
        {
            _id = string.Empty;
            _clips = Array.Empty<AudioClip>();
            _audioProperties = new AudioPlayerProperties(volume:1f);
            _filters = Array.Empty<IFilterProperty>();
        }

        public AudioEntry(
            string id,
            AudioClip[] audioClips,
            bool bypassListenerEffects,
            bool loop,
            float volume,
            float minPitch,
            float maxPitch,
            float spatialBlend,
            float stereoPan,
            float reverbZoneMix,
            float dopplerLevel,
            float spread,
            AudioRolloffMode volumeRolloff,
            float minRolloff,
            float maxRolloff,
            int priority)
        {
            _id = id;
            _clips = audioClips;
            _audioProperties = new AudioPlayerProperties(bypassListenerEffects, loop, priority, volume, minPitch,
                maxPitch, spatialBlend, stereoPan, reverbZoneMix, dopplerLevel, spread, volumeRolloff, minRolloff,
                maxRolloff);
            _filters = Array.Empty<IFilterProperty>();
        }

        [SerializeField] private string _id;
        [SerializeField] private AudioClip[] _clips;
        [SerializeField] private AudioPlayerProperties _audioProperties;
        [HideInInspector] [SerializeReference] private IFilterProperty[] _filters;

        public IFilterProperty[] Filters => _filters;

        [ContextMenu("Debug Filters")]
        private void DebugFilters()
        {
            for (int i = 0; i < _filters.Length; i++)
            {
                Debug.Log(_filters[i]);
            }
        }
    }
}