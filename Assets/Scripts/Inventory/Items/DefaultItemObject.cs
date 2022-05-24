using UnityEngine;

[CreateAssetMenu(fileName = "Default", menuName = "Inventory System/Items/Default Items")]
public class DefaultItemObject : BaseItemObject
{
    public void Awake()
    {
        type = ItemType.Default;
        tagName = "default";
    }
}
