using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using static UnityEditor.AssetDatabase;
using UnityEditor.Animations;
#endif

[ExecuteInEditMode]
public class ACFactory : MonoBehaviour{

    #if UNITY_EDITOR

    public bool generate = false;

    void Update(){ if(generate) Generate(); generate = false; }

    // Scripts are UnityEditor.MonoScript
    void Generate(){
        CheckPath("Assets/Scripts/Duelist.cs");
        CheckPath("Assets/Scripts/Foo.cs");
    }

    void CheckPath(string x){
        Object[] data = LoadAllAssetsAtPath(x);
        foreach (Object o in data){
            print(o.name);
            print(o.GetType());
            if(o is MonoScript s){
                print(s.GetClass());
            }
        }
    }

    void GenerateOld(){
        var guids = FindAssets("footman");
        var clips = new Dictionary<string, AnimationClip>();
        foreach(var k in guids){
            var path = GUIDToAssetPath(k);
            FindAnimations(path, clips);
        }
        foreach(var k in clips) print(k);

        var ac = AnimatorController.CreateAnimatorControllerAtPath(
            "Assets/Animation/TestAC.controller");
        var root = ac.layers[0].stateMachine;
        var A = root.AddState("A"); A.motion = clips["attack_01"];
        var B = root.AddState("B"); B.motion = clips["taunt"];
        var C = root.AddState("C"); C.motion = clips["run"];
    }

    void FindAnimations(string path, Dictionary<string, AnimationClip> @out){
        Object[] data = LoadAllAssetsAtPath(path);
        foreach (Object o in data)
        if(o is AnimationClip clip && !clip.name.Contains("_preview_"))
            @out[clip.name] = clip;
    }

    #endif

}
