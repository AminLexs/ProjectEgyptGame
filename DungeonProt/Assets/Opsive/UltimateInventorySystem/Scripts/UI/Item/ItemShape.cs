namespace Opsive.UltimateInventorySystem.UI.Item
{
    using System;
    using UnityEngine;

    [Serializable]
    public class ItemShape
    {
        [SerializeField] protected Vector2Int m_Size = Vector2Int.one;
        [SerializeField] protected bool m_UseCustomShape;
        [SerializeField] internal bool[] m_CustomShape;
        [SerializeField] protected Vector2Int m_Anchor;

        public Vector2Int Size => m_Size;
        public bool UseCustomShape => m_UseCustomShape;
        public int Rows => m_Size.y;
        public int Cols => m_Size.x;
        public int Count => m_Size.x*m_Size.y;
        public Vector2Int Anchor => m_Anchor;
        public int AnchorIndex => m_Anchor.y * m_Size.x + m_Anchor.x;

        public void SetSize(int rows, int columns)
        {
            SetSize(new Vector2Int(columns, rows));
        }
        
        public void SetSize(Vector2Int newSize)
        {
            var previousSize = m_Size;
            m_Size = new Vector2Int(Mathf.Max(1,newSize.x), Mathf.Max(1, newSize.y) );

            if (m_UseCustomShape == false) { return; }
            
            if (m_CustomShape == null) {
                m_CustomShape = new bool[m_Size.x * m_Size.y];
            } else if (previousSize.x != m_Size.x || previousSize.y != m_Size.y){
                m_CustomShape = ResizeArray(m_CustomShape, previousSize, m_Size);
            }
            
            if (m_Anchor.x >= m_Size.x || m_Anchor.y >= m_Size.y) {
                m_Anchor = Vector2Int.zero;
            }

            if (m_CustomShape[AnchorIndex] == false) {
                m_CustomShape[AnchorIndex] = true;
            }
        }

        public void SetUseCustomShape(bool useCustomShape)
        {
            if(m_UseCustomShape == useCustomShape){ return; }

            m_UseCustomShape = useCustomShape;
            
            if(m_UseCustomShape == false){ return; }

            m_CustomShape = new bool[m_Size.x*m_Size.y];
            for (int i = 0; i < m_CustomShape.Length; i++) { m_CustomShape[i] = true; }

        }

        /// <summary>
        /// Starts at the top left corner and starts horizontally.
        /// </summary>
        /// <param name="x">The horizontal index.</param>
        /// <param name="y">The vertical index.</param>
        /// <returns></returns>
        public bool IsIndexOccupied(int x, int y)
        {
            if (m_Size.x < x || m_Size.y < y) { return false; }

            if (m_UseCustomShape == false) { return true; }

            return m_CustomShape[y * m_Size.x + x];
        } 
    
        protected T[] ResizeArray<T>(T[] original, Vector2Int previousSize, Vector2Int newSize)
        {
            var newArray = new T[newSize.x*newSize.y];
            int minRows = Math.Min(newSize.y, previousSize.y);
            int minCols = Math.Min(newSize.x, previousSize.x);
            for (int y = 0; y < minRows; y++) {
                for (int x = 0; x < minCols; x++) {

                    newArray[y*newSize.x+x] = original[y*previousSize.x+x];
                }
            }
            
                
            return newArray;
        }

        public override string ToString()
        {
            return string.Format(m_UseCustomShape ? "[{0},{1}] Custom Shape" : "[{0},{1}]", m_Size.x, m_Size.y);
        }

        public void SetAnchor(Vector2Int newAnchor)
        {
            if (IsIndexOccupied(newAnchor.x, newAnchor.y)) { m_Anchor = newAnchor; }
        }
    }
}
