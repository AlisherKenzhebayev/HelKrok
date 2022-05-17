using UnityEngine;

[CreateAssetMenu(fileName = "Key", menuName = "Inventory System/Items/Key Items")]
public class KeyItemObject : BaseItemObject
{
    public void Awake()
    {
        type = ItemType.Key;
        tagName = "key";
    }
}
