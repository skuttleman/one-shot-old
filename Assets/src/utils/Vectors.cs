﻿using System;
using UnityEngine;

namespace Game.Utils {
    public static class Vectors {
        public static Vector3 Upgrade(Vector2 vector) =>
            new(vector.x, vector.y, 0f);
        public static Vector2 Downgrade(Vector3 vector) =>
            new(vector.x, vector.y);

        public static float AngleTo(Vector2 direction) {
            Vector2 dir = direction.normalized;
            return Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + 90f;
        }

        public static float AngleTo(Vector2 origin, Vector2 position) =>
            AngleTo(origin - position);

        public static bool NonZero(Vector2 vector) => NonZero(Upgrade(vector));

        public static bool NonZero(Vector3 vector) {
            return Maths.NonZero(vector.x)
                || Maths.NonZero(vector.y)
                || Maths.NonZero(vector.z);
        }

        public static Vector3 ToVector3(float angle) {
            float angleRad = angle * (Mathf.PI / 180f);
            return new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
        }

        public static Vector2 ToVector2(float angle) =>
            Downgrade(ToVector3(angle));
    }
}
