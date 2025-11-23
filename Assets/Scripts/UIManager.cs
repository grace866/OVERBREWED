using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI currOrder;
    public float time = 120f;
    public bool runningTime = false;
    public string displayTime;
    public float tempTime;
    public TextMeshProUGUI timeDisplay;
    public TextMeshProUGUI scoreDisplay;
    public GameObject pauseMenuUI;
    public Button resumeButton;
    private bool isPaused = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        runningTime = true;
        tempTime = 0;
        displayTime = "2:00";
        pauseMenuUI.SetActive(false); // Initially hide the pause menu
        resumeButton.onClick.AddListener(Resume);
        scoreDisplay.text = "Score: 0";

    }

    // Update is called once per frame
    void Update()
    {
        tempTime += Time.deltaTime;

        if (runningTime)
        {
            if (tempTime >= 1f)
            {
                time--;
                displayTime = Mathf.Floor(time / 60).ToString() + ":" + (time % 60);
                tempTime = 0f;
                timeDisplay.text = displayTime;
                if (time == 0f) runningTime = false;
            }
        }
        else
        {
            displayTime = "0:00";
            timeDisplay.text = displayTime;
        }
        Console.WriteLine(displayTime);

    }

    public void updateOrderText(string type, string sugar)
    {
        currOrder.text = "Current Order: \n" + type + " milk \n" + sugar;
    }

    public void updateScoreText(String newScore)
    {
        scoreDisplay.text = "Score: " + newScore;
    }


    public void Pause()
    {
        if (!isPaused)
        {
            Time.timeScale = 0;
            pauseMenuUI.SetActive(true);
            isPaused = true;
        }
    }
    public void Resume()
    {
        Time.timeScale = 1;
        pauseMenuUI.SetActive(false);
        isPaused = false;
    }
}

