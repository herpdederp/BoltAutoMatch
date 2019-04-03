using System;
using Bolt;
using UdpKit;
using UnityEngine;
using udpkit.platform.photon;

using Bolt.Utils;
using Bolt.Photon;
using System.Linq;

public class Menu : Bolt.GlobalEventListener
{

    bool redTeam;
    bool blueTeam;

    bool noServersFound;

    bool connecting;

    public override void BoltStartBegin()
    {
        BoltNetwork.RegisterTokenClass<PhotonRoomProperties>();
    }




    public override void BoltShutdownBegin(AddCallback registerDoneCallback)
    {
        registerDoneCallback(Test0);
    }



    void Test0()
    {
        connecting = false;

        if (blueTeam)
            BoltLauncher.StartClient();
    }


    private void OnGUI()
    {
        if (BoltNetwork.IsRunning == false)
        {

            GUILayout.BeginArea(new Rect(10, 10, Screen.width - 20, Screen.height - 20));

            if (GUILayout.Button("Red Team", GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true)))
            {
                redTeam = true;
                // START CLIENT
                BoltLauncher.StartServer();
            }

            if (GUILayout.Button("Blue Team", GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true)))
            {
                blueTeam = true;
                // START CLIENT
                BoltLauncher.StartClient();
            }

            GUILayout.EndArea();
        }
    }

    public override void BoltStartDone()
    {
        if (BoltNetwork.IsServer)
        {


            PhotonRoomProperties token = new PhotonRoomProperties();
            token.IsOpen = true;
            token.IsVisible = true;

            if (blueTeam == true)
                token.AddRoomProperty("t", 1);

            if (redTeam == true)
                token.AddRoomProperty("t", 2);
            staticData.lobbyName = Guid.NewGuid().ToString();
            var matchName = staticData.lobbyName;

            BoltNetwork.SetServerInfo(matchName, token);
            BoltNetwork.LoadScene("game");
        }
    }


    public override void SessionListUpdated(Map<Guid, UdpSession> sessionList)
    {
        Debug.LogFormat("Session list updated: {0} total sessions", sessionList.Count);

        var shuffled = sessionList.OrderBy(a => Guid.NewGuid()).ToList();


        if (connecting == false)
            foreach (var session in shuffled)
            {
                UdpSession udpSession = session.Value as UdpSession;
                PhotonSession photonSession = udpSession as PhotonSession;

                if (photonSession.Source == UdpSessionSource.Photon)
                {

                    if (photonSession.Properties.ContainsKey("t"))
                    {
                        if (redTeam)
                        {
                            if ((int)photonSession.Properties["t"] == 1)
                            {
                                BoltNetwork.Connect(photonSession);
                                connecting = true;
                            }
                        }
                        else if (blueTeam)
                            if ((int)photonSession.Properties["t"] == 2)
                            {
                                BoltNetwork.Connect(photonSession);
                                connecting = true;
                            }
                    }
                }
            }
    }

    public override void ConnectRefused(UdpEndPoint endpoint, IProtocolToken token)
    {
        base.ConnectRefused(endpoint, token);
        BoltLauncher.Shutdown();
    }

    public override void SessionConnectFailed(UdpSession session, IProtocolToken token)
    {
        Debug.Log("1");
        BoltLauncher.Shutdown();
    }

    public override void SessionCreationFailed(UdpSession session)
    {
        Debug.Log("2");
    }

    public override void SessionCreated(UdpSession session)
    {
        Debug.Log("3");
    }



}