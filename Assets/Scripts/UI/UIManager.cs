using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    GameManager gameManager;
    OrderManager orderManager;
    // A "Text" version of our numerical variables (for UI stuff)
    public TextMeshProUGUI uiPlayerScore;
    public TextMeshProUGUI uiTimeRemaining;
    public TextMeshProUGUI uiOrdersRemaining;
    public TextMeshProUGUI uiLivesRemaining;
    public TextMeshProUGUI uiCurrentlyGrabbedItem;
    public GameObject itemButtonPrefab;
    public GameObject orderPanel;
    public bool orderPanelVisibility;


    void Awake()
    {
        // Initialize one (and only one) instance of this UIManager
        if (instance != null)
        {
            Debug.LogError("More than one UIManager in scene!");
            return;
        }
        instance = this;
    }

    void Start()
    {
        // Set managers to correct singleton-instance
        gameManager = GameManager.instance;
        orderManager = OrderManager.instance;
    }

    public void UpdateOrderPanel()
    {
        // Generate buttons in orderPanel based on current order
        ClearOrderPanel();//Reset the orderPanel
        foreach(var item in orderManager.itemsInCurrentOrder)
        {
            // Creates an itemButtonPrefab and add it to orderPanel
            GameObject button = (GameObject)Instantiate(itemButtonPrefab);
            button.transform.SetParent(orderPanel.transform);
            button.transform.GetChild(0).GetComponent<Text>().text = $"{item.name}"; //Set text on button
        }
    }

    private void ClearOrderPanel()
    {
        // Destroy every item in the orderPanel (with the purpose of resetting it and preparing for a new order)
        Button[] buttons = orderPanel.GetComponentsInChildren<Button>();
        foreach (var button in buttons)
        {
            Destroy(button.gameObject);
        }
    }

    //used to hide Order panel on startup
    public void hideOrderPanel(){
        orderPanelVisibility = false;
        orderPanel.gameObject.SetActive(orderPanelVisibility);
    }

    //used to hide Order panel on startup
    public void openOrderPanel(){
        UpdateOrderPanel();
        orderPanelVisibility = true;
        orderPanel.gameObject.SetActive(orderPanelVisibility);
    }

    public void ToggleOrderPanelVisibility()
    {
        orderPanelVisibility = !orderPanelVisibility;
        orderPanel.gameObject.SetActive(orderPanelVisibility);
    }

    public void UpdateScoreUI(int playerScore)
    {
        // Update UI element to match the score GameManager variable
        uiPlayerScore.text = $"Current score: {playerScore}";
    }
    public void UpdateTimeUI(float timeRemaining)
    {
        // Update UI element to match the timeRemaining GameManager variable
        uiTimeRemaining.text = $"Time remaining: {Mathf.RoundToInt(timeRemaining)}";
    }
    public void UpdateOrdersUI(int ordersRemaining)
    {
        // Update UI element to match the ordersRemaining GameManager variable
        uiOrdersRemaining.text = $"Orders remaining: {ordersRemaining}";
    }
    public void UpdateLivesUI(int livesRemaining)
    {
        // Update UI element to match the livesRemaining GameManager variable
        uiLivesRemaining.text = $"Lives remaining: {livesRemaining}";
    }
    public void UpdateGrabbedItem(GameObject grabbedItem)
    {
        // Update UI element to match the grabbedItem OrderManager variable
        uiCurrentlyGrabbedItem.text = $"Currently grabbed item: {grabbedItem.name}";
    }
    public void ResetGrabbedItem()
    {
        // Update UI element to match the grabbedItem OrderManager variable
        uiCurrentlyGrabbedItem.text = $"No items are currently being held";
    }
}
