using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ScreenUtils
{
    #region Fields

    static int screenWidth;
    static int screenHeight;
    static float screenLeft;
    static float screenRight;
    static float screenTop;
    static float screenBottom;

    #endregion

    #region Properties

    public static float ScreenLeft
    {
        get
        {
            CheckScreenSizeChanged();
            return screenLeft;
        }
    }

    public static float ScreenRight
    {
        get
        {
            CheckScreenSizeChanged();
            return screenRight;
        }
    }
    public static float ScreenTop
    {
        get
        {
            CheckScreenSizeChanged();
            return screenTop;
        }
    }

    public static float ScreenBottom
    {
        get 
        {
            CheckScreenSizeChanged();
            return screenBottom; 
        }
    }
    public static float ScreenWidth{
        get{
            CheckScreenSizeChanged();
            return ScreenRight - ScreenLeft;
        }
    }
    public static float ScreenHeight{
        get{
            CheckScreenSizeChanged();
            return ScreenTop - ScreenBottom;
        }
    }

    #endregion

    #region Methods
    public static void Initialize()
    {
        // save to support resolution changes
        screenWidth = Screen.width;
        screenHeight = Screen.height;

        // save screen edges in world coordinates
        float screenZ = -Camera.main.transform.position.z;
        Vector3 lowerLeftCornerScreen = new Vector3(0, 0, screenZ);
        Vector3 upperRightCornerScreen = new Vector3(
            screenWidth, screenHeight, screenZ);
        Vector3 lowerLeftCornerWorld =
            Camera.main.ScreenToWorldPoint(lowerLeftCornerScreen);
        Vector3 upperRightCornerWorld =
            Camera.main.ScreenToWorldPoint(upperRightCornerScreen);
        screenLeft = lowerLeftCornerWorld.x;
        screenRight = upperRightCornerWorld.x;
        screenTop = upperRightCornerWorld.y;
        screenBottom = lowerLeftCornerWorld.y;
    }

    static void CheckScreenSizeChanged()
    {
        if (screenWidth != Screen.width ||
            screenHeight != Screen.height)
        {
            Initialize();
        }
    }

    #endregion
}
