using UnityEngine;
using UnityEditor;
using System;
[CustomEditor(typeof(UI_Manager))]
public class ChangeButton : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();


        UI_Manager UI = (UI_Manager)target;
        if (GUILayout.Button("Weapon Change !!"))
        {
            UI.Weapon_Chage();
        }

        if (GUILayout.Button("Animation Play !!"))
        {
            UI.Animation_Play();
        }

        if(GUILayout.Button("Level UP !!"))
        {
            UI.Level_Up();
        }
    }
}
