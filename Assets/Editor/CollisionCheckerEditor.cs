using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(CollisionCheckerComponent))]
public class CollisionCheckerEditor : Editor
{
    public VisualTreeAsset visualTree;

    public override VisualElement CreateInspectorGUI()
    {
        VisualElement root = new VisualElement();
        
        visualTree.CloneTree(root);

        return root;
    }
}
