using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinSceneBehavior : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Invoke("Switch", 4f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Switch()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
