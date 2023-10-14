using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Entity/Character/Enemy Data")]
public class EnemyData : CharacterData
{
    [SerializeField] List<LearnableAbility> learnableAbilities;
    public List<LearnableAbility> LearnableAbilities => learnableAbilities;
}
