using System.Collections.Generic;
using UnityEngine;
using static UnitBehavior;

public class StructureFormationController : MonoBehaviour
{
    public GameObject MainFormationPoint;
    [SerializeField] public GameObject formationPointPrefab;
    private List<GameObject> formationPointsList = new List<GameObject>();

    [SerializeField] public List<GameObject> selectedUnits = new List<GameObject>();

    public Vector2 structurePosition;

    [Header("Formation Settings")]
    [SerializeField] float spacingCircle = 3f;
    [SerializeField] float maxUnitsInCircle = 10;

    public enum Formation
    {
        CIRCLE = 1,
        FRONT = 2,
        BACK = 3,
        LEFT = 4,
        RIGHT = 5
    }
    public Formation formation;

    private List<GameObject> circleFormationUnits = new List<GameObject>();
    private List<GameObject> frontFormationUnits = new List<GameObject>();
    private List<GameObject> backFormationUnits = new List<GameObject>();
    private List<GameObject> leftFormationUnits = new List<GameObject>();
    private List<GameObject> rightFormationUnits = new List<GameObject>();

    void Start()
    {
        formation = Formation.CIRCLE;
    }

    void Update()
    {
        structurePosition = transform.position;
        srovnejJednotky();
    }

    /**
     * Metoda pro sronani jednotek na zaklade nastaveni formace.
     * */
    public void srovnejJednotky()
    {
        int requiredPoints = selectedUnits.Count;

        if (requiredPoints > formationPointsList.Count)
        {
            int additionalPoints = requiredPoints - formationPointsList.Count;

            for (int i = 0; i < additionalPoints; i++)
            {
                GameObject formationPoint = Instantiate(formationPointPrefab, Vector2.zero, Quaternion.identity);
                formationPointsList.Add(formationPoint);
            }
        }
        else if (requiredPoints < formationPointsList.Count)
        {
            int pointsToRemove = formationPointsList.Count - requiredPoints;

            for (int i = 0; i < pointsToRemove; i++)
            {
                GameObject pointToRemove = formationPointsList[formationPointsList.Count - 1];
                formationPointsList.Remove(pointToRemove);
                Destroy(pointToRemove);
            }
        }

        if (formation == Formation.CIRCLE)
        {
            circleFormationPointsSort(requiredPoints);
        }
    }

    /**
     * Sorvnani formace do kruhu.
     * */
    private void circleFormationPointsSort(int requiredPoints)
    {
        for (int i = 0; i < requiredPoints; i++)
        {
            GameObject followingUnit = selectedUnits[i];
            GameObject formationPoint = formationPointsList[i];

            float angle = i * (2 * Mathf.PI / requiredPoints);
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
