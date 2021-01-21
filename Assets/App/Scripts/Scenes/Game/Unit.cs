using UnityEngine;

namespace App.Scenes.Game
{
    public class Unit : MonoBehaviour
    {
        public GridCoord Coord { get; private set; }
        
        public static Unit Spawn(Unit prefab, Transform parent, GridCoord coord)
        {
            var unit = Instantiate(prefab, coord.ToVector3(), prefab.transform.rotation);
            unit.transform.parent = parent;
            unit.Coord = coord;
            
            return unit;
        }
    }
}
