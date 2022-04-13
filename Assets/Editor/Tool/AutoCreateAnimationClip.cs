using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class AutoCreateAnimationClip : EditorWindow
{
    public Sprite[] sprites = new Sprite[3];
    public string monsterAnimFileName;
    public string monsterAnimFilePath;
    
    private void OnEnable()
    {
        sprites = new Sprite[3];
    }

    [MenuItem("Window/Create AnimationClip")]
    static void OpenWindow()
    {
        AutoCreateAnimationClip window = (AutoCreateAnimationClip)GetWindow(typeof(AutoCreateAnimationClip));
        window.Show();
    }

    private void OnGUI()
    {
        ScriptableObject target = this;
        SerializedObject so = new SerializedObject(target);
        SerializedProperty spritesProperty = so.FindProperty("sprites");
        EditorGUILayout.PropertyField(spritesProperty, true); 
        so.ApplyModifiedProperties();

        GUILayout.Space(20);
        
        GUILayout.Label("Path");
        monsterAnimFilePath= GUILayout.TextField(monsterAnimFilePath);

        GUILayout.Space(10);
        GUILayout.Label("FileName");
        monsterAnimFileName = GUILayout.TextField(monsterAnimFileName);
        
        GUILayout.Space(20);
        
        if (GUILayout.Button("Create Monster AnimationClip"))
        {
            var path = monsterAnimFilePath + '/' + monsterAnimFileName;
            CreateAnimationClip(sprites,path);
        }
    }
    
    
    
    private static void CreateAnimationClip(Sprite[] sprites, string filePath)
    {
        if (sprites.Length <= 0)
        {
            Debug.LogError("Sprite is null");
            return;
        }
        
        Sprite[] resources = sprites;
        
        var animationClip = new AnimationClip {frameRate = 60f};
        var editorCurveBinding = EditorCurveBinding.PPtrCurve("", typeof(SpriteRenderer), "m_Sprite");
        var objectReferenceKeyframes = new ObjectReferenceKeyframe[resources.Length];
        
        for (int i = 0; i < resources.Length; i++)
        {
            objectReferenceKeyframes[i] = new ObjectReferenceKeyframe {time = i*0.5f, value = resources[i]};
        }
        
        AnimationUtility.SetObjectReferenceCurve(animationClip,editorCurveBinding,objectReferenceKeyframes);

        AssetDatabase.CreateAsset(animationClip,$"Assets/Animation/{filePath}.anim");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log(AssetDatabase.GetAssetPath(animationClip));
    }
}
