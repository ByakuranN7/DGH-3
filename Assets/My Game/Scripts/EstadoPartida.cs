public enum EstadoPartida
{
    NarrativaInicial, //Fala para o professor contar a narrativa inicial e dar um tempo para os alunos se prepararem antes de realmente começar a partida
    TurnoEquipe_EsperaComecar, //Informa a equipe e permite que o professor pressione um botão para começar o turno da equipe em questão
    TurnoEquipe_Pergunta, //Informa que a equipe tem direito a uma pergunta, apresentando um botão para informar quando o professor respondeu a pergunta, para ir para o proximo estado
    TurnoEquipe_SelecionarProcedimento, //Libera a seleção de procedimento para a equipe em questao
    TurnoEquipe_Explicacao, //Mostra na tela do professor o procedimento escolhido, além de pedir para ele escolher o líder da rodada (listando os memebros) e permitindo que ele utilize botoes para aprovar ou n a explicação
    TurnoEquipe_Dado, //Libera a rolagem do dado por parte da equipe em questão, informando o valor para o professor e apresentando um botao para ir para o proximo estado
    TurnoEquipe_Pontuar, // Permite que o professor escolha entre revelar ou não uma carta. Se clicar em revelar, vai para o turno abaixo e contabiliza +1 na pontuação da equipe, se não, vai para o turno "FimTurno"
    TurnoEquipe_RevelarCarta, //Requisita que o professor clique em uma de suas cartas para a revelar, com o click mostrando a carta para todos e indo para "FimTurno"
    FimTurno, //Só avisa que o turno acabou e que o proximo vai começar, indo de volta para "turnoEquipe_EsperaComecar"
    FimPartida
}

