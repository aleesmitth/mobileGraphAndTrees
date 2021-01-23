using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class HeightLimitException : Exception {
    public HeightLimitException(string message) : base(message) { }
}
