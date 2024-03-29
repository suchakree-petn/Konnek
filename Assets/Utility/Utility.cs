using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Tutor.Utility
{
    public class Utility
    {
        public static Vector2 RandomPosition2D(Vector2 center, float randomPositionRadius)
        {
            // Generate a random angle in radians
            float randomAngle = Random.Range(0f, 2 * Mathf.PI);

            // Calculate a random position within the spawn radius
            Vector3 spawnPosition = center + new Vector2(Mathf.Cos(randomAngle), Mathf.Sin(randomAngle)) * Random.Range(0, randomPositionRadius);

            return spawnPosition;
        }
        public static Vector2 GetCurrentMousePosition()
        {
            return Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        }
    }
    public class WeightedRandom<T>
    {
        private List<WeightedItem<T>> weightedItems = new List<WeightedItem<T>>();

        public void AddItem(T item, float weight)
        {
            weightedItems.Add(new WeightedItem<T> { Item = item, Weight = weight });
        }

        public T GetRandom()
        {
            float totalWeight = 0;
            foreach (var item in weightedItems)
            {
                totalWeight += item.Weight;
            }

            float randomValue = Random.Range(0, totalWeight);

            foreach (var item in weightedItems)
            {
                if (randomValue < item.Weight)
                {
                    return item.Item;
                }
                randomValue -= item.Weight;
            }

            // Fallback: If something went wrong, return the last item.
            return weightedItems[weightedItems.Count - 1].Item;
        }
    }

    public struct WeightedItem<T>
    {
        public T Item { get; set; }
        public float Weight { get; set; }
    }
}

