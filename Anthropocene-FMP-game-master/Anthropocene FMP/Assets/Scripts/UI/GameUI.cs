using System;
using UnityEngine;
using TMPro;

public class GameUI : MonoBehaviour
{
    public TextMeshProUGUI oxygenText, foodText, waterText, healthText, infoText, commandText;

    PlayerManager pm;

    private void Start()
    {
        pm = FindObjectOfType<PlayerManager>();
    }

    private void Update()
    {
        oxygenText.text = "Oxygen: " + Math.Round(pm.oxygen);
        foodText.text = "Food: " + Math.Round(pm.food);
        waterText.text = "Water: " + Math.Round(pm.water);
        healthText.text = "HEALTH: " + Math.Round((pm.currentPlayerHealth/pm.maxPlayerHealth) * 100);
    }
}
