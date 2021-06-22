namespace Opsive.UltimateInventorySystem.UI.Grid
{
    using Opsive.Shared.Utility;
    using Opsive.UltimateInventorySystem.Core.DataStructures;
    using Opsive.UltimateInventorySystem.UI.Item;
    using System;
    using System.Collections.Generic;
    using UnityEngine;


    public interface IFilterSorter { }
    
    public interface IFilterSorter<T> : IFilterSorter
    {
        ListSlice<T> Filter(ListSlice<T> input, ref T[] outputPooledArray);
    }

    public abstract class FilterSorter : MonoBehaviour, IFilterSorter
    { }
    
    public abstract class FilterSorter<T> : FilterSorter, IFilterSorter<T>
    {
        public abstract ListSlice<T> Filter(ListSlice<T> input, ref T[] outputPooledArray);

        public abstract bool CanContain(T input);
    }

    public abstract class ItemInfoFilterSorterBase : FilterSorter<ItemInfo>
    {
        public override string ToString()
        {
            return GetType().Name;
        }
    }

    public abstract class ItemInfoFilterBase : ItemInfoFilterSorterBase
    {
        public abstract bool Filter(ItemInfo itemInfo);

        public override ListSlice<ItemInfo> Filter(ListSlice<ItemInfo> input, ref ItemInfo[] outputPooledArray)
        {
            outputPooledArray.ResizeIfNecessary(ref outputPooledArray, input.Count);
            
            var count = 0;
            for (int i = 0; i < input.Count; i++) {
                if (Filter(input[i]) == false) { continue; }

                outputPooledArray[count] = input[i];
                count++;
            }
            
            return (outputPooledArray, 0, count);
        }

        public override bool CanContain(ItemInfo input)
        {
            return Filter(input);
        }
    }
    
    public abstract class ItemInfoSorterBase : ItemInfoFilterSorterBase
    {
        public abstract Comparer<ItemInfo> Comparer { get; }

        protected virtual void Awake()
        { }
        
        public override ListSlice<ItemInfo> Filter(ListSlice<ItemInfo> input, ref ItemInfo[] outputPooledArray)
        {
            outputPooledArray.ResizeIfNecessary(ref outputPooledArray, input.Count);
            var count =  input.Count;
            
            for (int i = 0; i < input.Count; i++) {
                outputPooledArray[i] = input[i];
            }

            Array.Sort(outputPooledArray,0,count,Comparer);

            return (outputPooledArray, 0, count);
        }
        
        public override bool CanContain(ItemInfo input)
        {
            return true;
        }
    }
}