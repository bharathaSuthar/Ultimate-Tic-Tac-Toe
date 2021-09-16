using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public static class EventManager
{
    public static bool isGameOver = false;
    public static SYMBOL currentSymbol, previousSymbol;
    public static Player previousPlayer;
    public static Player currentPlayer;
    public static Move lastMove = new Move( -1, -1) ;
    public static List<MiniBoard> nextPlayableBoards;
    public static Player playerMadeAMoveInvoker;
    public static UnityAction<int, int> playerMadeAMoveListener;
    public static Tile tileWasClickedInvoker;
    public static UnityAction<int, int> tileWasClickedListener;

    public static MainBoard gameoverInvoker;
    public static UnityAction<int> gameOverListener;
    public static void Restart(){
        isGameOver = false;
        currentPlayer = null;
        previousPlayer = null;
        lastMove = new Move( -1, -1);
        currentSymbol = SYMBOL.None;
        previousSymbol = SYMBOL.None;
        nextPlayableBoards = new List<MiniBoard>();
        playerMadeAMoveListener = null;
        tileWasClickedListener = null;
    }
    public static void SwitchSymbols(){
        SYMBOL temp = currentSymbol;
        currentSymbol = previousSymbol;
        previousSymbol = temp;
    }
    public static void AddPlayerMadeAMoveInvoker( Player invoker){
        playerMadeAMoveInvoker = invoker;
        if( playerMadeAMoveListener!= null ){
            invoker.AddPlayerMadeAMoveListener( playerMadeAMoveListener );
        }
    }
    public static void AddPlayerMadeAMoveListener( UnityAction<int, int> a){
        playerMadeAMoveListener += a;
        if( playerMadeAMoveInvoker!= null){
            playerMadeAMoveInvoker.AddPlayerMadeAMoveListener( playerMadeAMoveListener );
        }
    }
    public static void AddTileWasClickedListener( Tile invoker){
        tileWasClickedInvoker = invoker;
        if( tileWasClickedListener!= null ){
            invoker.AddTileWasClickedListener( tileWasClickedListener );
        }
    }
    public static void AddTileWasClickedListener( UnityAction<int, int> a){
        tileWasClickedListener += a;
        if( tileWasClickedInvoker!= null){
            tileWasClickedInvoker.AddTileWasClickedListener( tileWasClickedListener );
        }
    }

    public static void AddGameoverInvoker( MainBoard invoker ){
        gameoverInvoker = invoker;
        if( gameOverListener!= null){
            invoker.AddGameoverListener( gameOverListener );
        }
    }
    public static void AddGameoverListener( UnityAction<int> listener){
        gameOverListener = listener;
        if( gameoverInvoker!=null ){
            gameoverInvoker.AddGameoverListener( gameOverListener );
        }
    }
}
