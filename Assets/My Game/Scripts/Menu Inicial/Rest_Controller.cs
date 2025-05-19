using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Networking;
 
public class Rest_Controller : MonoBehaviour
{
    string WEB_URL = "localhost:3000";
    string routeLogin = "/login";
    string routeRegister = "/register";




#region ######### PUBLIC FUNCTIONS #########
    public void SendRestGetLogin(string p_email, string p_password, System.Action<Message> callBack)
    {

        Login login = new Login(p_email, p_password);

        //Função REST
        StartCoroutine(LoginGet(WEB_URL, routeLogin, login, callBack));
    }

 

        public void SendRestPostRegister(string p_email, string p_password, System.Action<Message> callBack)
        {

        Login login = new Login(p_email, p_password);

        //Função REST
        StartCoroutine(RegisterPost(WEB_URL, routeRegister, login, callBack));
    }
#endregion

 

#region ######### LOGIN GET #########
    public IEnumerator LoginGet(string url, string route, Login loginPlayer, System.Action<Message> callBack){

        string urlNew = string.Format("{0}{1}/{2}/{3}", url, route, loginPlayer.Email, loginPlayer.Password); //"localhost:3000/login/teste@teste.com/123456"

        Debug.Log(urlNew);

        using (UnityWebRequest www = UnityWebRequest.Get(urlNew)){

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError)
            {
                Message msg_err = new Message((int)www.responseCode, www.error);
                Debug.Log(www.error);
                callBack(msg_err);
            }else{

                if (www.isDone)
                {
                    string jsonResult = System.Text.Encoding.UTF8.GetString(www.downloadHandler.data);
                    
                    Debug.Log(jsonResult);
                    
                    Message msg_result = JsonUtility.FromJson<Message>(jsonResult);
                    
                    callBack(msg_result);
                }
            }


        }
    }
#endregion


#region ######### REGISTER POST #########

    public IEnumerator RegisterPost(string url, string route, Login loginPlayer, System.Action<Message> callBack)
    {

        string urlNew = string.Format("{0}{1}", url, route); //"localhost:3000/register"

        Debug.Log(urlNew);

        string jsonData = JsonUtility.ToJson(loginPlayer);

        using (UnityWebRequest www = UnityWebRequest.Post(urlNew, jsonData))
        {

            www.SetRequestHeader("content-type", "application/json");
            www.uploadHandler.contentType = "application/json";
            www.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(jsonData));
            yield return www.SendWebRequest(); //faz o envio

            if (www.result == UnityWebRequest.Result.ConnectionError)
            {
                Message msg_err = new Message((int)www.responseCode, www.error);
                Debug.Log(www.error);
                www.Dispose(); 
                callBack(msg_err);
                 
                
                
            }else{

                if (www.isDone)
                {
                    string jsonResult = System.Text.Encoding.UTF8.GetString(www.downloadHandler.data);
                    
                    Debug.Log(jsonResult);
                    
                    Message msg_result = JsonUtility.FromJson<Message>(jsonResult);
                    Debug.Log(msg_result);
                    www.Dispose(); 
                    callBack(msg_result);
                }//else{
                //isso aqui ta errado, é pra quando botam o email em formato errado ou um email existente, mas quando adiciono ele buga o registro (apesar de ajeitar o resto)
                //Message msg_err = new Message((int)www.responseCode, www.error);
                //Debug.Log(www.error);
                //www.Dispose(); 
              // callBack(msg_err);
            //}
            }


        }
        
    }

#endregion

}
