/// ---------------------------------------------
/// Ultimate Inventory System
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------

namespace Opsive.UltimateInventorySystem.Utility
{
    using Opsive.Shared.Utility;
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;

    public static class IEnumerableHelper
    {
        public static IEnumerable<T> DepthFirstTreeTraversal<T>(Stack<T> stack, T root, bool includeRoot, Func<T, ResizableArray<T>> getChildren)
        {
            stack.Push(root);
            while (stack.Count != 0) {
                var current = stack.Pop();

                var children = getChildren(current);
                for (var i = 0; i < children.Count; i++) { stack.Push(children[i]); }

                if (!includeRoot) {
                    includeRoot = true;
                    continue;
                }

                yield return current;
            }
        }

        public static IEnumerable<T> DepthFirstTreeTraversal<T>(Stack<T> stack, T root, bool includeRoot, Func<T, IEnumerable<T>> getChildren)
        {
            stack.Push(root);
            while (stack.Count != 0) {
                var current = stack.Pop();

                var children = getChildren(current);
                if (children != null) {
                    foreach (var child in children) {
                        stack.Push(child);
                    }
                }

                if (!includeRoot) {
                    includeRoot = true;
                    continue;
                }

                yield return current;
            }
        }

        public static IEnumerable<T> BreadthFirstTreeTraversal<T>(ConcurrentQueue<T> queue, T root, bool includeRoot, Func<T, T[]> getChildren)
        {
            queue.Enqueue(root);

            while (queue.TryDequeue(out var current)) {

                var children = getChildren(current);
                if (children != null) {
                    for (var i = 0; i < children.Length; ++i) // use for instead of foreach to avoid enumerator creation
                    {
                        queue.Enqueue(children[i]);
                    }
                }

                if (!includeRoot) {
                    includeRoot = true;
                    continue;
                }

                yield return current;
            }
        }
    }

}

