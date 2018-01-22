using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MaterialManager : MonoBehaviour
{
    public Material[] materials;
    public Dictionary<string, Material> _materials;

    void Start ()
    {
        _materials = new Dictionary<string, Material>();
        
        for (int i = 0; i < materials.Length; ++i)
        {
            _materials.Add(materials[i].name, materials[i]);
        }
	}

    public Material GetMaterial(string _material)
    {
        if (_materials.ContainsKey(_material))
        {
            return _materials[_material];
        }
        return null;
    }
}