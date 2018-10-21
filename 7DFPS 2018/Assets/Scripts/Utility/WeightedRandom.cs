using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class WeightedRandom<T>
{
    public static T GetRandom(IEnumerable<Item> collection)
    {
        int totalWeight = GetTotalWeight(collection);
        int randomValue = Random.Range(0, totalWeight - 1);
        return GetRandom(collection, randomValue);
    }

    public static T GetRandom(IEnumerable<Item> collection, int value)
    {
        foreach(Item item in collection)
        {
            if (value < item.weight)
                return item.item;
            else
                value -= item.weight;
        }
        return default(T);
    }

    public static int GetTotalWeight(IEnumerable<Item> collection)
    {
        int totalWeight = 0;
        foreach (Item item in collection)
            totalWeight += item.weight;
        return totalWeight;
    }

    public class Item
    {
        public T item;
        public int weight;

        public Item(T item, int weight)
        {
            this.item = item;
            this.weight = weight;
        }
    }
}