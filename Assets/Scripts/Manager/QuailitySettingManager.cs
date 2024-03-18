using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

public class QuailitySettingManager : MonoBehaviour
{
    // 성능별로 만들어진 PipelineAsset List
    [SerializeField]
    List<RenderPipelineAsset> RenderPipelineAssets;

    public void SetPipeline(int value)
    {
        QualitySettings.SetQualityLevel(value);
        QualitySettings.renderPipeline = RenderPipelineAssets[value];
    }
}
