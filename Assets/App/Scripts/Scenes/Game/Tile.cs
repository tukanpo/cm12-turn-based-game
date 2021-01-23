using UnityEngine;

namespace App.Scenes.Game
{
    public class Tile : MonoBehaviour
    {
        public GridCoord Coord { get; private set; }

        static readonly Color CheckerColor1 = new Color(0.85f, 0.85f, 0.85f);
        static readonly Color CheckerColor2 = new Color(0.42f, 0.42f, 0.42f);
        
        public static Tile Spawn(Tile prefab, Transform parent, GridCoord coord)
        {
            // 決め打ち
            const float meshSize = 1.2f;

            var position = new Vector3(coord.X * meshSize, 0, coord.Y * meshSize);
            var tile = Instantiate(prefab, position, prefab.transform.rotation);
            tile.transform.parent = parent;
            tile.Coord = coord;
            
            // とりあえず市松模様にする
            var color = (coord.X + coord.Y) % 2 == 0 ? CheckerColor1 : CheckerColor2;
            tile.GetComponent<Renderer>().material.color = color;
            
            return tile;
        }
    }
}
