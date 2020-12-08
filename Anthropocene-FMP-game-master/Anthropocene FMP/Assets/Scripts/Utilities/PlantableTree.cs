using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantableTree : DynamicMovement
{
    public Sprite[] treeSpritePhases;
    public int requiredTimeBetweenPhases = 3; //minutes
    int currentPhase = 0;  
    DateTime startTime = DateTime.UtcNow;
    PlayerManager playerManager;

    private void OnBecameVisible()
    {
        if (playerManager == null)
        {
            playerManager = FindObjectOfType<PlayerManager>();
        }
        growUp();
    }

    private void OnBecameInvisible()
    {
        growUp();
    }

    void growUp()
    {
        if (currentPhase + 1 < treeSpritePhases.Length)
        {
            DateTime currentTime = DateTime.UtcNow;
            if ((currentTime - startTime).TotalMinutes >= requiredTimeBetweenPhases)
            {
                currentPhase++;
                GetComponent<SpriteRenderer>().sprite = treeSpritePhases[currentPhase];
                updateSortOrder();
                startTime.AddMinutes(requiredTimeBetweenPhases);
            }
        }
    }

    private void Update()
    {
        if (Vector2.Distance(playerManager.transform.position, transform.position) < 1)
        {
            playerManager.oxygen += playerManager.rateOfOxygenDecrease * Time.deltaTime;
            }
    }
}
