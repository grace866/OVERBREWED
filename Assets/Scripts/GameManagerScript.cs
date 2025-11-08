using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem.Controls;

public class GameManagerScript : MonoBehaviour
//IInteractable
{
    public GameObject player;
    private List<OrderScript> orders;
    public TextMeshProUGUI currOrderText;
    public int score;
    public TextMeshProUGUI scoreDisplay;
    private OrderScript currOrder;
    private void Start()
    {
        orders = new List<OrderScript>();
        GenerateOrder();
        player = GameObject.FindWithTag("Player");

        scoreDisplay.text = "Score: 0";
        UpdateCurrOrderUI();
    }

    private void UpdateCurrOrderUI()
    {
        currOrder = orders[0];
        string milk;
        string sugar;
        if (currOrder.milk == 0) milk = "None";
        else if (currOrder.milk == 1) milk = "Whole";
        else if (currOrder.milk == 2) milk = "Almond";
        else milk = "Oat";
        if (currOrder.sugar == 1) sugar = "Sugar added";
        else sugar = "No sugar";

        currOrderText.text = "Current Order: \n Milk:" + milk + "\n Sugar: " + sugar;

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Mug"))
        {
            Debug.Log("triggered");
            MugScript mug = other.gameObject.GetComponent<MugScript>();
            mug.ChangeMugColor(Color.green);
            List<int> contents = mug.contents;

            // compare currOrder with submitted order
            currOrder = orders[0]; 
            AssignPoints(CheckOrder(currOrder, contents));
            orders.RemoveAt(0);
            UpdateCurrOrderUI();
            Debug.Log("Mug placed on the counter!");
        }
    }

    private bool CheckOrder(OrderScript lastOrder, List<int> contents) 
    {
        // contents: index 0 is type of milk (0 - 3), index 1 is sugar added (0 - 1), index 2 is espresso added (0 - 1)
        return (lastOrder.milk == contents[0] && contents[2] == 1 && lastOrder.sugar == contents[1]);
    }

    private void AssignPoints(bool isOrderCorrect)
    {
        Debug.Log("points assigned");
        if (isOrderCorrect)
        {
            score += 10;
        }
        else
        {
            score += 2;
        }
        scoreDisplay.text = "Score: " + score.ToString();
    }

    private void GenerateOrder()
    {
        for (int i = 0; i < 30; i++)
        {
            OrderScript order = new(Random.Range(0, 4), Random.Range(0, 2));
            orders.Add(order);
        }
    }
}
