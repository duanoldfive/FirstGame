using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

//创建Texture2DArray

public class TextureArray : EditorWindow
{
    public int PropertyNum = 10;

    public List<Texture2D> textures = new List<Texture2D>();

    [MenuItem("Tools/Texture2DArray")]
    static void Init()
    {   
        TextureArray window = (TextureArray)EditorWindow.GetWindow(typeof(TextureArray), false, "TextureArray", true);
        window.Show();
    }
    

    private float spaceNumber = 10f;

    private void OnGUI()
    {   
        GUILayout.Space(spaceNumber);
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        EditorGUILayout.LabelField("要合成的贴图:", GUILayout.Width(100), GUILayout.Height(30));
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();
        for (int i=0; i<textures.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            textures[i] = (Texture2D)EditorGUILayout.ObjectField(textures[i], typeof(Texture2D), true, GUILayout.Width(64), GUILayout.Height(64));
            if (GUILayout.Button("-", GUILayout.Width(30), GUILayout.Height(30)))
            {
                textures.RemoveAt(i);
                i--;
            }
            EditorGUILayout.EndHorizontal();
        }
        

        GUILayout.Space(spaceNumber);
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("+", GUILayout.Height(30)))
        {
            textures.Add(null);
        }
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(spaceNumber);
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("创  建", GUILayout.Height(30)))
        {
            CreateTextureArray();
        }
        EditorGUILayout.EndHorizontal();
    }

    public void CreateTextureArray()
    {
        //如果没有指定要合成的贴图，或者都为空，则直接返回
        textures.RemoveAll(tex => tex == null);
        if (textures.Count == 0)
        {
            Debug.LogError("Please select textures for combine");
            return;
        }

        Texture2D firstTex = textures[0];

        //Create texture2DArray
        Texture2DArray texture2DArray = new Texture2DArray(firstTex.width,firstTex.height, textures.Count, firstTex.format, false, false);
        // Apply settings
        
        //texture2DArray.filterMode = firstTex.filterMode;
        //texture2DArray.wrapMode = firstTex.wrapMode;

        texture2DArray.filterMode = FilterMode.Point;
        texture2DArray.wrapMode = TextureWrapMode.Clamp;

        int index = 0;
        foreach(Texture2D tex in textures)
        {
            for (int m = 0; m < tex.mipmapCount; m++)
            {
                Graphics.CopyTexture(tex, 0, m, texture2DArray, index, m);
            }
            index++;
        }

        

        //Save 
        string path = EditorUtility.SaveFilePanel("Save As", "Assets", "texArray", "asset");
        if (path.Length > 0)
        {
            path = path.Substring(Application.dataPath.Length - 6);

            AssetDatabase.CreateAsset(texture2DArray, path);
        }
    }
}
