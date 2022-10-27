using System;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

public class Whiteboard : MonoBehaviour
{
    [SerializeField] private GlyphExercise _glyphExercise;
    [AssetsOnly] [SerializeField] private RenderTexture _renderTexture;

    public void TryAddPosition(Vector3 position)
    {
        _glyphExercise.TryAddPoint(position);
    }
    
    public async void Clear()
    {
        _glyphExercise.TryAccept();

        await UniTask.Delay(TimeSpan.FromSeconds(0.1f));
        
        _renderTexture.Release();
    }
}