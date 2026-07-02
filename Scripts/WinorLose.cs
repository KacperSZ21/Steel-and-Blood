using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinorLose : MonoBehaviour
{
    public GameObject winImage;
    public GameObject loseImage;
    public List<GameObject> exisitngEnemyBuildings;
    public List<GameObject> existingPlayerBuilding;

    private GameObject view;
    private GameObject panel;
    private GameObject unitsPanel;
    private GameObject minimapPanel;
    private GameObject woodUI;
    private GameObject goldUI;
    private GameObject populationUI;

    // Start is called before the first frame update
    void Start()
    {
        view = GameObject.Find("CameraController");
        panel = GameObject.Find("Sidepanel");
        unitsPanel = GameObject.Find("UnitsPanel");
        minimapPanel = GameObject.Find("MinimapPanel");
        woodUI = GameObject.Find("WoodUIBackground");
        goldUI = GameObject.Find("GoldUIBackground");
        populationUI = GameObject.Find("SupplyUIBackground");
    }

    // Update is called once per frame
    void Update()
    {
        exisitngEnemyBuildings = GameObject.FindGameObjectsWithTag("EnemyBuilding").ToList();
        existingPlayerBuilding = GameObject.FindGameObjectsWithTag("Building").ToList();
        if (UnitSelectionManager.Instance.allUnitsList.Count <= 0 && AllNull(existingPlayerBuilding))
        {
            Cameracontroll.Instance.ResetCameraCursor();
            UnitSelectionManager.Instance.DeselectAll();
            UnitSelectionManager.Instance.techPanel.SetActive(false);
            UnitSelectionManager.Instance.magePanel.SetActive(false);
            loseImage.SetActive(true);
            var script = view.GetComponent<Cameracontroll>();
            script.enabled = false;
            panel.SetActive(false);
            unitsPanel.SetActive(false);
            minimapPanel.SetActive(false);
            if (woodUI != null && goldUI != null && populationUI != null)
            {
                woodUI.SetActive(false);
                goldUI.SetActive(false);
                populationUI.SetActive(false);
            }
        }

        if (AllNull(exisitngEnemyBuildings))
        {
            Cameracontroll.Instance.ResetCameraCursor();
            UnitSelectionManager.Instance.DeselectAll();
            UnitSelectionManager.Instance.techPanel.SetActive(false);
            UnitSelectionManager.Instance.magePanel.SetActive(false);
            winImage.SetActive(true);
            var script = view.GetComponent<Cameracontroll>();
            script.enabled = false;
            panel.SetActive(false);
            unitsPanel.SetActive(false);
            minimapPanel.SetActive(false);
            if (woodUI != null && goldUI != null && populationUI != null)
            {
                woodUI.SetActive(false);
                goldUI.SetActive(false);
                populationUI.SetActive(false);
            }
        }
    }

    private bool AllNull(List<GameObject> list)
    {
        return list.All(item => item == null);
    }

    public void RestartWinORLosemanager()
    {
        exisitngEnemyBuildings.Clear();
        existingPlayerBuilding.Clear();
    }
}
