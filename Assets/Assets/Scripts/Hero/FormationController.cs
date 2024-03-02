using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnitBehavior;

public class FormationController : MonoBehaviour
{
    public GameObject MainFormationPoint;
    [SerializeField] public GameObject formationPointPrefab;
    [SerializeField] public float collectUnitDistance;
    
    private List<GameObject> formationPointsList = new List<GameObject>();

    [SerializeField] public List<GameObject> selectedUnits = new List<GameObject>();
    [SerializeField] public List<GameObject> followingUnits = new List<GameObject>();
    public Vector2 playerPosition;

    [Header("Formation Settings")]
    [SerializeField] float startX = -4f;
    [SerializeField] float startY = -4f;
    [SerializeField] float spacingX = 2f;
    [SerializeField] float spacingY = 2f;
    [SerializeField] int maxUnitsInRow = 5;
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

    private List<GameObject> circleFormationUnits   = new List<GameObject>();
    private List<GameObject> frontFormationUnits    = new List<GameObject>();
    private List<GameObject> backFormationUnits     = new List<GameObject>();
    private List<GameObject> leftFormationUnits     = new List<GameObject>();
    private List<GameObject> rightFormationUnits    = new List<GameObject>();

    void Start()
    {
        formation = Formation.CIRCLE;
    }

    // Update is called once per frame
    void Update()
    {
        playerPosition = transform.position;

        prepniFormaci();
        manipulateCircleFormationSpacing();
        srovnejJednotky();

        //controlSelectedUnits();
    }

    public void srovnejJednotky()
    {
        int requiredPoints = followingUnits.Count;

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

        if(formation == Formation.CIRCLE)
        {
            circleFormationPointsSort(requiredPoints);
        }
        else if(formation == Formation.FRONT)
        {
            frontFormationPointsSort(requiredPoints);
        }
        else if (formation == Formation.BACK)
        {
            backFormationPointsSort(requiredPoints);
        }
        else if (formation == Formation.LEFT)
        {
            leftFormationPointsSort(requiredPoints);
        }
        else if (formation == Formation.RIGHT)
        {
            rightFormationPointsSort(requiredPoints);
        }
    }

    private void backFormationPointsSort(int requiredPoints)
    {
        // Umístìní bodù formace
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

    private void frontFormationPointsSort(int requiredPoints)
    {
        // Umístìní bodù formace
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

    private void circleFormationPointsSort(int requiredPoints)
    {
        for(int i = 0; i < requiredPoints; i++)
        {
            GameObject followingUnit = selectedUnits[i];
            GameObject formationPoint = formationPointsList[i];

            float angle =  i * (2 * Mathf.PI / requiredPoints);
            float x = Mathf.Cos(angle) * spacingCircle;
            float y = Mathf.Sin(angle) * spacingCircle;

            formationPoint.transform.position = new Vector2 (playerPosition.x + x, playerPosition.y + y);

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

    private void rightFormationPointsSort(int requiredPoints)
    {
        // Umístìní bodù formace
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

    private void leftFormationPointsSort(int requiredPoints)
    {
        // Umístìní bodù formace
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
    }

    private void prepniFormaci()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            int maxFormationValue = (int)Enum.GetValues(typeof(Formation)).Cast<Formation>().Max();

            // Pøepni na následující formaci
            formation = (Formation)(((int)formation % maxFormationValue) + 1);
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

    /* Metoda ktera po stisknuti praveho tlacitka prikaze vybranym jednotkam udelat akci */
    private void controlSelectedUnits()
    {
        //tady se bude skenovat raycast, ktery na zaklade typu raycastu udela akci
        // friendly unit/building/player -> defend
        // enemy unit/building/player -> attack
        // ground -> move
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(new Vector3(playerPosition.x, playerPosition.y, 0), collectUnitDistance);
    }
}
