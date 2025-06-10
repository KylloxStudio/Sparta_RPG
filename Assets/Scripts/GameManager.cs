using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public bool IsGameEnded { get; private set; }

    protected override void OnAwake()
    {
        IsGameEnded = false;
    }

    public void ExitGame()
    {
        IsGameEnded = true;
        Application.Quit();
    }
}