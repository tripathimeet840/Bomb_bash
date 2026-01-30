using UnityEngine;

/// <summary>
/// Put on every character prefab (player & enemy prefabs) and set `characterIndex` to match the
/// element index in HealthUpgradeUI.characters[] for that prefab.
/// Used at runtime to find active instances of a given character type.
/// </summary>
public class CharacterIdentifier : MonoBehaviour
{
    public int characterIndex = 0;
}
