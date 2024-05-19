using System;
using UnityEngine;

public class UnitSelectionSystem : MonoBehaviour
{
    private Vector3 startPosition;
    private Vector3 endPosition;
    public Camera cam;

    [SerializeField] private Transform selectionAreaTransform;
    [SerializeField] private GameObject hero;

    private void Awake()
    {
        selectionAreaTransform.gameObject.SetActive(false);
    }

    private void Update()
    {
        if(Input.GetButtonDown("Fire1"))
        {
            startPosition = cam.ScreenToWorldPoint(Input.mousePosition);
            selectionAreaTransform.gameObject.SetActive(true);
        }

        if(Input.GetButton("Fire1"))
        {
            Vector3 currentMousePosition = cam.ScreenToWorldPoint(Input.mousePosition);
            Vector3 lowerLeft = new Vector3(
                Mathf.Min(startPosition.x, currentMousePosition.x),
                Mathf.Min(startPosition.y, currentMousePosition.y)
                );
            Vector3 upperRigth = new Vector3(
                Mathf.Max(startPosition.x, currentMousePosition.x),
                Mathf.Max(startPosition.y, currentMousePosition.y)
                );
            selectionAreaTransform.position = lowerLeft;
            selectionAreaTransform.localScale = upperRigth - lowerLeft;
        }

        if (Input.GetButtonUp("Fire1"))
        {
            selectionAreaTransform.gameObject.SetActive(false);
            endPosition = cam.ScreenToWorldPoint(Input.mousePosition);

            Collider2D[] collider2DArray = Physics2D.OverlapAreaAll(startPosition, endPosition);
            foreach (Collider2D collider2D in collider2DArray)
            {
                UnitBehavior unitBehavior = collider2D.GetComponent<UnitBehavior>();
                if (unitBehavior != null)
                {
                    if(unitBehavior.getSelectable())
                    {
                        try
                        {
                            unitBehavior.AddToGroup(hero);
                        }
                        catch (Exception e)
                        {
                            //Jednotka již existuje v partì hrdiny
                        }
                    }
                }
            }
        }
    }
}
