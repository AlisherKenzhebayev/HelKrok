using UnityEngine;

[CreateAssetMenu(fileName = "Hp Potion", menuName = "Inventory System/Consumables/Default Hp Potion")]
public class DefaultHpConsumableItemObject : BaseConsumableItemObject
{
    public float restoreAmount = 5f;

    internal GameObject player;
    internal DamageTaker playerDT;

    internal override void Awake()
    {
        base.Awake();
        this.tagName = "hpPotion";
    }

    public override void Execute() {

        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }

        if (playerDT == null)
        {
            playerDT = player.GetComponentInChildren<DamageTaker>();
        }

        playerDT.RestoreFlat(Mathf.FloorToInt(restoreAmount));
    }
}
