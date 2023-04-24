using BehaviorDesigner.Runtime;
using UnityEngine;

namespace App
{
	public class GameMgr : MonoBehaviour
	{
        public GameObject enemy, player;
        public int numEnemy = 1, numPlayer = 1;
        private static GameMgr _instance = null;
        
        public static GameMgr Instance => _instance;
        public SharedGameObjectList enemies, players;

        private void Awake()
        {
            _instance = this;
            enemies = GlobalVariables.Instance.GetVariable("AllEnemy") as SharedGameObjectList;
            players = GlobalVariables.Instance.GetVariable("AllPlayer") as SharedGameObjectList;
        }

        private void Start()
        {
            for (int i = 0; i < numPlayer; i++)
            {
                GameObject temp = Instantiate(player, new Vector3(Random.Range(-50.0f, 50.0f), 1, Random.Range(-50.0f, 50.0f)), Quaternion.identity);
                players.Value.Add(temp);
            }
            
            for (int i = 0; i < numEnemy; i++)
            {
                GameObject temp = Instantiate(enemy, new Vector3(Random.Range(-50.0f, 50.0f), 1, Random.Range(-50.0f, 50.0f)), Quaternion.identity);
                enemies.Value.Add(temp);
            }
        }
    }
}
