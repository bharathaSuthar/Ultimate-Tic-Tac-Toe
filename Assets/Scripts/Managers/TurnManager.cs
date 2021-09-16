using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

public class TurnManager : MonoBehaviour
{
    public char[] icons;
    [SerializeField] public Player[] players = new Player[2];
    
    public Player currentPlayer, nextPlayer;
    char icon;
    private void Awake() {
        EventManager.AddTileWasClickedListener(SwitchTurn);
        icons = new char[]{'O', 'X'};
    }
    void Start()
    {
        SetUpTurn();
    }
    void SetUpTurn(){
        if( players[0].isPlaying ){
            currentPlayer = players[0];
            nextPlayer = players[1];
            nextPlayer.isPlaying = false;
        }
        else{
            currentPlayer = players[1];
            nextPlayer = players[0];
            nextPlayer.isPlaying = false;
            currentPlayer.isPlaying = true;
        }
        EventManager.currentSymbol = currentPlayer.Icon;
        EventManager.currentPlayer = currentPlayer;
        EventManager.previousSymbol = nextPlayer.Icon;
        EventManager.previousPlayer = nextPlayer;

        icon = icons[ (int)currentPlayer.Icon ];
    }
    void SwitchTurn(int r, int c){


        EventManager.previousPlayer = currentPlayer;
        EventManager.currentPlayer = nextPlayer;
        
        Move lastMove = new Move( r, c);
        EventManager.lastMove = lastMove;
        icon = icons[ (int)currentPlayer.Icon ];
        Debug.LogFormat("Current Player: {0} played, move: {1}, {2}", icon, lastMove.row, lastMove.col);

        Player temp = currentPlayer;
        currentPlayer = nextPlayer;
        nextPlayer = temp;

        EventManager.currentSymbol = currentPlayer.Icon;
        EventManager.previousSymbol = nextPlayer.Icon;
        
        currentPlayer.isPlaying = true;
        nextPlayer.isPlaying = false;

        EventManager.playerMadeAMoveInvoker.playerMadeAMove.Invoke( r, c);
        
        currentPlayer.SwitchSprites();
        nextPlayer.SwitchSprites();
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
