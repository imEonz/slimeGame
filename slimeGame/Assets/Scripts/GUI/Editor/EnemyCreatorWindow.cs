using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EnemyCreatorWindow : EditorWindow
{
    private EnemyData temp;
    private SerializedObject so;
    private Vector2 scroll;
    private const string SaveFolder = "Assets/Resources/EnemyDatas";

    [MenuItem("Tools/Enemy Creator")]
    public static void Open()
    {
        GetWindow<EnemyCreatorWindow>("Enemy Creator");
    }

    private void OnEnable() => ResetForm();

    private void OnDisable()
    {
        if (temp != null && !EditorUtility.IsPersistent(temp)) DestroyImmediate(temp);
    }

    private void ResetForm()
    {
        if (temp != null && !EditorUtility.IsPersistent(temp)) DestroyImmediate(temp);

        temp = CreateInstance<EnemyData>();
        so = new SerializedObject(temp);
    }

    private void OnGUI()
    {
        if (so == null || so.targetObject == null) ResetForm();

        so.Update();
        scroll = EditorGUILayout.BeginScrollView(scroll);

        EditorGUILayout.LabelField("New Enemy", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        DrawField("enemyName");
        DrawField("maxHP");
        DrawField("speed");
        DrawField("damage");
        DrawField("staggerChance");
        DrawField("staggerTime");
        DrawField("level");
        DrawField("sprite");
        DrawField("lootTable");
        DrawBehaviourField();
        DrawField("enemySkills");
        DrawField("timeBetweenCasts");

        EditorGUILayout.EndScrollView();
        so.ApplyModifiedProperties();

        EditorGUILayout.Space();

        List<string> errors = Validate();
        foreach (string e in errors) EditorGUILayout.HelpBox(e, MessageType.Error);

        using (new EditorGUI.DisabledScope(errors.Count > 0))
        {
            if (GUILayout.Button("Create Enemy", GUILayout.Height(32))) CreateAsset();
        }
    }

    private void DrawField(string name)
    {
        SerializedProperty p = so.FindProperty(name);
        EditorGUILayout.PropertyField(p, true);
    }

    private void DrawBehaviourField()
    {
        SerializedProperty p = so.FindProperty("enemyBehaviour");

        EditorGUILayout.BeginHorizontal();

        p.objectReferenceValue = EditorGUILayout.ObjectField("Enemy Behaviour", p.objectReferenceValue, typeof(EnemyBehaviour), false);

        if (GUILayout.Button("Pick", GUILayout.Width(50))) ShowBehaviourMenu();

        EditorGUILayout.EndHorizontal();
    }
    private void ShowBehaviourMenu()
    {
        var menu = new GenericMenu();
        string[] guids = AssetDatabase.FindAssets("t:Prefab");
        bool any = false;

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (prefab == null || !prefab.TryGetComponent(out EnemyBehaviour behaviour)) continue;

            any = true;
            EnemyBehaviour captured = behaviour;
            menu.AddItem(new GUIContent(prefab.name), so.FindProperty("enemyBehaviour").objectReferenceValue == captured, () =>
            {
                so.Update();
                so.FindProperty("enemyBehaviour").objectReferenceValue = captured;
                so.ApplyModifiedProperties();
                Repaint();
            });
        }

        if (!any) menu.AddDisabledItem(new GUIContent("No prefabs with an EnemyBehaviour found"));

        menu.ShowAsContext();
    }

    private List<string> Validate()
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(so.FindProperty("enemyName").stringValue)) errors.Add("Enemy name is required.");

        if (so.FindProperty("maxHP").floatValue <= 0f) errors.Add("Max HP must be greater than 0.");

        if (so.FindProperty("speed").floatValue <= 0f) errors.Add("Speed must be greater than 0.");

        if (so.FindProperty("damage").floatValue <= 0f) errors.Add("Damage must be greater than 0.");

        if (so.FindProperty("level").intValue <= 0) errors.Add("Level must be at least 1.");

        if (so.FindProperty("sprite").objectReferenceValue == null) errors.Add("Sprite is required.");

        SerializedProperty loot = so.FindProperty("lootTable");
        if (loot.arraySize == 0) errors.Add("Loot table needs at least one entry.");

        SerializedProperty behaviour = so.FindProperty("enemyBehaviour");
        if (behaviour.propertyType == SerializedPropertyType.ObjectReference && behaviour.objectReferenceValue == null)
        errors.Add("Enemy behaviour is required.");


        if (so.FindProperty("enemySkills").arraySize > 0 && so.FindProperty("timeBetweenCasts").floatValue <= 0f)
        errors.Add("Time between casts must be greater than 0 when the enemy has skills.");

        return errors;
    }

    private void CreateAsset()
    {
        EnsureFolder("Assets", "Resources");
        EnsureFolder("Assets/Resources", "EnemyDatas");

        string enemyName = so.FindProperty("enemyName").stringValue.Trim();
        foreach (char c in System.IO.Path.GetInvalidFileNameChars()) enemyName = enemyName.Replace(c.ToString(), "");

        string path = AssetDatabase.GenerateUniqueAssetPath($"{SaveFolder}/{enemyName}.asset");

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
