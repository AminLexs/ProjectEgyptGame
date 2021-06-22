namespace Opsive.UltimateInventorySystem.Core.InventoryCollections
{
    using Opsive.Shared.Game;
    using Opsive.UltimateInventorySystem.Exchange;
    using Opsive.UltimateInventorySystem.ItemActions;
    using UnityEngine;

    /// <summary>
    /// Inventory Identifier registers itself on the Inventory System Manager such that you can easily access it from anywhere.
    /// </summary>
    public class InventoryIdentifier : MonoBehaviour, IObjectWithIDReadOnly
    {
        [Tooltip("The ID unique to this inventory, Cannot be 0.")]
        [SerializeField] protected uint m_ID = 1;

        protected Inventory m_Inventory;
        protected CurrencyOwner m_CurrencyOwner;
        protected ItemUser m_ItemUser;

        public uint ID
        {
            get => m_ID;
            set => m_ID = value;
        }

        public Inventory Inventory
        {
            get => m_Inventory;
            set => m_Inventory = value;
        }

        public CurrencyOwner CurrencyOwner
        {
            get => m_CurrencyOwner;
            set => m_CurrencyOwner = value;
        }

        public ItemUser ItemUser
        {
            get => m_ItemUser;
            set => m_ItemUser = value;
        }

        private void Awake()
        {
            m_Inventory = gameObject.GetCachedComponent<Inventory>();
            m_CurrencyOwner = gameObject.GetCachedComponent<CurrencyOwner>();
            m_ItemUser = gameObject.GetCachedComponent<ItemUser>();
            
            InventorySystemManager.InventoryIdentifierRegister.Register(this);
        }
    }
}