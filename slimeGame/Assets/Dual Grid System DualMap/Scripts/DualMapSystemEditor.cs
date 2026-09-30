using UnityEditor;
using UnityEngine.UIElements;
using UnityEngine;



namespace FK.DualMap
{
#if UNITY_EDITOR
    [CustomEditor(typeof(DualMapSystem))]
    public class DualMapSystemEditor : Editor
    {
        [SerializeField] public VisualTreeAsset VisualTree;

        DualMapSystem dualSystem;

        Button createDualTilemapButton;
        Button clearTilemapButton;

        private void OnEnable()
        {
            dualSystem = target as DualMapSystem;
        }


        public override VisualElement CreateInspectorGUI()
        {
            VisualElement root = new VisualElement();
            VisualTree.CloneTree(root);

            createDualTilemapButton = root.Q<Button>("CreateDualTilemap");
            createDualTilemapButton.RegisterCallback<ClickEvent>(CreateDualTilemap);

            clearTilemapButton = root.Q<Button>("ClearTilemap");
            clearTilemapButton.RegisterCallback<ClickEvent>(ClearTilemap);

            return root;
        }

        public void CreateDualTilemap(ClickEvent evn)
        {
            dualSystem.CreateDualMap();
        }

        public void ClearTilemap(ClickEvent evn)
        {
            dualSystem.dualMap.ClearAllTiles();
        }
    }
#endif
}