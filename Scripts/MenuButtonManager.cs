using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class MenuButtonManager : MonoBehaviour
{
    [Header("Starg game Button")]
    public List<Button> turnOffButtons;

    public List<Button> turnOnButtons;

    [Header("Creator button")]
    public List<GameObject> turnOffButtons2;
    public List<GameObject> turnOnButtons2;

    public void SwitchButtons()
    {
        foreach (Button button in turnOffButtons)
        {
            button.gameObject.SetActive(false);
        }

        foreach (Button button in turnOnButtons)
        {
            button.gameObject.SetActive(true);
        }
    }

    public void SwitchButtonsCreators()
    {
        foreach (var button in turnOffButtons2)
        {
            button.gameObject.SetActive(false);
        }

        foreach (var button in turnOnButtons2)
        {
            button.gameObject.SetActive(true);
        }
    }

    public void ExitCreators()
    {
        foreach (var button in turnOffButtons2)
        {
            button.gameObject.SetActive(true);
        }

        foreach (var button in turnOnButtons2)
        {
            button.gameObject.SetActive(false);
        }
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}