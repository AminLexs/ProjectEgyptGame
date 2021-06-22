/// ---------------------------------------------
/// Ultimate Inventory System
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------

namespace Opsive.UltimateInventorySystem.UI.CompoundElements
{
    using System;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;

    /// <summary>
    /// A action button class, used when the default Button is not flexible enough.
    /// </summary>
    public class ActionButton : Selectable, IPointerClickHandler, ISubmitHandler, ICancelHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IDropHandler
    {
        public event Action OnSubmitE;
        public event Action OnSelectE;
        public event Action OnDeselectE;
        public event Action OnCancelE;
        public event Action<AxisEventData, bool> OnMoveE;
        public event Action<PointerEventData> OnPointerDownE;
        public event Action<PointerEventData> OnPointerUpE;
        public event Action<PointerEventData> OnPointerEnterE;
        public event Action<PointerEventData> OnPointerExitE;
        public event Action<PointerEventData> OnBeginDragE;
        public event Action<PointerEventData> OnEndDragE;
        public event Action<PointerEventData> OnDragE;
        public event Action<PointerEventData> OnDropE;

        [Tooltip("Disable and selected sprite, an extension for the selectable sprite transitions.")]
        [SerializeField] protected Sprite m_DisabledAndSelectedSprite;
        [Tooltip("Disable and selected color, an extension for the selectable color transitions.")]
        [SerializeField] protected Color m_DisabledAndSelectedColor = Color.gray;
        [Tooltip("The text that displays the action name.")]
        [SerializeField] protected Text m_Text;
        [Tooltip("The on click Unity Action")]
        [SerializeField] internal UnityEvent m_OnClickEvent;

        protected Action m_CachedHandler;
        protected bool m_Selected = false;

        /// <summary>
        /// Press the button.
        /// </summary>
        private void Press()
        {
            if (!IsActive() || !IsInteractable()) {
                return;
            }
            UISystemProfilerApi.AddMarker("Button.onClick", (UnityEngine.Object)this);

            if (!m_Selected) {
                Select();
                return;
            }

            m_OnClickEvent.Invoke();
            OnSubmitE?.Invoke();
        }

        /// <summary>
        /// Invoke OnSelect even if it was already selected.
        /// </summary>
        public override void Select()
        {
            base.Select();
            OnSelectE?.Invoke();
        }

        /// <summary>
        /// Set the name of the button.
        /// </summary>
        /// <param name="newName">The new name.</param>
        public virtual void SetButtonName(string newName)
        {
            m_Text.text = newName;
        }

        /// <summary>
        /// Set the Button action. Only one at a time.
        /// </summary>
        /// <param name="buttonAction">The new button action.</param>
        public virtual void SetButtonAction(Action buttonAction)
        {
            OnSubmitE -= m_CachedHandler;
            OnSubmitE += buttonAction;
            m_CachedHandler = buttonAction;
        }

        /// <summary>
        ///   <para>Registered IPointerClickHandler callback.</para>
        /// </summary>
        /// <param name="eventData">Data passed in (Typically by the event system).</param>
        public virtual void OnPointerClick(PointerEventData eventData)
        {
            //Debug.Log("Pointer Click");
            if (eventData.button != PointerEventData.InputButton.Left) {
                return;
            }

            Press();
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            //Debug.Log("Pointer Down");
            OnPointerDownE?.Invoke(eventData);
            base.OnPointerDown(eventData);
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            //Debug.Log("Pointer Up");
            OnPointerUpE?.Invoke(eventData);
            base.OnPointerUp(eventData);
        }
        
        /// <summary>
        /// Call the On Pointer enter event.
        /// </summary>
        /// <param name="eventData">The event data.</param>
        public override void OnPointerEnter(PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);

            if (eventData.IsPointerMoving() == false) { return; }

            if (!EventSystem.current.alreadySelecting) {
                EventSystem.current.SetSelectedGameObject(gameObject);
            }
            OnPointerEnterE?.Invoke(eventData);
            OnSelectE?.Invoke();
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            //Debug.Log("Pointer Up");
            base.OnPointerExit(eventData);
            OnPointerExitE?.Invoke(eventData);
            OnDeselectE?.Invoke();
        }
        
        public virtual void OnBeginDrag(PointerEventData eventData)
        {
            //Debug.Log("Begin drag");
            OnBeginDragE?.Invoke(eventData);
        }

        public virtual void OnEndDrag(PointerEventData eventData)
        {
            OnEndDragE?.Invoke(eventData);
        }

        public virtual void OnDrag(PointerEventData eventData)
        {
            //Debug.Log("Dragging");
            OnDragE?.Invoke(eventData);
        }

        public virtual void OnDrop(PointerEventData eventData)
        {
            OnDropE?.Invoke(eventData);
        }

        /// <summary>
        ///   <para>Registered ISubmitHandler callback.</para>
        /// </summary>
        /// <param name="eventData">Data passed in (Typically by the event system).</param>
        public virtual void OnSubmit(BaseEventData eventData)
        {
            Press();
            if (!IsActive() || !IsInteractable()) {
                return;
            }
            DoStateTransition(SelectionState.Pressed, false);
        }

        /// <summary>
        /// Call the On cancel event.
        /// </summary>
        /// <param name="eventData">The event data.</param>
        public virtual void OnCancel(BaseEventData eventData)
        {
            OnCancelE?.Invoke();
        }

        /// <summary>
        /// Call the On Select event.
        /// </summary>
        /// <param name="eventData">The event data.</param>
        public override void OnSelect(BaseEventData eventData)
        {
            base.OnSelect(eventData);
            m_Selected = true;
            OnSelectE?.Invoke();
            if (interactable == false) { DoStateTransition(currentSelectionState, false); }
        }

        /// <summary>
        /// Call the On Deselect event.
        /// </summary>
        /// <param name="eventData">The event data.</param>
        public override void OnDeselect(BaseEventData eventData)
        {
            base.OnDeselect(eventData);
            m_Selected = false;
            OnDeselectE?.Invoke();
            if (interactable == false) { DoStateTransition(currentSelectionState, false); }
        }

        /// <summary>
        /// Call the OnMove event.
        /// </summary>
        /// <param name="eventData">The event data.</param>
        public override void OnMove(AxisEventData eventData)
        {
            var moved = false;

            switch (eventData.moveDir) {
                case MoveDirection.Left:
                    moved = FindSelectableOnLeft() != null;
                    break;
                case MoveDirection.Up:
                    moved = FindSelectableOnUp() != null;
                    break;
                case MoveDirection.Right:
                    moved = FindSelectableOnRight() != null;
                    break;
                case MoveDirection.Down:
                    moved = FindSelectableOnDown() != null;
                    break;
            }

            base.OnMove(eventData);

            OnMoveE?.Invoke(eventData, moved);
        }

        /// <summary>
        /// Transition from one state to another.
        /// </summary>
        /// <param name="state">The state.</param>
        /// <param name="instant">Do the transition instantly.</param>
        protected override void DoStateTransition(Selectable.SelectionState state, bool instant)
        {
            if (!this.gameObject.activeInHierarchy)
                return;
            Color color;
            Sprite newSprite;
            string triggername;
            switch (state) {
                case Selectable.SelectionState.Normal:
                    color = colors.normalColor;
                    newSprite = null;
                    triggername = animationTriggers.normalTrigger;
                    break;
                case Selectable.SelectionState.Highlighted:
                    color = colors.highlightedColor;
                    newSprite = spriteState.highlightedSprite;
                    triggername = animationTriggers.highlightedTrigger;
                    break;
                case Selectable.SelectionState.Pressed:
                    color = colors.pressedColor;
                    newSprite = spriteState.pressedSprite;
                    triggername = animationTriggers.pressedTrigger;
                    break;
                case Selectable.SelectionState.Selected:
                    color = colors.selectedColor;
                    newSprite = spriteState.selectedSprite;
                    triggername = animationTriggers.selectedTrigger;
                    break;
                case Selectable.SelectionState.Disabled:
                    color = colors.disabledColor;
                    newSprite = spriteState.disabledSprite;
                    triggername = animationTriggers.disabledTrigger;
                    break;
                default:
                    color = Color.black;
                    newSprite = null;
                    triggername = string.Empty;
                    break;
            }
            switch (transition) {
                case Selectable.Transition.ColorTint:
                    color = m_Selected && !interactable ? m_DisabledAndSelectedColor : color;
                    StartColorTween(color * colors.colorMultiplier, instant);
                    break;
                case Selectable.Transition.SpriteSwap:
                    newSprite = m_Selected && !interactable ? m_DisabledAndSelectedSprite : newSprite;
                    this.DoSpriteSwap(newSprite);
                    break;
                case Selectable.Transition.Animation:
                    this.TriggerAnimation(triggername);
                    break;
            }
        }

        /// <summary>
        /// Start the color transitions.
        /// </summary>
        /// <param name="targetColor">The target color.</param>
        /// <param name="instant">Is the transition instant.</param>
        protected virtual void StartColorTween(Color targetColor, bool instant)
        {
            if ((UnityEngine.Object)targetGraphic == (UnityEngine.Object)null)
                return;
            targetGraphic.CrossFadeColor(targetColor, !instant ? colors.fadeDuration : 0.0f, true, true);
        }

        /// <summary>
        /// Do a sprite swap.
        /// </summary>
        /// <param name="newSprite">The new sprite.</param>
        protected virtual void DoSpriteSwap(Sprite newSprite)
        {
            if ((UnityEngine.Object)this.image == (UnityEngine.Object)null)
                return;
            this.image.overrideSprite = newSprite;
        }

        /// <summary>
        /// Trigger an animation.
        /// </summary>
        /// <param name="triggername">The rigger name.</param>
        protected virtual void TriggerAnimation(string triggername)
        {
            if (this.transition != Selectable.Transition.Animation || (UnityEngine.Object)this.animator == (UnityEngine.Object)null || (!this.animator.isActiveAndEnabled || !this.animator.hasBoundPlayables) || string.IsNullOrEmpty(triggername))
                return;
            this.animator.ResetTrigger(animationTriggers.normalTrigger);
            this.animator.ResetTrigger(animationTriggers.highlightedTrigger);
            this.animator.ResetTrigger(animationTriggers.pressedTrigger);
            this.animator.ResetTrigger(animationTriggers.selectedTrigger);
            this.animator.ResetTrigger(animationTriggers.disabledTrigger);
            this.animator.SetTrigger(triggername);
        }
        
        /*
        // Select on enable and add to the list.
        protected override void OnEnable()
        {
            if(Application.isPlaying == false){return;}
            
            base.OnEnable();
        }
        
        // Remove from the list.
        protected override void OnDisable()
        {
            if(Application.isPlaying == false){return;}
            base.OnDisable();
        }*/
    }
}
