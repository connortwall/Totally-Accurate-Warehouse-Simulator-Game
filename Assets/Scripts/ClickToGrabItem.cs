using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickToGrabItem : MonoBehaviour
{
    GameManager gameManager;
    OrderManager orderManager;
    UIManager uiManager;
    void Start()
    {
        // Set managers to correct singleton-instance
        gameManager = GameManager.instance;
        uiManager = UIManager.instance;
        orderManager = OrderManager.instance;
    }

    void OnMouseDown()
    {
        // User clicked on this gameObject - set it as the grabbedItem in OrderManager
        Debug.Log($"User clicked on {gameObject.name} and item has been picked up");
        orderManager.GrabItem(gameObject.transform.root.gameObject);
    }
}
