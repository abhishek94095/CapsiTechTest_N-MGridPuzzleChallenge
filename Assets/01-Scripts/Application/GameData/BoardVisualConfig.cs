using UnityEngine;

namespace AV.Framework.GameData
{
    [CreateAssetMenu(fileName = "BoardVisualConfig", menuName = "Game/Board Visual Config")]
    public sealed class BoardVisualConfig : ScriptableObject
    {
        [SerializeField] private GameObject normalCellPrefab;
        [SerializeField] private GameObject stoneCellPrefab;
        [SerializeField] private GameObject startCellPrefab;
        [SerializeField] private GameObject goalCellPrefab;
        [SerializeField] private GameObject playerPrefab;
        [SerializeField] private GameObject obstaclePrefab;

        public GameObject NormalCellPrefab => normalCellPrefab;
        public GameObject StoneCellPrefab => stoneCellPrefab;
        public GameObject StartCellPrefab => startCellPrefab;
        public GameObject GoalCellPrefab => goalCellPrefab;
        public GameObject PlayerPrefab => playerPrefab;
        public GameObject ObstaclePrefab => obstaclePrefab;
    }
}