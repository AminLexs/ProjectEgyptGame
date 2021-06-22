/// ---------------------------------------------
/// Ultimate Inventory System
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------

namespace Opsive.UltimateInventorySystem.Utility
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// A static class used to extend the Array class.
    /// </summary>
    public static class ListUtility
    {
        /// <summary>
        /// Check if an array is equivalent to another.
        /// </summary>
        /// <param name="arr1">Array 1.</param>
        /// <param name="arr2">Array 2.</param>
        /// <typeparam name="T">The array type.</typeparam>
        /// <returns>True if equivalent.</returns>
        public static bool SequenceEquals<T>(T[] arr1, T[] arr2) where T : class
        {
            // If arr1 and arr2 refer to the same array, they are trivially equal.
            if (ReferenceEquals(arr1, arr2)) return true;

            // If either arr1 or arr2 is null and they are not both null (see the previous
            // check), they are not equal.
            if (arr1 == null || arr2 == null) return false;

            // If both arrays are non-null but have different lengths, they are not equal.
            if (arr1.Length != arr2.Length) return false;

            // Failing which, do an element-by-element compare.
            for (int i = 0; i < arr1.Length; ++i) {
                // Early out if we find corresponding array elements that are not equal.
                if (!arr1[i].Equals(arr2[i])) return false;
            }

            // If we get here, all of the corresponding array elements were equal, so the
            // arrays are equal.
            return true;
        }

        /// <summary>
        /// Hash an array.
        /// </summary>
        /// <param name="arr">The array to hash.</param>
        /// <typeparam name="T">The array type.</typeparam>
        /// <returns>The hash.</returns>
        public static int SequenceHash<T>(T[] arr)
        {
            int hash = 0;

            for (int i = 0; i < arr.Length; ++i) {
                hash += arr[i].GetHashCode();
            }

            return hash;
        }
        
        /// <summary>
        /// Check if the array contains an element between the start and end index.
        /// </summary>
        /// <param name="arr">The array.</param>
        /// <param name="element">The elements to check.</param>
        /// <param name="start">The start index.</param>
        /// <param name="end">The end index.</param>
        /// <typeparam name="T">The array type.</typeparam>
        /// <returns>True if the array contains the element.</returns>
        public static bool Contains<T>(this IList<T> arr, T element) where T : class
        {
            for (int i = 0; i < arr.Count; i++) {
                if (ReferenceEquals(arr[i], element)) { return true; }
            }

            return false;
        }

        /// <summary>
        /// Check if the array contains an element between the start and end index.
        /// </summary>
        /// <param name="arr">The array.</param>
        /// <param name="element">The elements to check.</param>
        /// <param name="start">The start index.</param>
        /// <param name="end">The end index.</param>
        /// <typeparam name="T">The array type.</typeparam>
        /// <returns>True if the array contains the element.</returns>
        public static bool Contains<T>(this IList<T> arr, T element, int start, int end) where T : class
        {
            for (int i = start; i < end; i++) {
                if (ReferenceEquals(arr[i], element)) { return true; }
            }

            return false;
        }

        /// <summary>
        /// Check if any of the element in the array passes the condition.
        /// </summary>
        /// <param name="arr">The array to check.</param>
        /// <param name="condition">The condition.</param>
        /// <typeparam name="T">The array type.</typeparam>
        /// <returns>True if the the array has at least one element that passes the condition.</returns>
        public static bool Any<T>(this IList<T> arr, Func<T, bool> condition) where T : class
        {
            return arr.Any(condition, 0, arr.Count);
        }

        /// <summary>
        /// Check if any of the element in the array passes the condition.
        /// </summary>
        /// <param name="arr">The array to check.</param>
        /// <param name="condition">The condition.</param>
        /// <param name="start">The start index.</param>
        /// <param name="end">The end index.</param>
        /// <typeparam name="T">The array type.</typeparam>
        /// <returns>True if the the array has at least one element that passes the condition.</returns>
        public static bool Any<T>(this IList<T> arr, Func<T, bool> condition, int start, int end) where T : class
        {
            for (int i = start; i < end; i++) {
                if (condition(arr[i])) { return true; }
            }

            return false;
        }

        /// <summary>
        /// Return the first element that passes the condition.
        /// </summary>
        /// <param name="arr">The array.</param>
        /// <param name="condition">The condition.</param>
        /// <typeparam name="T">The array type.</typeparam>
        /// <returns>Returns the first element in the list that passes the condition.</returns>
        public static T GetAny<T>(this IList<T> arr, Func<T, bool> condition) where T : class
        {
            return arr.GetAny(condition, 0, arr.Count);
        }

        /// <summary>
        /// Return the first element that passes the condition.
        /// </summary>
        /// <param name="arr">The array.</param>
        /// <param name="condition">The condition.</param>
        /// <param name="start">The start index.</param>
        /// <param name="end">The end index.</param>
        /// <typeparam name="T">The array type.</typeparam>
        /// <returns>Returns the first element in the list that passes the condition.</returns>
        public static T GetAny<T>(this IList<T> arr, Func<T, bool> condition, int start, int end) where T : class
        {
            for (int i = start; i < end; i++) {
                if (condition(arr[i])) { return arr[i]; }
            }

            return null;
        }

        /// <summary>
        /// Creates an array of the correct length.
        /// </summary>
        /// <param name="length">The length of the copy.</param>
        /// <param name="source">The source array.</param>
        /// <typeparam name="T">The element type of the array.</typeparam>
        /// <returns>The new array.</returns>
        public static T[] CreateArrayCopy<T>(int length, T[] source)
        {
            if (length <= 0) { return new T[0]; }

            var newArray = new T[length];
            Array.Copy(source, newArray, length);
            return newArray;
        }

        /// <summary>
        /// Increase the size of the list to ensure it has a certain size.
        /// </summary>
        /// <param name="list">The list to increase.</param>
        /// <param name="size">The size.</param>
        /// <param name="value">The value to populate in each new element.</param>
        /// <typeparam name="T">The list element type.</typeparam>
        /// <returns>The list </returns>
        public static void EnsureSize<T>(this List<T> list, int size, T value = default(T))
        {
            if (list == null) throw new ArgumentNullException("list");
            if (size < 0) throw new ArgumentOutOfRangeException("size");

            int count = list.Count;
            if (count >= size) { return; }

            int capacity = list.Capacity;
            if (capacity < size)
                list.Capacity = Math.Max(size, capacity * 2);

            while (count < size)
            {
                list.Add(value);
                ++count;
            }
        }
        
        /// <summary>
        /// Increase the size of the list to ensure it has a certain size.
        /// </summary>
        /// <param name="list">The list to print.</param>
        /// <returns>The list </returns>
        public static string ToStringDeep<T>(this List<T> list)
        {
            if (list == null) throw new ArgumentNullException("list");

            var str = "List Content: \n";
            for (int i = 0; i < list.Count; i++) {
                str += $"i: {i} -> {list[i]} \n";
            }

            return str;
        }
    }
}

