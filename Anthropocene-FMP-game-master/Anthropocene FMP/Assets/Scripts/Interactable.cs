using UnityEngine;

public class Interactable : MonoBehaviour
{
    public InteractType interactType;
    public Item item; 
    public Dialogue dialogue; 
    public Item itemNeeded;

    PlayerManager playerManager;
    bool canTrigger = false;
    public bool isConsumed = true;
    public bool isDisabled = true;

    bool hasInteracted = false; //dialogue only

    private void Start()
    {
        playerManager = FindObjectOfType<PlayerManager>();
    }
    public void interact()
    {
        if (itemNeeded != null)
        {
            foreach (Item item in playerManager.inventory)
            {
                if (item == itemNeeded)
                {
                    canTrigger = true;
                }
            }
        }
        else { canTrigger = true; }

        switch(interactType)
        {
            case InteractType.Item:
                if(item != null && canTrigger)
                {
                    bool isInInventory = false;
                    foreach(Item itemInInventory in playerManager.inventory)
                    {
                        if(itemInInventory == item)
                        {
                            item.quantity += 1;
                            isInInventory = true;
                        }
                    }
                    if (!isInInventory)
                    {
                        playerManager.inventory.Add(item);
                        if (item.quantity <= 0)
                        {
                            item.quantity = 1;
                        }
                    }

                    playerManager.CalculateWeight();
                    playerManager.AddLog("Picked up: " + item.itemName);
                    if (isConsumed)
                    {
                        Destroy(gameObject);
                    }
                    else
                    {
                        if(isDisabled)
                            gameObject.SetActive(false);
                    }
                }
                break;
            case InteractType.Dialogue:
                if (!dialogue.isActivated)
                {
                    TriggerDialogue();
                    dialogue.isActivated = true;
                    hasInteracted = true;
                }
                else
                    FindObjectOfType<DialogueManager>().DisplayNextSentence();
                break;
            case InteractType.Crafting:
                //crafting
                break;
            case InteractType.Button:
                //button
                break;
        }
    }

    void TriggerDialogue()
    {
        FindObjectOfType<DialogueManager>().StartDialogue(dialogue);
    }

    private void OnBecameInvisible()
    {
        if(hasInteracted)
        {
            Destroy(this.gameObject);
        }
    }
}

public enum InteractType
{
    Item,
    Dialogue,
    Crafting,
    Button,
    //different things that interactables can do
}
