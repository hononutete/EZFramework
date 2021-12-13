using System;
using UnityEngine;
namespace UnityEditor.Rendering.Universal.ShaderGUI
{
    class UnlitNoisedGrass : BaseShaderGUI
    {

        private UnlitNoisedGrassGUI.NoisedGrassProperties shadingModelProperties;

        // collect properties from the material properties
        public override void FindProperties(MaterialProperty[] properties)
        {
            base.FindProperties(properties);

            shadingModelProperties = new UnlitNoisedGrassGUI.NoisedGrassProperties(properties);
            //shadingModelProperties = new SimpleLitGUI.SimpleLitProperties(properties);
        }


        // material changed check
        public override void MaterialChanged(Material material)
        {
            if (material == null)
                throw new ArgumentNullException("material");
            
            SetMaterialKeywords(material);
        }

        // material main surface options
        public override void DrawSurfaceOptions(Material material)
        {
            if (material == null)
                throw new ArgumentNullException("material");

            // Use default labelWidth
            EditorGUIUtility.labelWidth = 0f;

            // Detect any changes to the material
            EditorGUI.BeginChangeCheck();
            {
                base.DrawSurfaceOptions(material);
            }
            if (EditorGUI.EndChangeCheck())
            {
                foreach (var obj in blendModeProp.targets)
                    MaterialChanged((Material)obj);
            }
        }

        // material main surface inputs
        public override void DrawSurfaceInputs(Material material)
        {
            base.DrawSurfaceInputs(material);
            DrawTileOffset(materialEditor, baseMapProp);
            DrawNoisedTexture(material);
            DrawTileOffset(materialEditor, shadingModelProperties.noiseMapProp);
        }

        //protected MaterialProperty noiseMapProp { get; set; }
        //protected MaterialProperty noiseColorProp { get; set; }

        void DrawNoisedTexture(Material material)
        {
            if (shadingModelProperties.noiseMapProp != null) // Draw the baseMap, most shader will have at least a baseMap
            {
                GUIContent noiseStyle = new GUIContent("noise map GUIContent");
                materialEditor.TexturePropertySingleLine(noiseStyle, shadingModelProperties.noiseMapProp);
                
                if (material.HasProperty("_NoiseMap"))
                {
                    material.SetTexture("_NoiseMap", shadingModelProperties.noiseMapProp.textureValue);
                    var noiseMapTiling = shadingModelProperties.noiseMapProp.textureScaleAndOffset;
                    
                    material.SetTextureScale("_NoiseMap", new Vector2(noiseMapTiling.x, noiseMapTiling.y));
                    material.SetTextureOffset("_NoiseMap", new Vector2(noiseMapTiling.z, noiseMapTiling.w));
                }
            }
        }

        public override void AssignNewShaderToMaterial(Material material, Shader oldShader, Shader newShader)
        {
            if (material == null)
                throw new ArgumentNullException("material");

            // _Emission property is lost after assigning Standard shader to the material
            // thus transfer it before assigning the new shader
            if (material.HasProperty("_Emission"))
            {
                material.SetColor("_EmissionColor", material.GetColor("_Emission"));
            }

            base.AssignNewShaderToMaterial(material, oldShader, newShader);

            if (oldShader == null || !oldShader.name.Contains("Legacy Shaders/"))
            {
                SetupMaterialBlendMode(material);
                return;
            }

            SurfaceType surfaceType = SurfaceType.Opaque;
            BlendMode blendMode = BlendMode.Alpha;
            if (oldShader.name.Contains("/Transparent/Cutout/"))
            {
                surfaceType = SurfaceType.Opaque;
                material.SetFloat("_AlphaClip", 1);
            }
            else if (oldShader.name.Contains("/Transparent/"))
            {
                // NOTE: legacy shaders did not provide physically based transparency
                // therefore Fade mode
                surfaceType = SurfaceType.Transparent;
                blendMode = BlendMode.Alpha;
            }
            material.SetFloat("_Surface", (float)surfaceType);
            material.SetFloat("_Blend", (float)blendMode);

            MaterialChanged(material);
        }
    }
}