using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StringPlayer : IPlayer
{
    public GameObject Builder1GameObject;
    public GameObject Builder2GameObject;
    Coordinate startLocationBuilder1 = new Coordinate(0, 0);
    Coordinate startLocationBuilder2 = new Coordinate(0, 1);

    int turnCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        Builder1 = Builder1GameObject.GetComponent<Builder>();
        Builder2 = Builder2GameObject.GetComponent<Builder>();
        addMoves();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public override IEnumerator beginTurn(Game g)
    {
        throw new System.NotImplementedException();
    }

    public void addMoves()
    {
        List<string> moveString = new List<string>();
        moveString.Add("A0B0B1");
        moveString.Add("A1B1B2");
        moveString.Add("B0C0C1");


        foreach (string s in moveString)
        {
            turns.Add(new Turn(Coordinate.stringToCoord(s.Substring(0, 2)), Coordinate.stringToCoord(s.Substring(2, 2)), Coordinate.stringToCoord(s.Substring(4, 2))));
        }
    }

    //places builders at 0,0 and 0,1 if not a player
    public override string PlaceBuilder(int i, Game g)
    {
        moveBuidler(i, i == 1 ? startLocationBuilder1 : startLocationBuilder2, g);
        return "";
    }

    public override Turn getNextTurn()
    {
        return turns[turnCount++];
    }
}
