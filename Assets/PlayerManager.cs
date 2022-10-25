using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerRoles
{
    Whitehat = 0,
    Blackhat,
    Observer
}

public enum PlayerSpecies
{
    AI = 0,
    Human,
    Unknown
}
//later we might add PlayerSide, side1 could have both Whitehats and Blackhats


public class TaiserPlayer
{
    public string name;
    public PlayerRoles role;
    public PlayerSpecies species;
    public List<PlayerSpecies> teammateSpeciesList = new List<PlayerSpecies>();

    public TaiserPlayer(string pName, PlayerRoles pRole, PlayerSpecies pSpecies,
        List<PlayerSpecies> teamSpeciesList = null)
    {
        name = pName;
        role = pRole;
        species = pSpecies;
        teammateSpeciesList = teamSpeciesList;
    }
}



public class PlayerMgr : MonoBehaviour
{

    public static PlayerMgr inst;
    private void Awake()
    {
        inst = this;
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


}
