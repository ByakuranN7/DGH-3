using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class LobbyDisplay : MonoBehaviourPunCallbacks
{
    public TextMeshProUGUI[] equipeTexts; // Textos para nomes das equipes
    public TextMeshProUGUI professorStatusText; // Texto para status do professor
    public TextMeshProUGUI roomNameText; // Texto para o nome da sala


    void Start()
    {
        AtualizarLobby(); // Atualiza lobby ao iniciar

        // Define o nome da sala no topo
        if (PhotonNetwork.InRoom)
        roomNameText.text = "Sala: " + PhotonNetwork.CurrentRoom.Name;
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








//adicionando identificador para cada jogador (com exceção do professor que é facilmente identificado por ser o masterclient)
public void OnClickFixarIdentificadoresDeEquipe()
{
    int equipeIndex = 1; // Começa em 1 porque no lobby é mostrado "Equipe 1", "Equipe 2", etc.

    foreach (Player player in PhotonNetwork.PlayerList)
    {
        if (player.IsMasterClient)
            continue; // Pula o professor

        // Verifica se o jogador tem nomes definidos na propriedade "teamNames"
        if (player.CustomProperties.TryGetValue("teamNames", out object nomesObj))
        {
            // Cria um novo conjunto de propriedades personalizadas
            ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable
            {
                { "equipeId", equipeIndex } // Define o identificador da equipe com base na ordem visual
            };

            player.SetCustomProperties(props); // Envia essa propriedade para o Photon
            //Debug.Log($"Equipe com teamNames '{(string)nomesObj}' recebeu equipeId: {equipeIndex}");
            equipeIndex++; // Avança para o próximo número de equipe
        }
    }
}
    
}