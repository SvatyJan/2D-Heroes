using UnityEngine;

public class StructureCursor : MonoBehaviour
{
    //Cursory
    [SerializeField] Texture2D defendCursor;

    // Metoda, která se volá, když myš pøejede nad objekt
    void OnMouseEnter()
    {
        // Zkontroluj, zda má objekt tag "Player Flag"
        if (gameObject.CompareTag("Player Flag") || gameObject.CompareTag("Player Structure"))
        {
            // Zmìò kurzor na customCursor
            Cursor.SetCursor(defendCursor, Vector2.zero, CursorMode.Auto);
        }
    }

    // Metoda, která se volá, když myš opustí objekt
    void OnMouseExit()
    {
        // Vrátí kurzor na defaultní kurzor
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }
}
