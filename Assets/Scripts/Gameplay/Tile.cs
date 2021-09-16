using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Tile : MonoBehaviour
{
    [SerializeField] protected GameObject[] symbols;
    public bool hasOwner;
    public int row, col;
    public SYMBOL winner;
    public int maxSteps, currentStep;
    [SerializeField] protected Sprite[] backgroundSprites;
    protected SpriteRenderer backgroundSprite;
    protected BoxCollider2D boxCollider2D;
    public float size, tileToSymbolRatio;
    TileWasClicked tileWasClicked;
    private void Awake() {
        tileWasClicked = new TileWasClicked();
        EventManager.AddTileWasClickedListener(this);

        boxCollider2D = GetComponent<BoxCollider2D>();
        backgroundSprite = GetComponent<SpriteRenderer>();
    }
    protected void Start()
    {
        Initialize();
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    protected virtual void OnMouseOver() {
        if(!hasOwner){
            backgroundSprite.sprite = backgroundSprites[1];

            if(Input.GetMouseButtonDown(0) && EventManager.currentPlayer.title=="HUMAN"){
                PlayAMove();
            }
        }
    }
    public void PlayAMove(){
        //Debug.LogFormat("Playing a move on Tile: {0}, {1}", row, col);
        hasOwner = true;
        AddSymbol( EventManager.currentSymbol );
        winner = EventManager.currentSymbol;
        
        Move moveMade = new Move( row, col);
        MiniBoard m = this.transform.parent.GetComponent<MiniBoard>();
        m.PlayMove( moveMade, winner );
        m.UpdateYourself();
        tileWasClicked.Invoke( m.row*3 + row, m.col*3 + col);
    }
    protected virtual void OnMouseExit() {
        if(!hasOwner){
            backgroundSprite.sprite = backgroundSprites[0];
        }
    }
    public void AddTileWasClickedListener(UnityAction<int, int> listener){
        tileWasClicked.AddListener(listener);
    }
    virtual protected void Initialize(){
        currentStep = 0;
        maxSteps = 1;
        winner = SYMBOL.None;
        size = (float)Mathf.Min( ScreenUtils.ScreenWidth, ScreenUtils.ScreenHeight) * 0.25f / 3.3f;
        backgroundSprite.sprite = backgroundSprites[0];
        backgroundSprite.size = size * Vector2.one;
        boxCollider2D.size = size * Vector2.one;
    }
    public void AddSymbol( SYMBOL sym ){
        Vector3 symbolPosition = transform.position;
        symbolPosition.z -= 0.1f * size ;

        GameObject symbol = Instantiate( symbols[ (int)sym ], symbolPosition, Quaternion.identity );
        symbol.GetComponent<SpriteRenderer>().size = size * Vector2.one/1.5f ;
        symbol.AddComponent<BoxCollider2D>();
        BoxCollider2D symbolBox = symbol.GetComponent<BoxCollider2D>();
        symbolBox.size = size * Vector2.one;
        symbol.transform.parent = transform;
    }
}
