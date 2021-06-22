/// ---------------------------------------------
/// Ultimate Inventory System
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------

namespace Opsive.UltimateInventorySystem.Demo.UI
{
    using Opsive.UltimateInventorySystem.UI;
    using Opsive.UltimateInventorySystem.UI.CompoundElements;
    using System.Threading.Tasks;
    using UnityEngine;

    public class TextPanel : MonoBehaviour
    {
        [SerializeField] protected GameObject m_Panel;
        [SerializeField] protected Text m_Text;

        protected float m_Timer;

        public void DisplayText(string text, float displayTime)
        {
            m_Text.text = text;
            m_Panel.SetActive(true);

#pragma warning disable 4014
            DisableAfterDelay(displayTime);
#pragma warning restore 4014
        }

        protected async Task DisableAfterDelay(float delay)
        {
            m_Timer = Time.unscaledTime + delay;
            while (Time.unscaledTime < m_Timer) {
                await Task.Yield();
            }
            m_Panel.SetActive(false);
        }
    }
}
