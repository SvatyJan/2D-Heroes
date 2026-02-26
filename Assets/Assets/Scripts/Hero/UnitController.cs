using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
        RIGHT = 5,
        CENTER = 6,
        STAR = 7
    }
    public Formation formation;

    public enum Stance
    {
        AGRESSIVE = 1,
        DEFENSIVE = 2,
        PASSIVE = 3,
    }
    public Stance stance;

    void Start()
    {
        formation = Formation.CIRCLE;
    }

    private void Update()
    {
        selectedUnits.RemoveAll(unit => unit == null);

        SelectAllUnits();
        ManipulateCircleFormationSpacing();
        groupSelectedUnits();
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
        if (unit == null)
            return;

        selectedUnits.Remove(unit);

        UnitBehavior ub = unit.GetComponent<UnitBehavior>();
        if (ub != null)
            ub.isHighlighted = false;
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
            selectedUnit.GetComponent<UnitBehavior>().setFormation((UnitBehavior.Formation)chaningFormation);
            if(selectedUnit.GetComponent<UnitBehavior>().getFollowTarget() != null)
            {
                selectedUnit.GetComponent<UnitBehavior>().getFollowTarget().transform.parent.GetComponent<ObjectFormationController>().RecalculateFormations();
            }
        }
    }

    public void ChangeStance(Stance changingStance)
    {
        stance = changingStance;
        foreach (GameObject selectedUnit in selectedUnits)
        {
            selectedUnit.GetComponent<UnitBehavior>().setStance((UnitBehavior.Stance)changingStance);
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

    private void groupSelectedUnits()
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
            UnselectAllUnits();
            foreach (GameObject unitInGroup in unitGroup1)
            {
                AddSelectUnit(unitInGroup);
            }
        }
    }

    public void UnselectAllUnits()
    {
        foreach (GameObject selectedUnit in selectedUnits)
        {
            selectedUnit.GetComponent<UnitBehavior>().isHighlighted = false;
        }
        selectedUnits.Clear();
    }
}
