using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SetMaterialBasedOnInt : MonoBehaviour
{
    //Should have been a dictionary but Unity serialization decided to not cooperate
    [Serializable]
    public class IntMaterial 
    {
        [SerializeField] public int index;
        [SerializeField] public Material material;
    }

    [SerializeField, Required] private List<IntMaterial> _materials;
    [SerializeField, Required] private Renderer _renderer;

    public void SetMaterial(int i)
    {
        if (_renderer == null)
        {
            Debug.LogError("Renderer cannot be null!");
            return;
        }
        if (_materials == null || !_materials.Any())
        {
            Debug.LogError("Materials cannot be null or empty!");
            return;
        }
        var intMat = _materials.FirstOrDefault(x => x.index == i);
        if (intMat != null)
            _renderer.sharedMaterial = intMat.material;
        else
            Debug.LogWarning($"Material for int {i} not found!");
    }
}
