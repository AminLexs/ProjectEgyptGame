/// ---------------------------------------------
/// Ultimate Inventory System
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------

namespace Opsive.UltimateInventorySystem.DropsAndPickups
{
    using Opsive.Shared.Game;
    using Opsive.UltimateInventorySystem.Interactions;
    using Opsive.UltimateInventorySystem.Utility;
    using UnityEngine;

    /// <summary>
    /// Base class of a pickup interactable behavior.
    /// </summary>
    public abstract class PickupBase : InteractableBehavior
    {
        [Tooltip("The audio clip to play when the object is picked up.")]
        [SerializeField] protected AudioClip m_AudioClip;

        /// <summary>
        /// Deactivate.
        /// </summary>
        public override void Deactivate()
        {
            base.Deactivate();

            if (m_ScheduleReactivationTime <= 0) {
                DestroyPickup();
            }
        }

        /// <summary>
        /// Set the pickup interactable.
        /// </summary>
        private void OnEnable()
        {
            if (m_Interactable == null) { m_Interactable = GetComponent<Interactable>(); }
            m_Interactable.SetIsInteractable(true);
        }

        /// <summary>
        /// Play the audio source.
        /// </summary>
        protected virtual void PlaySound()
        {
            if (m_AudioClip == null) { return; }
            AudioManager.PlayClipAt(m_AudioClip, transform.position);
        }

        /// <summary>
        /// Return the pickup to the pool.
        /// </summary>
        protected virtual void DestroyPickup()
        {
            if (ObjectPool.IsPooledObject(gameObject)) {
                ObjectPool.Destroy(gameObject);
            }
        }
    }
}