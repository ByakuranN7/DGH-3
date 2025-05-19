using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class TeamNameSender : MonoBehaviourPunCallbacks
{
    public TMP_InputField[] nameInputs; // Arraste os InputFields aqui no Inspector

    public void OnConfirmClicked()
    {
        List<string> validNames = new List<string>();

        foreach (TMP_InputField input in nameInputs)
        {
            if (!string.IsNullOrWhiteSpace(input.text))
                validNames.Add(input.text);
        }

        // Junta todos os nomes com separador, exemplo: "João|Maria|Lucas"
        string teamNames = string.Join("|", validNames);

        // Salva isso como uma propriedade customizada do jogador local
        ExitGames.Client.Photon.Hashtable customProps = new ExitGames.Client.Photon.Hashtable();
        customProps["teamNames"] = teamNames;
        PhotonNetwork.LocalPlayer.SetCustomProperties(customProps);

        // Vai pro Lobby
        SceneManager.LoadScene("Lobby");
    }
}
