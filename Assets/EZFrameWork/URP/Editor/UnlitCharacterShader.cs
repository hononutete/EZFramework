using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace UnityEditor.Rendering.Universal.ShaderGUI
{
    internal class UnlitCharacterShader : BaseShaderGUI
    {
        protected MaterialProperty rimLightColorProp { get; set; }

        protected MaterialProperty rimLightPower { get; set; }

        protected MaterialProperty rimLightThickness { get; set; }

        protected MaterialProperty faceMapProp { get; set; }

        protected MaterialProperty faceColorProp { get; set; }

        public override void FindProperties(MaterialProperty[] properties)
        {
            base.FindProperties(properties);

            rimLightColorProp = FindProperty("_RimLightColor", properties);
            rimLightPower = FindProperty("_RimLightPower", properties);
            rimLightThickness = FindProperty("_RimLightThickness", properties);
            faceMapProp = FindProperty("_FaceMap", properties);
            faceColorProp = FindProperty("_FaceColor", properties);
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

            //表情テクスチャ用
            if (faceMapProp != null && faceColorProp != null) 
            {
                materialEditor.TexturePropertySingleLine(Styles.baseMap, faceMapProp, faceColorProp);
                // TODO Temporary fix for lightmapping, to be replaced with attribute tag.
                //if (material.HasProperty("_MainTex"))
                //{
                //    material.SetTexture("_MainTex", baseMapProp.textureValue);
                //    var baseMapTiling = baseMapProp.textureScaleAndOffset;
                //    material.SetTextureScale("_MainTex", new Vector2(baseMapTiling.x, baseMapTiling.y));
                //    material.SetTextureOffset("_MainTex", new Vector2(baseMapTiling.z, baseMapTiling.w));
                //}
            }
        }

        public override void DrawAdvancedOptions(Material material)
        {
            base.DrawAdvancedOptions(material);
            DrawRimLightProperty(materialEditor, material);
        }

        void DrawRimLightProperty(MaterialEditor materialEditor, Material material)
		{
            if(rimLightColorProp != null)
			{
                EditorGUI.BeginChangeCheck();

                Color color = materialEditor.ColorProperty(rimLightColorProp, "RimLightColor");

                if (EditorGUI.EndChangeCheck())
                    rimLightColorProp.colorValue = color;
            }

            if (rimLightPower != null)
            {
                EditorGUI.BeginChangeCheck();

                float rimLightPowerValue = materialEditor.RangeProperty(rimLightPower, "RimLightPower");

                if (EditorGUI.EndChangeCheck())
                    rimLightPower.floatValue = rimLightPowerValue;
            }

            if (rimLightThickness != null)
            {
                EditorGUI.BeginChangeCheck();

                float rimLightThicknessValue = materialEditor.RangeProperty(rimLightThickness, "RimLightThickness");

                if (EditorGUI.EndChangeCheck())
                    rimLightThickness.floatValue = rimLightThicknessValue;
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