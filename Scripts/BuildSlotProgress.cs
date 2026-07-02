using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class BuildSlotProgress : MonoBehaviour
{
    [SerializeField]
    private UnityEngine.UI.Image progressImage;
    private float buildTime;
    private float startTime;
    private bool isBuilding;

    public void StartBuilding(float time)
    {
        buildTime = time;
        startTime = Time.time;
        isBuilding = true;
        progressImage.fillAmount = 1f;
        progressImage.enabled = true;

        gameObject.GetComponentInParent<Button>().enabled = false;

        if (GetComponent<TooltipTrigger>() != null)
        {
            gameObject.GetComponentInParent<TooltipTrigger>().tooltipObject.SetActive(false);
            gameObject.GetComponentInParent<TooltipTrigger>().enabled = false;
        }
    }

    private void Update()
    {
        if (!isBuilding) return;

        float elapsed = Time.time - startTime;
        float progress = elapsed / buildTime;
        progressImage.fillAmount = Mathf.Clamp01(1f - progress); // countdown form 1 to 0

        if (elapsed >= buildTime)
        {
            isBuilding = false;
            progressImage.fillAmount = 0f;
            progressImage.enabled = false;
            OnBuildComplete();
        }
    }

    private void OnBuildComplete()
    {
        gameObject.GetComponentInParent<Button>().enabled = true;

        if (GetComponent<TooltipTrigger>() != null)
        {
            gameObject.GetComponentInParent<TooltipTrigger>().enabled = true;
        }
    }
}