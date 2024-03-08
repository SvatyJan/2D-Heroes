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

    /*public void srovnejNasledujiciJednotky()
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
        }
    }*/

    /* private void circleFormationPointsSort()
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
     }*/

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

    /*private void getFormationPoints(List<GameObject> unitsList, List<GameObject> formationPointsList)
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
    }*/

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
            formationSpacing++;
        }
        if (Input.GetKeyDown(KeyCode.KeypadMinus))
        {
            formationSpacing--;
        }
    }
}
