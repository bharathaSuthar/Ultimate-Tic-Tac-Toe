using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Player : MonoBehaviour
{
    public string playerName, title;
    [SerializeField] protected GameObject[] symbols, profileBackgrounds, statusSprites, titleSprites;
    protected GameObject waitingSprite, yourTurnSprite, titleSprite;
    private SYMBOL icon;
    float profileSize;
    private char charIcon;
    public bool isPlaying;
    public PlayerMadeAMove playerMadeAMove;
    void Start()
    {   
        playerMadeAMove = new PlayerMadeAMove();
        EventManager.AddPlayerMadeAMoveInvoker(this);
        playerName = "Default Name";
        title = "HUMAN";
        SetupProfile();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void SwitchSprites(){
        if(isPlaying){
            yourTurnSprite.SetActive(true);
            waitingSprite.SetActive(false);
        }
        else{
            yourTurnSprite.SetActive(false);
            waitingSprite.SetActive(true);
        }
    }
    public SYMBOL Icon{
        set{
            icon = value;
        }
        get{
            return icon;
        }
    }
    public char CharIcon{
        set{
            charIcon = new char[]{'O', 'X', '_'}[ (int)Icon ];
        }
        get{
            return charIcon;
        }
    }
    public void AddPlayerMadeAMoveListener(UnityAction<int, int> listener){
        playerMadeAMove.AddListener(listener);
    }
    public void SetupProfile(){
        profileSize = ScreenUtils.ScreenHeight / 5;
        GameObject pb = Instantiate( profileBackgrounds[1] );
        GameObject symbol;
        
        pb.GetComponent<SpriteRenderer>().size = profileSize * Vector2.one;
        if(Icon == SYMBOL.O){
            symbol = Instantiate( symbols[0] );
        }
        else{
            symbol = Instantiate( symbols[1] );
        }
        yourTurnSprite = Instantiate( statusSprites[1] );
        waitingSprite = Instantiate( statusSprites[0] );
        titleSprite = Instantiate( titleSprites[0] );

        symbol.transform.position = transform.position + new Vector3( 0, 0, -0.2f);
        pb.transform.position = transform.position + new Vector3( 0, 0, -0.1f);
        yourTurnSprite.transform.position = transform.position + new Vector3(0, -profileSize, 0);
        titleSprite.transform.position = transform.position + new Vector3(0, -profileSize*0.75f, 0);
        waitingSprite.transform.position = yourTurnSprite.transform.position;

        symbol.transform.parent = transform;
        pb.transform.parent = transform;
        yourTurnSprite.transform.parent = transform;
        waitingSprite.transform.parent = transform;
        titleSprite.transform.parent = transform;

        symbol.GetComponent<SpriteRenderer>().size = profileSize * Vector2.one * 0.75f;
        Vector2 sSize = yourTurnSprite.GetComponent<SpriteRenderer>().size;
        Vector2 tSize = titleSprite.GetComponent<SpriteRenderer>().size;
        yourTurnSprite.GetComponent<SpriteRenderer>().size = new Vector2( profileSize, sSize.y/sSize.x*profileSize );
        titleSprite.GetComponent<SpriteRenderer>().size = new Vector2( profileSize, tSize.y/tSize.x*profileSize );
        waitingSprite.GetComponent<SpriteRenderer>().size = yourTurnSprite.GetComponent<SpriteRenderer>().size ;
        SwitchSprites();
    }
}
