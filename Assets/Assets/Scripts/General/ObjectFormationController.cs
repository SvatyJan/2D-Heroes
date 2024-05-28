using System.Collections.Generic;
using System.Linq;
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
    [SerializeField] int maxUnitsInRow = 5;
    [SerializeField] float startX = 2f;
    [SerializeField] float startY = 1f;
    [SerializeField] float spacingX = 1f;
    [SerializeField] float spacingY = 1f;

    [SerializeField] private List<GameObject> circleFormationUnits = new List<GameObject>();
    [SerializeField] private List<GameObject> frontFormationUnits = new List<GameObject>();
    [SerializeField] private List<GameObject> backFormationUnits = new List<GameObject>();
    [SerializeField] private List<GameObject> leftFormationUnits = new List<GameObject>();
    [SerializeField] private List<GameObject> rightFormationUnits = new List<GameObject>();
    [SerializeField] private List<GameObject> starFormationUnits = new List<GameObject>();
    [SerializeField] private List<GameObject> centerFormationUnits = new List<GameObject>();

    [SerializeField] private List<GameObject> circleformationPointsList = new List<GameObject>();
    [SerializeField] private List<GameObject> frontformationPointsList = new List<GameObject>();
    [SerializeField] private List<GameObject> backformationPointsList = new List<GameObject>();
    [SerializeField] private List<GameObject> leftformationPointsList = new List<GameObject>();
    [SerializeField] private List<GameObject> rightformationPointsList = new List<GameObject>();
    [SerializeField] private List<GameObject> starformationPointsList = new List<GameObject>();
    [SerializeField] private List<GameObject> centerformationPointsList = new List<GameObject>();

    [SerializeField] private bool deleteOnNoFollow = false;
    private float emptyTime = 0f;
    private float emptyDuration = 1f;

    void Update()
    {
        structurePosition = transform.position;
        if(deleteOnNoFollow)
        {
            deleteOnNoFollowingUnits();
        }
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
            structurePosition = transform.position;
            RecalculateFormations();
        }
    }

    public void RemoveUnit(GameObject unit)
    {
        if(unit.GetComponent<UnitBehavior>().getFollowTarget() != null)
        {
            GameObject oldFollowPoint = unit.GetComponent<UnitBehavior>().getFollowTarget();
            unit.GetComponent<UnitBehavior>().setFollowTarget(null);

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
            else if (starFormationUnits.Contains(oldFollowPoint))
            {
                starFormationUnits.Remove(oldFollowPoint);
            }
            else if (centerFormationUnits.Contains(oldFollowPoint))
            {
                centerFormationUnits.Remove(oldFollowPoint);
            }
            Destroy(oldFollowPoint.gameObject);
        }

        if (followingUnits.Contains(unit))
        {
            if (unit.GetComponent<UnitBehavior>().getFormation() == Formation.CIRCLE && circleFormationUnits.Contains(unit))
            {
                circleFormationUnits.Remove(unit);
            }
            else if(unit.GetComponent<UnitBehavior>().getFormation() == Formation.FRONT && frontFormationUnits.Contains(unit))
            {
                frontFormationUnits.Remove(unit);
            }
            else if (unit.GetComponent<UnitBehavior>().getFormation() == Formation.BACK && backFormationUnits.Contains(unit))
            {
                backFormationUnits.Remove(unit);
            }
            else if (unit.GetComponent<UnitBehavior>().getFormation() == Formation.LEFT && leftFormationUnits.Contains(unit))
            {
                leftFormationUnits.Remove(unit);
            }
            else if (unit.GetComponent<UnitBehavior>().getFormation() == Formation.RIGHT && rightFormationUnits.Contains(unit))
            {
                rightFormationUnits.Remove(unit);
            }
            else if (unit.GetComponent<UnitBehavior>().getFormation() == Formation.STAR && starFormationUnits.Contains(unit))
            {
                starFormationUnits.Remove(unit);
            }
            else if (unit.GetComponent<UnitBehavior>().getFormation() == Formation.CENTER && centerFormationUnits.Contains(unit))
            {
                centerFormationUnits.Remove(unit);
            }

            followingUnits.Remove(unit);
        }
        RecalculateFormations();
    }

    public float GetStartX()
    {
        return startX;
    }

    public void SetStartX(float newstartX)
    {
        startX = newstartX;
    }

    public float GetStartY()
    {
        return startY;
    }

    public void SetStartY(float newstartY)
    {
        startY = newstartY;
    }

    public float GetSpacingX()
    {
        return spacingX;
    }

    public void SetSpacingX(float newSpacingX)
    {
        spacingX = newSpacingX;
    }

    public float GetSpacingY()
    {
        return spacingY;
    }

    public void SetSpacingY(float newSpacingY)
    {
        spacingY = newSpacingY;
    }

    public float GetSpacingCircle()
    {
        return spacingCircle;
    }

    public void SetSpacingCircle(float newspacingCircle)
    {
        spacingCircle = newspacingCircle;
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
            starformationPointsList.Clear();
            centerformationPointsList.Clear();
        }
        else if (followingUnits.Count() > 0)
        {
            circleFormationUnits.Clear();
            frontFormationUnits.Clear();
            backFormationUnits.Clear();
            leftFormationUnits.Clear();
            rightFormationUnits.Clear();
            starFormationUnits.Clear();
            centerFormationUnits.Clear();

            foreach (GameObject followingUnit in followingUnits)
            {
                if (followingUnit.GetComponent<UnitBehavior>().getFormation() == (UnitBehavior.Formation)Formation.CIRCLE)
                {
                    circleFormationUnits.Add(followingUnit);
                }
                else if (followingUnit.GetComponent<UnitBehavior>().getFormation() == (UnitBehavior.Formation)Formation.FRONT)
                {
                    frontFormationUnits.Add(followingUnit);
                }
                else if (followingUnit.GetComponent<UnitBehavior>().getFormation() == (UnitBehavior.Formation)Formation.BACK)
                {
                    backFormationUnits.Add(followingUnit);
                }
                else if (followingUnit.GetComponent<UnitBehavior>().getFormation() == (UnitBehavior.Formation)Formation.LEFT)
                {
                    leftFormationUnits.Add(followingUnit);
                }
                else if (followingUnit.GetComponent<UnitBehavior>().getFormation() == (UnitBehavior.Formation)Formation.RIGHT)
                {
                    rightFormationUnits.Add(followingUnit);
                }
                else if (followingUnit.GetComponent<UnitBehavior>().getFormation() == (UnitBehavior.Formation)Formation.STAR)
                {
                    starFormationUnits.Add(followingUnit);
                }
                else if (followingUnit.GetComponent<UnitBehavior>().getFormation() == (UnitBehavior.Formation)Formation.CENTER)
                {
                    centerFormationUnits.Add(followingUnit);
                }
                else
                {
                    circleFormationUnits.Add(followingUnit);
                }
            }

            SetFormationRecalculatedPointsList(circleformationPointsList, circleFormationUnits);
            circleFormationPointsSort();

            SetFormationRecalculatedPointsList(frontformationPointsList, frontFormationUnits);
            frontFormationPointsSort();

            SetFormationRecalculatedPointsList(backformationPointsList, backFormationUnits);
            backFormationPointsSort();

            SetFormationRecalculatedPointsList(leftformationPointsList, leftFormationUnits);
            leftFormationPointsSort();

            SetFormationRecalculatedPointsList(rightformationPointsList, rightFormationUnits);
            rightFormationPointsSort();

            SetFormationRecalculatedPointsList(starformationPointsList, starFormationUnits);
            starFormationPointsSort();

            SetFormationRecalculatedPointsList(centerformationPointsList, centerFormationUnits);
            centerFormationPointsSort();
        }
    }

    private void SetFormationRecalculatedPointsList(List<GameObject> formationPointsList, List<GameObject> formationUnits)
    {
        foreach (GameObject point in formationPointsList)
        {
            Destroy(point);
        }

        formationPointsList.Clear();
        int requiredPoints = formationUnits.Count();

        for (int i = 0; i < requiredPoints; i++)
        {
            GameObject formationPoint = Instantiate(formationPointPrefab, Vector2.zero, Quaternion.identity);
            formationPoint.transform.parent = transform;
            formationPointsList.Add(formationPoint);
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

            circleFormationfollowingUnit.GetComponent<UnitBehavior>().setFollowTarget(circleFormationPoint);
        }
    }

    private void starFormationPointsSort()
    {
        List<Vector2> offsets = GetStarFormationDynamicOffsets(starFormationUnits.Count);

        for (int i = 0; i < starFormationUnits.Count; i++)
        {
            GameObject starformationFollowingUnit = starFormationUnits[i];
            GameObject starformationPoint = starformationPointsList[i];

            starformationPoint.name = starformationPoint.name + ' ' + starformationFollowingUnit.name;

            Vector2 offset = offsets[i];

            starformationPoint.transform.position = new Vector2(structurePosition.x + offset.x, structurePosition.y + offset.y);

            starformationFollowingUnit.GetComponent<UnitBehavior>().setFollowTarget(starformationPoint);
        }
    }

    private List<Vector2> GetStarFormationDynamicOffsets(int unitCount)
    {
        List<Vector2> offsets = new List<Vector2>();
        int layer = 1; // Vrstva, zaèíná od 1

        offsets.Add(new Vector2(-1 * spacingX, 0));
        offsets.Add(new Vector2(0, 1 * spacingX));
        offsets.Add(new Vector2(1 * spacingX, 0));
        offsets.Add(new Vector2(0, -1 * spacingX));

        while (offsets.Count < unitCount)
        {
            float distance = layer * spacingX;

            // Pøidání rohù
            offsets.Add(new Vector2(-distance, distance));
            offsets.Add(new Vector2(distance, distance));
            offsets.Add(new Vector2(distance, -distance));
            offsets.Add(new Vector2(-distance, -distance));

            layer++;
            distance = layer * spacingX;

            // Pøidání dalších jednotek na ose x a y
            offsets.Add(new Vector2(-distance, 0));
            offsets.Add(new Vector2(0, distance));
            offsets.Add(new Vector2(distance, 0));
            offsets.Add(new Vector2(0, -distance));
        }

        return offsets;
    }

    private void centerFormationPointsSort()
    {
        List<Vector2> offsets = GetCenterFormationDynamicOffsets(centerFormationUnits.Count);

        for (int i = 0; i < centerFormationUnits.Count; i++)
        {
            GameObject centerformationFollowingUnit = centerFormationUnits[i];
            GameObject centerformationPoint = centerformationPointsList[i];

            centerformationPoint.name = centerformationPoint.name + ' ' + centerformationFollowingUnit.name;

            Vector2 offset = offsets[i];

            centerformationPoint.transform.position = new Vector2(structurePosition.x + offset.x, structurePosition.y + offset.y);

            centerformationFollowingUnit.GetComponent<UnitBehavior>().setFollowTarget(centerformationPoint);
        }
    }

    private List<Vector2> GetCenterFormationDynamicOffsets(int unitCount)
    {
        List<Vector2> offsets = new List<Vector2>();
        int layer = 1;
        int unitsInLayer = 8;

        while (offsets.Count < unitCount)
        {
            float angleStep = 2 * Mathf.PI / unitsInLayer;
            float radius = layer * spacingX;

            for (int i = 0; i < unitsInLayer; i++)
            {
                float angle = i * angleStep;
                float x = Mathf.Cos(angle) * radius;
                float y = Mathf.Sin(angle) * radius;
                offsets.Add(new Vector2(x, y));

                if (offsets.Count >= unitCount)
                {
                    break;
                }
            }

            // Zvýšíme poèet jednotek pro další vrstvu
            layer++;
            unitsInLayer += 6 * (layer - 1); // Poèet jednotek v další vrstvì
        }

        return offsets;
    }

    private void frontFormationPointsSort()
    {
        for (int i = 0; i < frontformationPointsList.Count(); i++)
        {
            GameObject frontFormationFollowingUnit = frontFormationUnits[i];
            GameObject formationPoint = frontformationPointsList[i];

            int row = i / maxUnitsInRow;
            int col = i % maxUnitsInRow;

            float x = -startX + (col * spacingX);
            float y = startY + (row * spacingY);

            Vector3 newPosition = new Vector3(x, y, 0);
            formationPoint.transform.position = transform.position + newPosition;

            frontFormationFollowingUnit.GetComponent<UnitBehavior>().setFollowTarget(formationPoint);
        }
    }

    private void backFormationPointsSort()
    {
        for (int i = 0; i < backformationPointsList.Count(); i++)
        {
            GameObject backFormationFollowingUnit = backFormationUnits[i];
            GameObject formationPoint = backformationPointsList[i];

            int row = i / maxUnitsInRow;
            int col = i % maxUnitsInRow;

            float x = -startX + (col * spacingX);
            float y = -startY + (row * -spacingY);

            Vector3 newPosition = new Vector3(x, y, 0);
            formationPoint.transform.position = transform.position + newPosition;

            backFormationFollowingUnit.GetComponent<UnitBehavior>().setFollowTarget(formationPoint);
        }
    }

    private void leftFormationPointsSort()
    {
        for (int i = 0; i < leftformationPointsList.Count(); i++)
        {
            GameObject leftFormationFollowingUnit = leftFormationUnits[i];
            GameObject formationPoint = leftformationPointsList[i];

            int row = i / maxUnitsInRow;
            int col = i % maxUnitsInRow;

            float x = -startX + (col * -spacingX);
            float y = row * spacingY;
            Vector3 newPosition = new Vector3(x, y, 0);
            formationPoint.transform.position = transform.position + newPosition;

            leftFormationFollowingUnit.GetComponent<UnitBehavior>().setFollowTarget(formationPoint);
        }
    }

    private void rightFormationPointsSort()
    {
        for (int i = 0; i < rightformationPointsList.Count(); i++)
        {
            GameObject rightFormationFollowingUnit = rightFormationUnits[i];
            GameObject formationPoint = rightformationPointsList[i];

            int row = i / maxUnitsInRow;
            int col = i % maxUnitsInRow;

            float x = startX + (col * spacingX);
            float y = row * spacingY;
            Vector3 newPosition = new Vector3(x, y, 0);
            formationPoint.transform.position = transform.position + newPosition;

            rightFormationFollowingUnit.GetComponent<UnitBehavior>().setFollowTarget(formationPoint);
        }
    }

    private void deleteOnNoFollowingUnits()
    {
        if (followingUnits == null || followingUnits.Count == 0)
        {
            emptyTime += Time.deltaTime;
            if (emptyTime >= emptyDuration)
            {
                Destroy(gameObject);
            }
        }
        else
        {
            emptyTime = 0f;
        }
    }
}