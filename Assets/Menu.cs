using System;
using Bolt;
using UdpKit;
using UnityEngine;
using udpkit.platform.photon;
using udpkit.platform.photon.photon;
using Bolt.photon;
using Bolt.Utils;

public class Menu : Bolt.GlobalEventListener
{

    bool redTeam;
    bool blueTeam;

    bool noServersFound;
    float timer = 0;

    public override void BoltStartBegin()
    {
        BoltNetwork.RegisterTokenClass<PhotonRoomProperties>();
    }


    private void FixedUpdate()
    {
        if (BoltNetwork.IsClient == true)
        {
            timer += Time.fixedDeltaTime;

            if (timer > 5f)
            {
                noServersFound = true;
                BoltLauncher.Shutdown();
            }

        }
    }

    public override void BoltShutdownBegin(AddCallback registerDoneCallback)
    {
        registerDoneCallback(Test0);
    }

    void Test0()
    {
        BoltLauncher.StartServer();
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
                BoltLauncher.StartClient();
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

            var matchName = Guid.NewGuid().ToString();

            BoltNetwork.SetServerInfo(matchName, token);
            BoltNetwork.LoadScene("game");
        }
    }

    public override void SessionListUpdated(Map<Guid, UdpSession> sessionList)
    {
        Debug.LogFormat("Session list updated: {0} total sessions", sessionList.Count);

        foreach (var session in sessionList)
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
                            BoltNetwork.Connect(photonSession);
                    }
                    else if (blueTeam)
                        if ((int)photonSession.Properties["t"] == 2)
                            BoltNetwork.Connect(photonSession);

                }
            }
        }
    }

}