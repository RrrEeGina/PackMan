﻿using UnityEngine;
using System.Collections;

public class Ferr_LogoLock : MonoBehaviour
{
    private enum Ferr_LockPosition
    {
        Left,
        Center,
        Right
    }

    [SerializeField]
    private Camera mCamera;
    //[SerializeField]
    //private Ferr_LockPosition mLockHorizontal;
    //[SerializeField]
    //private Ferr_LockPosition mLockVertical;
    //[SerializeField]
    //private float mPadding;
    //[SerializeField]
    private float mScale = 1;

    void Start()
    {
        if (mCamera == null)
        {
            mCamera = Camera.main;
        }
        transform.parent = mCamera.transform;
        //transform.localPosition = GetLockPosition(mCamera, mLockHorizontal, mLockVertical, mPadding);
        float s = GetPixelScale(mCamera, GetComponent<SpriteRenderer>().sprite);
        transform.localScale = new Vector3(s, s, s) * mScale;
    }

    private static float GetPixelScale(Camera aCam, Sprite aSprite)
    {
        float result = 1;
        Vector2 viewSize = GetViewSizeAtDistance(1, aCam);
        float ratio = aSprite.textureRect.width / Screen.width;

        result = (viewSize.x * ratio) / (aSprite.bounds.extents.x * 2);

        return result;
    }

    private static Vector3 GetLockPosition(Camera aCam, Ferr_LockPosition aHLock, Ferr_LockPosition aVLock, float aPadding)
    {
        Vector3 result = Vector3.zero;
        Vector2 viewSize = GetViewSizeAtDistance(1, aCam);
        result.z = 1;
        aPadding = aPadding * ((1f / Screen.width) * viewSize.x);

        if (aHLock == Ferr_LockPosition.Left) result.x = -viewSize.x / 2 + aPadding;
        else if (aHLock == Ferr_LockPosition.Center) result.x = 0;
        else if (aHLock == Ferr_LockPosition.Right) result.x = viewSize.x / 2 - aPadding;

        if (aVLock == Ferr_LockPosition.Left) result.y = viewSize.y / 2 - aPadding;
        else if (aVLock == Ferr_LockPosition.Center) result.y = 0;
        else if (aVLock == Ferr_LockPosition.Right) result.y = -viewSize.y / 2 + aPadding;

        return result;
    }

    private static Vector2 GetViewSizeAtDistance(float aDist, Camera aCamera = null)
    {
        if (aCamera == null) aCamera = Camera.main;
        if (aCamera.orthographic)
            return new Vector2(((float)Screen.width / Screen.height) * aCamera.orthographicSize * 2, aCamera.orthographicSize * 2);
        float frustumHeight = 2f * aDist * Mathf.Tan(aCamera.fieldOfView * 0.5f * Mathf.Deg2Rad);
        return new Vector2(frustumHeight * aCamera.aspect, frustumHeight);
    }
}
