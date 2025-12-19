using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EffectPerformanceAnalysis
{
    public interface ITreeNode 
    {
        RenderNode firstRenderNode { get; set; }

        ECompareType compareType { get; set; }

        ITreeNode next { get; set; }

        ITreeNode parent { get; set; }

        ITreeNode child { get; set; }

        ITreeNode AddNode(RenderNode renderNode);

    }

}