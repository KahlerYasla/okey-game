using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;


public class Dictionaries : MonoBehaviour
{
    public static readonly Dictionary<string, Color> ColorCodesToHexCodeDictionary = new()
    {
        { "d", Color.black },
        { "b", new Color(0.3294118f, 0.6156863f, 1, 1) }, // #548dff
        { "r", new Color(0.7333333f, 0.3137255f, 0.3137255f, 1) }, // #bb5050
        { "o", new Color(1, 0.6470588f, 0, 1) }, // #FFA500FF     
        { "g", new Color(0.1333333f, 0.4941176f, 0.3647059f, 1) }, // #227e5d
        { "j", Color.magenta },
    };
}
