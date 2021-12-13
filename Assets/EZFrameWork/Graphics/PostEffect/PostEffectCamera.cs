using UnityEngine;

namespace EZFramework.Graphics
{

    [ExecuteInEditMode, ImageEffectAllowedInSceneView]
    public class PostEffectCamera : MonoBehaviour
    {

        [SerializeField]
        private Material _material;

        private void OnRenderImage(RenderTexture source, RenderTexture dest)
        {
            UnityEngine.Graphics.Blit(source, dest, _material);
        }
    }
}