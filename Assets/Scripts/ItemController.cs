using UnityEngine;
using System.Collections;

/// <summary>
/// This class is mostly a collection of stats for the item
/// </summary>
public class ItemController : MonoBehaviour {

    // powerup values add effects like damage boost or health restore on use
    // on player carrying the item. 

    //passive stat boosts when equipped
    public int healthPowerup, damagePowerup, armorPowerup;

    //one time uses like healing
    public int healthOnUse, armorOnUse;

    //other stuff
    public bool equippable, flying, used;    
    
    // I think the damage caused by a thrown item
    // should be calculated from the speed and weight of the flying item rather than
    // just using the damage stat?

	// Use this for initialization
	void Start () {
	
	}
}
