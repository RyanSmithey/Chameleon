using System.Collections;
using System.Collections.Generic;
using PlayerStates;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Table
{
    public class ChildController : MonoBehaviour
    {
        public GameObject PlayerVisual;
        public GameObject EmptyVisual;
        
        private void Start()
        {
            PlayerAtTable.OnNewPlayerAtTable += GenerateAllVisuals;
            
            GenerateAllVisuals(null);
        }

        [Button]
        private void DestroyChildren()
        {
            while (transform.childCount > 1)
            {
                DestroyImmediate(transform.GetChild(1).gameObject);
            }
        }

        private void GenerateAllVisuals(PlayerAtTable obj)
        {
            DestroyChildren();

            foreach (PlayerAtTable PAT in PlayerAtTable.AllPlayers)
            {
                GeneratePlayerVisual(PAT);
            }

            while (transform.childCount < 9)
            {
                Instantiate(EmptyVisual, transform);
            }
        }

        private void GeneratePlayerVisual(PlayerAtTable obj)
        {
            GameObject G = Instantiate(PlayerVisual.gameObject, this.transform);

            G.GetComponent<PlayerVisual>().Initialize(obj);
        }
    }
}
