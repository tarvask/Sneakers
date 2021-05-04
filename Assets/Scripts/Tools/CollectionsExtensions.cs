using System.Collections.Generic;
using UnityEngine;

namespace Sneakers
{
    public static class CollectionsExtensions
    {
        public static void ShuffleList<T>(ref List<T> candidates)
        {
            int maxIndex = candidates.Count;
            
            for (int i = 0; i < candidates.Count; i++)
            {
                T temp = candidates[i];
                int randomIndex = Random.Range(0, maxIndex);
                candidates[i] = candidates[randomIndex];
                candidates[randomIndex] = temp;
            }
        }
    }
}