using System;
using System.Collections.Generic;
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
        manipulateCircleFormationSpacing();
    }

    public List<GameObject> GetSelectedUnits()
    {
        return this.selectedUnits;
    }

    public void AddSelectUnit(GameObject unit)
    {
        selectedUnits.Add(unit);
        unit.GetComponent<UnitBehavior>().isHighlighted = true;
    }

    public void RemoveSelectedUnit(GameObject unit)
    {
        selectedUnits.Remove(unit);
        unit.GetComponent<UnitBehavior>().isHighlighted = false;
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

    private void manipulateCircleFormationSpacing()
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
            ObjectFormationController.SetSpacingX(spacingX-1);
            ObjectFormationController.SetSpacingY(spacingY-1);
            ObjectFormationController.SetSpacingCircle(spacingCircle-1);
            ObjectFormationController.RecalculateFormations();
        }
    }
}
