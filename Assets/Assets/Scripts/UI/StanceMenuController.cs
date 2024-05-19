using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StanceMenuController : MonoBehaviour
{
    public GameObject StanceMenuObject;
    public GameObject CursorRootObject;
    public GameObject HighlightRootObject;

    public Camera cam;

    public TextMeshProUGUI StanceMenuItemLabel;
    public List<GameObject> StanceMenuItems = new List<GameObject>();

    private int selectedItem = 0;

    private void Start()
    {
        StanceMenuObject.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            StanceMenuObject.SetActive(true);
        }

        if (Input.GetKey(KeyCode.G))
        {
            Vector2 mousePos = Input.mousePosition;
            mousePos.x -= Screen.width / 2;
            mousePos.y -= Screen.height / 2;

            int itemChangeAngle = (int)Mathf.Round(360 / StanceMenuItems.Count);

            float angle = Mathf.Atan2(mousePos.y, mousePos.x) * Mathf.Rad2Deg - 90f;
            CursorRootObject.transform.rotation = Quaternion.Euler(0, 0, angle);
            angle += 360;
            angle = angle % 360;

            float highlightAngle = Mathf.Round(angle / itemChangeAngle) * itemChangeAngle;
            highlightAngle += 360 / StanceMenuItems.Count;
            HighlightRootObject.transform.rotation = Quaternion.Euler(0, 0, highlightAngle);

            selectedItem = (int)Mathf.Round(angle / itemChangeAngle);
            selectedItem = selectedItem % (StanceMenuItems.Count);

            StanceMenuItemLabel.text = StanceMenuItems[selectedItem].name;
        }

        if (Input.GetKeyUp(KeyCode.F))
        {
            StanceMenuItems[selectedItem].GetComponent<StanceChanger>().ChangeStance(this.gameObject);
            StanceMenuObject.SetActive(false);
        }
    }
}
