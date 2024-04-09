using System;
using System.Collections.Generic;
using System.Collections;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using static UnitBehavior;

public class UnitController : MonoBehaviour
{
    public GameObject MainFormationPoint;
    [SerializeField] public GameObject formationPointPrefab;
    [SerializeField] private List<GameObject> selectedUnits = new List<GameObject>();    
    [SerializeField] float formationSpacing = 3f;

    public Vector2 playerPosition;

    [SerializeField] public List<GameObject> unitGroup1 = new List<GameObject>();

    public enum Formation
    {
        CIRCLE = 1,
        FRONT = 2,
        BACK = 3,
        LEFT = 4,
        RIGHT = 5
    }
    public Formation formation;

    void Start()
    {
        formation = Formation.CIRCLE;
    }

    private void Update()
    {
        SelectAllUnits();
        ManipulateCircleFormationSpacing();
        groupSelectedUnity();
    }

    public List<GameObject> GetSelectedUnits()
    {
        return this.selectedUnits;
    }

    public void AddSelectUnit(GameObject unit)
    {
        if(!selectedUnits.Contains(unit))
        {
            selectedUnits.Add(unit);
            unit.GetComponent<UnitBehavior>().isHighlighted = true;
        }
    }

    public void RemoveSelectedUnit(GameObject unit)
    {
        selectedUnits.Remove(unit);
        unit.GetComponent<UnitBehavior>().isHighlighted = false;
    }

    public void SelectAllUnits()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            GameObject[] allPlayerUnits;
            allPlayerUnits = GameObject.FindGameObjectsWithTag("Player Unit");
            if (selectedUnits.Count < allPlayerUnits.Count())
            {
                foreach (GameObject playerUnit in allPlayerUnits)
                {
                    AddSelectUnit(playerUnit);
                }
            }
            else
            {
                foreach (GameObject playerUnit in allPlayerUnits)
                {
                    RemoveSelectedUnit(playerUnit);
                }
                selectedUnits.Clear();
                allPlayerUnits = null;
            }
        }
    }

    public void ChangeFormation(Formation chaningFormation)
    {
        formation = chaningFormation;
        foreach (GameObject selectedUnit in selectedUnits)
        {
            selectedUnit.GetComponent<UnitBehavior>().SetFormation((UnitBehavior.Formation)chaningFormation);
            if(selectedUnit.GetComponent<UnitBehavior>().GetFollowTarget() != null)
            {
                selectedUnit.GetComponent<UnitBehavior>().GetFollowTarget().transform.parent.GetComponent<ObjectFormationController>().RecalculateFormations();
            }
        }
    }

    private void ManipulateCircleFormationSpacing()
    {
        if (Input.GetKeyDown(KeyCode.KeypadPlus))
        {
            ObjectFormationController ObjectFormationController = GetComponent<ObjectFormationController>();
            float startX = ObjectFormationController.GetStartX();
            float startY = ObjectFormationController.GetStartY();
            float spacingX = ObjectFormationController.GetSpacingX();
            float spacingY = ObjectFormationController.GetSpacingY();
            float spacingCircle = ObjectFormationController.GetSpacingCircle();
            ObjectFormationController.SetStartX(startX + 2);
            ObjectFormationController.SetStartY(startY + 2);
            ObjectFormationController.SetSpacingX(spacingX+1);
            ObjectFormationController.SetSpacingY(spacingY+1);
            ObjectFormationController.SetSpacingCircle(spacingCircle+1);
            ObjectFormationController.RecalculateFormations();
        }
        if (Input.GetKeyDown(KeyCode.KeypadMinus))
        {
            ObjectFormationController ObjectFormationController = GetComponent<ObjectFormationController>();
            float startX = ObjectFormationController.GetStartX();
            float startY = ObjectFormationController.GetStartY();
            float spacingX = ObjectFormationController.GetSpacingX();
            float spacingY = ObjectFormationController.GetSpacingY();
            float spacingCircle = ObjectFormationController.GetSpacingCircle();
            ObjectFormationController.SetStartX(startX + -2);
            ObjectFormationController.SetStartY(startY + -2);
            ObjectFormationController.SetSpacingX(spacingX-1);
            ObjectFormationController.SetSpacingY(spacingY-1);
            ObjectFormationController.SetSpacingCircle(spacingCircle-1);
            ObjectFormationController.RecalculateFormations();
        }
    }

    private void groupSelectedUnity()
    {
        KeyCode numberOne = KeyCode.Alpha1;
        KeyCode shift = KeyCode.LeftShift;
        if (Input.GetKey(shift) && Input.GetKey(numberOne))
        {
            unitGroup1.Clear();
            foreach(GameObject selectedUnit in selectedUnits)
            {
                unitGroup1.Add(selectedUnit);
            }
        }
        else if(Input.GetKey(numberOne))
        {
            //TODO: pøi selectu vyber jen ty jednotky, ostatni vyhod z party.
            selectedUnits.Clear();
            foreach (GameObject unitInGroup in unitGroup1)
            {
                AddSelectUnit(unitInGroup);
            }
        }
    }
}
