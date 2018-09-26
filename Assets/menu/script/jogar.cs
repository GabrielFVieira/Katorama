using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class jogar : MonoBehaviour {

    public GameObject pausePanel;


    void Start()
    {

    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            Time.timeScale = 1;
            pausePanel.SetActive(false);
        }
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            pausePanel.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Time.timeScale = 0;
        }

    }
    public void Sair()
    {
        SceneManager.LoadScene("Menu1");
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1;
        pausePanel.SetActive(false);

    }
    public void SinglePlayer()
    {
        SceneManager.LoadScene("LevelSelection");
        Time.timeScale = 1;
        pausePanel.SetActive(false);

    }}
