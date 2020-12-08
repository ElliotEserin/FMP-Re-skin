using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Workbench : MonoBehaviour
{
    public List<CraftingRecipe> workbenchRecipes;
    public CraftingUI craftingUI;
    public PlayerManager playerManager;

    private void Start()
    {
        playerManager = FindObjectOfType<PlayerManager>();
        GetComponent<Renderer>().sortingOrder = (int)(gameObject.transform.position.y * -100);
        craftingUI = playerManager.craftingUI;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Entered");
        foreach (CraftingRecipe recipe in workbenchRecipes)
        {
            if (!craftingUI.recipes.Contains(recipe))
            {
                craftingUI.recipes.Add(recipe);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        foreach (CraftingRecipe recipe in workbenchRecipes)
        {
            if (craftingUI.recipes.Contains(recipe))
            {
                craftingUI.recipes.Remove(recipe);
            }
        }
    }
}
