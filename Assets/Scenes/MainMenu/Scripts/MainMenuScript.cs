using UnityEngine;

public class MainMenuScript : MonoBehaviour
{

    public void NewGame()
    {
        Debug.Log("The new game button was clicked!");
        //TODO: start a new game
        //TODO: transition into overworld
    }
    public void LoadGame()
    {
        Debug.Log("The load game button was clicked!");
        //TODO: load a game
    }

    public void Settings()
    {
        Debug.Log("The Settings button has been clicked!");
        //TODO: create a settings screen/panel
    }
   public void Quit()
    {
        Debug.Log("The Quit button has been clicked!");
        Application.Quit();
    }
}
