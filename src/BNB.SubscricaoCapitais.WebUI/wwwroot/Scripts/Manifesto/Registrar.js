
$(document).ready(function () {

    var cpfCnpj = $('#CPFOuCNPJ').val();
    if (cpfCnpj) {
        ConsultarManifestos(cpfCnpj);
    }

    $('#btn-consultar').on('click', function (e) {
        e.preventDefault();
        var cpfCnpj = $('#CPFOuCNPJ').val();

        if (cpfCnpj) {
            ConsultarDados(cpfCnpj);
            ConsultarManifestos(cpfCnpj);
        }
    });

    $('#Quantidade').on('change', function (e) {
        e.preventDefault();
        AtualizaValorTotal();
    });

    $('#btn-atualizarcliente').on('click', function (e) {
        e.preventDefault();
        AtualizarCliente();
    });
});

function AtualizarCliente() {
    var model = $("form").serialize();
    var cpfCnpj = $('#CPFOuCNPJ').val();
    if (cpfCnpj) {
        $.ajax({
            type: 'POST',
            url: ROOT_URL + '/Manifesto/AtualizarCliente',
            data: model,
        }).done(function (data, textStatus, jqXHR) {
            $("html, body").animate({ scrollTop: $('html, body').height() }, 1500, "easeInOutExpo");
            $('.ui.modal').modal('hide');
            PreencheCampos(data);
            showMessage('positive', 'Cliente atualizado com sucesso!');
        }).fail(function (request, textStatus, errorThrown) {
            $('.ui.modal').modal('hide');
            showMessage('negative', 'Não foi possível atualizar o cliente.');

            var vm = request.responseJSON;
            if (request.status === 400 && request.responseJSON !== undefined && request.responseJSON !== null) {
                $.ajax({
                    type: 'POST',
                    url: ROOT_URL + '/Erros/MostrarErrosValidacao',
                    contentType: 'application/json',
                    data: JSON.stringify({ erros: vm })
                }).done(function (data) {
                    $('#erros-validacao').html(data);
                }).fail(function (error) {
                    console.log('Implementar error generico no controller de erros.');
                });
            }
        });
    }
}


function ConsultarDados(cpfCnpj) {
    if (cpfCnpj) {
        $.ajax({
            type: 'POST',
            url: ROOT_URL + '/Manifesto/ConsultarDados',
            data: {
                cpfCnpj: cpfCnpj
            }
        }).done(function (data, textStatus, jqXHR) {
            $('.ui.modal').modal('hide');
            PreencheCampos(data);
        }).fail(function (request, textStatus, errorThrown) {
            $('.ui.modal').modal('hide');

            if (request.status === 404) {
                textStatus = 'Cliente não encontrado!';
            }

            showMessage('negative', textStatus);
            LimparCampos();
            $('#CPFOuCNPJ').val(cpfCnpj);
        });
    }    
}

function PreencheCampos(lstRetorno) {
    $('#field_NomeInvestidor').removeClass("disabled");
    $('#field_quantidade').removeClass("disabled");
    $('#field_QuantidadeMaxima').removeClass("disabled");
    $('#field_telefone').removeClass("disabled");
    $('#field_endereco').removeClass("disabled");
    $('#field_email').removeClass("disabled");

    $('#NomeInvestidor').prop("disabled", false);
    $('#Quantidade').prop("disabled", false);
    $('#QuantidadeMaxima').prop("disabled", false);
    $('#Telefone').prop("disabled", false);
    $('#Endereco').prop("disabled", false);
    $('#Email').prop("disabled", false);

    var valorAcao = lstRetorno.ValorAcao || 0;
    valorAcao = mmoeda(String(valorAcao));
    $('#ValorAcao').val(valorAcao);


    AtualizaSelect("TipoPessoa", lstRetorno.TipoPessoa);
    AtualizaSelect("TipoCustodia", lstRetorno.TipoCustodia);

    $('#CPFOuCNPJ').val(lstRetorno.CPFOuCNPJ);
    $('#NomeInvestidor').val(lstRetorno.NomeInvestidor);
    $('#Telefone').val(lstRetorno.Telefone);
    $('#Endereco').val(lstRetorno.Endereco);
    $('#Email').val(lstRetorno.Email);
    $('#Quantidade').val(lstRetorno.QuantidadeMaxima);
    $('#QuantidadeMaxima').val(lstRetorno.QuantidadeMaxima);
    $('#Quantidade').prop("max", lstRetorno.QuantidadeMaxima);

    AtualizaValorTotal();

    $('#field_NomeInvestidor').addClass("disabled");
    $('#NomeInvestidor').prop("disabled", false);

    $('#field_QuantidadeMaxima').addClass("disabled");
    $('#QuantidadeMaxima').prop("disabled", false);
}

function AtualizaSelect(id, value) {
    $("#" + id).val(value);

    var desc = $("#" + id + " option[value='" + value + "']").text();
    $("#field_" + id + " label.ui-dropdown-label").text(desc);
}

function LimparCamposButton() {
    LimparCampos();
    location.reload(true);
}

function LimparCampos() {
    $('#CPFOuCNPJ').val("");
    $('#TipoPessoa').val("");
    $('#NomeInvestidor').val("");
    $('#Telefone').val("");
    $('#Endereco').val("");
    $('#Email').val("");
    $('#TipoCustodia').val("");
    $('#ValorAcao').val("");
    $('#Quantidade').val("");
    $('#ValorTotal').val("");

    $('#field_quantidade').addClass("disabled");
    $('#field_telefone').addClass("disabled");
    $('#field_endereco').addClass("disabled");
    $('#field_email').addClass("disabled");

    $('#Quantidade').prop("disabled", true);
    $('#Telefone').prop("disabled", true);
    $('#Endereco').prop("disabled", true);
    $('#Email').prop("disabled", true);
}

function AtualizaValorTotal() {
    var valorAcao = $("#ValorAcao").val() || 0;
    valorAcao = parseFloat(valorAcao.replace(' ', '').replace('.', '').replace(',', '.'));

    var quantidade = parseInt($("#Quantidade").val()) || 0;
    var total = quantidade * valorAcao;
    total = mmoeda(String(total.toFixed(2)));

    $('#ValorTotal').val(total);
}

function showMessage(classPositiveOrNegative, message) {
    $('#messageContent').text(message);

    $("#message").removeClass('visible positive negative');
    $("#message").addClass('hidden ' + classPositiveOrNegative);
    $("#message").transition('fade');

    addEventToMessageAutoClose();
}
