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
    }

    public override IEnumerator beginTurn(Game g)
    {
        throw new System.NotImplementedException();
    }

    private void setBuilderLocations(int player)
    {
        startLocationBuilder1 = StringGameReader.BuilderLocation(player).Item1;
        startLocationBuilder2 = StringGameReader.BuilderLocation(player).Item2;
    }


    public override IEnumerator PlaceBuilder(int builder, int player, Game g)
    {
        setBuilderLocations(player);
        moveBuidler(builder, builder == 1 ? startLocationBuilder1 : startLocationBuilder2, g);
        return null;
    }

    public override Turn getNextTurn()
    {
        Turn t = new Turn(StringGameReader.getMoves());
        return t;
    }
}
