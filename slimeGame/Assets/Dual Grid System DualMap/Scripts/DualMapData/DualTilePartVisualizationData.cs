using UnityEngine;


namespace FK.DualMap.Data
{
    [CreateAssetMenu(fileName = "new DualTilePartVisualizationData", menuName = "DualMap Visual Data/Dual Tile Part Visualization")]
    public class DualTilePartVisualizationData : ScriptableObject
    {
        public Sprite _voidSprite;
        public Sprite _mainSprite;
        public Sprite _stairSprite;
        public Sprite _mainOrStairSprite;
        public Sprite _voidOrStairSprite;
    }
}