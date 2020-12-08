using System.Text;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraftingUI : MonoBehaviour
{
    public Text recipeListUI, recipeTitleUI, recipeDetailUI;
    public List<CraftingRecipe> recipes;

    int positionInList;

    PlayerManager playerManager;
    CraftingRecipe recipeSelected;

    private void OnEnable()
    {
        positionInList = 0;
        playerManager = FindObjectOfType<PlayerManager>();
        playerManager.uISelection = true;
        UpdateUI();
    }

    private void Update()
    {
        if (!playerManager.uISelection)
        {
            recipeListUI.color = recipeTitleUI.color = recipeDetailUI.color = Color.black;
            //getting scroll input
            var up = Input.GetKeyDown(KeyCode.W);
            var down = Input.GetKeyDown(KeyCode.S);
            var eDown = Input.GetKeyDown(KeyCode.E);

            //adjusting UI accordingly
            if (up && recipes.Count > 1)
            {
                positionInList -= 1;
                if (positionInList < 0) { positionInList = recipes.Count - 1; }
                UpdateUI();
            }
            if (down && recipes.Count > 1)
            {
                positionInList += 1;
                if (positionInList > recipes.Count - 1) { positionInList = 0; }
                UpdateUI();
            }
            if (eDown)
            {
                bool isCraftable = true;

                foreach(Item requiredItem in recipeSelected.requiredItems)
                {
                    Item item = playerManager.inventory.Find(x => x.itemName == requiredItem.itemName);
                    if(item == null || item.quantity < recipeSelected.quantityOfRequredItem)
                    {
                        isCraftable = false;
                        Debug.Log("missing items or incorrect quantity.");
                    }
                }
                if (isCraftable)
                {
                    recipeSelected.Craft(playerManager);
                    FindObjectOfType<InventoryUI>().UpdateUI();
                }
            }
        }
        else
        {
            recipeListUI.color = recipeTitleUI.color = recipeDetailUI.color = Color.grey;
        }
    }

    void UpdateUI()
    {
        recipeListUI.text = "";

        for (int i = 0; i < recipes.Count; i++)
        {
            if (positionInList == i) //display selected items info
            {
                StringBuilder detailOutput = new StringBuilder();
                recipeSelected = recipes[i];
                recipeListUI.text += ">> ";

                detailOutput.Append("RESULT: " + recipes[i].craftedItem.itemName + "\n\n");
                detailOutput.Append("INGREDIENTS: \n");

                foreach(Item item in recipeSelected.requiredItems)
                {
                    detailOutput.Append(item.itemName + " " + "(x" + recipeSelected.quantityOfRequredItem + ")" + "\n");
                }

                detailOutput.Append("\nPRESS 'E' TO CRAFT...");

                recipeDetailUI.text = detailOutput.ToString();
            }
            StringBuilder name = new StringBuilder();
            name.Append(recipes[i].craftedItem.itemName + "\n"); //display item name

            recipeListUI.text += name.ToString();
        }
    }
}

[System.Serializable]
public struct CraftingRecipe
{
    public Item[] requiredItems;
    public int quantityOfRequredItem;
    public Item craftedItem;

    public void Craft(PlayerManager pm)
    {
        List<Item> itemsToRemove = new List<Item>();

        foreach(Item inventoryItem in pm.inventory)
        {
            foreach(Item requiredItem in requiredItems)
            {
                if(requiredItem == inventoryItem)
                {
                    inventoryItem.quantity -= quantityOfRequredItem;
                    if(inventoryItem.quantity <= 0)
                    {
                        itemsToRemove.Add(inventoryItem);
                    }
                }
            }
        }
        for (int i = 0; i < itemsToRemove.Count; i++)
        {
            pm.inventory.Remove(itemsToRemove[i]);
        }
        
        if (craftedItem.quantity <= 0)
        {
            pm.inventory.Add(craftedItem);
            craftedItem.quantity = 1;
        }
        else if (craftedItem.quantity > 0)
        {
            craftedItem.quantity += 1;
        }
        pm.CalculateWeight();
    }
}
