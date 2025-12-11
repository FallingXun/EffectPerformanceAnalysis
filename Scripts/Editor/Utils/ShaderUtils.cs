using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace EffectPerformanceAnalysis
{
    public static class ShaderUtils
    {
        public static Shader GetShader(Material material)
        {
            if (material != null)
            {
                return material.shader;
            }
            return null;
        }

        public static int GetPassCount(Shader shader)
        {
            if (shader != null)
            {
                return shader.passCount;
            }
            return 0;
        }

        public static int GetRenderQueue(Shader shader)
        {
            if(shader != null)
            {
                return shader.renderQueue;
            }
            return 0;
        }

        public static int GetInstanceId(Shader shader)
        {
            if(shader != null)
            {
                return shader.GetInstanceID();
            }
            return 0;
        }
    }
}