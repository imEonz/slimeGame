using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class SkillCreatorWindow : EditorWindow
{
    private const string SaveFolder = "Assets/Resources/Skills";

    private Skill temp;
    private SerializedObject so;
    private Vector2 scroll;

    private List<Type> skillTypes;
    private string[] typeNames;
    private int typeIndex;

    [MenuItem("Tools/Skill Creator")]
    public static void Open()
    {
        GetWindow<SkillCreatorWindow>("Skill Creator");
    }

    private void OnEnable()
    {
        skillTypes = TypeCache.GetTypesDerivedFrom<Skill>()
            .Where(t => !t.IsAbstract)
            .OrderBy(t => t.Name)
            .ToList();
        
        skillTypes.Insert(0, typeof(Skill));
        typeNames = skillTypes.Select(t => t.Name).ToArray();
        typeIndex = 0;

        ResetForm();
    }

    private void OnDisable()
    {
        DestroyTemp();
    }

    private void DestroyTemp()
    {
        if (temp != null && !EditorUtility.IsPersistent(temp)) DestroyImmediate(temp);
        temp = null;
    }

    private void ResetForm()
    {
        DestroyTemp();
        temp = (Skill)CreateInstance(skillTypes[typeIndex]);
        so = new SerializedObject(temp);
    }

    private void OnGUI()
    {
        if (so == null || so.targetObject == null) ResetForm();

        EditorGUILayout.LabelField("New Skill", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        int newIndex = EditorGUILayout.Popup("Skill Type", typeIndex, typeNames);
        if (newIndex != typeIndex)
        {
            typeIndex = newIndex;
            ResetForm();
        }

        so.Update();
        scroll = EditorGUILayout.BeginScrollView(scroll);

        SerializedProperty prop = so.GetIterator();
        bool enterChildren = true;
        while (prop.NextVisible(enterChildren))
        {
            enterChildren = false;
            if (prop.name == "m_Script") continue;
            EditorGUILayout.PropertyField(prop, true);
        }

        EditorGUILayout.EndScrollView();
        so.ApplyModifiedProperties();

        EditorGUILayout.Space();

        List<string> errors = Validate();
        foreach (string e in errors) EditorGUILayout.HelpBox(e, MessageType.Error);

        using (new EditorGUI.DisabledScope(errors.Count > 0))
        {
            if (GUILayout.Button("Create Skill", GUILayout.Height(32))) CreateAsset();
        }
    }

    private List<string> Validate()
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(so.FindProperty("skillName").stringValue)) errors.Add("Skill name is required.");

        SerializedProperty bindings = so.FindProperty("bindings");
        if (bindings.arraySize == 0)
        {
            errors.Add("The skill needs at least one binding.");
            return errors;
        }

        for (int i = 0; i < bindings.arraySize; i++)
        {
            SerializedProperty b = bindings.GetArrayElementAtIndex(i);

            SerializedProperty chance = b.FindPropertyRelative("chance");
            if (chance != null && chance.floatValue <= 0f) errors.Add($"Binding {i}: chance must be greater than 0.");

            SerializedProperty effects = b.FindPropertyRelative("effects");
            if (effects == null) continue;

            if (effects.arraySize == 0)
            {
                errors.Add($"Binding {i}: needs at least one effect.");
                continue;
            }

            for (int j = 0; j < effects.arraySize; j++)
            {
                SerializedProperty e = effects.GetArrayElementAtIndex(j);
                bool empty =
                (e.propertyType == SerializedPropertyType.ObjectReference && e.objectReferenceValue == null) ||
                (e.propertyType == SerializedPropertyType.ManagedReference && e.managedReferenceValue == null);

                if (empty) errors.Add($"Binding {i}: effect {j} is empty.");
            }
        }

        return errors;
    }

    private void CreateAsset()
    {
        EnsureFolder("Assets", "Resources");
        EnsureFolder("Assets/Resources", "Skills");

        string skillName = so.FindProperty("skillName").stringValue.Trim();
        foreach (char c in System.IO.Path.GetInvalidFileNameChars()) skillName = skillName.Replace(c.ToString(), "");

        string path = AssetDatabase.GenerateUniqueAssetPath($"{SaveFolder}/{skillName}.asset");

        AssetDatabase.CreateAsset(temp, path);
        AssetDatabase.SaveAssets();
        EditorGUIUtility.PingObject(temp);
        Selection.activeObject = temp;

        temp = null;
        ResetForm();
    }

    private static void EnsureFolder(string parent, string child)
    {
        if (!AssetDatabase.IsValidFolder($"{parent}/{child}")) AssetDatabase.CreateFolder(parent, child);
    }
}
