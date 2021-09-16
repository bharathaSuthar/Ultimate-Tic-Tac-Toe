using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI : Player
{
    [SerializeField] MainBoard mainBoard;
    protected bool isNextMoveFound;
    // Start is called before the first frame update
    private void Awake() {
        isNextMoveFound = false;
    }
    void Start()
    {
        playerMadeAMove = new PlayerMadeAMove();
        playerName = "@*@";
        title = "AI";
        EventManager.AddPlayerMadeAMoveInvoker(this);
        mainBoard = GameObject.FindGameObjectWithTag("MainBoard").GetComponent<MainBoard>();
        SetupProfile();
    }

    // Update is called once per frame
    void Update()
    {
        if(isPlaying){
            if( !isNextMoveFound && !EventManager.isGameOver ){
                isNextMoveFound = true;
                StartCoroutine( MakeNextMove(EventManager.lastMove) );
            }
        }
    }
    IEnumerator MakeNextMove( Move lastMove){
        //Debug.LogFormat("It's AIs turn");
        //Debug.LogFormat("last Move is: {0}, {1}", lastMove.row, lastMove.col);
        //Debug.LogFormat("AI is finding a move");
        Move nextMove = mainBoard.FindBestMove( EventManager.lastMove );
        //Debug.LogFormat("AI is playing, move: {0},{1}", nextMove.row, nextMove.col);
        yield return new WaitForSeconds(1);
        Tile targetTile = mainBoard.miniBoards[ nextMove.row/3, nextMove.col/3 ].children[ nextMove.row%3, nextMove.col%3];
        targetTile.PlayAMove();
        isPlaying = false;
        isNextMoveFound = false;

        //mainBoard.PlayMove(move, Icon);
        playerMadeAMove.Invoke( lastMove.row, lastMove.col);
    }
}
