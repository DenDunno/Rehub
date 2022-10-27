using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static ZeppelinGames.GlyphRecogniser;

public class GlyphMatching 
{
    private readonly List<Vector3> _glyphPoints = new List<Vector3>();
    private const float _maxPointDist = 0.025f;

    public void TryAddPoint(Vector3 position)
    {
        if (_glyphPoints.Count < 1)
        {
            _glyphPoints.Add(position);
        }
        else
        {
            if (Vector3.Distance(_glyphPoints[_glyphPoints.Count - 1], position) > _maxPointDist)
            {
                _glyphPoints.Add(position);
            }
        }
    }

    public bool TryMatch(GlyphData match)
    {
        ProjectInputPointsTo2D();
        GlyphReturnData glyphData = GetMatchData(match);
        _glyphPoints.Clear();

        return glyphData.keyPointsPercent + glyphData.matchPercent > match.minMatchPercent;
    }

    private void ProjectInputPointsTo2D()
    {
        float z = _glyphPoints[0].z;
        Vector3 sumOfPoints = Vector3.zero;
        
        for (int i = 0; i < _glyphPoints.Count; ++i)
        {
            Vector3 position = _glyphPoints[i];
            position.z = z;

            sumOfPoints += position;
            
            _glyphPoints[i]= position;
        }

        Vector3 centroid = sumOfPoints / _glyphPoints.Count;
        
        for (int i = 0; i < _glyphPoints.Count; ++i)
        {
            Vector3 position = _glyphPoints[i];
            position -= centroid;

            _glyphPoints[i] = position;
        }
    }

    private GlyphReturnData GetMatchData(GlyphData matchGlyph)
    {
        Vector2[] points = _glyphPoints.Select(point => new Vector2(point.x, point.y)).ToArray();
        GlyphReturnData glyphData = MatchGlyph(points, matchGlyph);

        return glyphData;
    }
}