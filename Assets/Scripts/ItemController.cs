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

    //audio effects
    public string onUseAudioFxFilename, onBreakAudioFxFilename, onHitAudioFxFilename;
    private AudioClip onUseAudioFx, onBreakAudioFx, onHitAudioFx;
    private AudioSource audioSource;

    //other stuff
    public int hitpoints; //used to break the item
    private bool dead;
    public bool equippable, flying, used, destructable, invulnerable; 
    //if the item is destructable, the parts should be de-parented rather than removed upon destruction
    
    // I think the damage caused by a thrown item
    // should be calculated from the speed and weight of the flying item rather than
    // just using the damage stat?

	// Use this for initialization
	private void Start () {

        audioSource = GetComponent<AudioSource>();

        if (audioSource != null)
        {
            if (!string.IsNullOrEmpty(onUseAudioFxFilename))
                onUseAudioFx = Resources.Load<AudioClip>(onUseAudioFxFilename);

            if (!string.IsNullOrEmpty(onHitAudioFxFilename))
                onHitAudioFx = Resources.Load<AudioClip>(onHitAudioFxFilename);

            if (!string.IsNullOrEmpty(onBreakAudioFxFilename))
                onBreakAudioFx = Resources.Load<AudioClip>(onBreakAudioFxFilename);
        }
    }

    private void Update()
    {
        if(!dead && hitpoints <= 0)
        {
            Kill();
        }

        //let the sound play first before destroying the object?
        if(dead && !destructable && !audioSource.isPlaying)
        {
            Destroy(this.gameObject);
        }
    }

    public void Damage(int damage)
    {
        if (!invulnerable)
        {
            this.hitpoints -= damage;
            Debug.Log(this.name + " took " + damage + " damage");
        }
    }

    private void Kill()
    {
        TriggerSoundEffect(ItemSoundFx.Break);

        if (destructable)
        {
            var parts = this.GetComponentsInChildren<Rigidbody>();
            for(int i = 0; i < parts.Length; i++)
            {
                parts[i].transform.parent = null;
                parts[i].isKinematic = false;
                parts[i].velocity += Vector3.forward / parts[i].mass;

                //TODO: figure out how to make the parts equippable
            }
        }
        this.dead = true;
        Debug.Log(this.name + " died!");
    }

    public void TriggerSoundEffect(ItemSoundFx effect)
    {
        if (audioSource == null || audioSource.isPlaying) return;

        switch (effect) {
            case ItemSoundFx.Use:
                if(onUseAudioFx != null)
                    audioSource.PlayOneShot(onUseAudioFx);
                break;
            case ItemSoundFx.Hit:
                if(onHitAudioFx != null)
                    audioSource.PlayOneShot(onHitAudioFx);
                break;
            case ItemSoundFx.Break:
                if(onBreakAudioFx != null)
                    audioSource.PlayOneShot(onBreakAudioFx);
                break;
        }
    }

    public enum ItemSoundFx
    {
        Use,
        Hit,
        Break
    }
}
