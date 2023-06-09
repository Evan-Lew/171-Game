using System.Collections.Generic;
using UnityEngine;

public class FollowPosition : MonoBehaviour
{
    [SerializeField] private GameObject targetObject;
    [Tooltip("special only works for object that has different origin in sprite renderer")]
    [SerializeField] private enum spritePos {center, bottom, ObjPosition}
    [SerializeField] private spritePos targetPoint;
    
    [SerializeField] GameObject player, enemy;
    
    // List to keep track of all player sprites
    public List<GameObject> playerSpriteGameObjects = new();
    
    // List to keep track of all enemy sprites
    public List<GameObject> enemySpriteGameObjects = new();

    // Enum to keep track if the character is a player or enemy
    private enum TypeOfCharacter {Player, Enemy}
    [SerializeField] private TypeOfCharacter characterIs;

    void Update()
    {
        Character playerCharacter = player.GetComponent<Character>();
        // Change the target object for the position of the particles for each player
        if (playerCharacter.CharacterData.characterName == "Xu Xuan" && characterIs == TypeOfCharacter.Player)
        {
            targetObject = playerSpriteGameObjects[0];
        }
        else if (playerCharacter.CharacterData.characterName == "Bai She Zhuan" && characterIs == TypeOfCharacter.Player)
        {
            targetObject = playerSpriteGameObjects[1];
        }
        
        
        Character enemyCharacter = enemy.GetComponent<Character>();
        // Change the target object for the position of the particles for each enemy
        if (enemyCharacter.CharacterData.characterName == "Peng Hou" && characterIs == TypeOfCharacter.Enemy)
        {
            targetObject = enemySpriteGameObjects[0];
        }
        else if (enemyCharacter.CharacterData.characterName == "Ink Golem" && characterIs == TypeOfCharacter.Enemy)
        {
            targetObject = enemySpriteGameObjects[1];
        }
        else if (enemyCharacter.CharacterData.characterName == "Ink Chimera" && characterIs == TypeOfCharacter.Enemy)
        {
            targetObject = enemySpriteGameObjects[2];
        }
        else if (enemyCharacter.CharacterData.characterName == "Zhenniao" && characterIs == TypeOfCharacter.Enemy)
        {
            targetObject = enemySpriteGameObjects[3];
        }
        else if (enemyCharacter.CharacterData.characterName == "Xiao Qing" && characterIs == TypeOfCharacter.Enemy)
        {
            targetObject = enemySpriteGameObjects[4];
        }
        else if (enemyCharacter.CharacterData.characterName == "Fa Hai" && characterIs == TypeOfCharacter.Enemy)
        {
            targetObject = enemySpriteGameObjects[5];
        }
        
        // Get the center of the sprite
        if (targetPoint == spritePos.center)
        {
            this.gameObject.transform.position = targetObject.GetComponent<GetSpriteObjCenter>().center;
        }
        // Get the bottom of the sprite
        else if (targetPoint == spritePos.bottom)
        {
            this.gameObject.transform.position = targetObject.GetComponent<GetSpriteObjCenter>().bottom;
        }
        else
        {
            this.gameObject.transform.position = targetObject.transform.position;
        }
     
    }
}
