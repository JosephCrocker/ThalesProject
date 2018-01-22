using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CoPilotTesting : MonoBehaviour
{
    public PathController[] Paths;

	void Start ()
    {
        Debug.Log("2");
        CachePathIntersections();
	}
	
	void Update ()
    {}

    private void CachePathIntersections()
    {
        for (int i = 0; i < Paths.Length; ++i)
        {
            for (int j = 0; j < Paths.Length; ++j)
            {
                if (i != j && j > i)
                {
                    CachePathIntersections(Paths[i], Paths[j]);
                }
            }
        }
    }

    private void CachePathIntersections(PathController _pathA, PathController _pathB)
    {
        int aCount = _pathA._nodes.Count;
        int bCount = _pathB._nodes.Count;

        for (int i = 0; i < aCount; ++i)
        {
            for (int j = 0; j < bCount; ++j)
            {
                if (_pathA._nodes[i].transform.position != _pathB._nodes[j].transform.position)
                {
                    continue;
                }
                else
                {
                    Debug.Log("Intersection Found! At Position: " + _pathA._nodes[i].transform.position);
                    if (!_pathA.IntersectingPaths.ContainsKey(_pathB))
                    {
                        _pathA.IntersectingPaths[_pathB] = new List<NodeController>();
                    }

                    Debug.Log("Intersection Time for Path A is: " + _pathA._nodes[i]);

                    _pathA.IntersectingPaths[_pathB].Add(_pathA._nodes[i]);

                    if (!_pathB.IntersectingPaths.ContainsKey(_pathA))
                    {
                        _pathB.IntersectingPaths[_pathA] = new List<NodeController>();
                    }

                    Debug.Log("Intersection Time for Path B is: " + _pathB._nodes[j]);
                    _pathB.IntersectingPaths[_pathA].Add(_pathB._nodes[j]);
                }
            }
        }
    }
}