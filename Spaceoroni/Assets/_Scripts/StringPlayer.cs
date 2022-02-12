using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StringPlayer : IPlayer
{
    public GameObject Builder1GameObject;
    public GameObject Builder2GameObject;
    Coordinate startLocationBuilder1;
    Coordinate startLocationBuilder2;
    public int playerI;

    // Start is called before the first frame update
    void Start()
    {
        Builder1 = Builder1GameObject.GetComponent<Builder>();
        Builder2 = Builder2GameObject.GetComponent<Builder>();
        setBuilderLocations();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public override IEnumerator beginTurn(Game g)
    {
        throw new System.NotImplementedException();
    }

    private void setBuilderLocations()
    {
        string s = Game.getStartLocations(playerI);
        startLocationBuilder1 = Coordinate.stringToCoord(s.Substring(0, 2));
        startLocationBuilder2 = Coordinate.stringToCoord(s.Substring(2, 2));

    }


    //places builders at 0,0 and 0,1 if not a player
    public override string PlaceBuilder(int i, Game g)
    {
        moveBuidler(i, i == 1 ? startLocationBuilder1 : startLocationBuilder2, g);
        return "";
    }

    public override Turn getNextTurn()
    {
        Turn t = new Turn(Game.getMoves());
        return t;
    }
}
