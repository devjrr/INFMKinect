using System;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using UnityEngine.Networking;

namespace NetClientLib
{
    class UnityRemoteService : IRemoteService
    {
        private IEnumerator<UnityWebRequestAsyncOperation> serverCall;

        private IList<CloudPoint> points = new List<CloudPoint>();

        private readonly MonoBehaviour monoBehaviour;
        private readonly Deserializer deserializer;
        private readonly string url;
        private readonly int width;
        private readonly int height;



        public UnityRemoteService(MonoBehaviour monoBehaviour, Deserializer deserializer, string url, int height, int width)
        {
            this.monoBehaviour = monoBehaviour;
            this.deserializer = deserializer;
            this.url = url;
            this.height = height;
            this.width = width;
            serverCall = CallServer(ProcessResult);
        }

        public IList<CloudPoint> GetCloudpoints()
        {
            if (serverCall.Current.isDone)
            {
                monoBehaviour.StartCoroutine(serverCall);
            }

            return points;
        }

        private void ProcessResult(string result)
        {
            points = deserializer.deserialze(result, height, width); ;
        }

        private IEnumerator<UnityWebRequestAsyncOperation> CallServer(Action<string> result)
        {
            UnityWebRequest www = UnityWebRequest.Get(url);
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                string text = www.downloadHandler.text;

                XmlDocument xml = new XmlDocument();
                xml.LoadXml(text);
                string inner = xml.FirstChild.InnerText;

                result(inner);
            }
        }

        public string GetSkeletonData()
        {
            throw new NotImplementedException();
        }

        public bool isServerOnline()
        {
            throw new NotImplementedException();
        }

        public bool isKinectOnline()
        {
            throw new NotImplementedException();
        }

        public float GetCurrentUpload()
        {
            throw new NotImplementedException();
        }

        public float GetCurrentDownload()
        {
            throw new NotImplementedException();
        }
    }
}
