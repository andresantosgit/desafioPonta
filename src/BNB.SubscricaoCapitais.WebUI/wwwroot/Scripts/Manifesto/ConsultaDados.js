
$(function () {
    $('#btn-consultar').on('click', function (e) {
        e.preventDefault();
        ConsultarDados();
    });

    $('#Quantidade').on('change', function (e) {
        e.preventDefault();
        AtualizaValorTotal();
    });
});

function ConsultarDados() {
    
    var cpfCnpj = $('#CPFOuCNPJ').val();

    if (cpfCnpj) {
        $('.ui.modal')
            .modal({
                closable: false
            })
            .modal('show');

        $.ajax({
            type: 'POST',
            url: ROOT_URL + '/Manifesto/ConsultarDados',
            data: {
                cpfCnpj: cpfCnpj
            }
        }).done(function (data, textStatus, jqXHR) {
            $("html, body").animate({ scrollTop: $('html, body').height() }, 1500, "easeInOutExpo");
            $('.ui.modal').modal('hide');
            PreencheCampos(data);
        }).fail(function (request, textStatus, errorThrown) {
            $('.ui.modal').modal('hide');
            
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
function PreencheCampos(lstRetorno) {

    $('#field_NomeInvestidor').removeClass("disabled");
    $('#field_quantidade').removeClass("disabled");
    $('#field_telefone').removeClass("disabled");
    $('#field_endereco').removeClass("disabled");
    $('#field_email').removeClass("disabled");

    $('#NomeInvestidor').prop("disabled", false);
    $('#Quantidade').prop("disabled", false);
    $('#Telefone').prop("disabled", false);
    $('#Endereco').prop("disabled", false);
    $('#Email').prop("disabled", false);

    var valorAcao = lstRetorno.ValorAcao || 0;
    valorAcao = mmoeda(String(valorAcao));
    $('#ValorAcao').val(valorAcao);

    AtualizaValorTotal();

    AtualizaSelect("TipoPessoa", lstRetorno.TipoPessoa);
    AtualizaSelect("TipoCustodia", lstRetorno.TipoCustodia);

    $('#CPFOuCNPJ').val(lstRetorno.CPFOuCNPJ);
    $('#NomeInvestidor').val(lstRetorno.NomeAcionista);
    $('#Telefone').val(lstRetorno.Telefone);
    $('#Endereco').val(lstRetorno.Endereco);
    $('#Email').val(lstRetorno.Email);

    $('#field_NomeInvestidor').addClass("disabled");
    $('#NomeInvestidor').prop("disabled", false);
}

function AtualizaSelect(id, value) {
    $("#" + id).val(value);

    var desc = $("#" + id + " option[value='" + value + "']").text();
    $("#field_" + id + " label.ui-dropdown-label").text(desc);
}
function limparCampos() {
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

    location.reload(true);
};

function AtualizaValorTotal() {
    var valorAcao = $("#ValorAcao").val() || 0;
    valorAcao = parseFloat(valorAcao.replace(' ', '').replace('.', '').replace(',', '.'));

    var quantidade = parseInt($("#Quantidade").val()) || 0;
    var total = quantidade * valorAcao;
    total = mmoeda(String(total.toFixed(2)));

    $('#ValorTotal').val(total);
}
