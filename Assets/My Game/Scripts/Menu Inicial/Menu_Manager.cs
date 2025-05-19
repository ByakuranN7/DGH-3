using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;

using UnityEngine.UI; //para ter acesso aos elementos de interface

public class Menu_Manager : MonoBehaviourPunCallbacks
{

[Space]
[Header("Login")]
public GameObject login_canvas;
public InputField login_email;
public InputField login_password;
public Button login_button;
public Button login_newreg_button;

[Space]
[Header("Register")]
public GameObject register_canvas;
public InputField register_email;
public InputField register_password;
public InputField register_repassword;
public Button register_button;
public Button register_back_button;

[Space]
[Header("Message")]
public GameObject message_canvas;
public Text message_text;
public Button message_button;

[Space]
[Header("MenuGame")]
public GameObject menugame_canvas;
public Button Button_Iniciar;
public Button Button_Baralho;
public Button Button_Sugestões;
public Button Button_Sair;
public Button Button_Regras;

[Space]
[Header("Cartas (Baralho)")]
public GameObject Baralho_invasao_inicial_canvas;
public Button Button_proximo_obt_priv;
public Button Button_menu_principal_1;


public GameObject Baralho_obtencao_privilegios_canvas;
public Button Button_proximo_persistencia;
public Button Button_anterior_invas_ini;
public Button Button_menu_principal_2;

  
public GameObject Baralho_persistencia_canvas;
public Button Button_proximo_c2_exfill;
public Button Button_anterior_obt_priv;
public Button Button_menu_principal_3;


public GameObject Baralho_c2_exfill_canvas;
public Button Button_proximo_procedimento;
public Button Button_anterior_persistencia;
public Button Button_menu_principal_4;

public GameObject Baralho_procedimento_canvas;
public Button Button_anterior_c2_exfill;
public Button Button_menu_principal_5;


[Space]
[Header("Manual")]
public GameObject manual_abrir_canvas;
public Button Button_1_prox;
public Button Button_1_anterior; 
public Button Button_1_menu;
public RawImage manual_imagens;
public Texture[] myTextures = new Texture[25];
public int currentPage = 0;


[Space]
[Header("Nickname-Team")]
//public GameObject abrir_cena_lobby;
//public InputField Nickname_team;
//public Button Conectar;


[Space]
[Header("Lobby")]
public GameObject lobby_abrir_lobby;
//public InputField InputField_roomname;
//public Button Button_createRoom;


[Space]
[Header("Room")]
//public GameObject lobby_abrir_room;
//public Text Text_sala;


[Space]
[Header("REST")]
public Rest_Controller rest_script;




string menu_email; //apenas para testes, deletar depois
string menu_password; //apenas para testes, deletar depois


private void Start()
{

     menu_email = ""; //apenas para testes, deletar depois
     menu_password = ""; //apenas para testes, deletar depois

    //Login
    login_button.onClick.AddListener(Button_Login); //quando tenta logar (clica no botão de login), aciona a função Button.Login
    login_newreg_button.onClick.AddListener(Button_Login_NewReg);
    
    login_email.text = PlayerPrefs.GetString("PLAYER_EMAIL", ""); //apenas para testes, deletar depois
    login_password.text = PlayerPrefs.GetString("PLAYER_PASSWORD", ""); //apenas para testes, deletar depois


    //Message
    message_button.onClick.AddListener(Button_MessageClose); //fecha a mensagem clicando no X

    //Register
    register_button.onClick.AddListener(Button_Register); //Botão de criar conta
    register_back_button.onClick.AddListener(Button_RegisterBack); //voltar para a tela de login

    //Menu
    Button_Sair.onClick.AddListener(Exit_game); //sai do jogo
    Button_Regras.onClick.AddListener(delegate{MenuActive(manual_abrir_canvas);}); //abre o manual
    Button_Iniciar.onClick.AddListener(delegate{MenuActive(lobby_abrir_lobby);}); //abre o lobby


    //Baralho
    Button_Baralho.onClick.AddListener(delegate{MenuActive(Baralho_invasao_inicial_canvas);});//mostra todas as cartas
    Button_proximo_obt_priv.onClick.AddListener(delegate{MenuActive(Baralho_obtencao_privilegios_canvas);});
    Button_menu_principal_1.onClick.AddListener(delegate{MenuActive(menugame_canvas);});

    Button_proximo_persistencia.onClick.AddListener(delegate{MenuActive(Baralho_persistencia_canvas);});
    Button_anterior_invas_ini.onClick.AddListener(delegate{MenuActive(Baralho_invasao_inicial_canvas);});
    Button_menu_principal_2.onClick.AddListener(delegate{MenuActive(menugame_canvas);});

    Button_proximo_c2_exfill.onClick.AddListener(delegate{MenuActive(Baralho_c2_exfill_canvas);});
    Button_anterior_obt_priv.onClick.AddListener(delegate{MenuActive(Baralho_obtencao_privilegios_canvas);});
    Button_menu_principal_3.onClick.AddListener(delegate{MenuActive(menugame_canvas);});

    Button_proximo_procedimento.onClick.AddListener(delegate{MenuActive(Baralho_procedimento_canvas);});
    Button_anterior_persistencia.onClick.AddListener(delegate{MenuActive(Baralho_persistencia_canvas);});
    Button_menu_principal_4.onClick.AddListener(delegate{MenuActive(menugame_canvas);});

    Button_anterior_c2_exfill.onClick.AddListener(delegate{MenuActive(Baralho_c2_exfill_canvas);});
    Button_menu_principal_5.onClick.AddListener(delegate{MenuActive(menugame_canvas);});


    //Manual
    manual_imagens.texture = myTextures[currentPage]; //vai mudando a pagina do manual

    Button_1_prox.onClick.AddListener(next_button);
    Button_1_anterior.onClick.AddListener(back_button);
    Button_1_menu.onClick.AddListener(manual_close);


    //Nickname-team
    //Conectar.onClick.AddListener(dele)

    //Lobby
    
    //Button_createRoom.onClick.AddListener(delegate{MenuActive(lobby_abrir_lobby);}); //vem da tela de nickname para a tela de lobby (criação de sala)
    //Button_createRoom.onClick.AddListener(delegate{MenuActive(lobby_abrir_room);}); //cria uma sala e vai pra sala







    //MenuActive(login_canvas); //começa o jogo pela tela de login
    MenuActive(menugame_canvas); //começa o jogo direto pela tela principal, sem necessidade de logar

}




#region ##################### FUNCTIONS #####################
//Função que Gerencia o canvas que será ativado (se o nome for igual, true; se o nome for diferente, false)
//isso tudo é definido dentro da unity, sendo assim n há referencia ao nome real dos gameobject nessa parte

void MenuActive(GameObject canvas)
{
    login_canvas.gameObject.SetActive(login_canvas.name.Equals(canvas.name)); //tela de login
    register_canvas.gameObject.SetActive(register_canvas.name.Equals(canvas.name));
    message_canvas.gameObject.SetActive(message_canvas.name.Equals(canvas.name));
    menugame_canvas.gameObject.SetActive(menugame_canvas.name.Equals(canvas.name)); //menu inicial

    Baralho_invasao_inicial_canvas.gameObject.SetActive(Baralho_invasao_inicial_canvas.name.Equals(canvas.name)); //lista de cartas no menu inicial
    Baralho_obtencao_privilegios_canvas.gameObject.SetActive(Baralho_obtencao_privilegios_canvas.name.Equals(canvas.name));
    Baralho_persistencia_canvas.gameObject.SetActive(Baralho_persistencia_canvas.name.Equals(canvas.name));
    Baralho_c2_exfill_canvas.gameObject.SetActive(Baralho_c2_exfill_canvas.name.Equals(canvas.name));
    Baralho_procedimento_canvas.gameObject.SetActive(Baralho_procedimento_canvas.name.Equals(canvas.name));
    
    manual_abrir_canvas.gameObject.SetActive(manual_abrir_canvas.name.Equals(canvas.name)); //abre o manual

    //abrir_cena_lobby.gameObject.SetActive(abrir_cena_lobby.name.Equals(canvas.name));
    lobby_abrir_lobby.gameObject.SetActive(lobby_abrir_lobby.name.Equals(canvas.name));
    //lobby_abrir_room.gameObject.SetActive(lobby_abrir_room.name.Equals(canvas.name));
}

void GetMessage(Message p_msg){
    //função para receber os dados no formato Message, essa em especifico é utilizada na tela de login para informar os erros
    if (p_msg.GetMessage() != "")
    {
        message_text.text = p_msg.GetMessage();
        message_canvas.gameObject.SetActive(true);
        return;
    }

    StartGame();
}


void StartGame(){

//apenas para testes, deletar depois esse ifelse inteiro
    if(menu_email != "" && menu_password != "")
    {
        //Gravar os dados no PlayerPrefs para preenchimento automatico
        PlayerPrefs.SetString("PLAYER_EMAIL", menu_email);
        PlayerPrefs.SetString("PLAYER_PASSWORD", menu_password);
    }else{
        Debug.Log("Erro ao tentar salvar E-mail e Senha no PlayerPrefs");
    }
//apenas para testes, deletar depois
 



    //Se estiver tudo certo, iniciar jogo indo para o menu principal
    MenuActive(menugame_canvas);
}

#endregion


#region ##################### LOGIN #####################

void Button_Login()
{

    bool err = false;


    string email_temp = login_email.text;
    string pw_temp = login_password.text;

    if (email_temp == "")
    {
        message_text.text = "Digite um e-mail";
        err = true;
    }

        if (pw_temp == "" && err == false)
    {
        message_text.text = "Digite uma senha";
        err = true;
    }



    if (err == true)
    {
        message_canvas.gameObject.SetActive(true);
        //MenuActive(message_canvas); dá pra fazer desse jeito também, mas tem q ajeitar umas coisas no codigo
        return;
    }

     menu_email = email_temp; //apenas para testes, deletar depois
     menu_password = pw_temp; //apenas para testes, deletar depois


        // Envio Rest
        Login_send(email_temp, pw_temp);

}



//Abre a janela de cadastro
void Button_Login_NewReg()
{
    MenuActive(register_canvas);
}
#endregion


#region ##################### MENSAGEM #####################

void Button_MessageClose()
{

    message_canvas.gameObject.SetActive(false);
    //login_canvas.gameObject.SetActive(true);

}
#endregion


#region ##################### REGISTER #####################

void Button_Register()
{

    bool err = false;

    string email_temp = register_email.text;
    string pw_temp = register_password.text;
    string rpw_temp = register_repassword.text;


    if (email_temp == "")
    {
        message_text.text = "Digite um e-mail";
        err = true;
    }

    if (pw_temp == "" && err == false)
    {
        message_text.text = "Digite uma senha";
        err = true;
    }

    if (rpw_temp == "" && err == false)
    {
        message_text.text = "Digite a senha novamente";
        err = true;
    }



        if (pw_temp != rpw_temp && err == false)
    {
        message_text.text = "Senhas diferentes digitadas";
        err = true;
    }

    if (err == true)
    {
        message_canvas.gameObject.SetActive(true);
        return;
    }


     menu_email = email_temp; //apenas para testes, deletar depois
     menu_password = pw_temp; //apenas para testes, deletar depois



    //Aqui vai chamar a função de envio dos dados para o Servidor que vai ativar ou não a proxima janela
    Register_send(email_temp, pw_temp);
}



//Função de retornar a janela de Login
void Button_RegisterBack()
{
    MenuActive(login_canvas);
}
#endregion


#region ##################### REST FUNCTIONS #####################

void Login_send(string p_email, string p_password){
    rest_script.SendRestGetLogin(p_email, p_password, GetMessage);
}

void Register_send(string p_email, string p_password){
    rest_script.SendRestPostRegister(p_email, p_password, GetMessage);
}

#endregion


public void Exit_game(){
    Application.Quit();
}

#region ##################### MANUAL #####################
//utilizado no manual
public void next_button(){
    
    if (currentPage < 24){
        currentPage++;
    }
    
    manual_imagens.texture = myTextures[currentPage];
}
//utilizado no manual
public void back_button(){

    if(currentPage > 0){
        currentPage--;
    }

   manual_imagens.texture = myTextures[currentPage];
}

public void manual_close(){
currentPage = 0;
MenuActive(menugame_canvas);
}
#endregion

















////////tentando adaptar lobby para uma unica cena
//ConnectToServer



 /*   public InputField usernameInput;
    public Text buttonText;

    public void OnClickConnect(){

        if(usernameInput.text.Length >= 1){
            PhotonNetwork.NickName = usernameInput.text;
            buttonText.text = "Conectando...";
            PhotonNetwork.ConnectUsingSettings();
        }
    }

public override void OnConnectedToMaster(){
    SceneManager.LoadScene("Lobby"); 
}
*/



}
