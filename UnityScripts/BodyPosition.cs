using System;
using System.Collections;
using System.Collections.Generic;
using HoloToolkit.Unity.InputModule;
using UnityEngine;
using NetClientLib;

public class BodyPosition : MonoBehaviour, ISpeechHandler
{
    private Renderer renderer;
    private Quaternion cameraRotation;
    public Color pointColor = Color.white;
    private Vector3 cameraPostion;
    private IRemoteService iRemoteService;

    private float offset = 0.5f;

    //private float scaleOffset = 0.005F;
    private float scaleOffset = 0.5F;

    GameObject[] bodyPoints;
    //public GameObject cube;

    public void setScale(float currentScale)
    {
        scaleOffset = scaleOffset + currentScale;
    }


    void Start()
    {
        try
        {
            renderer = GetComponent<Renderer>();
            renderer.material.color = Color.blue;

            //iRemoteService = RemoteServiceBuilder.GetRemoteService("http://192.168.0.185:56789/");
            iRemoteService = RemoteServiceBuilder.GetRemoteService();
            cameraPostion = Camera.main.transform.position;

            int width = 256;
            int height = 212;

            //bodyPoints = new GameObject[256 * 212];
            bodyPoints = new GameObject[10];

            for (int i = 0; i < bodyPoints.Length; i++)
            {
                bodyPoints[i] = GameObject.CreatePrimitive(PrimitiveType.Cube);
                //bodyPoints[i].AddComponent<OnClick>();
                bodyPoints[i].SetActive(false);
                bodyPoints[i].transform.parent = gameObject.transform;
            }

            renderer.material.color = Color.green;
        }
        catch (Exception e)
        {
            renderer.material.color = Color.red;
        }

        //UpdateBody();
    }


    // Update is called once per frame
    void Update()
    {
        UpdateBody();
    }

    IEnumerator TwoSeconds()
    {
        //StartCoroutine(TwoSeconds());
        //in anderem programm Startcourinte
        yield return new WaitForSeconds(5);
    }

    void UpdateBody()
    {
        IList<CloudPoint> cloudpoint = null;
        cloudpoint = iRemoteService.GetCloudpoints();
        int i = 0;
        pointColor.a = 0;
        foreach (var item in cloudpoint)
        {
            pointColor.r = item.GetR();
            pointColor.g = item.GetG();
            pointColor.b = item.GetB();
            bodyPoints[i].SetActive(true);
            bodyPoints[i].transform.localPosition = new Vector3(item.GetX() / item.GetZ() * 20 * offset - 1.3F,
                -item.GetY() / item.GetZ() * 20 * offset + 0.5F,
                item.GetZ() / 180 * offset - 2.6F);

            /*
             * bodyPoints[i].transform.localPosition = new Vector3(item.GetX()/item.GetZ() / 50 * offset - 1.3F,
             *   -item.GetY()/item.GetZ() / 50 * offset + 0.5F,
             *   item.GetZ() / 200 * offset - 2.6F);
             */

            bodyPoints[i].transform.localScale = new Vector3(scaleOffset, scaleOffset, scaleOffset);
            bodyPoints[i].GetComponent<Renderer>().material.color = pointColor;


            i++;
            if (i >= bodyPoints.Length)
            {
                break;
            }
        }

        pointColor.a = 1;
        for (int j = i; j < bodyPoints.Length; j++)
        {
            bodyPoints[j].SetActive(false);
        }
    }


    public void resetPosition()
    {
        cameraPostion = Camera.main.transform.position;
        cameraRotation = Camera.main.transform.rotation;
        gameObject.transform.position = cameraPostion;
        var angles = cameraRotation.eulerAngles;
        angles.y += 180;
        gameObject.transform.rotation = Quaternion.Euler(angles);
    }

    public void OnSpeechKeywordRecognized(SpeechEventData eventData)
    {
        resetPosition();
    }
}