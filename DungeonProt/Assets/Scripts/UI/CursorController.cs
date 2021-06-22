using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorController : MonoBehaviour
{
    public Sprite uiSpriteMode;
    public Sprite battleSptiteMode;

    void Start()
    {
        //Cursor.visible = false;

        GetComponent<SpriteRenderer>().sprite = battleSptiteMode;
    }

    void Update()
    {
        // Обновление позиции объекта по позиции мыши
        Vector3 cursorPos = Input.mousePosition;
        transform.position = Camera.main.ScreenToWorldPoint(cursorPos) - new Vector3(0, 0, Camera.main.transform.position.z);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("UI"))
        {
            GetComponent<SpriteRenderer>().sprite = uiSpriteMode;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        GetComponent<SpriteRenderer>().sprite = battleSptiteMode;
    }
}
