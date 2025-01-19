using System;
using System.Collections.Generic;
using System.Linq;

namespace HIEU_NL.Utilities {
    public static class ArrayExtensions {
        static Random rng;
        
        /// <summary>
        /// Determines whether a collection is null or has no elements
        /// without having to enumerate the entire collection to get a count.
        ///
        /// Uses LINQ's Any() method to determine if the collection is empty,
        /// so there is some GC overhead.
        /// </summary>
        public static bool IsNullOrEmpty<T>(this T[] array) {
            return array == null || !array.Any();
        }
        
        /// <summary>
        /// Check array is not valid
        /// </summary>
        public static bool IsValid<T>(this T[] array) {
            return array != null && array.Any();
        }

        /// <summary>
        /// Creates a new array that is a copy of the original array.
        /// </summary>
        public static T[] Clone<T>(this T[] array) {
            List<T> newList = new List<T>();
            foreach (T item in array) {
                newList.Add(item);
            }

            return newList.ToArray();
        }

        /// <summary>
        /// Swaps two elements in the array at the specified indices.
        /// </summary>
        public static void Swap<T>(this T[] array, int indexA, int indexB) {
            (array[indexA], array[indexB]) = (array[indexB], array[indexA]);
        }

        /// <summary>
        /// Shuffles the elements in the array using the Durstenfeld implementation of the Fisher-Yates algorithm.
        /// This method modifies the input array in-place, ensuring each permutation is equally likely, and returns the array for method chaining.
        /// </summary>
        public static T[] Shuffle<T>(this T[] array) {
            if (rng == null) rng = new Random();
            int count = array.Length;
            while (count > 1) {
                --count;
                int index = rng.Next(count + 1);
                (array[index], array[count]) = (array[count], array[index]);
            }
            return array;
        }

        /// <summary>
        /// Filters a collection based on a predicate and returns a new array
        /// containing the elements that match the specified condition.
        /// </summary>
        public static T[] Filter<T>(this T[] source, Predicate<T> predicate) {
            List<T> newList = new List<T>();
            foreach (T item in source) {
                if (predicate(item)) {
                    newList.Add(item);
                }
            }
            return newList.ToArray();
        }
    }
}
