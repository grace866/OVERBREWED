using UnityEngine;

public class UIManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class uiscript : MonoBehaviour
{
    public TextMeshProUGUI currOrder;
    public void updateText(string type, int amt)
    {
        currOrder.text = "Current Order: \n Milk:" + type + "\n MilkAmt: " + amt;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;           // for TextMeshPro

public class TimerScript : MonoBehaviour
{
    // Start is called before the first frame update
    public float time = 120f;
    public bool runningTime = false;
    public string displayTime;
    public float tempTime;
    public TextMeshProUGUI timeDisplay;
    public CounterScript counter;

    void Start()
    {
        runningTime = true;
        tempTime = 0;
        displayTime = "2:00";
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
            }
        }
        else
        {
            displayTime = "0:00";
            timeDisplay.text = displayTime;
        }
        Console.WriteLine(displayTime);
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseGameScript : MonoBehaviour
{
    public GameObject pauseMenuUI;
    public Button resumeButton;

    private bool isPaused = false;

    void Start()
    {
        pauseMenuUI.SetActive(false); // Initially hide the pause menu
        resumeButton.onClick.AddListener(Resume);
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


