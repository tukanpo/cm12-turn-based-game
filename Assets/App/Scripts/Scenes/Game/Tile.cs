using UnityEngine;

namespace App.Scenes.Game
{
    public class Tile : MonoBehaviour
    {
        public Vector2Int Coord { get; private set; }

        static readonly Color CheckerColor1 = new Color(0.85f, 0.85f, 0.85f);
        static readonly Color CheckerColor2 = new Color(0.42f, 0.42f, 0.42f);
        
        public static Tile Spawn(Tile prefab, Transform parent, Vector2Int coord)
        {
            // とりあえず決め打ち
            const float meshSize = 1.5f;

            var position = new Vector3(coord.x * meshSize, 0, coord.y * meshSize);
            var tile = Instantiate(prefab, position, prefab.transform.rotation);
            tile.transform.parent = parent;
            tile.Coord = coord;
            
            // とりあえずマテリアルの色を変えとく
            tile.SetColor(false);
            
            return tile;
        }

        public void SetColor(bool isStrongColor)
        {
            // 市松模様にする
            var color = (Coord.x + Coord.y) % 2 == 0 ? CheckerColor1 : CheckerColor2;
            if (isStrongColor)
            {
                color += new Color(0, 0.5f, 0.6f);
            }

            GetComponent<Renderer>().material.color = color;
        }
    }
}
