using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public static MainMenu instance;

    private void Awake()
    {
        instance = this;
    }

    public static bool isSinglePlayer = false;

    public void LoadScene(string sceneName)
    {

        SceneManager.LoadScene(sceneName);
       
    }

    public void LoadSceneSinglePlayer(string sceneName)
    {

        SceneManager.LoadScene(sceneName);
        isSinglePlayer = true;


    }

    public void quitGame()
    {
        Application.Quit(); // Exit Game
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
