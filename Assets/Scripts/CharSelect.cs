using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharSelect : MonoBehaviour {
	public Transform[] charPos;
	public GameObject[] chars;
	public int playerIndex;
	public int enemyIndex;
    public bool instatiatePlayer;
    public GameObject selectPanel;
    public GameObject levelPanel;
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(instatiatePlayer)
        {
            GameObject go = GameObject.FindGameObjectWithTag("Player");

            if (go != null)
                Destroy(go);

            Instantiate(chars[playerIndex], charPos[0].position, chars[playerIndex].transform.rotation);

            instatiatePlayer = false;
        }
	}
	
	public void ChangePos(int x)
	{
        playerIndex = x;
        instatiatePlayer = true;
    }

    public void LoadLevel(int i)
    {
        SceneManager.LoadScene(i);
    }

    public void Confirm()
    {
        GameObject go = GameObject.FindGameObjectWithTag("Player");

        if (go != null)
        {
            selectPanel.SetActive(false);
            levelPanel.SetActive(true);
        }
    }
}
