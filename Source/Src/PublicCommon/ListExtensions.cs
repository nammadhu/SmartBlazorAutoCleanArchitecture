namespace PublicCommon;

public static class ListExtensions
{
    public static bool AreListsEqualIgnoringOrder<T>(List<T>? list1, List<T>? list2, IEqualityComparer<T>? comparer = null)
    {
        if ((list1 == null && list2 == null) || (list1?.Count == 0 && list2?.Count == 0)) return true;
        if (list1 == null || list2 == null) return false; // Handle null cases

        if (list1.Count != list2.Count) return false; // Different lengths, cannot be equal

        comparer ??= EqualityComparer<T>.Default;

        var set1 = new HashSet<T>(list1, comparer);
        var set2 = new HashSet<T>(list2, comparer);

        return set1.SetEquals(set2);
    }

    private static void ReplaceItemsByDynamicKey<T>(this List<T> allItems, List<T> myItems, string keyName)
    {
        if (allItems == null || myItems == null)
        {
            throw new ArgumentNullException(allItems == null ? nameof(allItems) : nameof(myItems));
        }

        if (string.IsNullOrEmpty(keyName))
        {
            throw new ArgumentException("keyName cannot be null or empty.");
        }

        for (int i = 0; i < allItems.Count; i++)
        {
            T allItem = allItems[i];
            var allItemValue = typeof(T).GetProperty(keyName)?.GetValue(allItem);

            T matchingMyItem = myItems.FirstOrDefault(myItem =>
                myItem != null &&
                myItem.GetType().GetProperty(keyName)?.GetValue(myItem) == allItemValue);

            if (matchingMyItem != null)
            {
                allItems[i] = matchingMyItem;
            }
        }
    }

    /// <summary>
    /// List<Product> allProducts = new List<Product>() { ... };
    ///List<Product> myUpdatedProducts = new List<Product>() { ... };
    ///allProducts.ReplaceItemsByProperty(myUpdatedProducts, item => item.IdCard); // Or item.IdVerifiedCard
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="allItems"></param>
    /// <param name="myItems"></param>
    /// <param name="keySelector"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public static List<T> ReplaceItemsByProperty<T>(this List<T> allItems, List<T>? myItems, Func<T, object> keySelector)
    {
        if (myItems == null || !myItems.Any()) return allItems;
        if (allItems == null || !allItems.Any()) return new List<T>();
        if (keySelector == null) throw new ArgumentNullException(nameof(keySelector));

        var myItemsDictionary = myItems.Where(item => item != null).ToDictionary(keySelector, item => item);

        for (int i = 0; i < allItems.Count; i++)
        {
            var key = keySelector(allItems[i]);
            if (myItemsDictionary.ContainsKey(key))
            {
                allItems[i] = myItemsDictionary[key];
            }
        }

        return allItems;
    }

    /// <summary>
    /// make sure item is from the same list not any other response
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <param name="updatedItem"></param>
    /// <param name="updateAction"></param>
    public static void UpdateAndMoveToFront<T>(List<T> list, T updatedItem, Action<T> updateAction)
    {
        int index = list.IndexOf(updatedItem); // Find the index of the updated item
        if (index != -1)
        {
            updateAction(updatedItem); // Update the item at the found index

            // Remove the updated item from its original position
            list.RemoveAt(index);

            // Insert the updated item at the front
            list.Insert(0, updatedItem);
        }
    }

    public static void UpdateAndMoveToFront<T>(List<T> list, int itemIndex, Action<T> updateAction)
    {
        if (itemIndex < 0 || itemIndex >= list.Count)
        {
            throw new ArgumentOutOfRangeException(nameof(itemIndex));
        }

        T item = list[itemIndex];
        updateAction(item); // Update the item

        list.RemoveAt(itemIndex);
        list.Insert(0, item);
    }

    public static bool HasData<T>(this IEnumerable<T>? list)
    {
        //if(T is String)//make sure this is not called by any string //todo had to bypass
        return list != null && list.Any();
    }

    public static bool IsEmptyList<T>(this IEnumerable<T>? list)
    {
        return list == null || !list.Any();
    }

    public static List<T> MoveItemToTopById<T>(this List<T> list, T? item, Func<T, int> getId)
    {
        if (item == null) return list;

        if (list == null)
            return [item];

        int targetId = getId(item);

        // Remove existing occurrences of the item (if any)
        if (list.RemoveAll(x => getId(x) == targetId) > 0)
        {
            // Insert the item at the top
            list.Insert(0, item);
        }
        return list;
    }
}