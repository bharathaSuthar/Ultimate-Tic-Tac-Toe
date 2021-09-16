using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

public class MainBoard : MiniBoard
{
    [SerializeField] GameObject miniBoard;
    public MiniBoard[, ] miniBoards;
    GameOverEvent gameOverEvent;

    // Update is called once per frame
    void Update()
    {
        
    }
    protected override void OnMouseOver() {
        
    }
    protected override void OnMouseExit() {
        
    }
    
    override protected void Initialize()
    {
        gameOverEvent = new GameOverEvent();
        EventManager.AddGameoverInvoker(this);
        EventManager.AddPlayerMadeAMoveListener(FindNextPlayableBoards);
        currentStep = 0;
        maxSteps = 81;
        winner = SYMBOL.None;
        this.size = (float)Mathf.Min( ScreenUtils.ScreenWidth, ScreenUtils.ScreenHeight) * 0.8f;
        boardToTileRatio = 3.3f;

        backgroundSprite.sprite = backgroundSprites[0];
        backgroundSprite.size = size * Vector2.one;
        boxCollider2D.size = size * Vector2.one;
        inactiveSpriteObject.GetComponent<SpriteRenderer>().size = size * Vector2.one;
        inactiveSpriteObject.SetActive(false);
        inactiveSpriteObject.GetComponent<BoxCollider2D>().size = size * Vector2.one;

        AddMiniBoards();
    }
    public void FindNextPlayableBoards( int row, int col){
        //Debug.LogFormat("Player Made a move event called me, FindNextPlayalbeBoards");
        //ShowBoard();
        Move lastMove = new Move( row, col);
        EventManager.nextPlayableBoards = GetNextBoardsToPlay( lastMove );
        if(EventManager.nextPlayableBoards.Count == 0){
            gameOverEvent.Invoke((int)Winner);
        }
        foreach( var board in miniBoards){
            if( board.isActive ){
                if(!EventManager.nextPlayableBoards.Contains(board)){
                    board.isActive = false;
                    board.inactiveSpriteObject.SetActive(true);
                }
            }
            else{
                if(EventManager.nextPlayableBoards.Contains(board)){
                    board.isActive = true;
                    board.inactiveSpriteObject.SetActive(false);
                }
            }
        }
    }
    public char GetSubValue( int x, int y){
        int index = (int)miniBoards[ x/3, y/3 ].children[x%3, y%3].winner;
        char[] icons = new char[]{'O', 'X', '_', '_'};
        return icons[index];
    }
    public void ShowBoard(){
        for(int i=0; i<9; i++){
            //if ( i%3==0 ) Debug.LogFormat(". . . . . . . . . . . . .");
            string s = "";
            for(int j=0; j<9; j++){
                if ( j%3==0 ) s+="  |  ";
                s += " "+ GetSubValue(i, j).ToString() + " ";
                //Debug.LogFormat("{0} ", GetSubValue(i, j));
            }
            Debug.LogFormat(s + "  |  \n");
        }
        //Console.Error.WriteLine(". = = = . = = = . = = = .");
        ///Debug.LogFormat(". . . . . . . . . . . . .");
    }
    public override void PlayMove(Move move, SYMBOL w)
    {
        currentStep++;
        Move moveInMiniBoard = new Move( move.row%3, move.col%3 );
        MiniBoard miniBoardToMoveOn = miniBoards[ move.row/3, move.col/3];
        miniBoardToMoveOn.PlayMove(moveInMiniBoard, w);
    }
    public override void ReverseMove(Move move)
    {
        currentStep--;
        Move moveInMiniBoard = new Move( move.row%3, move.col%3 );
        MiniBoard miniBoardToMoveOn = miniBoards[ move.row/3, move.col/3];
        miniBoardToMoveOn.ReverseMove(moveInMiniBoard);
    }
    public void AddGameoverListener( UnityAction<int> listener){
        gameOverEvent.AddListener(listener);
    }
    void AddMiniBoards(){
        miniBoards = new MiniBoard[3, 3];
        children = new Tile[3, 3];
        for( int i= -1; i<2; i++){
            for( int j = -1; j<2; j++){
                Vector3 boardPosition = transform.position;
                Vector2 miniBoardSize = Vector2.one * size /boardToTileRatio;
                boardPosition.x += i* (miniBoardSize.x * 1.05f );
                boardPosition.y += j* (miniBoardSize.y * 1.05f );
                boardPosition.z += -0.1f;

                GameObject miniBoardObj = Instantiate( miniBoard, boardPosition, Quaternion.identity );
                miniBoardObj.transform.parent = transform;
                MiniBoard miniBoardScipt = miniBoardObj.GetComponent<MiniBoard>();
                miniBoardScipt.row = i + 1;
                miniBoardScipt.col = j + 1;
                miniBoards[ i+1, j + 1] = miniBoardScipt;
                children[ i+1, j + 1] = miniBoardScipt;
            }
        }
    }
    public Move ConvertMoveToBigBoard( MiniBoard board, Move m){
        return new Move( (m.row + 3*board.row)%9, (m.col + 3*board.col)%9 );
    }
    public List<MiniBoard> GetNextBoardsToPlay( Move lastMove ){
        List<MiniBoard> nextBoards = new List<MiniBoard>();
        if(lastMove.col == -1){
            foreach(var b in miniBoards){
                nextBoards.Add(b);
            }
        }
        else{
            MiniBoard nextBoard = miniBoards [ lastMove.row%3, lastMove.col%3];
            if( !nextBoard.AnyMovesLeft() || nextBoard.winner!=SYMBOL.None ){
                //Console.Error.WriteLine("nextBoard is blocked, finding another");
                for( int i=0; i<3; i++){
                    for(int j=0; j<3; j++){
                        if( miniBoards[i,j].AnyMovesLeft() && !miniBoards[i,j].hasOwner){
                            nextBoard = miniBoards[i, j];
                            nextBoards.Add(nextBoard);
                        }
                    }
                }
            }
            else{
                nextBoards.Add(nextBoard);
            }
        }
        return nextBoards;
    }
    public int Minimax( int depth, bool isMaximising, Move lastMove){
        bool debug = false;
        if(debug) Debug.LogFormat("starting Minimax on lastMove: {0},{1}", lastMove.row, lastMove.col);
        if(debug) Debug.LogFormat("starting Minimax with depth: {0} & is me: {1}", depth, isMaximising);
        if(depth > 2){
            int score =  EstimateScore(lastMove);
            if(debug) Debug.LogFormat("Depth reached: score: {0}", score);
            return score;
        } 
        SYMBOL winner = GetWinner();
        if( winner == EventManager.previousPlayer.Icon){
            //Debug.LogFormat("opponent is winner, returning -100");
            return -111;
        }
        else if ( winner == EventManager.currentSymbol){
            //Debug.LogFormat("player is winner, returning 100");
            return 111;
        }
        if(!AnyMovesLeft()){
            int score =  EstimateScore(lastMove);
            //Debug.LogFormat("Noone is winner, returning: {0}", score);
            return score;
        }
        
        int index = isMaximising? 1: 0;
        int bestScore = new List<int> {10000, -10000} [index];
        List<MiniBoard> nextBoards = GetNextBoardsToPlay(lastMove);
        if(debug) Console.Error.WriteLine("#nextBoards found: {0}", nextBoards.Count);
        Dictionary<MiniBoard, Move> bestMoves = new Dictionary<MiniBoard, Move>();

        foreach(var board in nextBoards){
            if(debug) Debug.LogFormat("Showing nextBoard:");
            if( board.Winner == SYMBOL.None ){
                Move bestMoveForCurrentBoard = new Move();
                if( !bestMoves.ContainsKey(board) ){
                    bestMoves.Add( board, ConvertMoveToBigBoard( board, board.FindBestMove() ));
                }
                bestMoveForCurrentBoard = bestMoves[board];
                for( int i = 0; i<3; i++){
                    for( int j = 0; j<3; j++){
                        if( board.children[i,j].winner == SYMBOL.None ){
                            // make a move
                            Move nextMoveInBoard = new Move(i, j);
                            Move nextMove = ConvertMoveToBigBoard(board, nextMoveInBoard);
                            SYMBOL nextPlayer = new SYMBOL[]{ EventManager.previousSymbol, EventManager.currentSymbol }[index];

                            PlayMove( nextMove, nextPlayer );
                            int thisMoveScore = Minimax(depth+1, !isMaximising, nextMove);

                            if( bestMoveForCurrentBoard.row == nextMove.row && bestMoveForCurrentBoard.col == nextMove.col){
                                thisMoveScore += 4;
                            }
                            else{
                                thisMoveScore -= 2;
                            }
                            

                            if(debug) Debug.LogFormat("Next move is: {0}, {1}", nextMove.row, nextMove.col);
                            if( index == 1){
                                bestScore = Math.Max( bestScore, thisMoveScore );
                            }
                            else{
                                bestScore = Math.Min( bestScore, thisMoveScore );
                            }
                            //reverse the move
                            ReverseMove( nextMove );
                            if(debug) Debug.LogFormat("Intermediate best score is: {0}", bestScore);
                        }
                    }
                }
            }
        }
        if(debug) Debug.LogFormat("Best Score we got is: {0}", bestScore);
        return bestScore;
    }
    public Move FindBestMove( Move lastMove){
        bool debug = false;
        if(debug) Debug.LogFormat("Finding BestMove on lastMove: {0},{1}", lastMove.row, lastMove.col);
        int bestVal = -1000;
        Move bestMove = new Move();
        List<MiniBoard> nextBoards = GetNextBoardsToPlay(lastMove);
        if( lastMove.row == -1 || lastMove.col == -1 ){
            return nextBoards[ UnityEngine.Random.Range(0, nextBoards.Count) ].FindBestMove();
        }
        int depth = 0;

        foreach( MiniBoard currentBoard in nextBoards){

            Move bestMoveForCurrentBoard = currentBoard.FindBestMove();
            if( currentBoard.IsBoardEmpty() ) depth = 1;

            foreach ( Tile tileBoard in currentBoard.children){
                Move nextMove = new Move();
                if( !tileBoard.hasOwner ){
                    nextMove = new Move( currentBoard.row*3 + tileBoard.row, currentBoard.col*3 + tileBoard.col);
                }
                else{
                    continue;
                }
                PlayMove( nextMove, EventManager.currentSymbol );
                int moveVal = Minimax(depth, false, nextMove);
                ReverseMove ( nextMove );

                if(debug) Debug.LogFormat("Move value before: {0} ", moveVal);       
                if(debug) Debug.LogFormat("best Move in currentBoard is: {0},{1}", bestMoveForCurrentBoard.row, bestMoveForCurrentBoard.col);
                if( bestMoveForCurrentBoard.row == nextMove.row && bestMoveForCurrentBoard.col == nextMove.col){
                    moveVal += 7;
                }
                else{
                    moveVal -= 2;
                }

                if(debug) Debug.LogFormat("next Move is: {0},{1}", nextMove.row, nextMove.col);
                if(debug) Debug.LogFormat("Move value after: {0} ", moveVal);       
                if (moveVal > bestVal)
                {
                    bestMove = nextMove;
                    bestVal = moveVal;
                }
                if(debug) Debug.LogFormat("Intermediate bestMove value: {0} ", bestVal);       
            }

        }
        Debug.LogFormat("Best Move found is: {0},{1}", bestMove.row, bestMove.col);
        //ShowBoard();  
        return bestMove;
    }
    public int EstimateScore( Move lastMove){
        int score = 0;
        MiniBoard currentBoard = miniBoards[ lastMove.row%3, lastMove.col%3];
        if(currentBoard.winner == SYMBOL.None){
            score += 9; //always find boards which are no won, else opponent gets more options to choose from
        }
        foreach( var board in miniBoards){
            if(board.Winner == EventManager.currentSymbol){
                score += 11;
            }
            else if(board.Winner == EventManager.previousPlayer.Icon){
                score -= 11;
            }
        }
        return score;
    }
    public override void UpdateYourself(){
        if( Winner != SYMBOL.None){
            foreach( var child in children){
                //child.winner = Winner;
                child.hasOwner = true;

                SpriteRenderer[] spritesToFade = child.gameObject.GetComponentsInChildren<SpriteRenderer>();
                foreach( var s in spritesToFade){
                    Color color = s.color;
                    color.a = color.a / 4;
                    s.color = color;
                }
            }
            if( winner != SYMBOL.Tie){
                Debug.LogFormat("{0} won", (int)winner);
                //AddSymbol(winner);
            }
            hasOwner = true;
            inactiveSpriteObject.SetActive(true);
            gameOverEvent.Invoke((int)winner);
        }
    }
    protected override SYMBOL GetWinner(){
        SYMBOL tempWinner = winner;

        if( currentStep >= maxSteps ){
            tempWinner = SYMBOL.Tie;
        }
        for(int r=0; r<3; r++){
            if( miniBoards[r,0].winner!=SYMBOL.None && miniBoards[r,0].winner==miniBoards[r,1].winner && miniBoards[r,0].winner==miniBoards[r,2].winner ){
                tempWinner = miniBoards[r,0].winner;
                return tempWinner;
            }
        }
        for(int c=0; c<3; c++){
            if( miniBoards[0, c].winner!=SYMBOL.None && miniBoards[0, c].winner==miniBoards[1, c].winner && miniBoards[0, c].winner==miniBoards[2, c].winner ){
                tempWinner = miniBoards[0, c].winner;
                return tempWinner;
            }
        }
        if( miniBoards[0, 0].winner!=SYMBOL.None && children[0, 0].winner== miniBoards[1, 1].winner && miniBoards[0, 0].winner == miniBoards[2, 2].winner){
            tempWinner =  miniBoards[1, 1].winner;
        }
        else if( miniBoards[1, 1].winner!=SYMBOL.None && miniBoards[2, 0].winner== miniBoards[1, 1].winner && miniBoards[1, 1].winner == miniBoards[0, 2].winner){
            tempWinner =  miniBoards[1, 1].winner;
        }
        return tempWinner;
    }
}
