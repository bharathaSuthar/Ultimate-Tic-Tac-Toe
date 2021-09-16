using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MiniBoard : Tile
{
    [SerializeField] protected GameObject tile;
    public float boardToTileRatio;
    public GameObject inactiveSpriteObject;
    public bool isActive;
    public Tile[,] children;
    public SYMBOL Winner{
        get{
            if( winner == SYMBOL.None ){
                winner = GetWinner();
            }
            return winner;
        }
    }
    public virtual void PlayMove( Move move, SYMBOL w){
        children[ move.row%3, move.col%3 ].winner = w;
        currentStep++;
        GetWinner();
    }
    public virtual void ReverseMove( Move move){
        children[ move.row, move.col ].winner = SYMBOL.None;
        currentStep--;
        winner = SYMBOL.None;
    }
    public bool AnyMovesLeft(){
        if( currentStep < 9 ){
            return true;
        }
        else{
            return false;
        }
    }
    public bool IsBoardEmpty(){
        if( currentStep == 0){
            return true;
        }
        else{
            return false;
        }
    }
    void Update()
    {
        
    }
    protected override void OnMouseOver() {
        
    }
    protected override void OnMouseExit() {
        
    }
    public virtual void UpdateYourself(){
        if( Winner != SYMBOL.None){
            Debug.LogFormat("UpdateYourself function called for Winner");
            foreach( var child in children){
                //child.winner = winner;
                child.hasOwner = true;
                
                SpriteRenderer[] spritesToFade = child.gameObject.GetComponentsInChildren<SpriteRenderer>();
                foreach( var s in spritesToFade){
                    Color color = s.color;
                    color.a = color.a / 4;
                    s.color = color;
                }
            }
            AddSymbol(winner);
            hasOwner = true;
            transform.parent.GetComponent<MainBoard>().UpdateYourself();
        }
    }
    override protected void Initialize(){
        currentStep = 0;
        maxSteps = 9;
        winner = SYMBOL.None;
        isActive = true;
        size = (float)Mathf.Min( ScreenUtils.ScreenWidth, ScreenUtils.ScreenHeight) * 0.25f;
        boardToTileRatio = 3.3f;
        backgroundSprite.sprite = backgroundSprites[0];
        backgroundSprite.size = size * Vector2.one;
        boxCollider2D.size = size * Vector2.one;

        inactiveSpriteObject.GetComponent<SpriteRenderer>().size = size * Vector2.one;
        inactiveSpriteObject.SetActive(false);
        inactiveSpriteObject.GetComponent<BoxCollider2D>().size = size * Vector2.one;

        AddTiles();
    }
    void AddTiles(){
        children = new Tile[3,3];
        for( int i= -1; i<2; i++){
            for( int j = -1; j<2; j++){
                Vector3 tilePosition = transform.position;
                Vector2 tileSizeToPut = Vector2.one * size/boardToTileRatio;
                tilePosition.x += i* (tileSizeToPut.x * 1.05f );
                tilePosition.y += j* (tileSizeToPut.y * 1.05f );
                tilePosition.z += -0.1f;

                GameObject tileObj = Instantiate( tile, tilePosition, Quaternion.identity );
                tileObj.transform.parent = transform;
                Tile tileObjScript = tileObj.GetComponent<Tile>();
                tileObjScript.row = i + 1;
                tileObjScript.col = j + 1;
                children[ i + 1, j + 1] = tileObjScript;
            }
        }
    }
    protected virtual SYMBOL GetWinner(){
        bool debug = false;
        SYMBOL localWinner = winner;
        if( currentStep >= maxSteps ){
            localWinner = SYMBOL.Tie;
        }
        for(int r=0; r<3; r++){
            if( children[r,0].winner!=SYMBOL.None && children[r,0].winner==children[r,1].winner && children[r,0].winner==children[r,2].winner ){
                localWinner = children[r,0].winner;
                if(debug) Debug.LogFormat( "A row: {1} matched, winner is {0}", (int)localWinner, r );
                if(debug) Debug.LogFormat("TileWineers are: {0}, {1}, {2} ",children[r, 0].winner, children[r, 1].winner, children[r, 2].winner );
                return localWinner;
            }
        }
        for(int c=0; c<3; c++){
            if( children[0, c].winner!=SYMBOL.None && children[0, c].winner==children[1, c].winner && children[0, c].winner==children[2, c].winner ){
                localWinner = children[0, c].winner;
                if(debug) Debug.LogFormat( "A col: {1} matched, winner is {0}", (int)localWinner, c );
                if(debug) Debug.LogFormat("TileWineers are: {0}, {1}, {2} ",children[0, c].winner, children[1, c].winner, children[2, c].winner );
                return localWinner;
            }
        }
        if( children[0, 0].winner!=SYMBOL.None && children[0, 0].winner== children[1, 1].winner && children[0, 0].winner == children[2, 2].winner){
            localWinner =  children[1, 1].winner;
            if(debug) Debug.LogFormat( "A diagonal \"/\" matched, winner is {0}", (int)localWinner );
            if(debug) Debug.LogFormat("TileWineers are: {0}, {1}, {2} ",children[0, 0].winner, children[1, 1].winner, children[2, 2].winner );
            return localWinner;
        }
        else if( children[1, 1].winner!=SYMBOL.None && children[2, 0].winner== children[1, 1].winner && children[1, 1].winner == children[0, 2].winner){
            localWinner =  children[1, 1].winner;
            if(debug) Debug.LogFormat( "A diagonal \"\\\" matched, winner is {0}", (int)localWinner );
            if(debug) Debug.LogFormat("TileWineers are: {0}, {1}, {2} ",children[2, 0].winner, children[1, 1].winner, children[0, 2].winner );
            return localWinner;
        }
        return localWinner;
    }
    public int Minimax( int depth, bool isMaximising){
        if(depth > 1) return 0;
        if( winner == EventManager.previousSymbol ){
            return -10;
        }
        else if ( winner == EventManager.currentSymbol ){
            //Console.Error.WriteLine("player is winner, returning 10");
            return 10;
        }
        if(!AnyMovesLeft()){
            //Console.Error.WriteLine("Noone is winner, returning 0");
            return 0;
        }
        int index = isMaximising? 1: 0;
        int bestScore = new List<int> {10000, -10000} [index];

        for (int i = 0; i<3; i++){
            for (int j = 0; j<3; j++){
                if (children[i,j].winner==SYMBOL.None){
                    Move miniMove = new Move(i, j);
                    SYMBOL newWinner = new SYMBOL[]{ EventManager.previousSymbol, EventManager.currentSymbol }[index];
                    // Make the move
                    PlayMove(miniMove, newWinner);

                    if( index == 1 ){
                        bestScore = Math.Max( bestScore, Minimax(depth+1, !isMaximising) );
                    }
                    else{
                        bestScore = Math.Min( bestScore, Minimax(depth+1, !isMaximising) );
                    }

                    // Undo the move
                    ReverseMove(miniMove);
                }
            }
        }
        //Console.Error.WriteLine("Best Score we got is: {0}", bestScore);
        return bestScore;
    }
    public Move FindBestMove(){
        int bestVal = -1000;
        Move bestMove = new Move();
        if( IsBoardEmpty() ){
            return new Move( UnityEngine.Random.Range(0,3), UnityEngine.Random.Range(0,3) );
        }
        for (int i = 0; i<3; i++){
            for (int j = 0; j<3; j++){
                // Check if cell is empty
                if (children[i, j].winner == SYMBOL.None )
                {
                    Move miniMove = new Move(i, j);
                    // Make the move
                    PlayMove( miniMove, EventManager.currentSymbol);

                    int moveVal = Minimax(0, false);
                    //Console.Error.WriteLine("Move value returned is: {0}", moveVal);
                    // Undo the move
                    ReverseMove(miniMove);

                    if (moveVal >= bestVal)
                    {
                        bestMove = new Move(i, j);
                        bestVal = moveVal;
                    }
                }
            }
        }
        return bestMove;
    }
}
