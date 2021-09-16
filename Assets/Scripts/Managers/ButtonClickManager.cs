using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonClickManager: MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] GameObject mainMenu, pauseMenu, gameoverMenu;
    [SerializeField] GameObject xWins, oWins, draw;
    void Start()
    {
        EventManager.AddGameoverListener( GameOver );
    }

    public void Play(){
        SceneManager.LoadScene("Main game");
    }
    public void Quit(){
        Application.Quit();
    }
    public void Pause(){
        Time.timeScale = 0;
        pauseMenu.SetActive(true);
    }
    public void Resume(){
        Time.timeScale = 1;
        pauseMenu.SetActive(false);
    }
    public void Restart(){
        EventManager.Restart();
        SceneManager.LoadScene("Main menu");
    }
    public void GameOver( int winnerPlayer ){
        Debug.LogFormat("Game is Over");
        EventManager.isGameOver = true;
        GameObject winnerTitle;
        Vector3 spritePosition = new Vector3( 0, ScreenUtils.ScreenHeight/3, -5);
        if( winnerPlayer == 0 ){
            winnerTitle = oWins;
        }
        else if( winnerPlayer == 1) {
            winnerTitle = xWins;
        }
        else{
            winnerTitle = draw;
        }
        GameObject winnerSprite = Instantiate(winnerTitle, spritePosition, Quaternion.identity);
        GameObject gameoverMenuAdded = Instantiate(gameoverMenu, spritePosition, Quaternion.identity);
    }
    public void OpenSettings(){

    }
}
