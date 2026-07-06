using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TooltipTrigger : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler
{
    public GameObject tooltipObject;
    public GameObject maxsupplytooltipObject;
    public TextMeshProUGUI goldCostText;
    public TextMeshProUGUI woodCostText;
    public TextMeshProUGUI supplyCostText;

    private BuySlot buySlot;
    private bool isVisible;
    private Color goldOriginalColor;
    private Color woodOriginalColor;
    private Color supplyOriginalColor;

    void Awake()
    {
        buySlot = gameObject.GetComponent<BuySlot>();
    }

    void Start()
    {
        goldOriginalColor = goldCostText.GetComponent<TextMeshProUGUI>().color;
        woodOriginalColor = woodCostText.GetComponent<TextMeshProUGUI>().color;
        if (supplyCostText != null)
        {
            supplyOriginalColor = supplyCostText.GetComponent<TextMeshProUGUI>().color;
        }
    }

    void Update()
    {
        if (isVisible)
        {
            Refresh();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isVisible = true;
        tooltipObject.SetActive(true);
        Refresh();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isVisible = false;
        tooltipObject.SetActive(false);
        if (maxsupplytooltipObject != null)
        {
            maxsupplytooltipObject.SetActive(false);
        }
        ResetColors();
    }

    private void Refresh()
    {
        ResetColors();

        if (PopulationManagement.Instance.MaxPopultionReached() && maxsupplytooltipObject != null)
        {
            tooltipObject.SetActive(false);
            maxsupplytooltipObject.SetActive(true);
            return;
        }

        if (!buySlot.HasEnoughResources())
        {
            goldCostText.color = Color.red;
            woodCostText.color = Color.red;
        }

        if (supplyCostText != null && !buySlot.HasEnoughPopulation())
        {
            supplyCostText.color = Color.red;
        }
    }

    private void ResetColors()
    {
        goldCostText.color = goldOriginalColor;
        woodCostText.color = woodOriginalColor;

        if (supplyCostText != null)
            supplyCostText.color = supplyOriginalColor;
    }
}