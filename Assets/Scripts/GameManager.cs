using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    OrderManager orderManager;
    UIManager uiManager;
    LevelAudio levelAudio;
    AudioManager audioManager;

    // Internal game state variables
    
   
    [Header("Game Mechanics")]
    public float timeLimit = 45f;
    public int numberOfLives = 3;
    public float timePenalty = 5f;
    public float distanceForOrderList = 3f;

    [Header("Order Logic")]
    public int numberOfOrders = 4;
    public int orderSize = 3;
    public int scoreGainPerOrderCompleted = 100;

    [Header("Debug")]
    public bool EnableDebugCommands = true;
    public bool printDebug = true;

    [Header("What scene should we go to after level completion?")]
    public string nextLevelScene = "";

    [Header("Character information")]
    public string nameOfCharacterGameObject = "HEAD_REF";

    //private vars for us
    [HideInInspector]
    public float timeRemaining = 45f;
    [HideInInspector]
    public int playerScore = 0;
    [HideInInspector]
    public int livesRemaining = 3;
    [HideInInspector]
    public int ordersRemaining = 4;
    [HideInInspector]
    public bool shippingEnabled = false;

    //list of shelves so we can be sure they are done spawning
    private GameObject[] shelves;

    //action map
    [Header("Attach Action Map Below")]
    public PlayerInput control;
    private InputAction orderListAction = null;

    //logic for order list distance
    private GameObject shippingBox;
    private GameObject player;
    private bool manualClose = false;

    void Awake()
    {
        // Initialize one (and only one) instance of this GameManager
        if (instance != null)
        {
            Debug.LogError("More than one GameManager in scene!");
            return;
        }
        instance = this;
    }

    void Start()
    {
        //set up controller
        orderListAction = control.actions["OpenOrderList"];


        //update internal values from inspector
        timeRemaining = timeLimit;
        ordersRemaining = numberOfOrders;

        //disable shipping until order complete
        shippingEnabled = false;

        // Set managers to correct singleton-instance
        orderManager = OrderManager.instance;
        uiManager = UIManager.instance;
        levelAudio = LevelAudio.instance;

        while (audioManager == null)
        {
            audioManager = AudioManager.instance;
        }
        

        //Initialize all UI elements with correct values
        uiManager.UpdateScoreUI(playerScore);
        uiManager.UpdateTimeUI(timeRemaining);
        uiManager.UpdateOrdersUI(ordersRemaining);

        //populate shelves
        shelves = GameObject.FindGameObjectsWithTag("Shelf");
        foreach (var shelf in shelves){
            shelf.GetComponent<ShelveAssets>().PopulateShelves();
        }

        /*
         * the main menu will save an empty game to the game slot upon 
         * the user clicking new scene. Therefore, we need to update the 
         * playerScore and numLivesRemaining from the save file. Implement below!
         */

        //get saved score
        playerScore = PlayerPrefs.GetInt("savedScore");

        //check for new game, use default num of lives if this is a new game
        if (PlayerPrefs.GetInt("newGame") == 1 || PlayerPrefs.GetInt("savedLives") == -1){
            livesRemaining = numberOfLives;
            //it is no longer a new game
            levelAudio.StartGame(true);
            PlayerPrefs.SetInt("newGame", 0);
            if (printDebug) Debug.Log("new game, getting base num lives " + numberOfLives);
        } 
        else {
            if (printDebug) Debug.Log("continued game; getting old numlives");
            livesRemaining = PlayerPrefs.GetInt("savedLives");
            levelAudio.StartGame(false);
        }

        uiManager.UpdateLivesUI(livesRemaining);
        
        //hide order panel on game startup
        uiManager.hideOrderPanel();

        //find shipping box & player
        shippingBox = GameObject.Find("StopTrigger");
        if (shippingBox == null) throw new Exception("GM could not find shipping box location");
        player = GameObject.Find(nameOfCharacterGameObject);
        if (shippingBox == null) throw new Exception("GM could not find player location");

        
        //after shelves are populated, create first order
        while(!orderManager.GenerateNewOrder());

        //calculate the audio events for the level
        levelAudio.CalculateTimerEvents(timeLimit);

    }

    void Update()
    {
        // Tick down timeRemaining and check if it has reached 0
        CheckIfTimeIsUp();
        timeRemaining -= Time.deltaTime;
        audioManager.timeRemaining = timeRemaining;
        uiManager.UpdateTimeUI(timeRemaining);



        //open automatically and close order panel if player gets too far away
        float distance = Vector3.Distance(player.transform.position, shippingBox.transform.position);            
        //if too far away, and the panel is open, close it
        if (distance > distanceForOrderList){
            manualClose = false;
        }
        //logic for opening the order list manually
        if (orderListAction.triggered)
        {
            //if it is open, close it no matter what
            if (uiManager.orderPanelVisibility){
                uiManager.hideOrderPanel();
                manualClose = true;
            }
            //only open on close distance
            else if (distance <= distanceForOrderList) {
                uiManager.openOrderPanel();
                VerifyOrder();
            } 
        }
        else if (distance > distanceForOrderList && uiManager.orderPanelVisibility){
                if (printDebug) print("Hiding Order List.. (user too far away)");
                uiManager.hideOrderPanel();
            }
        //if close enough, and the panel is closed, automatically open
        else if (distance <= distanceForOrderList && !uiManager.orderPanelVisibility && !manualClose){
                if (printDebug) print("Opening Order List.. (user close to box)");
                uiManager.openOrderPanel();
                VerifyOrder();
        }

        


        /*********** DEBUG KEYBOARD COMMANDS *************/
        if (!EnableDebugCommands) return;
        if (Input.GetKeyDown("tab"))
        {
            // Press Tab to toggle the visibility of the orderPanel UI element
            uiManager.ToggleOrderPanelVisibility();
        }
        if (Input.GetKeyDown("space"))
        {
            // Press spacebar to generate a new order
            orderManager.GenerateNewOrder();
        }
        if (Input.GetKeyDown("return"))
        {
            // Press Enter to deliver the item currently held by the player
            orderManager.DeliverItemDebug();
        }
        //DEBUG: press p to simulate first item in order dropped in box
        if (Input.GetKeyDown(KeyCode.L)){
            ItemSubmittedDebug();
        }
    }

    //triple check an order has been created
    private void VerifyOrder(){
        //create an order if it hasnt been created
        while (orderManager.orderCreated == false){
            orderManager.GenerateNewOrder();
        }
        //make sure order panel was updated
        uiManager.UpdateOrderPanel();

    }

    public void ItemSubmitted(GameObject submittedItem){
        if (orderManager.ItemIndex(submittedItem) != -1){
            if (printDebug) print("correct item; removing from list");
            orderManager.DeliverItem(submittedItem);
        }
        else{
            if (printDebug) print("wrong item!!!; applying time penalty");
            //destroy wrong object
            Destroy(submittedItem);
            ApplyTimePenalty(this.timePenalty);
            levelAudio.WrongItem();
        }
    }

    private void CheckIfTimeIsUp()
    {
        // Show fail-screen if timer reaches 0 (or is reduced below 0 by a time penalty)
        if (timeRemaining <= 0f)
        {
            LevelFailure();
        }
    }

    public void ApplyTimePenalty(float timePenalty)
    {
        // Take float as input and subtract it from timeRemaining
        timeRemaining -= timePenalty;
        CheckIfTimeIsUp();
    }

    public void increaseScore(int scoreGain)
    {
        // Take int as input and add it to current playerScore
        playerScore += scoreGain;
    }

    //called by conveyor belt when 
    public void shipBox(){
        shippingEnabled = false;
        //say a new order must be created 
        orderManager.orderCreated = false;

        levelAudio.ShipBoxAudio();
        // Generate the next order
        orderManager.GenerateNewOrder();
    }

    public void OrderCompleted()
    {
        //allow animation
        shippingEnabled = true;
        
        // An order has been completed! Increase the playerScore and check if there are more orders remaining
        increaseScore(scoreGainPerOrderCompleted);
        ordersRemaining -= 1;
        uiManager.UpdateOrdersUI(ordersRemaining);
        if (ordersRemaining <= 0)
        {
            // The player has won the level by completing all the orders
            LevelSuccess();
        }
        else
        {
            if (printDebug) Debug.Log($"Order completed! {ordersRemaining} more order(s) to go!, waiting for box to ship");
        }
    }

    void LevelSuccess()
    {
        if (printDebug) Debug.Log($"Level completed with {timeRemaining} time left!");
        //Time.timeScale = 0;

        /*
         * save level ending score, next level name and num lives before going to next level
         */

        PlayerPrefs.SetString("savedLevel", nextLevelScene);
        PlayerPrefs.SetInt("savedLives", livesRemaining);
        PlayerPrefs.SetInt("savedScore", playerScore);
        PlayerPrefs.Save();

        //load next level
        SceneManager.LoadScene(nextLevelScene);
    }

    void LevelFailure()
    {
        livesRemaining -= 1;
        if (livesRemaining > 0)
        {
            // Player loses a life but can continue to play the level
            if (printDebug) Debug.Log("You lost a life!");
            
            uiManager.UpdateLivesUI(livesRemaining);
            uiManager.UpdateTimeUI(timeRemaining);

            /*
            * load save file into variables below, then save the game
            * the ONLY CHANGE to the save file is num lives remaining
            * DO NOT SAVE any updates to the score or what level the player is on
            *
            */

            if (printDebug) Debug.Log("Setting number of lives to " + livesRemaining);
            //now save newNumLives
            PlayerPrefs.SetInt("savedLives", livesRemaining);
            PlayerPrefs.Save();

            SceneManager.LoadScene("LoseLife");
        }
        else
        {
            // Player is taken to a GameOver screen
            if (printDebug) Debug.Log("You're fired! (Game Over)");
            timeRemaining = 0f; //Ensure that timer does not show a negative value on GameOver screen if player lost as the result of a time penalty
            uiManager.UpdateTimeUI(timeRemaining);

            //SAVE FRESH GAME TO ENSURE CANNOT REPEAT
            /*
            * save with original values and first level of the game as the level
            */

            PlayerPrefs.SetInt("savedLives", -1);
            PlayerPrefs.SetInt("savedScore", 0);
            //this var tells the game manager to reset info when the game loads
            PlayerPrefs.SetInt("newGame", 1);
            PlayerPrefs.Save();

            //go to lose screen
            SceneManager.LoadScene("LoseScreen");

        }

        //uiManager.orderPanel.gameObject.SetActive(false);
    }

    /************** DEBUG FUNCTIONS BELOW ******************/
    public void ItemSubmittedDebug(){
        if (printDebug) print("DEBBG: DELIVER 1 ITEM");
        //dont allow func if it is not possible
        if (orderManager.itemsInCurrentOrder == null || orderManager.itemsInCurrentOrder.Count <= 0) return;

        GameObject submittedItem = orderManager.itemsInCurrentOrder[0];

        if (orderManager.ItemIndex(submittedItem) != -1){
            if (printDebug) print("correct item; removing from list");
            orderManager.DeliverItem(submittedItem);
        }
        else{
            if (printDebug) print("wrong item!!!; applying time penalty");
            ApplyTimePenalty(this.timePenalty);
        }
    }
}
