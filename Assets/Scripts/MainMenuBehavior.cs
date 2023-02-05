using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuBehavior : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void Play()
    {
        SceneManager.LoadScene("Intro");
    }

    public void Settings()
    {
        Debug.Log("TODO");
    }

    public void Jasmin()
    {
        Application.OpenURL("https://www.artstation.com/jasminyan");
    }

    public void Johannes()
    {
        Application.OpenURL("https://johannesambrosch.com/project-type/video-games/#content");
    }

    public void Pia()
    {
        Application.OpenURL("https://www.artstation.com/pillustration");
    }
}
