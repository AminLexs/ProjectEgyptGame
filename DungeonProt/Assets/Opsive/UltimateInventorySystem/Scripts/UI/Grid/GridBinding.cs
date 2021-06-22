namespace Opsive.UltimateInventorySystem.UI.Grid
{
    using UnityEngine;

    public abstract class GridBinding : MonoBehaviour
    {

        protected GridBase m_Grid;
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

        public virtual void Bind(GridBase grid)
        {
            Initialize(false);
            if(m_Grid == grid){ return; }

            UnBind();

            m_Grid = grid;
        }
        
        public virtual void UnBind()
        {
            Initialize(false);
            m_Grid = null;
        }
    }
}