using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using Cinemachine;
using EZFramework.Util;

namespace EZFramework.Game
{
    public class VirtualCameraManager : SingletonMonobehaviour<VirtualCameraManager>
    {
        [Serializable]
        public class VirualCameraEntry
        {
            public string key;
            public GameObject virtualCamera;
            public Vector3 startPosition;
        }

        public List<VirualCameraEntry> virtualCameras;
        VirualCameraEntry activeVirtualCamera = null;

        void Start()
        {
            //初期位置を保持
            foreach (VirualCameraEntry entry in virtualCameras)
            {
                entry.startPosition = entry.virtualCamera.transform.position;
            }
        }

        public void Init() { }

        public VirualCameraEntry Get(string key) => virtualCameras.FirstOrDefault(e => e.key == key);

        public void Reset()
        {
            if (virtualCameras.Count <= 0)
                return;

            activeVirtualCamera = null;

            //全て一度OFFにする
            DeactivateAll();
        }

        void DeactivateAll()
        {
            foreach (VirualCameraEntry entry in virtualCameras)
            {
                entry.virtualCamera.GetComponent<CinemachineVirtualCamera>().m_Follow = null;
                entry.virtualCamera.GetComponent<CinemachineVirtualCamera>().m_LookAt = null;
                entry.virtualCamera.transform.position = entry.startPosition;
                if (entry.virtualCamera.activeSelf)
                    entry.virtualCamera.SetActive(false);
            }
        }

        public void Deactivate(string key)
        {
            VirualCameraEntry entry = virtualCameras.FirstOrDefault(e => e.key == key);
            if (entry != null)
            {
                entry.virtualCamera.SetActive(false);
                if (entry == activeVirtualCamera)
                    activeVirtualCamera = null;

            }
        }

        public void Activate(string key, bool deactivateAll = true)
        {
            if (activeVirtualCamera != null && activeVirtualCamera.key == key)
                return;

            //全て一度OFFにする
            if (deactivateAll)
                DeactivateAll();

            VirualCameraEntry entry = virtualCameras.FirstOrDefault(e => e.key == key);
            if (entry != null)
            {
                entry.virtualCamera.SetActive(true);
                activeVirtualCamera = entry;
            }
        }


    }
}