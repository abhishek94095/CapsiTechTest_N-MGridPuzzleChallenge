namespace AV.Framework.GameData
{
    using System;
    using System.Collections.Generic;
    using AV.Framework.Application.Audio;
    using UnityEngine;

    [CreateAssetMenu(fileName = "GameAudioConfig", menuName = "AV/Configuration/Game Audio Config")]
    public sealed class GameAudioConfig : ScriptableObject
    {
        [Serializable]
        private sealed class AudioEntry
        {
            [SerializeField] private GameAudioType audioType;
            [SerializeField] private AudioClip audioClip;

            public GameAudioType AudioType => audioType;
            public AudioClip AudioClip => audioClip;
        }

        [SerializeField] private List<AudioEntry> audioEntries = new List<AudioEntry>();

        public bool TryGetClip(GameAudioType audioType, out AudioClip audioClip)
        {
            if (audioEntries != null)
            {
                for (int index = 0; index < audioEntries.Count; index++)
                {
                    AudioEntry audioEntry = audioEntries[index];
                    if (audioEntry == null || audioEntry.AudioType != audioType) continue;

                    audioClip = audioEntry.AudioClip;
                    return audioClip != null;
                }
            }

            audioClip = null;
            return false;
        }
    }
}
