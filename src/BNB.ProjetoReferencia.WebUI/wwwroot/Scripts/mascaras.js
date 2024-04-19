function mascara(o, f) {
    v_obj = o;
    v_fun = f;

    //Seta o tamanho do campo
    if (f === mcep) {
        $(v_obj).attr('maxlength', '9');
    }
    else if (f === mtelefone) {
        $(v_obj).attr('maxlength', '15');
    }
    else if (f === mcnpj) {
        $(v_obj).attr('maxlength', '18');
    }
    else if (f === mcpf) {
        $(v_obj).attr('maxlength', '14');
    }
    else if (f === mdata) {
        $(v_obj).attr('maxlength', '10');
    }
    else if (f === mhora) {
        $(v_obj).attr('maxlength', '5');
    }
    else if (f === mhoracompleta) {
        $(v_obj).attr('maxlength', '8');
    }
    else if (f === mdataHora) {
        $(v_obj).attr('maxlength', '16');
    }
    else if (f === mreftrabalho) {
        $(v_obj).attr('maxlength', '11');
    }
    
    setTimeout("execmascara()", 1);
}

function execmascara() {
    v_obj.value = v_fun(v_obj.value);
}

function mcep(v) {
    v = v.replace(/\D/g, "");                    //Remove tudo o que não é dígito
    v = v.replace(/^(\d{5})(\d)/, "$1-$2");
    v = v.substring(0, 9);
    return v;
}

function mtelefone(v) {
    v = v.replace(/\D/g, "");             //Remove tudo o que não é dígito
    v = v.replace(/^(\d{2})(\d)/g, "($1) $2"); //Coloca parênteses em volta dos dois primeiros dígitos
    v = v.replace(/(\d)(\d{4})$/, "$1-$2");    //Coloca hífen entre o quarto e o quinto dígitos
    v = v.substring(0, 15);
    return v;
}

function mcnpj(v) {
    v = v.replace(/\D/g, "");                           //Remove tudo o que não é dígito
    v = v.replace(/^(\d{2})(\d)/, "$1.$2");            //Coloca ponto entre o segundo e o terceiro dígitos
    v = v.replace(/^(\d{2})\.(\d{3})(\d)/, "$1.$2.$3"); //Coloca ponto entre o quinto e o sexto dígitos
    v = v.replace(/\.(\d{3})(\d)/, ".$1/$2");           //Coloca uma barra entre o oitavo e o nono dígitos
    v = v.replace(/(\d{4})(\d)/, "$1-$2");              //Coloca um hífen depois do bloco de quatro dígitos
    v = v.substring(0, 18);

    if (v.length == 14) {
        v = mcpf(v);
    }

    return v;
}

function mcpf(v) {
    v = v.replace(/\D/g, "");
    v = v.replace(/(\d{3})(\d)/, "$1.$2");
    v = v.replace(/(\d{3})(\d)/, "$1.$2");
    v = v.replace(/(\d{3})(\d{1,2})$/, "$1-$2");
    v = v.substring(0, 14);
    return v;
}

function mdata(v) {
    v = v.replace(/\D/g, "");                    //Remove tudo o que não é dígito
    v = v.replace(/(\d{2})(\d)/, "$1/$2");
    v = v.replace(/(\d{2})(\d)/, "$1/$2");
    v = v.replace(/(\d{2})(\d{2})$/, "$1$2");
    v = v.substring(0, 10);

    if (v.length == 10) {
        if (!isValidDate(v)) {
            return "";
        }
    }

    return v;
}

function mnumero(v) {
    v = v.replace(/\D/g, "");                                      //Remove tudo o que não é dígito
    return v;
}

function mmoeda(v) {
    v = v.replace(/\D/g, "");//Remove tudo o que não é dígito
    v = v.replace(/(\d)(\d{14})$/, "$1.$2");//coloca o ponto dos Trilhões
    v = v.replace(/(\d)(\d{11})$/, "$1.$2");//coloca o ponto dos Bilhões
    v = v.replace(/(\d)(\d{8})$/, "$1.$2");//coloca o ponto dos milhões
    v = v.replace(/(\d)(\d{5})$/, "$1.$2");//coloca o ponto dos milhares

    v = v.replace(/(\d)(\d{2})$/, "$1,$2");//coloca a virgula antes dos 2 últimos dígitos
    return v;
}

function mhora(v) {
    v = v.replace(/\D/g, "");
    v = v.replace(/(\d{2})(\d)/, "$1:$2");
    return v;
}

function mhoracompleta(v) {
    v = v.replace(/\D/g, "");
    v = v.replace(/(\d{2})(\d)/, "$1:$2");
    v = v.replace(/(\d{2})(\d)/, "$1:$2");
    return v;
}

function mdataHora(v) {
    v = v.replace(/\D/g, "");                    //Remove tudo o que não é dígito
    v = v.replace(/(\d{2})(\d)/, "$1/$2");
    v = v.replace(/(\d{2})(\d)/, "$1/$2");
    v = v.replace(/(\d{2})(\d{2})$/, "$1$2");

    v = v.replace(/\D/g, " ");
    v = v.replace(/(\d{2})(\d)/, "$1:$2");

    v = v.substring(0, 16);

    return v;
}

function mmetrico(v) {
    v = v.replace(/\D/g, "");
    v = v.replace(/(\d)(\d{2})$/, "$1,$2");
    return v;
}

function isValidDate(v) {
    var reDate = /(^(((0[1-9]|1[0-9]|2[0-8])[\/](0[1-9]|1[012]))|((29|30|31)[\/](0[13578]|1[02]))|((29|30)[\/](0[4,6,9]|11)))[\/](19|[2-9][0-9])\d\d$)|(^29[\/]02[\/](19|[2-9][0-9])(00|04|08|12|16|20|24|28|32|36|40|44|48|52|56|60|64|68|72|76|80|84|88|92|96)$)/;
    return reDate.test(v);
}

function mreftrabalho(v) {
    v = v.replace(/\D/g, ""); //Remove tudo o que não é dígito
    v = v.replace(/(\d{4})(\d)/, "$1$2");
    v = v.replace(/(\d{4})(\d{0})/, "$1/$2");
    return v;
}

function mdemanda(v) {
    v = v.replace(/\D/g, ""); //Remove tudo o que não é dígito
    v = v.replace(/(\d{6})(\d)/, "$1");
    return v;
}

function mOperacao(v) {
    v = v.replace(/\D/g, "");//Remove tudo o que não é dígito
    v = v.replace(/(\d)(\d{17})$/, "$1.$2");//coloca o ponto dos Trilhões
    v = v.replace(/(\d)(\d{13})$/, "$1.$2");//coloca o ponto dos Bilhões
    v = v.replace(/(\d)(\d{3})$/, "$1.$2");//coloca o ponto dos milhões
    return v;
}

function mOperacaoAlfanumerico(v) {
    v = v.replace(/[^a-zA-Z0-9\.]/, ""); //Remove tudo que não for alfanumerico
    v = v.replace(/(\w{1})(\w{3})(\w{10})(\w{3})/g, "\$1-\$2-\$3-\$4")
    return v;
}