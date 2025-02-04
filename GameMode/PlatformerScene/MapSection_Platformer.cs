using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace HIEU_NL.Platformer.Script.Game
{
    using HIEU_NL.Platformer.Script.Map;
    
    public class MapSection_Platformer : RyoMonoBehaviour
    {
        [field: SerializeField] public Map_Platformer mapPlatformer;
    }

}
