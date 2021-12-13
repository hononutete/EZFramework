using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EZFramework.Game
{
    public class OrbitalTerrain
    {
        public Vector3 Center;

        public float Radius;
    }

    public class OrbitalTransform : MonoBehaviour
    {
        public OrbitalTerrain terrain { get; private set; }

        Vector3 _position;
        public Vector3 position
        {
            get
            {
                //TODO
                return _position;
            }
            set
            {
                //Debug.LogError("set:" + gameObject.name);
                _position = value;
                if (terrain != null)
                    transform.position = DecaltToOrititalCoordMapper.Map(terrain.Center, terrain.Radius, _position);
            }
        }

        Quaternion _rotation = Quaternion.identity;
        public Quaternion rotation
        {
            get { return _rotation; }
            set
            {
                _rotation = value;
                UpdateRotation();
            }
        }

        public Vector3 eulerAngles
        {
            get
            {
                return rotation.eulerAngles;
            }
            set
            {
                rotation = Quaternion.Euler(value);
            }
        }

        public Vector3 Up
        {
            get
            {
                Vector3 myPos = new Vector3(0, transform.position.y, transform.position.z);
                Vector3 center = new Vector3(0, terrain.Center.y, terrain.Center.z);
                return (myPos - center).normalized;
            }
        }

        public Vector3 Down => -Up;

        public Vector3 Forward
        {
            get
            {
                return _rotation * Vector3.forward;
            }
        }

        public void SetTerrain(OrbitalTerrain terrain)
        {
            this.terrain = terrain;
        }

        public void UpdateRotation()
        {
            if (terrain != null)
            {
                transform.rotation = Quaternion.FromToRotation(Vector3.up, Up) * _rotation;
                //transform.rotation = _rotation * Quaternion.FromToRotation(Vector3.up, Up);
                //transform.rotation = Quaternion.AngleAxis(AngleAroundX(terrain.Radius, _position) - 90, Vector3.right) * _rotation;

                //transform.rotation = Quaternion.AngleAxis(AngleAroundX(terrain.Radius, _position) - 90, Vector3.right) * Quaternion.AngleAxis(45, Up);
            }
        }

        float AngleAroundX(float radius, Vector3 position)
        {
            float distance = position.z;
            return 360.0f * distance / (radius * 2.0f * Mathf.PI);
        }

        void Update()
        {
            UpdateRotation();
        }


    }

    /// <summary>
    /// 円筒系の座標系から、デカルト座標系への相互座標変換
    /// </summary>
    public static class DecaltToOrititalCoordMapper
    {
        const float angleAtZero = 0;

        public static Vector3 Map(Vector3 center, float radius, Vector3 position)
        {
            float r = radius + position.y;
            Vector3 p = center;
            float distance = position.z;

            float phi = Mathf.Deg2Rad * 360.0f * distance / (radius * 2.0f * Mathf.PI);
            float z = center.z - r * Mathf.Cos(angleAtZero + phi);
            float y = center.z + r * Mathf.Sin(angleAtZero + phi);
            return new Vector3(position.x, y, z);
        }

        public static Vector3 Unmap(Vector3 center, float radius, Vector3 position)
        {
            Vector3 linedCenter = new Vector3(position.x, center.y, center.z);
            float distance = Vector3.Distance(position, linedCenter);
            float y = distance - radius;
            float x = position.x;
            float h = position.y - linedCenter.y;
            float rad = Mathf.Asin(h / distance);

            float z = (distance * 2.0f * Mathf.PI) * ((rad) / (2.0f * Mathf.PI));
            return new Vector3(x, y, z);
        }
    }

    /// <summary>
    /// 円筒系の座標系から、デカルト座標系への相互ベクトル変換
    /// </summary>
    public static class OrititalToDecaltCoordVectorMapper
    {

    }
}

