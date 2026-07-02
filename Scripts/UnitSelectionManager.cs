using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UnitSelectionManager : MonoBehaviour
{
    public static UnitSelectionManager Instance { get; set; }

    public List<GameObject> allUnitsList = new();
    public List<GameObject> unitsSelected = new();
    public LayerMask clickable;
    public LayerMask ground;
    public LayerMask attackable;
    public LayerMask constructablelayer;
    public LayerMask node;
    public LayerMask unavailable;
    public LayerMask cursorPriority;
    public GameObject groundMarker;
    public GameObject techPanel;
    public GameObject magePanel;
    public bool attackCurosorvisible;

    private Camera cam;
    private List<GameObject> unitsToRemeber = new();

    void Awake()
    {
        if (Instance != null & Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (IsPointerOverUI())// Click on the UI
            {
                return;
            }

            Ray ray = cam.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))//Click
            {
                if (hit.collider.TryGetComponent(out Unit unit))// on Unit
                {
                    if (Input.GetKey(KeyCode.LeftShift))
                    {
                        if (unit != null)
                        {
                            MultiSelect(unit.gameObject);
                        }
                    }
                    else
                    {
                        if (unit != null)
                        {
                            SelectByClicking(unit.gameObject);
                        }
                    }
                }
                else if (hit.collider.TryGetComponent(out Constructable constructable))// on building
                {
                    Transform indicator = hit.collider.transform.Find("Indicator");
                    if (constructable.buildingType == BuildingType.Technology)
                    {
                        DeselectAll();
                        techPanel.SetActive(true);
                        magePanel.SetActive(false);
                        if (indicator != null)
                        {
                            indicator.gameObject.SetActive(true);
                        }
                    }
                    else if (constructable.buildingType == BuildingType.MageTower)
                    {
                        DeselectAll();
                        techPanel.SetActive(false);
                        magePanel.SetActive(true);
                        if (indicator != null)
                        {
                            indicator.gameObject.SetActive(true);
                        }
                    }
                }
                else if (Physics.Raycast(ray, out hit, Mathf.Infinity, ground))
                {
                    if (!Input.GetKey(KeyCode.LeftShift))
                    {
                        DeselectAll();
                        techPanel.SetActive(false);
                        magePanel.SetActive(false);
                    }
                }
                else if (Physics.Raycast(ray, out hit, Mathf.Infinity, cursorPriority))
                {
                    DeselectAll();
                    techPanel.SetActive(false);
                    magePanel.SetActive(false);
                    return;
                }
            }
        }

        if (Input.GetMouseButtonDown(1) && unitsSelected.Count > 0)
        {
            RaycastHit hit;
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);

            // Click on object
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, ground))
            {
                groundMarker.transform.position = hit.point;
                groundMarker.SetActive(false);
                groundMarker.SetActive(true);
                foreach (GameObject unit in unitsSelected)
                {
                    if (unit == null) continue;

                    if (unit.TryGetComponent(out Attackcontroller attackController))
                    {
                        attackController.targettoAttack = null;
                        attackController.enemyBuilding = false;
                    }
                }
            }
        }

        //Attack target
        if (unitsSelected.Count > 0 && AtleastOneOffensiveUnit(unitsSelected))
        {
            RaycastHit hit;
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);

            // Click on object
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, attackable))
            {
                Debug.Log("Przecinwnik najechany kursorem");

                attackCurosorvisible = true;

                if (Input.GetMouseButtonDown(1))
                {
                    Transform target = hit.transform;

                    foreach (GameObject unit in unitsSelected)
                    {
                        if (unit.GetComponent<Attackcontroller>())
                        {
                            if (target.CompareTag("EnemyBuilding"))
                            {
                                unit.GetComponent<Attackcontroller>().enemyBuilding = true;
                                unit.GetComponent<Attackcontroller>().targettoAttack = target;
                            }
                            else
                            {
                                unit.GetComponent<Attackcontroller>().targettoAttack = target;
                            }
                        }
                    }
                }
            }
            else
            {
                attackCurosorvisible = false;
            }
        }


        //The Harvester, together with a group of several units
        if (unitsSelected.Count > 0 && OnlyHarvesterSelected())
        {
            RaycastHit hit;
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);

            // Click on object
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, ~0, QueryTriggerInteraction.Collide))
            {
                ResourceNode resourceNode = hit.transform.GetComponent<ResourceNode>();
                if (resourceNode != null)
                {
                    if (Input.GetMouseButton(1))
                    {
                        Transform node = hit.transform;

                        foreach (GameObject unit in unitsSelected)
                        {
                            Harvester harvester = unit.GetComponent<Harvester>();
                            if (harvester != null)
                            {
                                if (harvester.currentResourceType == ResourceType.None | harvester.currentResourceType == resourceNode.resourceType)
                                {
                                    harvester.assignedNode = node;
                                }
                                else
                                {
                                    UnitMovement unitMovement = harvester.GetComponent<UnitMovement>();
                                    unitMovement.allowManualInput = false;
                                    harvester.MoveTo(harvester.supplyCenter);
                                    StartCoroutine(ReenableManualInputNextFrame(unitMovement));
                                }
                            }
                        }
                    }
                }

            }
        }

        if (unitsSelected.Count > 0 && Input.GetKeyDown(KeyCode.C))
        {
            DeselectAll();
            techPanel.SetActive(false);
            magePanel.SetActive(false);
        }

        CursorSelector();
    }

    private bool IsPointerOverUI()
    {
        return EventSystem.current != null &&
            EventSystem.current.IsPointerOverGameObject();
    }

    private IEnumerator ReenableManualInputNextFrame(UnitMovement unitMovement)
    {
        yield return null;
        unitMovement.allowManualInput = true;
    }

    private bool OnlyHarvesterSelected() // Returns a false value if at least one unit in the group is not a harvester
    {
        if (unitsSelected.Count == 0) return false;

        foreach (GameObject unit in unitsSelected)
        {
            if (unit == null || unit.GetComponent<Harvester>() == null)
            {
                return false;
            }
        }

        return true;
    }

    private void CursorSelector()
    {
        if (TrySetSelectableCursor()) return;
        if (TrySetSellCursor()) return;
        if (TrySetAttackCursor()) return;
        if (TrySetGatheringCursor()) return;
        if (TrySetUnAvailableCursor()) return;
        if (TrySetWalkableCursor()) return;

        CursorManager.Instance.SetMarkerType(CursorManager.CursorType.None);
    }

    private bool RayHits(LayerMask mask, out RaycastHit hit)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        return Physics.Raycast(ray, out hit, Mathf.Infinity, mask);
    }

    private bool TrySetSelectableCursor()
    {
        if (RayHits(clickable, out _))
        {
            CursorManager.Instance.SetMarkerType(CursorManager.CursorType.Selectable);
            return true;
        }
        return false;
    }
    private bool TrySetSellCursor()
    {
        if (ResourceManager.Instance.placementSystem.inSellMode)
        {
            CursorManager.Instance.SetMarkerType(CursorManager.CursorType.SellCursor);
            return true;
        }
        return false;
    }
    private bool TrySetAttackCursor()
    {
        if (unitsSelected.Count > 0 && AtleastOneOffensiveUnit(unitsSelected) && RayHits(attackable, out _))
        {
            CursorManager.Instance.SetMarkerType(CursorManager.CursorType.Attackable);
            return true;
        }
        return false;
    }
    private bool TrySetUnAvailableCursor()
    {
        if (unitsSelected.Count > 0 && (RayHits(constructablelayer, out _) || RayHits(node, out _) || RayHits(unavailable, out _)))
        {
            CursorManager.Instance.SetMarkerType(CursorManager.CursorType.UnAvailable);
            return true;
        }
        return false;
    }
    private bool TrySetGatheringCursor()
    {
        if (unitsSelected.Count > 0 && OnlyHarvesterSelected())
        {
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, Mathf.Infinity, ~0, QueryTriggerInteraction.Collide))
            {
                if (hit.transform.GetComponent<ResourceNode>() != null)
                {
                    CursorManager.Instance.SetMarkerType(CursorManager.CursorType.Gathering);
                    return true;
                }
            }
        }
        return false;
    }
    private bool TrySetWalkableCursor()
    {
        if (unitsSelected.Count > 0 && RayHits(ground, out _))
        {
            CursorManager.Instance.SetMarkerType(CursorManager.CursorType.Walkable);
            return true;
        }
        return false;
    }

    private bool AtleastOneOffensiveUnit(List<GameObject> unitsSelected)
    {
        foreach (GameObject unit in unitsSelected)
        {
            if (unit != null && unit.GetComponent<Attackcontroller>())
            {
                return true;
            }
        }
        return false;
    }

    private void MultiSelect(GameObject unit)
    {
        if (unitsSelected.Contains(unit) == false)
        {
            unitsSelected.Add(unit);
            SelectUnit(unit, true);
        }
        else
        {
            SelectUnit(unit, false);
            unitsSelected.Remove(unit);
        }
    }

    private void SelectByClicking(GameObject unit)
    {
        DeselectAll();

        unitsSelected.Add(unit);

        SelectUnit(unit, true);
    }

    private void EnableUnitMovement(GameObject unit, bool triggermove)
    {
        if (unit != null)
        {
            unit.GetComponent<UnitMovement>().enabled = triggermove;
        }
    }

    public void DeselectAll()
    {
        foreach (var unit in unitsSelected)
        {
            SelectUnit(unit, false);
        }
        groundMarker.SetActive(false);
        unitsSelected.Clear();

        List<GameObject> allBuildings = GameObject.FindGameObjectsWithTag("Building").ToList();
        if (allBuildings.Count > 0)
        {
            foreach (var building in allBuildings)
            {
                if (building.transform.Find("Indicator") != null)
                {
                    building.transform.Find("Indicator").gameObject.SetActive(false);
                }
                else
                {
                    continue;
                }
            }
        }
    }

    private void TriggerSelectionIndicator(GameObject unit, bool isVisible)
    {
        if (unit != null)
        {
            unit.transform.Find("Indicator").gameObject.SetActive(isVisible);
        }
    }

    internal void DragSelect(GameObject unit)
    {
        if (unitsSelected.Contains(unit) == false)
        {
            unitsSelected.Add(unit);
            SelectUnit(unit, true);
        }
    }

    private void SelectUnit(GameObject unit, bool isSelected)
    {
        //Unit selection sound
        TriggerSelectionIndicator(unit, isSelected);
        EnableUnitMovement(unit, isSelected);
    }

    public void RememberSelectedUnits()
    {
        unitsToRemeber = unitsSelected.ToList();
        DeselectAll();
    }

    public void ReselectRememberedUnits()
    {
        foreach (var unit in unitsToRemeber)
        {
            if (unit != null)
            {
                unitsSelected.Add(unit);
                SelectUnit(unit, true);
            }
        }
        unitsToRemeber.Clear();
    }

    public void RestartUnitSelectionManager()
    {
        allUnitsList.Clear();
        unitsSelected.Clear();
        unitsToRemeber.Clear();
        techPanel.SetActive(false);
        magePanel.SetActive(false);
        attackCurosorvisible = false;
    }
}
