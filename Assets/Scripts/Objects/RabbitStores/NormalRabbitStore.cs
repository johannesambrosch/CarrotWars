using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NormalRabbitStore : BaseRabbitStore
{
    
    protected override void OnPurchaseSuccess()
    {
        var newRabbitObject = Instantiate(spawnPrefab);
        var newRabbit = newRabbitObject.GetComponent<Rabbit>();
        newRabbit.isSpawnAnim = true;
        newRabbit.SetLocation(newRabbit.initialLocation.x, newRabbit.initialLocation.y);
        newRabbit.suggestedPath = spawnTiles.ToArray().ToList();
        newRabbit.CommandMove();
    }

}
