/// ---------------------------------------------
/// Ultimate Inventory System
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------

namespace Opsive.UltimateInventorySystem.Utility
{
    using Opsive.Shared.Game;
    using UnityEngine;

    /// <summary>
    /// A static class used to add audio sources efficiently.
    /// </summary>
    public static class AudioManager
    {

        static GameObject m_AudioSourcePrefab;

        /// <summary>
        /// Play a clip at a specific point in the scene.
        /// </summary>
        /// <param name="clip">The clip.</param>
        /// <param name="pos">THe position.</param>
        /// <returns>The audio source instantiated.</returns>
        public static AudioSource PlayClipAt(AudioClip clip, Vector3 pos)
        {

            if (m_AudioSourcePrefab == null) {
                GameObject newPrefab = new GameObject("Temporary Audio Source");
                AudioSource prefabSource = newPrefab.AddComponent<AudioSource>();
                m_AudioSourcePrefab = newPrefab;
            }

            var audioSourceTemp = ObjectPool.Instantiate(m_AudioSourcePrefab);
            audioSourceTemp.transform.position = pos;

            AudioSource audioSource = audioSourceTemp.GetComponent<AudioSource>();
            audioSource.clip = clip;

            audioSource.Play();
            Scheduler.Schedule(clip.length, () => { ObjectPool.Destroy(audioSourceTemp); });

            return audioSource;
        }
    }
}
