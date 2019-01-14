using System;
using System.Collections;
using System.Collections.Generic;
using HoloToolkit.Unity;
using HoloToolkit.Unity.InputModule;
using UnityEngine;
using NetClientLib;
using UnityEngine.Rendering;


class BodyPosition : MonoBehaviour
{
    #region Fields

    private string _serverIp = "http://192.168.0.185:56789/"; // "http://192.168.0.59:56789/"
    private Quaternion cameraRotation;
    private Vector3 cameraPostion;
    private IRemoteService iRemoteService;
    private float currentDistanceToHead = 0;
    private float distanceToHead = 0;
    private readonly float factor = 0.005f;
    private float cubeScale = SKELETONCUBESCALE;
    private const float GLOBALSCALE = 1.5F;

    #endregion

    #region Properties

    public Color pointColor = Color.white;
    public GameObject refCube;
    public bool detailedFace = false;
    public bool freeze = false;
    public bool skeletonMode = true;
    public const float SKELETONCUBESCALE = 0.2F;
    public const float POINTCUBESCALE = 0.04F;
    GameObject[] bodyPoints;
    int width = 256;
    int height = 212;

    #endregion

    #region UnityMethods

    void Start()
    {
        iRemoteService = RemoteServiceBuilder.GetRemoteService(this, _serverIp);
        cameraPostion = Camera.main.transform.position;
        bodyPoints = new GameObject[1000];

        for (int i = 0; i < bodyPoints.Length; i++)
        {
            bodyPoints[i] = Instantiate(refCube) as GameObject;
            bodyPoints[i].SetActive(false);
            bodyPoints[i].transform.parent = gameObject.transform;
        }

        var localScale = gameObject.transform.localScale;
        localScale = localScale.Div(new Vector3(22.5F, 22.5F, 22.5F));
        gameObject.transform.localScale = localScale;
    }

    void Update()
    {
        if (freeze)
        {
            return;
        }

        MaterialPropertyBlock props = new MaterialPropertyBlock();
        IList<CloudPoint> cloudpoint = null;

        cloudpoint = skeletonMode ? iRemoteService.GetSkeleton() : iRemoteService.GetCloudpoints();
        pointColor.a = 0;
        float max = 0;
        int i;
        int bodyPointIndex;

        int stepRange = 1;
        if (detailedFace)
        {
            stepRange = cloudpoint.Count / bodyPoints.Length;
        }

        for (bodyPointIndex = 0, i = 0;
            i < cloudpoint.Count && bodyPointIndex < bodyPoints.Length;
            i += stepRange, bodyPointIndex++)
        {
            CloudPoint item = cloudpoint[i];
            float x, y, z;
            float secondFactor;
            pointColor.r = item.GetR();
            pointColor.g = item.GetG();
            pointColor.b = item.GetB();
            bodyPoints[bodyPointIndex].SetActive(true);

            if (skeletonMode)
            {
                z = item.GetZ() * 10;
                x = (item.GetX() * 10);
                y = (item.GetY() * 10);
            }
            else
            {
                z = item.GetZ() / 100;
                secondFactor = z * factor;
                x = (item.GetX() - width / 2.0f) * secondFactor; // x und y mal z genommen
                y = -(item.GetY() - height / 2.0f) * secondFactor; //180° Drehung
            }

            // y - 2 für Cameraanpassung bei reset
            bodyPoints[bodyPointIndex].transform.localPosition = new Vector3(x, y - 2, z - distanceToHead);
            bodyPoints[bodyPointIndex].transform.localScale = new Vector3(cubeScale, cubeScale, cubeScale);
            props.SetColor("_Color", pointColor);
            bodyPoints[bodyPointIndex].GetComponent<MeshRenderer>().SetPropertyBlock(props);

            if (max < y)
            {
                max = y;
                currentDistanceToHead = z;
            }
        }

        pointColor.a = 1;
        for (int j = bodyPointIndex; j < bodyPoints.Length; j++)
        {
            bodyPoints[j].SetActive(false);
        }
    }

    #endregion

    #region VoiceCommands

    public void resetPosition()
    {
        cameraPostion = Camera.main.transform.position;
        cameraRotation = Camera.main.transform.rotation;
        distanceToHead = currentDistanceToHead;
        var angles = cameraRotation.eulerAngles;
        angles.y += 180;
        angles.x = 0;
        angles.z = 0;
        var localPosition = gameObject.transform.localPosition;
        localPosition.z = currentDistanceToHead;
        gameObject.transform.position = cameraPostion;
        gameObject.transform.rotation = Quaternion.Euler(angles);
    }

    public void makeBigger()
    {
        var localScale = gameObject.transform.localScale;
        localScale = localScale.Mul(new Vector3(GLOBALSCALE, GLOBALSCALE, GLOBALSCALE));
        gameObject.transform.localScale = localScale;
        cubeScale = cubeScale * 1.1F;
    }

    public void makeSmaller()
    {
        var localScale = gameObject.transform.localScale;
        localScale = localScale.Div(new Vector3(GLOBALSCALE, GLOBALSCALE, GLOBALSCALE));
        gameObject.transform.localScale = localScale;
        cubeScale = cubeScale / 1.1F;
    }

    public void makeRotation()
    {
        var angles = gameObject.transform.eulerAngles;
        angles.y += 90;
        gameObject.transform.rotation = Quaternion.Euler(angles);
    }

    public void toggleDetailedFace()
    {
        detailedFace = !detailedFace;
    }

    public void freezePoints()
    {
        freeze = true;
    }

    public void unfreezePoints()
    {
        freeze = false;
    }

    public void SkeletonMode()
    {
        skeletonMode = !skeletonMode;
        if (skeletonMode)
        {
            cubeScale = SKELETONCUBESCALE;
        }
        else
        {
            cubeScale = POINTCUBESCALE;
        }

        detailedFace = false;
    }

    #endregion
}