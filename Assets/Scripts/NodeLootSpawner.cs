using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using DefaultNamespace.NodeEffects;
using Random = UnityEngine.Random;

public enum RollType
{
    Default,
    High,
    NoWeapon,
    WeaponOnly
}

[Serializable]
public class RolledNode
{
    public NodeArchetype archetype;
    public float fireRate;
    public float nodePower;
    public float powerFactor;
    public Vector2[] pulseDirections;
    public PowerNode.Rarity rarity;
}

[Serializable]
public class StatRange
{
    public float min;
    public float max;

    public float Roll()
    {
        return Random.Range(min, max);
    }
}

[Serializable]
public class PulseDirection
{
    public float x;
    public float y;

    public Vector2 ToVector2()
    {
        return new Vector2(x, y);
    }
}

[Serializable]
public class NodeArchetype
{
    public string name;
    public string description;
    public string iconKey;
    public string effectType;
    public string rarity;
    public string[] tags;
    public StatRange fireRate;
    public StatRange nodePower;
    public StatRange powerFactor;
    public PulseDirection[] pulseDirections;
}

[Serializable]
public class LootTable
{
    public NodeArchetype[] archetypes;
}

[Serializable]
public class IconMapping
{
    public string key;
    public Sprite sprite;
}

public class NodeLootSpawner : MonoBehaviour
{
    public GameObject nodeBasePrefab;
    public string lootTablePath = "LootTables/NodeLootTable";
    public IconMapping[] iconMappings;
    public Camera spawnCamera;
    public Transform floatersCanvas;
    
    private LootTable lootTable;
    private Dictionary<string, Sprite> iconLookup = new Dictionary<string, Sprite>();
    private Dictionary<string, Type> effectTypeMap = new Dictionary<string, Type>();

    private void Awake()
    {
        InitializeEffectTypeMap();
        InitializeIconLookup();
        LoadLootTable();
        
        if (spawnCamera == null)
            spawnCamera = Camera.main;
    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.tKey.wasPressedThisFrame)
        {
            Vector3 spawnPos = GetRandomSpawnPosition();
            SpawnRandom(spawnPos);
        }
    }

    private Vector3 GetRandomSpawnPosition()
    {
        Vector3 viewportPos = new Vector3(Random.Range(0.6f, 0.9f), Random.Range(0.2f, 0.8f), 10f);
        return spawnCamera.ViewportToWorldPoint(viewportPos);
    }

    private void InitializeEffectTypeMap()
    {
        effectTypeMap["GeneratorEffect"] = typeof(GeneratorEffect);
        effectTypeMap["RailgunEffect"] = typeof(RailgunEffect);
        effectTypeMap["AutoGunEffect"] = typeof(AutoGunEffect);
        effectTypeMap["LaserBeamEffect"] = typeof(LaserBeamEffect);
        effectTypeMap["RepairEffect"] = typeof(RepairEffect);
        effectTypeMap["MissileEffect"] = typeof(MissileEffect);
        effectTypeMap["SpaceGeneratorEffect"] = typeof(SpaceGeneratorEffect);
    }

    private void InitializeIconLookup()
    {
        foreach (var mapping in iconMappings)
        {
            if (!string.IsNullOrEmpty(mapping.key) && mapping.sprite != null)
            {
                iconLookup[mapping.key] = mapping.sprite;
            }
        }
    }

    public Sprite GetIcon(string key)
    {
        if (string.IsNullOrEmpty(key)) return null;
        if (iconLookup.TryGetValue(key, out Sprite sprite))
            return sprite;
        return null;
    }

    private void LoadLootTable()
    {
        TextAsset json = Resources.Load<TextAsset>(lootTablePath);
        if (json == null)
        {
            Debug.LogError($"Failed to load loot table from: {lootTablePath}");
            return;
        }
        
        lootTable = JsonUtility.FromJson<LootTable>(json.text);
        CacheArchetypesByRarity();
        Debug.Log($"Loaded {lootTable.archetypes.Length} node archetypes");
    }

    private Dictionary<PowerNode.Rarity, List<NodeArchetype>> archetypesByRarity = new Dictionary<PowerNode.Rarity, List<NodeArchetype>>();

    private void CacheArchetypesByRarity()
    {
        archetypesByRarity[PowerNode.Rarity.Common] = new List<NodeArchetype>();
        archetypesByRarity[PowerNode.Rarity.Rare] = new List<NodeArchetype>();
        archetypesByRarity[PowerNode.Rarity.Legendary] = new List<NodeArchetype>();

        foreach (var archetype in lootTable.archetypes)
        {
            if (Enum.TryParse(archetype.rarity, true, out PowerNode.Rarity rarity))
            {
                archetypesByRarity[rarity].Add(archetype);
            }
        }
    }

    public List<NodeArchetype> GetArchetypesByRarity(PowerNode.Rarity rarity)
    {
        if (archetypesByRarity.TryGetValue(rarity, out var list))
            return list;
        return new List<NodeArchetype>();
    }

    public PowerNode.Rarity RollRarity(RollType rollType)
    {
        float roll = Random.Range(0f, 100f);

        if (rollType == RollType.High)
        {
            if (roll < 60f) return PowerNode.Rarity.Rare;
            return PowerNode.Rarity.Legendary;
        }
        else // Default, NoWeapon, WeaponOnly use default weights
        {
            if (roll < 60f) return PowerNode.Rarity.Common;
            if (roll < 90f) return PowerNode.Rarity.Rare;
            return PowerNode.Rarity.Legendary;
        }
    }

    private bool HasTag(NodeArchetype archetype, string tag)
    {
        if (archetype.tags == null) return false;
        foreach (var t in archetype.tags)
        {
            if (t == tag) return true;
        }
        return false;
    }

    private List<NodeArchetype> FilterByRollType(List<NodeArchetype> archetypes, RollType rollType)
    {
        List<NodeArchetype> filtered = new List<NodeArchetype>();
        
        foreach (var a in archetypes)
        {
            // Always exclude starter-only nodes from random rolls
            if (HasTag(a, "starter"))
                continue;
            
            if (rollType == RollType.NoWeapon && HasTag(a, "weapon"))
                continue;
            
            if (rollType == RollType.WeaponOnly && !HasTag(a, "weapon"))
                continue;
            
            filtered.Add(a);
        }
        
        return filtered;
    }

    public NodeArchetype PickRandomArchetype(RollType rollType)
    {
        PowerNode.Rarity rarity = RollRarity(rollType);
        var candidates = FilterByRollType(GetArchetypesByRarity(rarity), rollType);

        if (candidates.Count == 0)
        {
            candidates = FilterByRollType(GetArchetypesByRarity(PowerNode.Rarity.Rare), rollType);
            if (candidates.Count == 0)
                candidates = FilterByRollType(GetArchetypesByRarity(PowerNode.Rarity.Common), rollType);
        }

        if (candidates.Count == 0)
        {
            Debug.LogError($"No archetypes available for rollType: {rollType}");
            return null;
        }

        return candidates[Random.Range(0, candidates.Count)];
    }

    public RolledNode RollArchetype(NodeArchetype archetype)
    {
        RolledNode rolled = new RolledNode();
        rolled.archetype = archetype;
        rolled.fireRate = archetype.fireRate.Roll();
        rolled.nodePower = archetype.nodePower.Roll();
        rolled.powerFactor = archetype.powerFactor.Roll();

        if (Enum.TryParse(archetype.rarity, true, out PowerNode.Rarity rarity))
        {
            rolled.rarity = rarity;
        }

        if (archetype.pulseDirections != null && archetype.pulseDirections.Length > 0)
        {
            rolled.pulseDirections = new Vector2[archetype.pulseDirections.Length];
            for (int i = 0; i < archetype.pulseDirections.Length; i++)
            {
                rolled.pulseDirections[i] = archetype.pulseDirections[i].ToVector2();
            }
        }
        else
        {
            rolled.pulseDirections = new Vector2[0];
        }

        return rolled;
    }

    public RolledNode[] RollChoices(RollType rollType, int count = 3)
    {
        RolledNode[] choices = new RolledNode[count];
        for (int i = 0; i < count; i++)
        {
            NodeArchetype archetype = PickRandomArchetype(rollType);
            choices[i] = RollArchetype(archetype);
        }
        return choices;
    }

    public PowerNode Spawn(string archetypeName, Vector3 position)
    {
        NodeArchetype archetype = FindArchetype(archetypeName);
        if (archetype == null)
        {
            Debug.LogError($"Archetype not found: {archetypeName}");
            return null;
        }

        return SpawnFromArchetype(archetype, position);
    }

    public PowerNode SpawnRandom(Vector3 position)
    {
        if (lootTable == null || lootTable.archetypes.Length == 0)
        {
            Debug.LogError("No archetypes available");
            return null;
        }

        NodeArchetype archetype = lootTable.archetypes[Random.Range(0, lootTable.archetypes.Length)];
        return SpawnFromArchetype(archetype, position);
    }

    public PowerNode SpawnFromArchetype(NodeArchetype archetype, Vector3 position)
    {
        GameObject go;
        if (floatersCanvas != null)
        {
            go = Instantiate(nodeBasePrefab, position, Quaternion.identity, floatersCanvas);
        }
        else
        {
            go = Instantiate(nodeBasePrefab, position, Quaternion.identity);
        }
        PowerNode node = go.GetComponent<PowerNode>();

        node.NodeName = archetype.name;
        node.Description = archetype.description;

        node.FireRate = archetype.fireRate.Roll();
        node.NodePower = archetype.nodePower.Roll();
        node.powerFactor = archetype.powerFactor.Roll();

        if (!string.IsNullOrEmpty(archetype.rarity))
        {
            if (Enum.TryParse(archetype.rarity, true, out PowerNode.Rarity rarity))
            {
                node.rarity = rarity;
            }
        }

        if (archetype.pulseDirections != null && archetype.pulseDirections.Length > 0)
        {
            node.PulseDirections = new Vector2[archetype.pulseDirections.Length];
            for (int i = 0; i < archetype.pulseDirections.Length; i++)
            {
                node.PulseDirections[i] = archetype.pulseDirections[i].ToVector2();
            }
        }

        if (!string.IsNullOrEmpty(archetype.iconKey) && iconLookup.TryGetValue(archetype.iconKey, out Sprite icon))
        {
            SpriteRenderer sr = node.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.sprite = icon;
            }
        }

        if (!string.IsNullOrEmpty(archetype.effectType) && effectTypeMap.TryGetValue(archetype.effectType, out Type effectType))
        {
            NodeEffect effect = (NodeEffect)go.AddComponent(effectType);
            node.nodeEffect = effect;
        }

        return node;
    }

    public PowerNode SpawnFromRolledNode(RolledNode rolled, Vector3 position)
    {
        GameObject go;
        if (floatersCanvas != null)
        {
            go = Instantiate(nodeBasePrefab, position, Quaternion.identity, floatersCanvas);
        }
        else
        {
            go = Instantiate(nodeBasePrefab, position, Quaternion.identity);
        }
        PowerNode node = go.GetComponent<PowerNode>();

        node.NodeName = rolled.archetype.name;
        node.Description = rolled.archetype.description;
        node.FireRate = rolled.fireRate;
        node.NodePower = rolled.nodePower;
        node.powerFactor = rolled.powerFactor;
        node.rarity = rolled.rarity;
        node.PulseDirections = rolled.pulseDirections;

        if (!string.IsNullOrEmpty(rolled.archetype.iconKey) && iconLookup.TryGetValue(rolled.archetype.iconKey, out Sprite icon))
        {
            SpriteRenderer sr = node.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.sprite = icon;
            }
        }

        if (!string.IsNullOrEmpty(rolled.archetype.effectType) && effectTypeMap.TryGetValue(rolled.archetype.effectType, out Type effectType))
        {
            NodeEffect effect = (NodeEffect)go.AddComponent(effectType);
            node.nodeEffect = effect;
        }

        return node;
    }

    private NodeArchetype FindArchetype(string name)
    {
        if (lootTable == null) return null;

        foreach (var archetype in lootTable.archetypes)
        {
            if (archetype.name == name)
                return archetype;
        }
        return null;
    }
}
