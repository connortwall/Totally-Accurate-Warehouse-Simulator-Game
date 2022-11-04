using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

// KNOWN BUG:
// Hypothesis: Unity is slow at deleting gameObjects, so when generating a new order or when updating the UI for the current order,
// the last item to be handed in might accidentally be accessed which causes a "trying to access a deleted game object" error

public class OrderManager : MonoBehaviour
{
    // Any item that could possible be part of an order
    public List<GameObject> itemsInCurrentOrder = new List<GameObject>();
    public GameObject[] grabbableAssetsInScene;
    public GameObject grabbedItem;
    public static OrderManager instance;
    GameManager gameManager;
    UIManager uiManager;

    [HideInInspector]
    public bool orderCreated = false;

    void Awake()
    {
        // Initialize one (and only one) instance of this OrderManager
        if (instance != null)
        {
            Debug.LogError("More than one OrderManager in scene!");
            return;
        }
        instance = this;
    }

    void Start()
    {
        // Set managers to correct singleton-instance
        gameManager = GameManager.instance;
        uiManager = UIManager.instance;
        orderCreated = false;
    }

    //returns -1 if item is not in order
    //returns index of item if item is in order
    public int ItemIndex(GameObject item){
        int itemIndex = -1;
        for (int i = 0; i < itemsInCurrentOrder.Count; i++){
            if (itemsInCurrentOrder[i] != null && itemsInCurrentOrder[i].name == item.name){
                itemIndex = i;
                break;
            }
        }
        return itemIndex;
    }
    
    public bool GenerateNewOrder()
    {
        // TODO: Generate items based on what's currently in the level
        itemsInCurrentOrder.Clear();
        grabbableAssetsInScene = GameObject.FindGameObjectsWithTag("Item");

        //verify we actually have enough items for game 
        if (gameManager.orderSize > grabbableAssetsInScene.Length){
            throw new Exception("There are not enough items in the scene to create a new order.");
            return false;
        }

        int randomStartingIndex = UnityEngine.Random.Range(0, grabbableAssetsInScene.Length);
        for (int i = 0; i < gameManager.orderSize; i++)
        {
            // To ensure that the same item isn't picked multiple times, we choose a random starting index
            // We then increment the index position by 1 each loop-iteration while using modulus to avoid indexOutOfBounds
            int index = (i + randomStartingIndex) % grabbableAssetsInScene.Length;
            itemsInCurrentOrder.Add(grabbableAssetsInScene[index]);
            Debug.Log($"Order item {i+1}: {itemsInCurrentOrder[i]}");
        }
        uiManager.UpdateOrderPanel();

        orderCreated = true;
        return true;
    }


    public void DeliverItem(GameObject submittedItem){
        //use regular for loop to have access to index
        int remIndex = ItemIndex(submittedItem);
        //check if item found 
        if (remIndex == -1) throw new Exception("item could not be delivered; was not found in list!");
        else {
            //remove item from list
            itemsInCurrentOrder.RemoveAt(remIndex);
            //then remove from scene
            Destroy(submittedItem);
        } 

        uiManager.UpdateOrderPanel();
        
        //check if this was the last order needed
        if (itemsInCurrentOrder.Count == 0){
            gameManager.OrderCompleted();
        }
        
    }
    

    /*********** ALL FUNCTIONS BELOW FOR DEBUG PURPOSES ONLY *************/

    public void GrabItem(GameObject itemToBeGrabbed)
    {
        grabbedItem = itemToBeGrabbed;
        uiManager.UpdateGrabbedItem(grabbedItem);
    }

    public bool IsItemPartOfOrder()
    {
        // Returns "True" if grabbedItem is part of the current order
        return itemsInCurrentOrder.Contains(grabbedItem);
    }

    public void DeliverItemDebug()
    {
        // Attempt to hand in the item currently held by the player
        if (grabbedItem == null)
        {
            Debug.Log($"No item is currently being held");
            return;
        }
        // Iterate through each item in the order to see if any of the names match the name of the players' grabbedItem
        foreach (var item in itemsInCurrentOrder)
        {
            if ( item != null && item.name == grabbedItem.name)
            {
                // The item is successfully taken from the player and the current order is updated
                Debug.Log($"Item successfully handed in: {grabbedItem.name}");
                itemsInCurrentOrder.Remove(item);
                Destroy(grabbedItem);
                grabbedItem = null;
                uiManager.UpdateOrderPanel();
                uiManager.ResetGrabbedItem();

                // If this was the last item of an order, mark it as complete
                if (itemsInCurrentOrder.Count == 0)
                {
                    gameManager.OrderCompleted();
                }
                // Return out of the foreach-loop to ensure that an order containing x numbers of an item actually requires x items to be handed in instead of only one
                return;
            }
        }
        Debug.Log("Item rejected; the item currently held by the player is not part of the order!");
    }

}
