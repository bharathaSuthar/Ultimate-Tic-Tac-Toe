using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Initializes the game
/// </summary>
public class GameInitializer : MonoBehaviour 
{
    /// <summary>
    /// Awake is called before Start
    /// </summary>
    [SerializeField] GameObject player, AI;
    GameObject[] players;
    [SerializeField] GameObject mainBoard;
    [SerializeField] TurnManager turnManager;
    [SerializeField] int numberOfAI;
    float playerLocationY, playerLocationX;

	void Awake()
    {
        playerLocationY = ScreenUtils.ScreenHeight/6;
        playerLocationX = 2.5f*ScreenUtils.ScreenWidth/7;
        numberOfAI = 1;
        // initialize screen and configuration utils
        ScreenUtils.Initialize();
        //ConfigurationUtils.Initialize();
        players = new GameObject[2];
        if( numberOfAI == 1){
            players[1] = Instantiate(AI);
            players[0] = Instantiate(player);
        }
        else if( numberOfAI == 2){
            players[1] = Instantiate(AI);
            players[0] = Instantiate(AI);
        }
        else{
            players[1] = Instantiate(player);
            players[0] = Instantiate(player);
        }

        
        DecidePlayersIcon();
        DecidePlayerTurn();
        Instantiate(mainBoard);

        turnManager.players[0] = players[0].GetComponent<Player>();
        turnManager.players[1] = players[1].GetComponent<Player>();
    }
    private void Start() {
        players[0].transform.position = new Vector3( -playerLocationX, playerLocationY, 0);
        players[1].transform.position = new Vector3( playerLocationX, playerLocationY, 0);
    }
    void DecidePlayersIcon(){
        int randomNum1 = Random.Range(0,2);
        int randomNum2 = 1 - randomNum1;
        players[0].GetComponent<Player>().Icon = (SYMBOL)randomNum1;
        players[1].GetComponent<Player>().Icon = (SYMBOL)randomNum2;
    }
    void DecidePlayerTurn(){
        Player currentPlayer = players[Random.Range(0, 2)].GetComponent<Player>();
        currentPlayer.isPlaying = true;
        //Debug.LogFormat("Player is setup to play, player icon index is: {0}", (int)currentPlayer.icon);
    }
}
