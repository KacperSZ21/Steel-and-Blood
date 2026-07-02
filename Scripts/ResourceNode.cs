using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.VFX;

public class ResourceNode : MonoBehaviour
{
    public float resources = 100f; //Max amount
    public float depletionRate = 1f;
    public bool IsDepleted => resources <= 0;
    public ResourceType resourceType;
    public enum DepletionVisualMode
    {
        None,
        ScaleModel,
        ToggleChildren
    }
    public DepletionVisualMode visualMode = DepletionVisualMode.None;
    public Transform modelToScale;
    public List<GameObject> childrenToToggle = new();


    private float maxResouerce;


    private void Awake()
    {
        maxResouerce = resources;
    }

    public void Harvest(float amount)
    {
        if (!IsDepleted)
        {
            resources -= amount;
            resources = Mathf.Max(resources, 0);
            UpdateVisuals();
        }

        if (IsDepleted)
        {
            Destroy(gameObject);
        }
    }

    private void UpdateVisuals()
    {
        float percent = Mathf.Clamp01(resources / maxResouerce);
        switch (visualMode)
        {
            case DepletionVisualMode.ScaleModel:
                if (modelToScale != null)
                {
                    float scale = Mathf.Lerp(0f, 1f, percent);
                    modelToScale.localScale = new Vector3(scale, scale, scale);
                }
                break;
            case DepletionVisualMode.ToggleChildren:
                int total = childrenToToggle.Count;
                int visibleCount = Mathf.CeilToInt(total * percent);
                for (int i = 0; i < total; i++)
                {
                    childrenToToggle[i].SetActive(i < visibleCount);
                }
                break;
            case DepletionVisualMode.None:
            default:
                // Do nothing
                break;
        }
    }
}