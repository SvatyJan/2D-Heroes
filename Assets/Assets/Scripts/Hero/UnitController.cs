using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnitBehavior;

public class UnitController : MonoBehaviour
{
    public GameObject MainFormationPoint;
    [SerializeField] public GameObject formationPointPrefab;
    //private List<GameObject> formationPointsList = new List<GameObject>();

    [SerializeField] public List<GameObject> selectedUnits = new List<GameObject>();
    [SerializeField] public List<GameObject> followingUnits = new List<GameObject>();
    
    [Header("Formation Settings")]
    [SerializeField] float startX = -4f;
    [SerializeField] float startY = -4f;
    [SerializeField] float spacingX = 2f;
    [SerializeField] float spacingY = 2f;
    [SerializeField] int maxUnitsInRow = 5;
    [SerializeField] float spacingCircle = 3f;

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

    private List<GameObject> circleFormationUnits   = new List<GameObject>();
    private List<GameObject> frontFormationUnits    = new List<GameObject>();
    private List<GameObject> backFormationUnits     = new List<GameObject>();
    private List<GameObject> leftFormationUnits     = new List<GameObject>();
    private List<GameObject> rightFormationUnits    = new List<GameObject>();

    private List<GameObject> circleFormationPointsList = new List<GameObject>();

    void Start()
    {
        formation = Formation.CIRCLE;
    }

    void Update()
    {
        playerPosition = transform.position;

        //manipulateCircleFormationSpacing();
        srovnejNasledujiciJednotky();

        circleFormationPointsSort();
        /*frontFormationPointsSort();
        backFormationPointsSort();
        leftFormationPointsSort();
        rightFormationPointsSort();*/


        //controlSelectedUnits();
    }

    public void srovnejNasledujiciJednotky()
    {
        /*int requiredPoints = followingUnits.Count;

        // Ovìøení, zda je potøeba pøidat nebo odebrat body formace
        if (requiredPoints > formationPointsList.Count)
        {
            int additionalPoints = requiredPoints - formationPointsList.Count;

            for (int i = 0; i < additionalPoints; i++)
            {
                GameObject formationPoint = Instantiate(formationPointPrefab, Vector2.zero, Quaternion.identity);
                formationPoint.transform.SetParent(MainFormationPoint.transform);
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
        }*/

        foreach(GameObject followingUnit in followingUnits)
        {
            if(followingUnit.GetComponent<UnitBehavior>().formation == UnitBehavior.Formation.CIRCLE)
            {
                circleFormationUnits.Add(followingUnit);
            }
            else if (followingUnit.GetComponent<UnitBehavior>().formation == UnitBehavior.Formation.FRONT)
            {
                frontFormationUnits.Add(followingUnit);
            }
            else if (followingUnit.GetComponent<UnitBehavior>().formation == UnitBehavior.Formation.BACK)
            {
                backFormationUnits.Add(followingUnit);
            }
            else if (followingUnit.GetComponent<UnitBehavior>().formation == UnitBehavior.Formation.LEFT)
            {
                leftFormationUnits.Add(followingUnit);
            }
            else if (followingUnit.GetComponent<UnitBehavior>().formation == UnitBehavior.Formation.RIGHT)
            {
                rightFormationUnits.Add(followingUnit);
            }
        }
    }

    private void circleFormationPointsSort()
    {
        getFormationPoints(circleFormationUnits, circleFormationPointsList);

        int requiredPoints = circleFormationPointsList.Count();

        for (int i = 0; i < requiredPoints; i++)
        {
            GameObject followingUnitInCircleFormation = circleFormationUnits[i];
            GameObject formationPoint = circleFormationPointsList[i];

            float angle = i * (2 * Mathf.PI / requiredPoints);
            float x = Mathf.Cos(angle) * spacingCircle;
            float y = Mathf.Sin(angle) * spacingCircle;

            formationPoint.transform.position = new Vector2(playerPosition.x + x, playerPosition.y + y);

            // Nastavení cíle a chování vojáka
            if (followingUnitInCircleFormation.GetComponent<UnitBehavior>().attackTarget == null)
            {
                followingUnitInCircleFormation.GetComponent<UnitBehavior>().followTarget = formationPoint;
                followingUnitInCircleFormation.GetComponent<UnitBehavior>().behavior = Behavior.GUARD;
            }
            else
            {
                //když útoèí na terè
                followingUnitInCircleFormation.GetComponent<UnitBehavior>().behavior = Behavior.ATTACK;
            }
        }
    }

    /*private void backFormationPointsSort()
    {
        int requiredPoints = backFormationUnits.Count();

        for (int i = 0; i < requiredPoints; i++)
        {
            GameObject followingUnit = selectedUnits[i];
            GameObject formationPoint = formationPointsList[i];

            int row = i / maxUnitsInRow;
            int col = i % maxUnitsInRow;

            float x = startX + (col * spacingX) + MainFormationPoint.transform.position.x;
            float y = startY + (row * spacingY) + MainFormationPoint.transform.position.y;

            formationPoint.transform.position = new Vector2(x, y);
        }

        // Aktualizace pozic vojákù
        if (selectedUnits.Count > 0)
        {
            // Provádìjte operace s followingUnits pouze pokud obsahuje prvky
            for (int i = 0; i < selectedUnits.Count; i++)
            {
                GameObject followingUnit = selectedUnits[i];
                GameObject formationPoint = formationPointsList[i];

                // Nastavení cíle a chování vojáka
                if (followingUnit.GetComponent<UnitBehavior>().attackTarget == null)
                {
                    followingUnit.GetComponent<UnitBehavior>().followTarget = formationPoint;
                    followingUnit.GetComponent<UnitBehavior>().behavior = Behavior.GUARD;
                }
                else
                {
                    //když útoèí na terè
                    followingUnit.GetComponent<UnitBehavior>().behavior = Behavior.ATTACK;
                }
            }
        }
    }

    private void frontFormationPointsSort()
    {

        int requiredPoints = frontFormationUnits.Count();

        for (int i = 0; i < requiredPoints; i++)
        {
            GameObject followingUnit = selectedUnits[i];
            GameObject formationPoint = formationPointsList[i];

            int row = i / maxUnitsInRow;
            int col = i % maxUnitsInRow;

            float x = startX + (col * spacingX) + MainFormationPoint.transform.position.x;
            float y = -startY + (row * -spacingY) + MainFormationPoint.transform.position.y;

            formationPoint.transform.position = new Vector2(x, y);
        }

        // Aktualizace pozic vojákù
        if (requiredPoints > 0)
        {
            // Provádìjte operace s followingUnits pouze pokud obsahuje prvky
            for (int i = 0; i < requiredPoints; i++)
            {
                GameObject followingUnit = frontFormationUnits[i];
                GameObject formationPoint = formationPointsList[i];

                // Nastavení cíle a chování vojáka
                if (followingUnit.GetComponent<UnitBehavior>().attackTarget == null)
                {
                    followingUnit.GetComponent<UnitBehavior>().followTarget = formationPoint;
                    followingUnit.GetComponent<UnitBehavior>().behavior = Behavior.GUARD;
                }
                else
                {
                    //když útoèí na terè
                    followingUnit.GetComponent<UnitBehavior>().behavior = Behavior.ATTACK;
                }
            }
        }
    }

    private void rightFormationPointsSort()
    {
        int requiredPoints = rightFormationUnits.Count();

        for (int i = 0; i < requiredPoints; i++)
        {
            GameObject followingUnit = selectedUnits[i];
            GameObject formationPoint = formationPointsList[i];

            int row = i / maxUnitsInRow;
            int col = i % maxUnitsInRow;

            float x = 0;
            float y = 0;

            x = -startX + (col * spacingX) + MainFormationPoint.transform.position.x;
            y = row * spacingY + MainFormationPoint.transform.position.y;
            formationPoint.transform.position = new Vector2(x, y);
        }

        // Aktualizace pozic vojákù
        if (selectedUnits.Count > 0)
        {
            // Provádìjte operace s followingUnits pouze pokud obsahuje prvky
            for (int i = 0; i < selectedUnits.Count; i++)
            {
                GameObject followingUnit = selectedUnits[i];
                GameObject formationPoint = formationPointsList[i];

                // Nastavení cíle a chování vojáka
                if (followingUnit.GetComponent<UnitBehavior>().attackTarget == null)
                {
                    followingUnit.GetComponent<UnitBehavior>().followTarget = formationPoint;
                    followingUnit.GetComponent<UnitBehavior>().behavior = Behavior.GUARD;
                }
                else
                {
                    //když útoèí na terè
                    followingUnit.GetComponent<UnitBehavior>().behavior = Behavior.ATTACK;
                }
            }
        }
    }

    private void leftFormationPointsSort()
    {
        int requiredPoints = leftFormationUnits.Count();

        for (int i = 0; i < requiredPoints; i++)
        {
            GameObject followingUnit = selectedUnits[i];
            GameObject formationPoint = formationPointsList[i];

            int row = i / maxUnitsInRow;
            int col = i % maxUnitsInRow;

            float x = startX + (col * -spacingX) + MainFormationPoint.transform.position.x;
            float y = row * spacingY + MainFormationPoint.transform.position.y;

            formationPoint.transform.position = new Vector2(x, y);
        }

        // Aktualizace pozic vojákù
        if (selectedUnits.Count > 0)
        {
            // Provádìjte operace s followingUnits pouze pokud obsahuje prvky
            for (int i = 0; i < selectedUnits.Count; i++)
            {
                GameObject followingUnit = selectedUnits[i];
                GameObject formationPoint = formationPointsList[i];

                // Nastavení cíle a chování vojáka
                if (followingUnit.GetComponent<UnitBehavior>().attackTarget == null)
                {
                    followingUnit.GetComponent<UnitBehavior>().followTarget = formationPoint;
                    followingUnit.GetComponent<UnitBehavior>().behavior = Behavior.GUARD;
                }
                else
                {
                    //když útoèí na terè
                    followingUnit.GetComponent<UnitBehavior>().behavior = Behavior.ATTACK;
                }
            }
        }
    }*/

    private void getFormationPoints(List<GameObject> unitsList, List<GameObject> formationPointsList)
    {
        int requiredPoints = unitsList.Count();
        // Ovìøení, zda je potøeba pøidat nebo odebrat body formace
        if (requiredPoints > formationPointsList.Count)
        {
            int additionalPoints = requiredPoints - formationPointsList.Count;

            for (int i = 0; i < additionalPoints; i++)
            {
                GameObject formationPoint = Instantiate(formationPointPrefab, Vector2.zero, Quaternion.identity);
                formationPoint.transform.SetParent(MainFormationPoint.transform);
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
    }

    public void ChangeFormation(Formation chaningFormation)
    {
        formation = chaningFormation;
        foreach (GameObject selectedUnit in selectedUnits)
        {
            selectedUnit.GetComponent<UnitBehavior>().SetFormation((UnitBehavior.Formation)chaningFormation);
        }
    }

    private void manipulateCircleFormationSpacing()
    {
        if (Input.GetKeyDown(KeyCode.KeypadPlus))
        {
            spacingCircle++;
        }
        if (Input.GetKeyDown(KeyCode.KeypadMinus))
        {
            spacingCircle--;
        }
    }
}
