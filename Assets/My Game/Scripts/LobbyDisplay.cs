using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class LobbyDisplay : MonoBehaviourPunCallbacks
{
    public TextMeshProUGUI[] equipeTexts; // Textos para nomes das equipes
    public TextMeshProUGUI professorStatusText; // Texto para status do professor

    void Start()
    {
        AtualizarLobby(); // Atualiza lobby ao iniciar
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        AtualizarLobby(); // Atualiza quando alguém entra
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        AtualizarLobby(); // Atualiza quando alguém sai
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        AtualizarLobby(); // Atualiza quando propriedades mudam
    }

    void AtualizarLobby()
    {
        foreach (var txt in equipeTexts)
            txt.text = "Aguardando jogador..."; // Limpa textos

        bool professorConectado = false;
        int equipeIndex = 0;

        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (player.IsMasterClient)
            {
                professorConectado = true; // Marca professor conectado
                continue;
            }

            if (player.CustomProperties.TryGetValue("teamNames", out object nomesObj))
            {
                string nomesStrRaw = nomesObj as string;

                if (!string.IsNullOrEmpty(nomesStrRaw) && equipeIndex < equipeTexts.Length)
                {
                    string[] nomes = nomesStrRaw.Split('|');
                    string nomesStr = string.Join(", ", nomes);
                    equipeTexts[equipeIndex].text = $"Equipe {equipeIndex + 1}:\n{nomesStr}"; // Mostra nomes
                    equipeIndex++;
                }
            }
        }

        // Mostra se o professor está conectado
        professorStatusText.text = professorConectado ? "Professor conectado" : "Aguardando professor...";
    }
}