using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class CoordsExtractor {
    string path;
    string jsonString;
    private JObject jObject;

    public string getJsonString(){
        path = Application.streamingAssetsPath + "/../StreamingAssets/test.json";
        jsonString = File.ReadAllText(path);
        return jsonString;
    }

    public double[] getXCoords(){
        jObject = JObject.Parse(getJsonString());
        IEnumerable<JToken> xCoordinate = jObject.SelectTokens("$..X");
        JToken[] xArrayJTokens = xCoordinate.ToArray();
        var xArray = Array.ConvertAll(xArrayJTokens, item => (double) item);
        return xArray;
    }

    public double[] getYCoords(){
        IEnumerable<JToken> yCoordinate = jObject.SelectTokens("$..Y");
        JToken[] yArrayJTokens = yCoordinate.ToArray();
        var yArray = Array.ConvertAll(yArrayJTokens, item => (double) item);
        return yArray;
    }

    public double[] getZCoords(){
        IEnumerable<JToken> zCoordinate = jObject.SelectTokens("$..Z");
        JToken[] zArrayJTokens = zCoordinate.ToArray();
        var zArray = Array.ConvertAll(zArrayJTokens, item => (double) item);
        return zArray;
    }
}