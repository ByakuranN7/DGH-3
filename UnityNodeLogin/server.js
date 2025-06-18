const express = require('express');
const bodyParser = require('body-parser');
const mysql = require('mysql');
const validator = require('validator');


//#region  ################## CONFIG ##################
var port = process.env.PORT || 3000;

const app = express();

app.use(bodyParser.urlencoded({extended: false}));

app.use(bodyParser.json());

//censurado obviamente pq n sou doido \o/
var connection = mysql.createConnection({
    host: '*******',
    user: '*****',
    password: '****',
    database: '*',
    port: 3306
    //ssl: true (só tem em outras hospedagens)
});

connection.connect(function(err){
    if(err){
        console.log('error connecting ' + err.stack);
        return;
    }

        console.log('connected as id ' + connection.threadId);

});

//#endregion

//#region  ################## GET ##################
//Raiz
app.get('/', function (req, result) {
    console.log('Passando no: Entrando no GET/ ');
    result.send('Welcome!');    
});

//Login
app.get('/login/:email/:password', function (req, result) {

    console.log('Passando no: Entrando no GET/LOGIN ');
    
    var erro = false;


    var msg_result = {};
    msg_result.status = 200;
    msg_result.message = "";


    var login_temp = {};
    login_temp.email = req.params.email;
    login_temp.password = req.params.password;

    var status_code = 200;
    var msg_text = '';

    console.log(login_temp);


    if(!validator.isEmail(login_temp.email)){
        console.log('Passando no: Login > Validação de Formato de E-mail ');
        status_code = 400; //checar o código q n é o correto
        msg_text = 'E-mail em formato inválido';
        erro = true;
    }

    if(erro == false){
        //Consulta no banco de dados
        //SELECT
        login_select(login_temp).then((resultado) => {


            console.log('Passando no: Login > login_select.Then() ');

            //Caso não retorne dados compatíveis com Email e Senha
            if(parseInt(resultado.length) == 0){
                console.log('Passando no: Login > login_select.Then() > Verifica resultado = 0');
                status_code = 400; //código errado, checar
                msg_text = 'Login ou Senha incorreta, verifique os dados';
            }

            //Caso ocorra de conseguir fazer 2 registros com os mesmos dados
            if(parseInt(resultado.length) > 1){
                console.log('Passando no: Login > login_select.Then() > Verifica resultado = 0');
                status_code = 400; //código errado, checar
                msg_text = 'Existe um problema com seus dados, entre em contato';
            }

            //Carregando o Objeto de resposta
            msg_result.status = status_code;
            msg_result.message = msg_text;
            
                        
            //Retorno da mensagem com o status e mensagem
            result.status(msg_result.status).json(msg_result);


        }).catch((err) => {

            console.log('Passando no: Login > login_select.catch() ');

            if(err){
                msg_result.status = err.status_code;
                msg_result.message = err.msg.text;
            }else{
                //se n tiver um erro q venha do banco, mas disparou o catch por algum motivo:
                msg_result.status = 500; //codigo errado, checa depois
                msg_result.message = 'Não é possível executar a ação, tente novamente em breve ';
            }

            console.log('-->>> Login - catch - Erro: ' + msg_result.message );
            
            //Retorno da mensagem com o status e mensagem
            result.status(msg_result.status).json(msg_result);
        });
    }else{
    //Vou ajustar ainda------------------***************
    msg_result.status = status_code;
    msg_result.message = msg_text;

    result.status(msg_result.status).json(msg_result);
    }


    /*
    status_code:200,
    msg_text: ""
    */
    
});



//#endregion

//#region  ################## POST ##################
app.post('/register', function (req, result) {

    console.log('Passando no: Entrando no POST/REGISTER ');
    
    var erro = false;

    var msg_result = {};
    msg_result.status = 200;
    msg_result.message = "";

    var register_temp = {};
    register_temp = req.body;

    //login_temp.email = req.params.email;
    //login_temp.password = req.params.password;

    var status_code = 200;
    var msg_text = '';

    console.log(register_temp);

   //tem algum erro aki ou no Rest_Controller função RegisterPost!!!!!!!!!!!!!!!!
    if(!validator.isEmail(register_temp.email)){
        console.log('Passando no: Login > Validação de Formato de E-mail ');
        status_code = 400; //checar o código pq n é o correto
        msg_text = 'E-mail em formato inválido';
        erro = true;
    }
    
    if(erro == false){
        //Consulta no banco de dados
        register_select(register_temp).then((resultado) => {
            //Verifica se existe email cadastrado
            if(resultado.length > 0){
                console.log('Passando no: Register > register_select.Then() > Verifica resultado > 0');
                status_code = 400; //checar codigo pq acho q ta errado
                msg_text = 'Já existe um cadastro para esse E-mail';

                msg_result.status = status_code;
                msg_result.message = msg_text;

                //Retorno da mensagem
                result.status(msg_result.status).json(msg_result);


            }else{
                //Se não existir, faz a inclusão
                //promise
                register_insert(register_temp).then((resultado2) => {

                    console.log('Passando no: Register > register_insert.Then() ');

                    msg_result.status = status_code;
                    msg_result.message = msg_text;
    
                    //Retorno da mensagem
                    result.status(msg_result.status).json(msg_result);

                }).catch((err2) => {
                    console.log('Passando no: Register > register_insert.Catch() ');

                    msg_result.status = err2.status_code;
                    msg_result.message = err2.msg_text;
    
                    console.log('Register INSERT - catch - Erro: ' + msg_result.message);
                    
                    //Retorno da mensagem
                    result.status(msg_result.status).json(msg_result);

                }); 
            }

        }).catch((err) => {

            console.log('Passando no: Register > register_select.Catch() ');
            console.log(err.status_code);
            //verificando se o status de erro existe
            if (err.status_code){
                msg_result.status = err.status_code;
                msg_result.message = err.msg_text;
            }else{
                msg_result.status = 500; //código errado, checar depois
                msg_result.message = '--->>> Register - register_select - Catch = Erro no Then disparou a Catch...';
            }

                console.log('Register Select - catch - Erro: ' + msg_result.message);

                //Retorno da mensagem com o status e mensagem
                result.status(msg_result.status).json(msg_result);
        });



    }else{
        msg_result.status = status_code;
        msg_result.message = msg_text;
    
        result.status(msg_result.status).json(msg_result);
    }

  

    
});
//#endregion



//#region  ################## FUNCTIONS ##################

//#### LOGIN
function login_select(login_temp) {
    return new Promise((resolve, reject) => {

        connection.query(`SELECT * FROM login WHERE email = '${login_temp.email}' AND password = '${login_temp.password}' `, function (err, results, field){

            var obj_err = {};
            obj_err.msg_text = '--->>> login_select - Não entrou no erro ainda...';

            if (err){
                console.log('Erro: login_select dentro da PROMISE: ' + err);
                obj_err.status_code = 400; //checar o codigo pq ta errado
                obj_err.msg_text = err
                reject(obj_err);
            }else{
                console.log('Dentro da PROMISE login -> Selecionado: ' + results.length);
                resolve(results);
            }
        });


    });
    
}

//#### REGISTER (inserir)
function register_select(register_temp){
    return new Promise((resolve, reject) => {

        connection.query(`SELECT * FROM login WHERE email = '${register_temp.email}' `, function (err, results, field){

            var obj_err = {};
            obj_err.msg_text = '--->>> register_select - Não entrou no erro ainda...';

            if (err){
                console.log('Erro: register_select dentro da PROMISE: ' + err);
                obj_err.status_code = 400; //checar o codigo pq ta errado
                obj_err.msg_text = err;
                reject(obj_err);
            }else{
                console.log('Dentro da PROMISE select -> Selecionado: ' + results.length);
                resolve(results);
            }
        });


    });
    
}


function register_insert(register_temp){
    return new Promise((resolve, reject) => {

        connection.query(`INSERT INTO login (email, password) VALUES ('${register_temp.email}', '${register_temp.password}') `, function (err, results, field){

            var obj_err = {};
            obj_err.msg_text = '--->>> register_insert - Não entrou no erro ainda...';

            if (err){
                console.log('Erro: register_insert dentro da PROMISE: ' + err);
                obj_err.status_code = 400; //checar o codigo pq ta errado
                obj_err.msg_text = err;
                reject(obj_err);
            }else{
                console.log('Dentro da PROMISE -> Linhas afetadas: ' + results.length + ' | ID:' + results.insertId);
                resolve(results);
            }
        });


    });
}

//#endregion


















//Parte relacionada a criação/visualização de sugestões.
// GET - Listar sugestões
app.get('/sugestoes', function (req, result) {
    connection.query('SELECT * FROM sugestoes', function (err, results) {
        if (err) {
            console.log(err);
            result.status(500).json({ status: 500, message: 'Erro no banco de dados.' });
        } else {
            result.status(200).json(results);
        }
    });
});


app.post('/sugestoes', function (req, result) {
    const { titulo, cartaInvasaoInicial, cartaObtencaoPrivilegios, cartaPersistencia, cartaC2Exfiltracao, descricao } = req.body;

    const sql = `INSERT INTO sugestoes (titulo, cartaInvasaoInicial, cartaObtencaoPrivilegios, cartaPersistencia, cartaC2Exfiltracao, descricao) VALUES (?, ?, ?, ?, ?, ?)`;
    const values = [titulo, cartaInvasaoInicial, cartaObtencaoPrivilegios, cartaPersistencia, cartaC2Exfiltracao, descricao];

    connection.query(sql, values, function (err, results) {
        if (err) {
            console.log(err);
            result.status(500).json({ status: 500, message: 'Erro no banco de dados.' });
        } else {
            result.status(200).json({ status: 200, message: 'Sugestão cadastrada com sucesso.' });
        }
    });
});






app.listen(port, () => {
    console.log(`Listening port ${port}`);
});