using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Wrld.Resources.IndoorMaps
{
    public class DefaultIndoorMapMaterialFactory : IIndoorMapMaterialFactory
    {
        Material m_templateMaterial;
        Material m_highlightTemplateMaterial;
        Material m_prepassMaterial;
        Dictionary<string, Material> m_materialArchtypesByType = new Dictionary<string, Material>();

        public DefaultIndoorMapMaterialFactory()
        {
            m_templateMaterial = GetOrLoadMaterialArchetype("InteriorsDiffuseTexturedMaterial");
            m_highlightTemplateMaterial = GetOrLoadMaterialArchetype("InteriorsHighlightMaterial");
            m_prepassMaterial = GetOrLoadMaterialArchetype("InteriorsStencilMirrorMaskMaterial");
        }

        public IIndoorMapMaterial CreateMaterialFromDescriptor(IndoorMaterialDescriptor descriptor)
        {
            var sourceMaterial = descriptor.MaterialName.Contains("highlight") ? m_highlightTemplateMaterial : m_templateMaterial;
            string materialType;

            if (descriptor.Strings.TryGetValue("MaterialType", out materialType))
            {
                if (materialType.StartsWith("Interior"))
                {
                    sourceMaterial = GetOrLoadMaterialArchetype(materialType);
                }
            }
            else
            {
                materialType = string.Empty;
            }
            
            var material = new Material(sourceMaterial);

            Color diffuseColor;

            if (!descriptor.Colors.TryGetValue("DiffuseColor", out diffuseColor))
            {
                diffuseColor = Color.white;
            }

            material.color = diffuseColor;
            material.name = descriptor.MaterialName;
            bool isForReflectiveSurface = materialType == "InteriorsStencilMirrorMaterial";
            bool isForReflectedGeometry = materialType == "InteriorsReflectionMaterial";

            // Prevent semi-transparent stencil masks from being created.
            if(isForReflectiveSurface && diffuseColor.a < 1.0f)
            {
                diffuseColor.a = 1.0f;
                material.color = diffuseColor;
            }
            
            var drawOrder = CalculateDrawOrderForMaterial(isForReflectedGeometry, isForReflectiveSurface);
            var prepassMaterial = isForReflectiveSurface ? CreatePrepassMaterial(descriptor) : null;

            return new DefaultIndoorMapMaterial(material, diffuseColor, drawOrder, prepassMaterial);
        }

        private DefaultIndoorMapMaterial.DrawOrder CalculateDrawOrderForMaterial(bool isForReflectedGeometry, bool isForReflectiveSurface)
        {
            if (isForReflectedGeometry)
            {
                // The upside-down reflection geometry has to be drawn after the stencil mask is laid down.
                return DefaultIndoorMapMaterial.DrawOrder.StencilMirrorReflectedGeometry;
            }
            else if (isForReflectiveSurface)
            {
                // The stencil floor must be drawn after the upside-down reflection geometry to blend with it.
                return DefaultIndoorMapMaterial.DrawOrder.StencilMirrorReflectiveSurface;
            }

            return DefaultIndoorMapMaterial.DrawOrder.AfterStencilMirror;
        }

        private Material CreatePrepassMaterial(IndoorMaterialDescriptor descriptor)
        {
            Color mirrorClearColor;
            
            if (descriptor.Colors.TryGetValue("MirrorClearColor", out mirrorClearColor))
            {
                var copy = new Material(m_prepassMaterial);
                copy.SetColor("_MirrorClearColor", mirrorClearColor);

                return copy;
            }

            return m_prepassMaterial;
        }

        private Material GetOrLoadMaterialArchetype(string materialType)
        {
            if (!m_materialArchtypesByType.ContainsKey(materialType))
            {
                m_materialArchtypesByType[materialType] = (Material)UnityEngine.Resources.Load(Path.Combine("WrldMaterials/Archetypes", materialType), typeof(Material));
            }

            return m_materialArchtypesByType[materialType];
        }
    }
}


