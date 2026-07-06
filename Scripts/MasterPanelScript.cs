using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class MasterPanelScript : MonoBehaviour
{
    public GameObject masterPanel;
    public WinorLose winorLoseScript;
    public GameMenuController gameMenuController;
    public TextMeshProUGUI unitsSelectedText;
    public TextMeshProUGUI enemyBuildingsText;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F6))
        {
            gameMenuController.PauseGame();
            unitsSelectedText.text = UnitSelectionManager.Instance.unitsSelected.Count.ToString();
            enemyBuildingsText.text = winorLoseScript.exisitngEnemyBuildings.Count.ToString();
            masterPanel.SetActive(true);
            UnitSelectionManager.Instance.DeselectAll();
        }
    }
}
