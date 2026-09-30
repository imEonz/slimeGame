using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using FK.DualMap.Data;

namespace FK.DualMap
{
    [CustomEditor(typeof(DualTilesRuleData_2))]
    public class DualTilesRuleData_2Editor : Editor
    {
        DualTilesRuleData_2 data;

        public DualTilePartVisualizationData partVisualData;

        private Dictionary<DualRuleTile, bool> foldoutDict = new Dictionary<DualRuleTile, bool>();

        private DualRuleTile selectTile;



        public override void OnInspectorGUI()
        {
            for (int i = 0; i < data.tileRules.Count; i++)
            {
                var ruleTile = data.tileRules[i];

                if (ruleTile.moduleRule == null) ruleTile.moduleRule = new DualTileModule();
                if (!foldoutDict.ContainsKey(ruleTile)) foldoutDict.Add(ruleTile, true);

                foldoutDict[ruleTile] = EditorGUILayout.Foldout(foldoutDict[ruleTile], "Rule Tile " + i, true);
                if (foldoutDict[ruleTile])
                {
                    DrawDualRuleTile(ruleTile);
                    if (GUILayout.Button("Select"))
                    {
                        selectTile = ruleTile;
                    }
                    GUILayout.Space(50);
                }
            }


            var style = new GUIStyle(GUI.skin.button);
            style.fontSize = 30;

            if (GUILayout.Button("Unselect", style))
            {
                selectTile = null;
            }

            GUILayout.BeginHorizontal();


            if (GUILayout.Button("+", style))
            {
                if (!data.tileRules.Contains(selectTile)) selectTile = null;

                if (selectTile != null)
                    data.tileRules.Add(new DualRuleTile(selectTile));
                else data.tileRules.Add(new DualRuleTile());
            }

            if (GUILayout.Button("-", style))
            {
                if (data.tileRules.Count > 0 && (selectTile == null || !data.tileRules.Contains(selectTile))) selectTile = data.tileRules.Last();

                if (selectTile != null)
                    data.tileRules.Remove(selectTile);
            }

            GUILayout.EndHorizontal();


            if (GUI.changed)
            {
                EditorUtility.SetDirty(target);
                Undo.RecordObject(target, "Change target value");
            }

        }

        private void DrawDualRuleTile(DualRuleTile ruleTile)
        {
            GUILayout.BeginVertical();
            ruleTile.tileBase = (TileBase)EditorGUILayout.ObjectField(ruleTile.tileBase, typeof(TileBase), false);

            GUILayout.BeginHorizontal();

            DrawModule(ruleTile.moduleRule);
            DrawTileBase(ruleTile.tileBase, 7);

            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
        }




        private void DrawModule(DualTileModule module, float scale = 2)
        {
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();

            GUILayout.BeginVertical();
            module.topLeft = (DualTilePartType)EditorGUILayout.EnumPopup(module.topLeft, GUILayout.Width(50));
            DrawDualTilePartTypeSprite(module.topLeft);
            GUILayout.EndVertical();

            GUILayout.BeginVertical();
            module.topRight = (DualTilePartType)EditorGUILayout.EnumPopup(module.topRight, GUILayout.Width(50));
            DrawDualTilePartTypeSprite(module.topRight);
            GUILayout.EndVertical();

            GUILayout.EndHorizontal();
            GUILayout.Space(10);
            GUILayout.BeginHorizontal();

            GUILayout.BeginVertical();
            module.bottomLeft = (DualTilePartType)EditorGUILayout.EnumPopup(module.bottomLeft, GUILayout.Width(50));
            DrawDualTilePartTypeSprite(module.bottomLeft);
            GUILayout.EndVertical();

            GUILayout.BeginVertical();
            module.bottomRight = (DualTilePartType)EditorGUILayout.EnumPopup(module.bottomRight, GUILayout.Width(50));
            DrawDualTilePartTypeSprite(module.bottomRight);
            GUILayout.EndVertical();

            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }

        private void DrawDualTilePartTypeSprite(DualTilePartType part, float scale = 3)
        {

            if (part == DualTilePartType.Void)
                DrawSprite(partVisualData._voidSprite, scale);
            else if (part == DualTilePartType.Main)
                DrawSprite(partVisualData._mainSprite, scale);
            else if (part == DualTilePartType.Stair)
                DrawSprite(partVisualData._stairSprite, scale);
            else if (part == DualTilePartType.MainORStair)
                DrawSprite(partVisualData._mainOrStairSprite, scale);
            else if (part == DualTilePartType.VoidORStair)
                DrawSprite(partVisualData._voidOrStairSprite, scale);
        }

        private void DrawTileBase(TileBase tileBase, float scale = 5)
        {
            if (tileBase == null) return;


            if (tileBase is Tile)
            {
                Sprite tileSprite = (tileBase as Tile).sprite;

                DrawSprite(tileSprite, scale);
            }
            if (tileBase is AnimatedTile)
            {
                Sprite tileSprite = (tileBase as AnimatedTile).m_AnimatedSprites.First();

                DrawSprite(tileSprite, scale);
            }
        }

        private void DrawSprite(Sprite sprite, float scale = 5)
        {
            if (sprite == null) return;

            Texture2D tileTexture = sprite.texture;

            Rect spriteRect = sprite.textureRect;
            Rect texCoords = new Rect(
                spriteRect.x / tileTexture.width,
                spriteRect.y / tileTexture.height,
                spriteRect.width / tileTexture.width,
                spriteRect.height / tileTexture.height
            );

            Rect drawRect = GUILayoutUtility.GetRect(spriteRect.width * scale, spriteRect.height * scale,
                GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(false));

            GUI.DrawTextureWithTexCoords(drawRect, tileTexture, texCoords);
        }

        private void OnEnable()
        {
            data = target as DualTilesRuleData_2;
        }


    }
}