using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Utils
{
    public static class Maths
    {
        public static bool NonZero(float value)
        {
            return Mathf.Abs(value) > Mathf.Epsilon;
        }
    }
}