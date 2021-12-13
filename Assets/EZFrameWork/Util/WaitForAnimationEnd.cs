using UnityEngine;

namespace EZFramework.Util
{
    public class WaitForAnimationEnd : CustomYieldInstruction
    {
        Animator animator;
        int layer;
        int lasthash;

        public WaitForAnimationEnd(Animator animator, int layer)
        {
            this.animator = animator;
            this.layer = layer;

            //現在のstateのハッシュを取得
            lasthash = animator.GetCurrentAnimatorStateInfo(layer).fullPathHash;

        }

        public override bool keepWaiting
        {
            get
            {
                AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(layer);

                //現在のstateのハッシュが同じ、かつ、正規時間が１未満では終わっていない
                return state.fullPathHash == lasthash && state.normalizedTime < 1;
            }
        }
    }
}
