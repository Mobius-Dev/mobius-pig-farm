using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

namespace PigFarm
{
    public class FruitTree : MonoBehaviour
    {
        [Header("Timing settings")]
        [SerializeField] private float respawnTime = 12f;

        [Header("Object settings")]
        [SerializeField] private GameObject fruitPrefab;
        [SerializeField] private Transform fruitSlotsParent;
        private List<GameObject> fruitSlots = new List<GameObject>();

        private bool IsFirstSpawnWave = true;
        private int attachedFruits = 0;

        void Awake()
        {
            FindFruitSlots();
            HideFruitGhosts();
        }

        void Start()
        {
            if (IsFirstSpawnWave) 
            {
                StartSpawnCountdown();
                IsFirstSpawnWave = false;
            }
        }

        private void StartSpawnCountdown()
        {
            DOVirtual.DelayedCall(respawnTime, () =>
            {
                Debug.Log("Spawning new fruits");
                SpawnFruits();
            });
        }
        private void SpawnFruits()
        {
            foreach (GameObject slot in fruitSlots)
            {
                if (slot != null && fruitPrefab != null)
                {
                    // ToDo replace this with object pooling later
                    // Spawn a new fruit from prefab
                    GameObject newFruit = Instantiate(fruitPrefab, slot.transform.position, slot.transform.rotation);

                    // Set parent to this slot
                    newFruit.transform.SetParent(slot.transform);

                    // Subscribe to an event fired when that fruit finishes ripening
                    Fruit fruitComponent = newFruit.GetComponent<Fruit>();
                    if (fruitComponent != null)
                    {
                        fruitComponent.OnFruitRipened += HandleFruitRipened;
                        attachedFruits++; // Count how many fruits are there on a tree
                    }
                }
            }
        }

        private void HandleFruitRipened(Fruit fruit)
        {
            // Unsub from a fruit
            fruit.OnFruitRipened -= HandleFruitRipened;
            
            // Deduce one fruit from a tree
            attachedFruits--;
            Debug.Log($"Fruit ripened. Remaining attached fruits: {attachedFruits}");

            // Check if all fruits ripened (detached), starting respawn
            if (attachedFruits == 0)
            {
                Debug.Log("All fruits collected. Starting new spawn countdown.");
                StartSpawnCountdown();
            }
        }

        private void FindFruitSlots()
        {
            //Find and add all fruit slots on this tree

            for (int i = 0; i < fruitSlotsParent.childCount; i++)
            {
                Transform currentFruitSlot = fruitSlotsParent.GetChild(i);
                fruitSlots.Add(currentFruitSlot.gameObject);
            }
        }

        private void HideFruitGhosts()
        {
            // Hides fruit ghosts, which should never be visible in the Play mode

            int i = fruitSlots.Count;
            foreach (GameObject fruitSlot in fruitSlots)
            {
                fruitSlot.GetComponent<MeshRenderer>().enabled = false;
            }
            Debug.Log("Found " + i + " fruit spawn slots");
        }
    }
  
}
