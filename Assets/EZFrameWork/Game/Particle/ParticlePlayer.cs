using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EZFramework.Game
{

    public class ParticlePlayer : MonoBehaviour
    {
        public List<ParticleSystem> particleSystems;

        public bool IsPlaying { get; private set; }

        void Start()
        {
            Stop();
        }

        public void SetPlay(bool doPlay)
        {
            if (doPlay)
                Play();
            else
                Stop();
        }

        public void Play()
        {
            IsPlaying = true;
            foreach (ParticleSystem ps in particleSystems)
                ps.Play();
        }

        public void Emit(int count)
        {
            foreach (ParticleSystem ps in particleSystems)
                ps.Emit(count);
        }

        public void Stop()
        {
            IsPlaying = false;
            foreach (ParticleSystem ps in particleSystems)
                ps.Stop();
        }
    }
}
