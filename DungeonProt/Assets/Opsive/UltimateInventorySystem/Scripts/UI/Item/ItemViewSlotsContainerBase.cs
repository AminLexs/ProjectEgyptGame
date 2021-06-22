namespace Opsive.UltimateInventorySystem.UI.Item
{
    using Opsive.UltimateInventorySystem.Core;
    using Opsive.UltimateInventorySystem.Core.DataStructures;
    using Opsive.UltimateInventorySystem.Core.InventoryCollections;
    using Opsive.UltimateInventorySystem.UI.Item.ItemViewModules;
    using Opsive.UltimateInventorySystem.UI.Panels;
    using Opsive.UltimateInventorySystem.UI.Panels.ItemViewSlotContainers;
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Serialization;
    using UnityEngine.UI;

    public class ItemViewSlotActionEvent
    {
        protected bool m_StopPropagation; 
        
        public ItemViewSlotEventHandler Action { get; protected set; }
        public bool Enabled { get; protected set; }

        public bool StopPropagation => Enabled && m_StopPropagation;

        public ItemViewSlotActionEvent(ItemViewSlotEventHandler action, bool stopPropagation)
        {
            Action = action;
            m_StopPropagation = stopPropagation;
            Enabled = true;
        }

        public void Invoke(ItemViewSlotEventData slotEventData)
        {
            if(Enabled == false){ return; }
            
            Action?.Invoke(slotEventData);
        }

        public void Enable(bool enable)
        {
            Enabled = enable;
        }
    }
    
    /// <summary>
    /// The hot item bar component allows you to use an item action for an item that was added to the hot bar.
    /// </summary>
    public abstract class ItemViewSlotsContainerBase : MonoBehaviour
    {
        public event ItemViewSlotEventHandler OnItemViewSlotSelected;
        public event ItemViewSlotEventHandler OnItemViewSlotDeselected;
        public event ItemViewSlotEventHandler OnItemViewSlotClicked;
        public event ItemViewSlotPointerEventHandler OnItemViewSlotPointerDownE;
        public event ItemViewSlotPointerEventHandler OnItemViewSlotBeginDragE;
        public event ItemViewSlotPointerEventHandler OnItemViewSlotEndDragE;
        public event ItemViewSlotPointerEventHandler OnItemViewSlotDragE;
        public event ItemViewSlotPointerEventHandler OnItemViewSlotDropE;

        [SerializeField] protected string m_ContainerName; 
        [FormerlySerializedAs("m_SlotMouseCursor")]
        [FormerlySerializedAs("m_SlotCursorManager")]
        [FormerlySerializedAs("m_CursorManager")]
        [Tooltip("The parent of all the itemBoxSlots.")]
        [SerializeField]protected ItemViewSlotCursorManager m_SlotCursor;

        protected ItemViewSlot m_SelectedSlot;
        protected ItemViewSlot[] m_ItemViewSlots;

        protected bool m_IsInitialized;
        protected ItemViewSlotEventData m_ItemViewSlotEventData;
        protected ItemViewSlotPointerEventData m_ItemViewSlotPointerEventData;
        protected ItemViewSlotPointerEventData m_ItemViewSlotDropEventData;
        protected DisplayPanel m_DisplayPanel;

        public int SlotCount => m_ItemViewSlots.Length;
        public IReadOnlyList<ItemViewSlot> ItemViewSlots => m_ItemViewSlots;
        public ItemViewSlotCursorManager ItemViewSlotCursor => m_SlotCursor;
        public DisplayPanel Panel => m_DisplayPanel;
        public string ContainerName => m_ContainerName;

        protected ItemViewSlotActionEvent m_OneTimeClick;

        protected virtual void Awake()
        {
            Initialize(false);
        }

        public void SetName(string containerName)
        {
            m_ContainerName = containerName;
        }

        /// <summary>
        /// Set up the item hot bar slots
        /// </summary>
        public virtual void Initialize(bool force)
        {
            if(m_IsInitialized && !force){ return; }

            
            if (m_SlotCursor == null) {
                m_SlotCursor = GetComponentInParent<ItemViewSlotCursorManager>();
                if (m_SlotCursor == null && Application.isPlaying) {
                    Debug.LogWarning("The Item View Slot Cursor Manager is missing, please add one on your canvas.");
                }
            }
            
            m_ItemViewSlotEventData = new ItemViewSlotEventData();
            m_ItemViewSlotPointerEventData = new ItemViewSlotPointerEventData();
            m_ItemViewSlotDropEventData = new ItemViewSlotPointerEventData();
            
            
            for (int i = 0; i < m_ItemViewSlots.Length; i++) {
                var itemViewSlot =m_ItemViewSlots[i];

                var localIndex = i;
                
                itemViewSlot.AssignIndex(i);
                
                itemViewSlot.OnSubmitE += () =>
                {
                    m_ItemViewSlotEventData.SetValues(this, localIndex);

                    if (m_OneTimeClick != null) {
                        var oneTimeClick = m_OneTimeClick;
                        m_OneTimeClick = null;

                        oneTimeClick.Invoke(m_ItemViewSlotEventData);
                        if (oneTimeClick.StopPropagation) {
                            return;
                        }
                    }
                    
                    OnItemViewSlotClicked?.Invoke(m_ItemViewSlotEventData);
                };
                itemViewSlot.OnSelectE += ()=>
                {
                    m_SelectedSlot = GetItemViewSlot(localIndex);
                    m_ItemViewSlotEventData.SetValues(this, localIndex);
                    OnItemViewSlotSelected?.Invoke(m_ItemViewSlotEventData);
                };
                itemViewSlot.OnDeselectE += ()=>
                {
                    m_SelectedSlot = null;
                    m_ItemViewSlotEventData.SetValues(this, localIndex);
                    OnItemViewSlotDeselected?.Invoke(m_ItemViewSlotEventData);
                };
                itemViewSlot.OnPointerDownE += (pointerEventData) =>
                {
                    m_ItemViewSlotPointerEventData.SetValues(this, localIndex);
                    m_ItemViewSlotPointerEventData.PointerEventData = pointerEventData;
                    OnItemViewSlotPointerDownE?.Invoke(m_ItemViewSlotPointerEventData);
                };
                itemViewSlot.OnBeginDragE += (pointerEventData) =>
                {
                    m_ItemViewSlotPointerEventData.SetValues(this, localIndex);
                    m_ItemViewSlotPointerEventData.PointerEventData = pointerEventData;
                    OnItemViewSlotBeginDragE?.Invoke(m_ItemViewSlotPointerEventData);
                };
                itemViewSlot.OnEndDragE += (pointerEventData) =>
                {
                    //m_ItemBoxPointerEventData.SetValues(this, GetItemBoxAt(index),index);
                    m_ItemViewSlotPointerEventData.PointerEventData = pointerEventData;
                    OnItemViewSlotEndDragE?.Invoke(m_ItemViewSlotPointerEventData);
                };
                itemViewSlot.OnDragE += (pointerEventData) =>
                {
                    //m_ItemBoxPointerEventData.SetValues(this, GetItemBoxAt(index),index);
                    m_ItemViewSlotPointerEventData.PointerEventData = pointerEventData;
                    OnItemViewSlotDragE?.Invoke(m_ItemViewSlotPointerEventData);
                };
                itemViewSlot.OnDropE += (pointerEventData) =>
                {
                    m_ItemViewSlotDropEventData.SetValues(this, localIndex);
                    m_ItemViewSlotDropEventData.PointerEventData = pointerEventData;
                    OnItemViewSlotDropE?.Invoke(m_ItemViewSlotDropEventData);
                };
            }

            var bindings = GetComponents<ItemViewSlotsContainerBinding>();
            for (int i = 0; i < bindings.Length; i++) {
                bindings[i].Bind(this);
            }

            m_IsInitialized = true;
        }

        public void SetOneTimeClickAction(ItemViewSlotActionEvent action)
        {
            m_OneTimeClick = action;
        }

        /// <summary>
        /// Assign an item to a slot.
        /// </summary>
        /// <param name="itemInfo">The item.</param>
        /// <param name="slot">The item slot.</param>
        protected virtual void AssignItemToSlot(ItemInfo itemInfo, int slot)
        {
            m_ItemViewSlots[slot].SetItemInfo(itemInfo);
        }

        /// <summary>
        /// Reset the items in the hot bar when destroyed (Use for domain reload disabled)
        /// </summary>
        protected virtual void OnDestroy()
        {
            if(m_ItemViewSlots == null){ return; }
            
            for (int i = 0; i < m_ItemViewSlots.Length; i++) {
                if(m_ItemViewSlots[i]?.ItemView == null){continue;}
                //TODO this causes a problem with Dynamic ItemCategory, is there another solution? 
                //m_ItemViewSlots[i].SetItemInfo(new ItemInfo());
            }
        }

        public virtual ItemInfo RemoveItem(ItemInfo itemInfo, int index)
        {
            var removedItem = GetItemAt(index);

            AssignItemToSlot(new ItemInfo((null,0)), index);
            
            return removedItem;
        }
        
        public virtual bool CanAddItem(ItemInfo itemInfo, int index)
        {
            if (itemInfo.Amount <= 0 || itemInfo.Item == null) { return false; }

            if (index < 0 || index >= m_ItemViewSlots.Length) { return false;}

            if (m_ItemViewSlots[index].CanContain(itemInfo) == false) { return false; }

            return true;
        }

        public virtual ItemInfo AddItem(ItemInfo itemInfo, int index)
        {
            if (CanAddItem(itemInfo,index) == false) {
                return (0, null, null);
            }
            
            AssignItemToSlot(itemInfo, index);
            return itemInfo;
        }
        
        public virtual bool CanMoveItem(int sourceIndex, int destinationIndex)
        {
            if (sourceIndex < 0 || sourceIndex >= m_ItemViewSlots.Length) { return false;}
            if (destinationIndex < 0 || destinationIndex >= m_ItemViewSlots.Length) { return false;}
            
            return m_ItemViewSlots[sourceIndex].CanContain(m_ItemViewSlots[destinationIndex].ItemInfo)
                && m_ItemViewSlots[destinationIndex].CanContain(m_ItemViewSlots[sourceIndex].ItemInfo);
        }

        public virtual void MoveItem(int sourceIndex, int destinationIndex)
        {
            if (CanMoveItem(sourceIndex, destinationIndex) == false) { return; }

            var itemAtSource = m_ItemViewSlots[sourceIndex].ItemInfo;
            AssignItemToSlot(m_ItemViewSlots[destinationIndex].ItemInfo, sourceIndex);
            AssignItemToSlot(itemAtSource, destinationIndex);
        }

        public virtual int GetItemCount()
        {
            return m_ItemViewSlots.Length;
        }

        public virtual ItemInfo GetItemAt(int index)
        {
            return m_ItemViewSlots[index].ItemInfo;
        }

        public virtual int GetItemViewSlotCount()
        {
            return m_ItemViewSlots.Length;
        }

        public virtual ItemView GetItemViewAt(int index)
        {
            return m_ItemViewSlots[index].ItemView;
        }

        public virtual void Draw()
        {
            for (int i = 0; i < m_ItemViewSlots.Length; i++) {
                AssignItemToSlot(m_ItemViewSlots[i].ItemInfo, i);
            }
        }

        /// <summary>
        /// Get the Box prefab for the item specified.
        /// </summary>
        /// <param name="itemInfo">The item info.</param>
        /// <returns>The box prefab game object.</returns>
        public abstract GameObject GetBoxPrefabFor(ItemInfo itemInfo);

        public ItemViewSlot GetItemViewSlot(int index)
        {
            return m_ItemViewSlots[index];
        }

        public virtual void SetDisplayPanel(DisplayPanel display)
        {
            m_DisplayPanel = display;
        }

        public void SelectSlot(int index)
        {
            if(index < 0 || index >= m_ItemViewSlots.Length){ return; }
            m_ItemViewSlots[index].Select();
        }

        public ItemViewSlot GetSelectedSlot()
        {
            return m_SelectedSlot;
        }

        public int GetItemIndex(ItemInfo itemInfo)
        {
            for (int i = 0; i < m_ItemViewSlots.Length; i++) {
                if (itemInfo == m_ItemViewSlots[i].ItemInfo) {
                    return i;
                }
            }

            return -1;
        }
    }
}