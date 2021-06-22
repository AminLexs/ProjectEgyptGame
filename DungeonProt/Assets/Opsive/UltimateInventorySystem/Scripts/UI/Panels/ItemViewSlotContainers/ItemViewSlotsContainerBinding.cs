namespace Opsive.UltimateInventorySystem.UI.Panels.ItemViewSlotContainers
{
    using Opsive.UltimateInventorySystem.UI.Item;
    using UnityEngine;

    public abstract class ItemViewSlotsContainerBinding : MonoBehaviour
    {

        protected ItemViewSlotsContainerBase m_ItemViewSlotsContainer;
       
        protected bool m_IsInitialized;
        
        protected virtual void Awake()
        {
            Initialize(false);
        }

        public virtual void Initialize(bool force)
        {
            if(m_IsInitialized && force == false){ return; }

            m_IsInitialized = true;
        }

        public virtual void Bind(ItemViewSlotsContainerBase container)
        {
            Initialize(false);
            if(m_ItemViewSlotsContainer == container){ return; }

            UnBind();

            m_ItemViewSlotsContainer = container;
           
            OnBind();
            
        }

        protected abstract void OnBind();

        public virtual void UnBind()
        {
            Initialize(false);
            if(m_ItemViewSlotsContainer == null){ return; }
            
            OnUnBind();
            m_ItemViewSlotsContainer = null;
        }

        protected abstract void OnUnBind();
    }
    
    public abstract class ItemViewSlotsContainerInventoryGridBinding : ItemViewSlotsContainerBinding
    {

        protected InventoryGrid m_InventoryGrid;

        public override void Bind(ItemViewSlotsContainerBase container)
        {
            Initialize(false);
            if(m_ItemViewSlotsContainer == container){ return; }
            
            UnBind();

            var inventoryGrid = container as InventoryGrid;
            if (inventoryGrid == null) { return; }

            m_ItemViewSlotsContainer = container;
            m_InventoryGrid = inventoryGrid;
           
            OnBind();
        }


        public override void UnBind()
        {
            Initialize(false);
            if(m_ItemViewSlotsContainer == null){ return; }
            
            OnUnBind();
            m_ItemViewSlotsContainer = null;
            m_InventoryGrid = null;
        }
    }
}