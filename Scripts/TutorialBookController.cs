using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class TutorialBookController : MonoBehaviour
{
    [Header("References")]
    public Transform cameraTransform;
    public Transform book;
    public Button tutorialButton;
    public GameMenuController gameMenuController;
    public GameObject[] uiDisableList;

    [Header("Positions")]
    public Vector3 highoffset;
    public Vector3 lowoffset;

    [Header("Animation")]
    public float moveDuration = 0.5f;
    public AnimationCurve easeCurve;

    [Header("Tutorial UI")]
    public GameObject page1;
    public GameObject page2;
    public GameObject tutorialMovement;
    public GameObject tutorilaBuildings;
    public GameObject tutorialUnits;
    public GameObject tutorialUpgrades;
    public GameObject tutorialGold;
    public GameObject tutorialWood;
    public GameObject tutorialSupply;
    public GameObject tutorialEnemies;


    private bool isOpen = false;
    private Coroutine currentRoutine;

    #region Book Toggles

    public void ToggleBook()
    {
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        if (!isOpen)
        {
            currentRoutine = StartCoroutine(ShowBook());
            tutorialButton.GetComponentInChildren<TMP_Text>().text = "Exit";
        }
        else
        {
            currentRoutine = StartCoroutine(HideBook());
            tutorialButton.GetComponentInChildren<TMP_Text>().text = "Tutorials";
        }

        isOpen = !isOpen;
    }

    private IEnumerator ShowBook()
    {
        book.gameObject.SetActive(true);
        cameraTransform.gameObject.GetComponentInParent<Cameracontroll>().enabled = false;
        foreach (GameObject obj in uiDisableList)
        {
            obj.SetActive(false);
        }

        Vector3 camPos = cameraTransform.position;

        Vector3 startPos = camPos + highoffset;
        Vector3 endPos = camPos + lowoffset;

        float time = 0f;

        while (time < moveDuration)
        {
            float t = time / moveDuration;
            float eased = easeCurve.Evaluate(t);

            book.position = Vector3.Lerp(startPos, endPos, eased);

            time += Time.deltaTime;
            yield return null;
        }

        book.position = endPos;
        page1.SetActive(true);
        gameMenuController.PauseGame(false);
    }

    private IEnumerator HideBook()
    {
        gameMenuController.ResumeGame(false);
        if (page1.activeSelf)
            page1.SetActive(false);
        if (page2.activeSelf)
            page2.SetActive(false);
        Vector3 camPos = cameraTransform.position;

        Vector3 startPos = book.position;
        Vector3 endPos = camPos + highoffset;

        float time = 0f;

        while (time < moveDuration)
        {
            float t = time / moveDuration;
            float eased = easeCurve.Evaluate(t);

            book.position = Vector3.Lerp(startPos, endPos, eased);

            time += Time.deltaTime;
            yield return null;
        }

        book.position = endPos;
        book.gameObject.SetActive(false);
        cameraTransform.gameObject.GetComponentInParent<Cameracontroll>().enabled = true;
        foreach (GameObject obj in uiDisableList)
        {
            obj.SetActive(true);
        }
    }

    #endregion

    #region Tutorial UI management

    public void TogglePage1()
    {
        page1.SetActive(true);
        page2.SetActive(false);
    }

    public void TogglePage2()
    {
        page1.SetActive(false);
        page2.SetActive(true);
    }

    public void BackToPage1(GameObject activepage)
    {
        activepage.SetActive(false);
        page1.SetActive(true);
    }

    public void BackToPage2(GameObject activepage)
    {
        activepage.SetActive(false);
        page2.SetActive(true);
    }

    public void TurnOnMovementTutorial()
    {
        page1.SetActive(false);
        tutorialMovement.SetActive(true);
    }

    public void TurnOnBuildingsTutorial()
    {
        page1.SetActive(false);
        tutorilaBuildings.SetActive(true);
    }

    public void TurnOnUnitsTutorial()
    {
        page1.SetActive(false);
        tutorialUnits.SetActive(true);
    }

    public void TurnOnUpgradesTutorial()
    {
        page1.SetActive(false);
        tutorialUpgrades.SetActive(true);
    }

    public void TurnOnGoldTutorial()
    {
        page2.SetActive(false);
        tutorialGold.SetActive(true);
    }

    public void TurnOnWoodTutorial()
    {
        page2.SetActive(false);
        tutorialWood.SetActive(true);
    }

    public void TurnOnSupplyTutorial()
    {
        page2.SetActive(false);
        tutorialSupply.SetActive(true);
    }

    public void TurnOnEnemiesTutorial()
    {
        page2.SetActive(false);
        tutorialEnemies.SetActive(true);
    }

    #endregion
}