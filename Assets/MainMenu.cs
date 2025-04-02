using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Method to start the game
    public void StartGame()
    {
        SceneManager.LoadScene("LevelOne"); // Change "LevelOne" to the actual name of your first scene
    }

    // Method to quit the game
    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Game Closed!"); // Shows in editor but does nothing in the build
    }
}
