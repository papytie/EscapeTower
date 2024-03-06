using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class EditorGameMenu {

    [MenuItem("EscapeTower/Game Settings")]
    private static void EditGameSettings() {
        GameSettings settings = SRResources.GameSettings;
        Selection.activeObject = settings;
    }
}