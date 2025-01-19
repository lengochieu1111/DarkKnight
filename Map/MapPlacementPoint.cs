using NaughtyAttributes;
using UnityEngine;

namespace HIEU_NL.Platformer.Script.Map
{
    public class MapPlacementPoint : RyoMonoBehaviour
    {
        [field: SerializeField, BoxGroup("SPAWN POINT")] public Transform SpawnPoint { get; private set; }
        [field: SerializeField, BoxGroup("SPAWN POINT")] public Transform[] SpawnPointArray { get; private set; }

        [field: SerializeField, BoxGroup("DESTINATION POINT")] public Transform DestinationPoint { get; private set; }
        [field: SerializeField, BoxGroup("DESTINATION POINT")] public Transform[] DestinationPointArray { get; private set; }

        [field: SerializeField, BoxGroup("ATTACK POINT")] public Transform AttackPoint { get; private set; }
        [field: SerializeField, BoxGroup("ATTACK POINT")] public Transform[] AttackPointArray { get; private set; }

        [field: SerializeField, BoxGroup("HIDE POINT")] public Transform HidePoint { get; private set; }
        [field: SerializeField, BoxGroup("HIDE POINT")] public Transform[] HidePointArray { get; private set; }
    }
}