using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class LevelManager : MonoBehaviour {
    public PlayerHealth player;
    public PlayerHealth enemy;
    public float timer = 240;
    public TextMeshProUGUI[] text;

    public Animator[] publicAnims;
    private float waitingTimer;
    private float dmgGiven;

    public GameObject winPanel;
    public GameObject losePanel;
    public GameObject pausePanel;
    // Use this for initialization
    void Start () {
        winPanel.SetActive(false);
        losePanel.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
		if(player.curHealth > 0 && enemy.curHealth > 0)
        {
            if (timer > 0)
            {
                timer -= Time.deltaTime;
                if (publicAnims.Length > 0)
                {
                    PublicBehavior();
                }
            }
            else if (timer < 0)
                timer = 0;

            if(timer <= 0)
            {
                if (player.curHealth > enemy.curHealth)
                    enemy.curHealth = 0;

                else if (player.curHealth < enemy.curHealth)
                    player.curHealth = 0;

                else
                    player.curHealth = enemy.curHealth = 0;

                if (publicAnims.Length > 0)
                {
                    foreach (Animator a in publicAnims)
                        a.SetBool("Clap", true);
                }
            }

                foreach (TextMeshProUGUI t in text)
                    t.text = Mathf.RoundToInt(timer).ToString();
        }

        else
        {
            if (publicAnims.Length > 0)
            {
                foreach (Animator a in publicAnims)
                    a.SetBool("Clap", true);
            }

            if (enemy.curHealth <= 0 && player.curHealth > 0)
                winPanel.SetActive(true);

            else if (enemy.curHealth > 0 && player.curHealth <= 0)
                losePanel.SetActive(true);

            if (Input.GetKeyDown(KeyCode.R))
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);

            if (Input.GetKeyDown(KeyCode.Escape))
                SceneManager.LoadScene("Menu1");
        }   
        if (Input.GetKeyDown(KeyCode.Escape) && !losePanel.activeSelf && !winPanel.activeSelf)
        {
            if(pausePanel.activeSelf == false)
            {
                pausePanel.SetActive(true);
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                Time.timeScale = 0;
            }

            else
            {
                Resume();
            }
        }
	}

    public void PublicBehavior()
    {
        waitingTimer += Time.deltaTime;

        if (waitingTimer > 30)
        {
            for (int i = 0; i < publicAnims.Length; i++)
            {
                publicAnims[i].SetTrigger("Celebrate");

                if (i == publicAnims.Length)
                    waitingTimer = 0;
            }
        }

        if (dmgGiven >= 100)
        {
            foreach (Animator a in publicAnims)
                a.SetTrigger("Excited");

            dmgGiven = 0;

            waitingTimer = 0;
        }
    }

    public void Comemorate(float dmg)
    {
        dmgGiven += dmg;
        waitingTimer = 0;
    }

    public void Resume()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1;
        pausePanel.SetActive(false);
    }

    public void Restart()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Menu()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1;
        SceneManager.LoadScene("Menu1");
    }
}
