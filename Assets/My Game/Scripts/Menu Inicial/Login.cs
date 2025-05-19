
[System.Serializable]
public class Login
{
    public string email;
    public string password;

    public Login(string p_email, string p_password) {
            this.email = p_email;
            this.password = p_password;
    }

    public string Email {
        get { return email; }
        set { email = value; }
    } 


     public string Password {
     get { return password; }
     set { password = value; }
    }


}
