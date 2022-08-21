using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Audio;

namespace JeanLF.AudioService
{
    public sealed partial class AudioService : IAudioService
    {
        private readonly AudioConfig _configuration;
        private readonly AudioPool _pool;
        private readonly Dictionary<GroupId, AudioPlayerGroup> _audioGroups = new();
        private readonly Dictionary<EntryId, AudioEntry> _audioEntries = new();
        private readonly Stack<AudioMixerSnapshot> _snapshotsStack = new();

        private void Test()
        {
            AudioMixerGroup group = default;
            AudioPlayerGroup playerGroup = new AudioPlayerGroup("te", group, new AudioPool(null,10,10));

            IReadOnlyList<AudioPlayer> test = playerGroup.GetPlayingAudios();
        }

        public AudioService()
        {
            AudioServiceSettings settings = Resources.Load<AudioServiceSettings>("JeanLF_AS_Settings.asset");
            _configuration = settings.Configuration;

            if (_configuration == null)
            {
                throw new NullReferenceException(@"Audio Service configuration can't be null.\n
                You can set the configuration on the service settings in <b>Project Settings/JeanLF/Audio Service</b>");
            }

            _pool = new AudioPool(_configuration, settings.PoolSize, settings.FilteredSources);
            IReadOnlyList<AudioGroup> groups = _configuration.AudioGroups;

            for (int i = 0; i < groups.Count; i++)
            {
                _audioGroups.Add(groups[i].ConvertedId, new AudioPlayerGroup(groups[i].Id, groups[i].MixerGroup, _pool));
            }

            IReadOnlyList<AudioEntry> entries = _configuration.AudioEntries;
            for (int i = 0; i < entries.Count; i++)
            {
                _audioEntries.Add(entries[i].ConvertedId, entries[i]);
            }
        }

        public AudioPlayer Play(AudioReference audio, AudioPlayerProperties? overrideProperties = null)
        {
            return Play(audio.EntryId, audio.GroupId, overrideProperties);
        }

        public AudioPlayer Play(EntryId entryId, GroupId groupId, AudioPlayerProperties? overrideProperties = null)
        {
            AudioEntry entry = _audioEntries[entryId];
            AudioPlayer player = _audioGroups[groupId].PlayAudio(entry, overrideProperties == null ? entry.AudioProperties : overrideProperties.Value);
            return player;
        }

        public void Pause(AudioReference audio)
        {
            Pause(audio.EntryId, audio.GroupId);
        }

        public void Pause(EntryId entryId, GroupId groupId)
        {
            _audioGroups[groupId].PauseAudio(entryId);
        }

        public void Resume(AudioReference audio)
        {
            Resume(audio.EntryId, audio.GroupId);
        }

        public void Resume(EntryId entryId, GroupId groupId)
        {
            _audioGroups[groupId].ResumeAudio(entryId);
        }

        public void PauseGroup(GroupId groupId)
        {
            _audioGroups[groupId].PauseAll();
        }

        public void ResumeGroup(GroupId groupId)
        {
            _audioGroups[groupId].Resume();
        }

        public void Stop(AudioReference audio)
        {
            Stop(audio.EntryId, audio.GroupId);
        }

        public void Stop(EntryId entryId, GroupId groupId)
        {
            _audioGroups[groupId].StopAudio(entryId);
        }

        public void StopGroup(GroupId groupId)
        {
            _audioGroups[groupId].StopAll();
        }

        public void StopAll()
        {
            foreach (KeyValuePair<GroupId, AudioPlayerGroup> keyPair in _audioGroups)
            {
                keyPair.Value.StopAll();
            }
        }
    }
}
