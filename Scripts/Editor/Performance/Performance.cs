using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;
using UnityEngine.Rendering;


namespace EffectPerformanceAnalysis
{
    public class Performance
    {
        private static RootNode m_RootNode = null;

        static Performance()
        {

        }


        public static RootNode Analyze(GameObject root, int sortingOrderStart)
        {
            if (root == null)
            {
                return null;
            }

            if(m_RootNode == null)
            {
                m_RootNode = new RootNode();
            }
            m_RootNode.Init(root.transform, sortingOrderStart);

            return m_RootNode;
        }
        
    }
}
