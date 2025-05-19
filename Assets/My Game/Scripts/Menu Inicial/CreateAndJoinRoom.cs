using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class CreateAndJoinRoom : MonoBehaviourPunCallbacks
{
    public TMP_InputField input_Create;
    public TMP_InputField input_Join;

    public void CreateRoom()
    {
        PhotonNetwork.CreateRoom(input_Create.text , new RoomOptions() {MaxPlayers = 4 , IsVisible = true , IsOpen=true} , TypedLobby.Default , null);
    }

    public void JoinRoom()
    {
        PhotonNetwork.JoinRoom(input_Join.text);
    }


    public override void OnJoinedRoom()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            // Vai direto para o lobby se for o professor
            SceneManager.LoadScene("Lobby");
        }
        else
        {
            // Vai para tela de nome da equipe se for jogador comum
            SceneManager.LoadScene("TeamNameEntry");
        }

        //PhotonNetwork.LoadLevel("Gameplay");
        //SceneManager.LoadScene("Gameplay");
    }
}
