namespace Opsive.UltimateInventorySystem.Demo.UI
{
    using System;
    using UnityEngine;

    public class ChestLockedMessage : MonoBehaviour
    {
        [Tooltip("The text that will be displayed when the interactor does not have the key.")]
        [SerializeField] protected string m_TextIfNoKey;
        [Tooltip("The text that will be displayed when the interactor does have the key.")]
        [SerializeField] protected string m_TextHasKey;
        [Tooltip("The amount of second the text will hte displayed for.")]
        [SerializeField] protected float m_TextDisplayTime;
        [SerializeField] protected TextPanel m_TextPanel;

        private void Awake()
        {
            if (m_TextPanel == null) {
                m_TextPanel = FindObjectOfType<TextPanel>();
            }
        }

        public void OnNoKey()
        {
            if (m_TextPanel != null) {
                m_TextPanel.DisplayText(m_TextHasKey, m_TextDisplayTime);
            }
        }

        public void HasKey()
        {
            if (m_TextPanel != null) {
                m_TextPanel.DisplayText(m_TextIfNoKey, m_TextDisplayTime);
            }
        }
    }
}