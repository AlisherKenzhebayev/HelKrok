using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class UIAbilityImageReplacement : MonoBehaviour
{
    private static GameObject player;
    private static PlayerController playerController;
    private static Inventory playerInventory;

    private Image abilityImage;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("Error - no Player tag exists");
        }

        playerController = player.GetComponent<PlayerController>();
        if (playerController == null)
        {
            Debug.LogError("Error - no PlayerController component exists");
        }

        playerInventory = player.GetComponentInChildren<Inventory>();
        if (playerInventory == null)
        {
            Debug.LogError("Error - no Inventory child component exists");
        }

        abilityImage = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        abilityImage.sprite = playerInventory.CurrentAbility().item.prefabUI.transform.GetChild(0).GetComponentInChildren<Image>().sprite;
    }
}
