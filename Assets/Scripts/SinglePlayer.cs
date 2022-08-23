using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class SinglePlayer : MonoBehaviour
{
    public static SinglePlayer instance;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        LoadScene("Single Player Test");
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void LoadScene(string sceneName)
    {

        SceneManager.LoadScene(sceneName);

    }



}