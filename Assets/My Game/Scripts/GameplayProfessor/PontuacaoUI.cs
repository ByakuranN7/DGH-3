//****Esse script tambem é utilizado na cena das equipes****

using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class PontuacaoUI : MonoBehaviourPunCallbacks
{
    public TextMeshProUGUI[] textosEquipes; // 3 TextMeshProUGUI, um para cada equipe

    void Start()
    {
        AtualizarPontuacao();
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        AtualizarPontuacao(); // Atualiza quando alguém muda de pontuação
    }

    void AtualizarPontuacao()
    {
        bool equipe3Existe = false;

        // Reseta textos com 0 pontos (Equipe 1 e 2 sempre aparecem)
        for (int i = 0; i < textosEquipes.Length; i++)
        {
            textosEquipes[i].gameObject.SetActive(i < 2); // Equipe 1 e 2 sempre ativas, equipe 3 decide depois
            textosEquipes[i].text = $"Equipe {i + 1}: 0";
        }

        foreach (var player in PhotonNetwork.PlayerList)
        {
            if (player.CustomProperties.TryGetValue("equipeId", out object equipeObj) &&
                player.CustomProperties.TryGetValue("pontosEquipe", out object pontosObj))
            {
                int equipeId = (int)equipeObj;
                int pontos = (int)pontosObj;

                if (equipeId >= 1 && equipeId <= textosEquipes.Length)
                {
                    textosEquipes[equipeId - 1].gameObject.SetActive(true);
                    textosEquipes[equipeId - 1].text = $"Equipe {equipeId}: {pontos}";
                    if (equipeId == 3) equipe3Existe = true;
                }
            }
        }

        // Se ninguém tem equipe 3, esconde o texto da equipe 3
        textosEquipes[2].gameObject.SetActive(equipe3Existe);
    }
}

