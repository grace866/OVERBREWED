using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem.Controls;
using UnityEditor.Build.Player;

public class GameManagerScript : MonoBehaviour
//IInteractable
{
    public GameObject player;
    private List<OrderScript> orders;
    //public TextMeshProUGUI currOrderText;
    public int score;
    //public TextMeshProUGUI scoreDisplay;
    private OrderScript currOrder;
    private UIManager UIManager;
    private bool isOrderCorrect = false;

    private void Start()
    {
        orders = new List<OrderScript>();
        GenerateOrder();
        player = GameObject.FindWithTag("Player");

        UpdateCurrOrder();
    }

    //private void Update()
    //{
        
    //}
    private void UpdateCurrOrder()
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
        UIManager.updateOrderText(milk, sugar);

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
            CheckOrder(currOrder, contents);
            orders.RemoveAt(0);
            UpdateCurrOrder();
            Debug.Log("Mug placed on the counter!");
        }
    }

    private void CheckOrder(OrderScript lastOrder, List<int> contents) 
    {
        // contents: index 0 is type of milk (0 - 3), index 1 is sugar added (0 - 1), index 2 is espresso added (0 - 1)
        Debug.Log(lastOrder.milk);
        Debug.Log(lastOrder.sugar);
        foreach (int c in contents) Debug.Log(c);
        if (lastOrder.milk == contents[0] && contents[2] == 1 && lastOrder.sugar == contents[1])
        {
            score += 1;
            UIManager.updateScoreText(score.ToString());
        }
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
