[System.Serializable]
public class Message 
{
    public int status;
    public string message;

    string message_default = "Ocorreu um problema, tente novamente em alguns minutos";

    public Message(){
    }

     public Message(int p_status, string p_msg){
        this.status = p_status;
        this.message = p_msg;

    }


    public string GetMessage() {
        if(this.message == "" && this.status != 200)
        {
            return message_default;
        }

        return this.message;
    }

}
