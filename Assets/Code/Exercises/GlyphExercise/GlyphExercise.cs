using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class GlyphExercise : MonoBehaviour, IScenarioComponent
{
    [AssetList]
    [SerializeField] private GlyphData[] _glyphGameRoundData;
    [SerializeField] private Image _imageToDraw;
    private readonly GlyphMatching _glyphMatching = new GlyphMatching();
    private int _currentIndex;
    
    void IScenarioComponent.Init(ScenarioConfig scenarioConfig)
    {
        SetImageToDraw();
    }

    public void TryAddPoint(Vector3 position)
    {
        _glyphMatching.TryAddPoint(position);
    }

    public void TryAccept()
    {
        if (_glyphMatching.TryMatch(_glyphGameRoundData[_currentIndex]))
        {
            _currentIndex++;
            
            if (_currentIndex >= _glyphGameRoundData.Length)
            {
                _currentIndex = 0;
            }
            
            SetImageToDraw();
        }
    }

    private void SetImageToDraw()
    {
        _imageToDraw.sprite = _glyphGameRoundData[_currentIndex].Image;
    }
}