/// ---------------------------------------------
/// Ultimate Inventory System
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------

namespace Opsive.UltimateInventorySystem.Demo.CharacterControl
{
    using Core.InventoryCollections;
    using Damageable;
    using Opsive.Shared.Game;
    using Opsive.UltimateInventorySystem.Equipping;
    using Opsive.UltimateInventorySystem.ItemActions;
    using UnityEngine;

    /// <summary>
    /// The character script fo the Demo.
    /// </summary>
    public class Character : MonoBehaviour
    {
        [Tooltip("The character speed in units/second.")]
        [SerializeField] protected float m_Speed = 1f;
        [Tooltip("A set of stats for the character.")]
        [SerializeField] protected Stats m_BaseStats;
        [Tooltip("Respawn on death.")]
        [SerializeField] protected bool m_RespawnOnDeath;

        protected Rigidbody m_Rigidbody;
        protected CharacterController m_CharacterController;
        protected Animator m_Anim;
        protected Inventory m_Inventory;
        protected IEquipper m_Equipper;
        protected DemoCharacterDamageable m_CharacterDamageable;

        protected ICharacterMover m_CharacterMover;
        protected CharacterRotator m_CharacterRotator;
        protected ICharacterInput m_CharacterInput;
        protected CharacterStats m_CharacterStats;
        protected CharacterAnimator m_CharacterAnimator;

        protected Camera m_Camera;
        protected ItemUser m_ItemUser;

        public CharacterController CharacterController => m_CharacterController;
        public ICharacterInput CharacterInput {
            get => m_CharacterInput;
            set => m_CharacterInput = value;
        }

        public Camera CharacterCamera => m_Camera;

        public float Speed => m_Speed;
        public IEquipper Equipper => m_Equipper;
        public CharacterStats CharacterStats => m_CharacterStats;
        public DemoCharacterDamageable CharacterDamageable => m_CharacterDamageable;
        public CharacterAnimator CharacterAnimator => m_CharacterAnimator;

        public Inventory Inventory => m_Inventory;
        public ItemUser ItemUser => m_ItemUser;

        /// <summary>
        /// Initialize all the properties.
        /// </summary>
        protected virtual void Awake()
        {
            m_Camera = Camera.main;

            m_CharacterController = GetComponent<CharacterController>();
            m_Rigidbody = GetComponent<Rigidbody>();
            m_Anim = GetComponent<Animator>();
            m_Equipper = GetComponent<IEquipper>();
            m_CharacterDamageable = GetComponent<DemoCharacterDamageable>();
            m_Inventory = GetComponent<Inventory>();
            m_ItemUser = GetComponent<ItemUser>();

            m_CharacterMover = new CharacterMover(this);
            m_CharacterRotator = new CharacterRotator(this);
            m_CharacterStats = new CharacterStats(m_BaseStats, m_Equipper);
            m_CharacterAnimator = new CharacterAnimator(m_Anim);

            if (m_CharacterInput == null) {
                m_CharacterInput = GetComponent<ICharacterInput>();
            }

            //Ignore the collisions between layer 8 (Character) and layer 10 (IgnoreCharacter).
            Physics.IgnoreLayerCollision(8, 10);
        }

        /// <summary>
        /// Tick all the properties which needs to update every frame.
        /// </summary>
        protected virtual void Update()
        {
            if (Time.timeScale == 0) { return; }

            m_CharacterMover?.Tick();
            m_CharacterRotator?.Tick();
            m_CharacterInput?.Tick();

            HandleAnimations();
        }

        /// <summary>
        /// Make the character die.
        /// </summary>
        public virtual void Die()
        {
            gameObject.SetActive(false);
            if (m_RespawnOnDeath) {
                Scheduler.Schedule(0.5f, Respawn);
            }

        }

        /// <summary>
        /// Respawn the character.
        /// </summary>
        public virtual void Respawn()
        {
            m_CharacterDamageable.Heal(int.MaxValue, false);
            transform.position = new Vector3(0, 1, 0);
            gameObject.SetActive(true);
        }

        /// <summary>
        /// Add animation property triggers and values.
        /// </summary>
        public virtual void HandleAnimations()
        {
            var velocity = m_CharacterController.velocity;
            var xzVelocity = new Vector2(velocity.x, velocity.z);
            m_CharacterAnimator.Move(xzVelocity.sqrMagnitude);

        }
    }
}

