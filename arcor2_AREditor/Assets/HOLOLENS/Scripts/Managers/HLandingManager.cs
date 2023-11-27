using System;
using System.Threading.Tasks;
using Base;
using UnityEngine.UI;
using Hololens;
using UnityEngine;
using MixedReality.Toolkit.UX;
using MixedReality.Toolkit;

public class HLandingManager : Singleton<HLandingManager>
{
    //public Button connectToServerBtn;
    public GameObject landingScreen;

    public StatefulInteractable connectButton;
    public MRTKUGUIInputField domain;
    public MRTKUGUIInputField port;
    public MRTKUGUIInputField user;

    private void Start()
    {
        Debug.Assert(domain != null);
        Debug.Assert(port != null);
        Debug.Assert(user != null);
        GameManagerH.Instance.OnConnectedToServer += ConnectedToServer;
        // GameManagerH.Instance.OnDisconnectedFromServer += DisconnectedFromServer;
        // domain.text = PlayerPrefs.GetString("arserver_domain", "");
        port.text = PlayerPrefs.GetInt("arserver_port", 6789).ToString();
        user.text = PlayerPrefs.GetString("arserver_username", "user1").ToString();
        connectButton.OnClicked.AddListener(() => ConnectToServer(true));
    }

    public void ConnectToServer(bool force = true)
    {
        if (!force)
        {
            if (PlayerPrefs.GetInt("arserver_keep_connected", 0) == 0)
            {
                return;
            }
        }

        var trimedDomain = domain.text.Trim();

        trimedDomain = "192.168.104.100";

#if UNITY_EDITOR
        trimedDomain = "127.0.0.1";
#endif


        int portInt = int.Parse(port.text);
        PlayerPrefs.SetString("arserver_domain", trimedDomain);
        PlayerPrefs.SetInt("arserver_port", portInt);
        PlayerPrefs.SetString("arserver_username", user.text);
        // PlayerPrefs.SetInt("arserver_keep_connected", KeepConnected.isOn ? 1 : 0);
        PlayerPrefs.Save();
        GameManagerH.Instance.ConnectToSever(trimedDomain, portInt);
    }

    internal string GetUsername()
    {
        return user.text;
    }

    private void ConnectedToServer(object sender, EventArgs args)
    {
        landingScreen.SetActive(false);
    }

    private void DisconnectedFromServer(object sender, EventArgs args)
    {
        landingScreen.SetActive(true);
    }
}
