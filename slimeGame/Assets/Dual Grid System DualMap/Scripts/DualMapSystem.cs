using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Tilemaps;
using FK.DualMap.Data;

namespace FK.DualMap
{
    [ExecuteInEditMode]
    public class DualMapSystem : MonoBehaviour
    {
        public Tilemap mainMap;
        public Tilemap dualMap;

        public TileBase targetMainTile;
        public bool excludeMain = false;
        public TileBase targetStairTile;

        public BoundsInt bounds;

        public DualTilesRuleData ruleTilesData;

        public bool dynamicUpdate = false;

        private void OnEnable()
        {
            Tilemap.tilemapTileChanged += DynamicUpdate;
        }

        private void OnDisable()
        {
            Tilemap.tilemapTileChanged -= DynamicUpdate;
        }

#if UNITY_EDITOR
        [MenuItem("GameObject/2D Object/DualMap/Grid")]
        public static void CreateDualMapGrid(MenuCommand menuCommand)
        {
            GameObject grid = ObjectFactory.CreateGameObject("DualMapGrid", typeof(Grid));
            PlaceGameObject(grid);

            GameObject dualMap = CreateDualMapTilemap(menuCommand);
        }

        [MenuItem("GameObject/2D Object/DualMap/Tilemap")]
        public static GameObject CreateDualMapTilemap(MenuCommand menuCommand)
        {
            if (Selection.activeGameObject == null || Selection.activeGameObject.GetComponent<Grid>() == null)
            {
                CreateDualMapGrid(menuCommand); return null;
            }

            GameObject grid = Selection.activeGameObject;

            GameObject mainTilemap = ObjectFactory.CreateGameObject("Main Tilemap", typeof(Tilemap), typeof(TilemapRenderer));
            PlaceGameObject(mainTilemap);
            mainTilemap.GetComponent<TilemapRenderer>().sortingOrder = -10;

            GameObject dualMap = ObjectFactory.CreateGameObject("Dual Tilemap", typeof(DualMapSystem), typeof(Tilemap), typeof(TilemapRenderer));
            PlaceGameObject(dualMap);
            dualMap.GetComponent<DualMapSystem>().dualMap = dualMap.GetComponent<Tilemap>();
            dualMap.GetComponent<DualMapSystem>().mainMap = mainTilemap.GetComponent<Tilemap>();
            dualMap.GetComponent<DualMapSystem>().bounds = new BoundsInt(Vector3Int.zero, new Vector3Int(100, 100, 0));
            dualMap.transform.position = new Vector3(0.5f, -0.5f, 0);


            mainTilemap.transform.parent = grid.transform;
            dualMap.transform.parent = mainTilemap.transform;

            return dualMap;
        }

        public static void PlaceGameObject(GameObject obj)
        {
            StageUtility.PlaceGameObjectInCurrentStage(obj);
            GameObjectUtility.EnsureUniqueNameForSibling(obj);

            Undo.RegisterCreatedObjectUndo(obj, $"CreateObject {obj.name}");
            Selection.activeGameObject = obj;
        }
#endif

        public void DynamicUpdate(Tilemap map, Tilemap.SyncTile[] tiles)
        {
            if (!dynamicUpdate) return;

            if (map == mainMap)
            {
                CreateDualMap();
            }
        }


        public void CreateDualMap()
        {

            dualMap.ClearAllTiles();
            DualTile[,] dualTiles = GetDualTiles(mainMap, bounds);

            for (int y = 0; y < dualTiles.GetLength(1); y++)
            {
                for (int x = 0; x < dualTiles.GetLength(0); x++)
                {
                    foreach (var dualRuleTile in ruleTilesData.tileRules)
                    {
                        DualTile dualTile = dualTiles[x, y];
                        if (dualRuleTile.tileBase != null &&
                            EqModules(dualRuleTile.moduleRule, dualTile.module))
                        {
                            Vector3Int tileCoord = new Vector3Int(x - (int)(bounds.size.x / 2),
                                -y + (int)(bounds.size.y / 2), 0);
                            dualMap.SetTile(tileCoord, dualRuleTile.tileBase);
                        }
                    }
                }
            }
        }


        private bool EqModules(DualTileModule a, DualTileModule b)
        {
            return EqParts(a.topRight, b.topRight) && EqParts(a.topLeft, b.topLeft) &&
                EqParts(a.bottomRight, b.bottomRight) && EqParts(a.bottomLeft, b.bottomLeft);
        }

        private bool EqParts(DualTilePartType a, DualTilePartType b)
        {
            if (a == b) return true;

            if (a != DualTilePartType.VoidORStair && a != DualTilePartType.MainORStair &&
                b != DualTilePartType.VoidORStair && b != DualTilePartType.MainORStair) return false;

            List<DualTilePartType> aParts = new List<DualTilePartType>();
            if (a == DualTilePartType.MainORStair) { aParts.Add(DualTilePartType.Main); aParts.Add(DualTilePartType.Stair); }
            else if (a == DualTilePartType.VoidORStair) { aParts.Add(DualTilePartType.Void); aParts.Add(DualTilePartType.Stair); }
            else aParts.Add(a);

            List<DualTilePartType> bParts = new List<DualTilePartType>();
            if (b == DualTilePartType.MainORStair) { bParts.Add(DualTilePartType.Main); bParts.Add(DualTilePartType.Stair); }
            else if (b == DualTilePartType.VoidORStair) { bParts.Add(DualTilePartType.Void); bParts.Add(DualTilePartType.Stair); }
            else bParts.Add(b);

            foreach (DualTilePartType aPart in aParts)
            {
                foreach (DualTilePartType bPart in bParts)
                {
                    if (aPart == bPart) return true;
                }
            }

            return false;
        }


        public DualTile[,] GetDualTiles(Tilemap map, BoundsInt bounds)
        {
            DualTile[,] dualTiles = new DualTile[bounds.size.x + 1, bounds.size.y + 1];

            for (int y = 0; y < dualTiles.GetLength(1); y++)
            {
                for (int x = 0; x < dualTiles.GetLength(0); x++)
                {
                    DualTile dualTile = new DualTile();
                    dualTiles[x, y] = dualTile;

                    dualTile.module.topLeft = MapTileToPart(map, bounds, x, y);
                    dualTile.module.topRight = MapTileToPart(map, bounds, x + 1, y);
                    dualTile.module.bottomLeft = MapTileToPart(map, bounds, x, y + 1);
                    dualTile.module.bottomRight = MapTileToPart(map, bounds, x + 1, y + 1);

                }
            }
            return dualTiles;
        }

        private DualTilePartType MapTileToPart(Tilemap map, BoundsInt bounds, int xDual, int yDual)
        {
            int x = xDual - (int)(bounds.size.x / 2);
            int y = -yDual + (int)(bounds.size.y / 2);

            Vector3Int tilePos = new Vector3Int(x, y, 0);

            if (excludeMain)
            {
                if (map.GetTile(tilePos) != targetMainTile) return DualTilePartType.Main;
                else if (map.GetTile(tilePos) == targetStairTile) return DualTilePartType.Stair;
                else return DualTilePartType.Void;
            }
            else
            {
                if (map.GetTile(tilePos) == targetMainTile) return DualTilePartType.Main;
                else if (map.GetTile(tilePos) == targetStairTile) return DualTilePartType.Stair;
                else return DualTilePartType.Void;
            }
        }

    }


    [Serializable]
    public class DualRuleTile
    {

        public TileBase tileBase;


        public DualTileModule moduleRule;

        public DualRuleTile()
        {
            moduleRule = new DualTileModule();
        }

        public DualRuleTile(TileBase tileBase)
        {
            this.tileBase = tileBase;
            moduleRule = new DualTileModule();
        }

        public DualRuleTile(DualRuleTile copy)
        {
            this.tileBase = copy.tileBase;
            moduleRule = new DualTileModule(copy.moduleRule.topRight, copy.moduleRule.topLeft,
                copy.moduleRule.bottomRight, copy.moduleRule.bottomLeft);
        }

    }

    public class DualTile
    {
        public DualTileModule module;

        public DualTile()
        {
            module = new DualTileModule();
        }
    }

    [Serializable]
    public class DualTileModule
    {

        public DualTilePartType topRight;
        public DualTilePartType topLeft;
        public DualTilePartType bottomRight;
        public DualTilePartType bottomLeft;

        public DualTileModule()
        {
            topRight = DualTilePartType.Void;
            topLeft = DualTilePartType.Void;
            bottomRight = DualTilePartType.Void;
            bottomLeft = DualTilePartType.Void;
        }

        public DualTileModule(DualTilePartType topRight, DualTilePartType topLeft, DualTilePartType bottomRight, DualTilePartType bottomLeft)
        {
            this.topRight = topRight;
            this.topLeft = topLeft;
            this.bottomRight = bottomRight;
            this.bottomLeft = bottomLeft;
        }


    }

    [Serializable]
    public enum DualTilePartType
    {
        Void,
        Main,
        Stair,
        MainORStair,
        VoidORStair
    }

}
