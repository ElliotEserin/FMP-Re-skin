using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PostProcess : MonoBehaviour
{
    public PlayerManager playerManager;
    public PostProcessVolume vol;
    ChromaticAberration healthGraphic;

    private void Start()
    {
        if(vol.profile.TryGetSettings(out healthGraphic))
            InvokeRepeating("UpdatePostProcess", 0.5f, 0.25f);
    }

    void UpdatePostProcess()
    {
        healthGraphic.intensity.value = 1 - (playerManager.currentPlayerHealth/100);
    }
}
