require('dotenv').config();

const express = require('express');
const bodyParser = require('body-parser');
const mysql = require('mysql2');
const validator = require('validator');
const bcrypt = require('bcrypt');

//#region  ################## CONFIG ##################
var port = process.env.PORT || 3000;
const SALT_ROUNDS = 10; // custo do hash bcrypt

const app = express();

app.use(bodyParser.urlencoded({ extended: false }));
app.use(bodyParser.json());

// Credenciais do banco NÃO ficam mais no código-fonte.
// Elas vêm de um arquivo .env.
var connection = mysql.createConnection({
    host: process.env.DB_HOST,
    user: process.env.DB_USER,
    password: process.env.DB_PASSWORD,
    database: process.env.DB_NAME,
    port: process.env.DB_PORT || 3306
    // ssl: true (dependendo da hospedagem)
});

connection.connect(function (err) {
    if (err) {
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

//#endregion

//#region  ################## LOGIN (agora via POST) ##################
app.post('/login', function (req, result) {

    console.log('Passando no: Entrando no POST/LOGIN ');

    var msg_result = { status: 200, message: '' };

    var email_temp = req.body.email;
    var password_temp = req.body.password;

    if (!email_temp || !password_temp) {
        msg_result.status = 400;
        msg_result.message = 'E-mail e senha são obrigatórios';
        return result.status(msg_result.status).json(msg_result);
    }

    if (!validator.isEmail(email_temp)) {
        console.log('Passando no: Login > Validação de Formato de E-mail ');
        msg_result.status = 400;
        msg_result.message = 'E-mail em formato inválido';
        return result.status(msg_result.status).json(msg_result);
    }

    login_select(email_temp).then((resultado) => {

        console.log('Passando no: Login > login_select.Then() ');

        if (resultado.length === 0) {
            msg_result.status = 400;
            msg_result.message = 'Login ou Senha incorreta, verifique os dados';
            return result.status(msg_result.status).json(msg_result);
        }

        if (resultado.length > 1) {
            // Isso só pode acontecer se o e-mail não tiver constraint UNIQUE no banco.
            msg_result.status = 400;
            msg_result.message = 'Existe um problema com seus dados, entre em contato';
            return result.status(msg_result.status).json(msg_result);
        }

        var usuario = resultado[0];

        // Compara a senha enviada com o HASH salvo no banco (nunca com texto puro)
        bcrypt.compare(password_temp, usuario.Senha, function (err, senhaCorreta) {
            if (err) {
                console.log('Erro ao comparar senha: ' + err);
                msg_result.status = 500;
                msg_result.message = 'Não é possível executar a ação, tente novamente em breve';
                return result.status(msg_result.status).json(msg_result);
            }

            if (!senhaCorreta) {
                msg_result.status = 400;
                msg_result.message = 'Login ou Senha incorreta, verifique os dados';
                return result.status(msg_result.status).json(msg_result);
            }

            msg_result.status = 200;
            msg_result.message = '';
            return result.status(msg_result.status).json(msg_result);
        });

    }).catch((err) => {

        console.log('Passando no: Login > login_select.catch() ');

        if (err && err.status_code) {
            msg_result.status = err.status_code;
            msg_result.message = err.msg_text;
        } else {
            msg_result.status = 500;
            msg_result.message = 'Não é possível executar a ação, tente novamente em breve';
        }

        console.log('-->>> Login - catch - Erro: ' + msg_result.message);
        result.status(msg_result.status).json(msg_result);
    });
});

//#endregion

//#region  ################## POST (REGISTER) ##################
app.post('/register', function (req, result) {

    console.log('Passando no: Entrando no POST/REGISTER ');

    var erro = false;

    var msg_result = {};
    msg_result.status = 200;
    msg_result.message = "";

    var register_temp = {};
    register_temp = req.body;

    var status_code = 200;
    var msg_text = '';

    console.log({ email: register_temp.email }); // não loga a senha, nem hash, no console

    if (!register_temp.email || !register_temp.password) {
        status_code = 400;
        msg_text = 'E-mail e senha são obrigatórios';
        erro = true;
    }

    if (!erro && !validator.isEmail(register_temp.email)) {
        console.log('Passando no: Register > Validação de Formato de E-mail ');
        status_code = 400;
        msg_text = 'E-mail em formato inválido';
        erro = true;
    }

    // Regra mínima de senha
    if (!erro && register_temp.password.length < 6) {
        status_code = 400;
        msg_text = 'A senha deve ter ao menos 6 caracteres';
        erro = true;
    }

    if (erro == false) {
        register_select(register_temp).then((resultado) => {

            if (resultado.length > 0) {
                console.log('Passando no: Register > register_select.Then() > Verifica resultado > 0');
                status_code = 400;
                msg_text = 'Já existe um cadastro para esse E-mail';

                msg_result.status = status_code;
                msg_result.message = msg_text;

                result.status(msg_result.status).json(msg_result);

            } else {

                // Gera o hash da senha ANTES de salvar no banco.
                // A senha em texto puro nunca é gravada.
                bcrypt.hash(register_temp.password, SALT_ROUNDS, function (errHash, hash) {
                    if (errHash) {
                        console.log('Erro ao gerar hash da senha: ' + errHash);
                        msg_result.status = 500;
                        msg_result.message = 'Não é possível executar a ação, tente novamente em breve';
                        return result.status(msg_result.status).json(msg_result);
                    }

                    var registerComHash = {
                        email: register_temp.email,
                        password: hash
                    };

                    register_insert(registerComHash).then((resultado2) => {

                        console.log('Passando no: Register > register_insert.Then() ');

                        msg_result.status = status_code;
                        msg_result.message = msg_text;

                        result.status(msg_result.status).json(msg_result);

                    }).catch((err2) => {
                        console.log('Passando no: Register > register_insert.Catch() ');

                        msg_result.status = err2.status_code || 500;
                        msg_result.message = err2.msg_text || 'Não é possível executar a ação, tente novamente em breve';

                        console.log('Register INSERT - catch - Erro: ' + msg_result.message);

                        result.status(msg_result.status).json(msg_result);
                    });
                });
            }

        }).catch((err) => {

            console.log('Passando no: Register > register_select.Catch() ');

            if (err && err.status_code) {
                msg_result.status = err.status_code;
                msg_result.message = err.msg_text;
            } else {
                msg_result.status = 500;
                msg_result.message = '--->>> Register - register_select - Catch = Erro no Then disparou a Catch...';
            }

            console.log('Register Select - catch - Erro: ' + msg_result.message);

            result.status(msg_result.status).json(msg_result);
        });

    } else {
        msg_result.status = status_code;
        msg_result.message = msg_text;

        result.status(msg_result.status).json(msg_result);
    }

});
//#endregion


//#region  ################## FUNCTIONS ##################

//#### LOGIN
// Query PARAMETRIZADA (?) em vez de concatenar string, evitando SQL Injection.
function login_select(email) {
    return new Promise((resolve, reject) => {

        connection.query('SELECT * FROM Usuario WHERE Email = ?', [email], function (err, results) {

            var obj_err = {};
            obj_err.msg_text = '--->>> login_select - Não entrou no erro ainda...';

            if (err) {
                console.log('Erro: login_select dentro da PROMISE: ' + err);
                obj_err.status_code = 500;
                obj_err.msg_text = 'Erro ao consultar login';
                reject(obj_err);
            } else {
                console.log('Dentro da PROMISE login -> Selecionado: ' + results.length);
                resolve(results);
            }
        });
    });
}

//#### REGISTER
function register_select(register_temp) {
    return new Promise((resolve, reject) => {

        connection.query('SELECT * FROM Usuario WHERE Email = ?', [register_temp.email], function (err, results) {

            var obj_err = {};
            obj_err.msg_text = '--->>> register_select - Não entrou no erro ainda...';

            if (err) {
                console.log('Erro: register_select dentro da PROMISE: ' + err);
                obj_err.status_code = 500;
                obj_err.msg_text = 'Erro ao consultar cadastro';
                reject(obj_err);
            } else {
                console.log('Dentro da PROMISE select -> Selecionado: ' + results.length);
                resolve(results);
            }
        });
    });
}

function register_insert(register_temp) {
    return new Promise((resolve, reject) => {

        connection.query(
            'INSERT INTO Usuario (Email, Senha) VALUES (?, ?)',
            [register_temp.email, register_temp.password], // aqui "password" já é o HASH, gerado antes de chamar essa função
            function (err, results) {

                var obj_err = {};
                obj_err.msg_text = '--->>> register_insert - Não entrou no erro ainda...';

                if (err) {
                    console.log('Erro: register_insert dentro da PROMISE: ' + err);
                    obj_err.status_code = 500;
                    obj_err.msg_text = 'Erro ao cadastrar usuário';
                    reject(obj_err);
                } else {
                    console.log('Dentro da PROMISE -> Linhas afetadas: ' + results.affectedRows + ' | ID:' + results.insertId);
                    resolve(results);
                }
            }
        );
    });
}

//#endregion


//#region  ################## SUGESTÕES ##################

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
    
    if (!titulo || !cartaInvasaoInicial || !cartaObtencaoPrivilegios || !cartaPersistencia || !cartaC2Exfiltracao || !descricao) {
        return result.status(400).json({ status: 400, message: 'Todos os campos são obrigatórios.' });
    }

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

//#endregion

app.listen(port, () => {
    console.log(`Listening port ${port}`);
});
