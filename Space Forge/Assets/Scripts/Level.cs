using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Level : MonoBehaviour
{
    float timeBeforeLoadingGameOver = 5f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadGameOver()
    {
        StartCoroutine(WaitBeforeLoadingGameOver());
    }

    IEnumerator WaitBeforeLoadingGameOver()
    {
        yield return new WaitForSeconds(timeBeforeLoadingGameOver);
        SceneManager.LoadScene("Game Over");
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("Main Menu");
    }

    public void LoadGameScene()
    {
        SceneManager.LoadScene("Game");
    }

    public void QuitGame()
    {
        Application.Quit();
    }


}
