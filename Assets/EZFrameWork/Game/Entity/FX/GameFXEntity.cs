using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EZFramework.Game
{
    /// <summary>
    /// 再利用可能なエフェクトのエンティティ
    /// </summary>
    public class GameFXEntity : GameEntity
    {
        ParticleSystem[] particles;

        public override void Init()
        {
            base.Init();

            particles = GetComponents<ParticleSystem>();
        }

        public override void InitOnReuse()
        {
            base.InitOnReuse();

            foreach (ParticleSystem ps in particles)
            {
                ps.time = 0;
                ps.Play();
            }
        }
    }
}