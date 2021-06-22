namespace Opsive.UltimateInventorySystem.UI.Audio
{
    using UnityEngine;

    /// <summary>
    /// A scriptable object used to map categories to Item Actions.
    /// </summary>
    [CreateAssetMenu(fileName = "ButtonAudio",
        menuName = "Ultimate Inventory System/UI/Button Audio", order = 51)]
    public class ButtonAudio : ScriptableObject
    {
        [Tooltip("The audio of clicking a button")]
        [SerializeField] protected AudioClip m_Click;
        [Tooltip("The audio of selecting a button")]
        [SerializeField] protected AudioClip m_Select;

        public AudioClip ClickClip => m_Click;
        public AudioClip SelectClip => m_Select;
    }
}