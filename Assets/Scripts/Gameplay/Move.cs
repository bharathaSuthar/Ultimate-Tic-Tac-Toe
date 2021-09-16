using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move
{
    public int row, col;
    public Move(){ row = -1; col = -1;}
    public Move(int r, int c) { row = r; col = c;}
    public void ShowMove(){
        Debug.LogFormat("Move played is: {0} {1}", row, col);
    }
}
