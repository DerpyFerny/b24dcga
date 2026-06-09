using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "AnimalEvolutionTree", menuName = "ScriptableObjects/Animal Evolution Tree")]
public class AnimalEvolutionTree : ScriptableObject
{
    public List<AnimalData> levels;
    public AnimalData GetLevelData(int level = 0)
    {
        if (level == 0)
        {
            // Tỉ lệ: 1-5 là 15%, 25%, 20%, 25%, 15%
            int[] cumulative = { 15, 40, 60, 85, 100 }; // Cộng dồn
            int rand = UnityEngine.Random.Range(0, 100);
            int chosenLevel = 0;
            for (int i = 0; i < cumulative.Length; i++)
            {
                if (rand < cumulative[i])
                {
                    chosenLevel = i;
                    break;
                }
            }
            level = chosenLevel;
            // level = UnityEngine.Random.Range(0, levels.Count);
            return levels[level];
        }
        return (level >= 0 && level < levels.Count) ? levels[level] : null;
    }

    public int GetMaxLevel() => levels.Count - 1;
}

[System.Serializable]
public class AnimalData
{
    public string name;
    public GameObject prefab;
    public float scaleRatio = 1f;
}
