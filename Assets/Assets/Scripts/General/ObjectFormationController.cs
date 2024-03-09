using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static UnitBehavior;

public class ObjectFormationController : MonoBehaviour
{
    public GameObject MainFormationPoint;
    [SerializeField] public GameObject formationPointPrefab;

    [SerializeField] private List<GameObject> followingUnits = new List<GameObject>();

    public Vector2 structurePosition;

    [Header("Formation Settings")]
    [SerializeField] float spacingCircle = 3f;
    [SerializeField] float maxUnitsInCircle = 10;

    [SerializeField] private List<GameObject> circleFormationUnits = new List<GameObject>();
    [SerializeField] private List<GameObject> frontFormationUnits = new List<GameObject>();
    [SerializeField] private List<GameObject> backFormationUnits = new List<GameObject>();
    [SerializeField] private List<GameObject> leftFormationUnits = new List<GameObject>();
    [SerializeField] private List<GameObject> rightFormationUnits = new List<GameObject>();

    [SerializeField] private List<GameObject> circleformationPointsList = new List<GameObject>();

    void Update()
    {
        structurePosition = transform.position;
    }

    public List<GameObject> GetFollowingUnits()
    {
        return this.followingUnits;
    }

    public void AddUnits(List<GameObject> units)
    {
        foreach (GameObject unit in units)
        {
            if (!followingUnits.Contains(unit))
            {
                followingUnits.Add(unit);
                RecalculateFormations();
            }
        }
    }

    public void AddUnit(GameObject unit)
    {
        if (!followingUnits.Contains(unit))
        {
            followingUnits.Add(unit);
            RecalculateFormations();
        }
    }

    public void RemoveUnit(GameObject unit)
    {
        if(unit.GetComponent<UnitBehavior>().GetFollowTarget() != null)
        {
            GameObject oldFollowPoint = unit.GetComponent<UnitBehavior>().GetFollowTarget();
            unit.GetComponent<UnitBehavior>().SetFollowTarget(null);
            unit.GetComponent<UnitBehavior>().stance = Stance.PASSIVE;

            if (circleFormationUnits.Contains(oldFollowPoint))
            {
                circleFormationUnits.Remove(oldFollowPoint);
            }
            else if (frontFormationUnits.Contains(oldFollowPoint))
            {
                frontFormationUnits.Remove(oldFollowPoint);
            }
            else if (backFormationUnits.Contains(oldFollowPoint))
            {
                backFormationUnits.Remove(oldFollowPoint);
            }
            else if (leftFormationUnits.Contains(oldFollowPoint))
            {
                leftFormationUnits.Remove(oldFollowPoint);
            }
            else if(rightFormationUnits.Contains(oldFollowPoint))
            {
                rightFormationUnits.Remove(oldFollowPoint);
            }
            Destroy(oldFollowPoint.gameObject);
        }

        if (followingUnits.Contains(unit))
        {
            if (unit.GetComponent<UnitBehavior>().GetFormation() == Formation.CIRCLE && circleFormationUnits.Contains(unit))
            {
                circleFormationUnits.Remove(unit);
            }
            else if(unit.GetComponent<UnitBehavior>().GetFormation() == Formation.FRONT && frontFormationUnits.Contains(unit))
            {
                frontFormationUnits.Remove(unit);
            }
            else if (unit.GetComponent<UnitBehavior>().GetFormation() == Formation.BACK && backFormationUnits.Contains(unit))
            {
                backFormationUnits.Remove(unit);
            }
            else if (unit.GetComponent<UnitBehavior>().GetFormation() == Formation.LEFT && leftFormationUnits.Contains(unit))
            {
                leftFormationUnits.Remove(unit);
            }
            else if (unit.GetComponent<UnitBehavior>().GetFormation() == Formation.RIGHT && rightFormationUnits.Contains(unit))
            {
                rightFormationUnits.Remove(unit);
            }

            followingUnits.Remove(unit);
        }
        RecalculateFormations();
    }

    /**
     * Metoda pro sronani jednotek na zaklade nastaveni formace.
     * */
    public void RecalculateFormations()
    {
        if (followingUnits.Count() == 0)
        {
            followingUnits.Clear();
            circleFormationUnits.Clear();
            frontFormationUnits.Clear();
            backFormationUnits.Clear();
            leftFormationUnits.Clear();
            rightFormationUnits.Clear();
            circleformationPointsList.Clear();
        }
        else if (followingUnits.Count() > 0)
        {
            circleFormationUnits.Clear();
            frontFormationUnits.Clear();
            backFormationUnits.Clear();
            leftFormationUnits.Clear();
            rightFormationUnits.Clear();

            foreach (GameObject followingUnit in followingUnits)
            {
                if (followingUnit.GetComponent<UnitBehavior>().GetFormation() == (UnitBehavior.Formation)Formation.CIRCLE)
                {
                    circleFormationUnits.Add(followingUnit);
                }
                else if (followingUnit.GetComponent<UnitBehavior>().GetFormation() == (UnitBehavior.Formation)Formation.FRONT)
                {
                    frontFormationUnits.Add(followingUnit);
                }
                else if (followingUnit.GetComponent<UnitBehavior>().GetFormation() == (UnitBehavior.Formation)Formation.BACK)
                {
                    backFormationUnits.Add(followingUnit);
                }
                else if (followingUnit.GetComponent<UnitBehavior>().GetFormation() == (UnitBehavior.Formation)Formation.LEFT)
                {
                    leftFormationUnits.Add(followingUnit);
                }
                else if (followingUnit.GetComponent<UnitBehavior>().GetFormation() == (UnitBehavior.Formation)Formation.RIGHT)
                {
                    rightFormationUnits.Add(followingUnit);
                }
                else
                {
                    circleFormationUnits.Add(followingUnit);
                }
            }

            SetCircleFormationRecalculatedPointsList();
            circleFormationPointsSort();
        }
    }

    private void SetCircleFormationRecalculatedPointsList()
    {
        // Zkontrolovat, zda existují nìjaké body a odstranit je
        foreach (GameObject point in circleformationPointsList)
        {
            Destroy(point);
        }
        // Vyèistit seznam bodù
        circleformationPointsList.Clear();

        int requiredPoints = circleFormationUnits.Count();

        for (int i = 0; i < requiredPoints; i++)
        {
            GameObject formationPoint = Instantiate(formationPointPrefab, Vector2.zero, Quaternion.identity);
            formationPoint.transform.parent = transform;
            circleformationPointsList.Add(formationPoint);
        }
    }

    private void circleFormationPointsSort()
    {
        for (int i = 0; i < circleformationPointsList.Count(); i++)
        {
            GameObject circleFormationfollowingUnit = circleFormationUnits[i];
            GameObject circleFormationPoint = circleformationPointsList[i];

            circleFormationPoint.name = circleFormationPoint.name + ' ' + circleFormationfollowingUnit.name;

            float angle = i * (2 * Mathf.PI / circleformationPointsList.Count());
            float x = Mathf.Cos(angle) * spacingCircle;
            float y = Mathf.Sin(angle) * spacingCircle;

            circleFormationPoint.transform.position = new Vector2(structurePosition.x + x, structurePosition.y + y);

            circleFormationfollowingUnit.GetComponent<UnitBehavior>().SetFollowTarget(circleFormationPoint);
            circleFormationfollowingUnit.GetComponent<UnitBehavior>().behavior = Behavior.GUARD;
        }
    }
}