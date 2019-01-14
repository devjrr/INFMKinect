using System;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using UnityEngine.Networking;

namespace NetClientLib
{
    class UnityRemoteService : IRemoteService
    {
        private IList<CloudPoint> cloudPoints = new List<CloudPoint>();
        private IList<CloudPoint> skeletonPoints = new List<CloudPoint>();

        private readonly MonoBehaviour monoBehaviour;
        private readonly Deserializer deserializer;
        private readonly string url;
        private readonly int width;
        private readonly int height;

        private bool isRunning = false;

        public UnityRemoteService(MonoBehaviour monoBehaviour, Deserializer deserializer, string url, int height, int width)
        {
            this.monoBehaviour = monoBehaviour;
            this.deserializer = deserializer;
            this.url = url;
            this.height = height;
            this.width = width;
     
            monoBehaviour.StartCoroutine(CallServer(ProcessResult, "Data"));
            monoBehaviour.StartCoroutine(CallServer(ProcesSkeletonResult, "SkeletonData"));
        }

        public IList<CloudPoint> GetCloudpoints()
        {
            if (!isRunning)
            {
                monoBehaviour.StartCoroutine(CallServer(ProcessResult, "Data"));
            }

            return cloudPoints;
        }

        public IList<CloudPoint> GetSkeleton()
        {
            if (!isRunning)
            {
                monoBehaviour.StartCoroutine(CallServer(ProcesSkeletonResult, "SkeletonData"));
            }

            return skeletonPoints;
        }

        private void ProcessResult(string result)
        {
            cloudPoints = deserializer.deserialze(result, height, width); ;
        }

        private void ProcesSkeletonResult(string result)
        {
            skeletonPoints = deserializer.deserialzeSkeleton(result); ;
        }

        private IEnumerator<UnityWebRequestAsyncOperation> CallServer(Action<string> result, string service)
        {
            isRunning = true;
            UnityWebRequest www = UnityWebRequest.Get(url +  service);
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

            isRunning = false;
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
