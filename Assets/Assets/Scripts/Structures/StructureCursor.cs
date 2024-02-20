using UnityEngine;

public class StructureCursor : MonoBehaviour
{
    //Cursory
    [SerializeField] Texture2D defendCursor;

    // Metoda, kter� se vol�, kdy� my� p�ejede nad objekt
    void OnMouseEnter()
    {
        // Zkontroluj, zda m� objekt tag "Player Flag"
        if (gameObject.CompareTag("Player Flag") || gameObject.CompareTag("Player Structure"))
        {
            // Zm�� kurzor na customCursor
            Cursor.SetCursor(defendCursor, Vector2.zero, CursorMode.Auto);
        }
    }

    // Metoda, kter� se vol�, kdy� my� opust� objekt
    void OnMouseExit()
    {
        // Vr�t� kurzor na defaultn� kurzor
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }
}
