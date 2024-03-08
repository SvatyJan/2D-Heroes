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
                Debug.Log(unit);
                followingUnits.Add(unit);
            }
        }
        RecalculateFormations();
    }

    public void AddUnit(GameObject unit)
    {
        if(!followingUnits.Contains(unit))
        {
            followingUnits.Add(unit);
        }
        RecalculateFormations();
    }

    public void RemoveUnit(GameObject unit)
    {
        if (followingUnits.Contains(unit))
        {
            followingUnits.Remove(unit);
        }
        RecalculateFormations();
    }

    /**
     * Metoda pro sronani jednotek na zaklade nastaveni formace.
     * */
    public void RecalculateFormations()
    {
        if(followingUnits.Count > 0)
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

            List<GameObject> circleFormationRequiredPoints = GetFormationRecalculatedPointsList(circleFormationUnits);
            circleFormationPointsSort(circleFormationRequiredPoints);
        }
        else
        {

        }        
    }

    private List<GameObject> GetFormationRecalculatedPointsList(List<GameObject> formationUnits)
    {
        int requiredPoints = formationUnits.Count;

        for (int i = 0; i < circleformationPointsList.Count; i++)
        {
            Destroy(circleformationPointsList[i]);
        }

        if (requiredPoints > circleformationPointsList.Count)
        {
            int additionalPoints = requiredPoints - circleformationPointsList.Count;

            for (int i = 0; i < additionalPoints; i++)
            {
                GameObject formationPoint = Instantiate(formationPointPrefab, Vector2.zero, Quaternion.identity);
                formationPoint.transform.parent = transform;
                circleformationPointsList.Add(formationPoint);
            }
        }
        else if (requiredPoints < circleformationPointsList.Count)
        {
            int pointsToRemove = circleformationPointsList.Count - requiredPoints;

            for (int i = 0; i < pointsToRemove; i++)
            {
                GameObject pointToRemove = circleformationPointsList[circleformationPointsList.Count - 1];
                circleformationPointsList.Remove(pointToRemove);
                Destroy(pointToRemove);
            }
        }
        return circleformationPointsList;
    }

    /**
     * Sorvnani formace do kruhu.
     * */
    private void circleFormationPointsSort(List<GameObject> circleFormationRequiredPoints)
    {
        for (int i = 0; i < circleFormationRequiredPoints.Count; i++)
        {
            GameObject followingUnit = followingUnits[i];
            GameObject formationPoint = circleFormationRequiredPoints[i];

            float angle = i * (2 * Mathf.PI / circleFormationRequiredPoints.Count);
            float x = Mathf.Cos(angle) * spacingCircle;
            float y = Mathf.Sin(angle) * spacingCircle;

            formationPoint.transform.position = new Vector2(structurePosition.x + x, structurePosition.y + y);

            if (followingUnit.GetComponent<UnitBehavior>().attackTarget == null)
            {
                followingUnit.GetComponent<UnitBehavior>().followTarget = formationPoint;
                followingUnit.GetComponent<UnitBehavior>().behavior = Behavior.GUARD;
            }
            else
            {
                followingUnit.GetComponent<UnitBehavior>().behavior = Behavior.ATTACK;
            }
        }
    }
}
