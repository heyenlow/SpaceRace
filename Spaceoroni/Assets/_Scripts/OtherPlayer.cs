using ExitGames.Client.Photon;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OtherPlayer : IPlayer
{
    bool recievedEvent = false;
    int builderInt;
    Coordinate builderCoord;

    //this needs to wait for a turn to be recieved
    public override IEnumerator beginTurn(Game g)
    {
        recievedEvent = false;
        while (!recievedEvent)
        {
            yield return new WaitForEndOfFrame();
        }
        yield return null;
    }
    public override IEnumerator PlaceBuilder(int builder, int player, Game g)
    {
        recievedEvent = false;
        while (!recievedEvent)
        {
            yield return new WaitForEndOfFrame();
        }
        moveBuidler(builder, builderCoord, g);

        Debug.Log("Moving builder " + builder + " to " + Coordinate.coordToString(builderCoord));
        yield return true;
    }
    public void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;
        Debug.Log("CaughtEvent code:" + eventCode);
        if (eventCode == NetworkingManager.RAISE_TURN)
        {
            object[] data = (object[])photonEvent.CustomData;

            Turn turn = new Turn(data);

            turns.Add(turn);
            recievedEvent = true;
        }
        if(eventCode == NetworkingManager.RAISE_INITIAL_BUILDER)
        {
            object[] data = (object[])photonEvent.CustomData;
            
            builderInt = (int)data[0];
            builderCoord = new Coordinate((int)data[1], (int)data[2]);

            Debug.Log(Coordinate.coordToString(builderCoord));

            recievedEvent = true;
        }
    }
    private void OnEnable()
    {
        PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
    }

    private void OnDisable()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
    }
}
