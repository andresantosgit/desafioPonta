$(document).ready(function () {
    $('#btnDownload').hide();

    $('#btn-consultarManifestos').click(function (e) {
        var cpfCnpj = $('#CPFOuCNPJ').val();
        ConsultarManifestos(cpfCnpj);
    });

    $('#btn-copyPix').click(function (e) {
        var pixCopiaCola = $('#pixCopiaCola').text();
        navigator.clipboard.writeText(pixCopiaCola);
    });
});

function ConsultarManifestos(cpfCnpj) {
    if (cpfCnpj) {
        $('#btn-consultarManifestos').prop('disabled', true);
        ListarManifestos(cpfCnpj);
    }
}

function ListarManifestos(cpfCnpj) {
    $('#listaManifestos').puidatatable({
        scrollable: false,
        animate: false,
        columns: [
                    { field: 'Id', headerText: 'ID', filter: true, sortable: true, headerStyle: "width:5%" },
                    { field: 'NomeInvestidor', headerText: 'NOME', filter: true, sortable: true, headerStyle: "width:20%" },
                    {
                        field: 'TipoPessoa', headerText: 'TIPO', filter: true, sortable: true, headerStyle: "width:10%", content: function (rowData) {
                            return rowData.TipoPessoaText;
                        }
                    },
                    {
                        field: 'DataCriacao', headerText: 'DATA DE CRIAÇÃO', filter: true, sortable: true, headerStyle: "width:15%", content: function (rowData) {
                            return rowData.DataCriacaoText;
                        }
                    },
                    {
                        field: 'DataAtualizacao', headerText: 'DATA DE ATUALIZAÇÃO', filter: true, sortable: true, headerStyle: "width:15%", content: function (rowData) {
                            return rowData.DataAtualizacaoText;
                        }
                    },
                    { field: 'Quantidade', headerText: 'QUANTIDADE', filter: true, sortable: true, headerStyle: "width:10%" },
                    {
                        field: 'ValorAcao', headerText: 'VALOR UNITÁRIO', filter: true, sortable: true, headerStyle: "width:15%", content: function (rowData) {
                            return rowData.ValorAcaoText;
                        }
                    },
                    {
                        field: 'ValorTotal', headerText: 'VALOR TOTAL', filter: true, sortable: true, headerStyle: "width:15%", content: function (rowData) {
                            return rowData.ValorTotalText;
                        }
                    },
                    { field: 'MatriculaSolicitante', headerText: 'SOLICITANTE', filter: true, sortable: true, headerStyle: "width:15%" },
                    {
                        field: 'Status', headerText: 'STATUS', filter: true, sortable: true, headerStyle: "width:15%", content: function (rowData) {
                            var classe = "ui horizontal label";
                            var btnStatus = CorStatus(rowData.Status, classe);
                            return [btnStatus];
                        }
                    },
                    {
                        field: 'actions', headerText: '', headerStyle: "width:15%", content: function (rowData) {
                            var btnPix = '<a onclick="popUpGerarPix(' + rowData.Id + ')" class="button default" data-tooltip="Gerar PIX"> <i class="money icon"></i></a>';

                            var btnVisualizarDocumento = '<a class="button default highlight" data-tooltip="Visualizar Documento" href="#" onclick="javascript:GerarRelatorioDemanda(\'' + rowData.Codigo + '\');"><i class="ui file red pdf outline icon"></i></a>';
                            var btns = [];

                            if (rowData.Status == "ATIVA") {
                                btns.push(btnPix);
                            }

                            if (["ATIVA", "CONCLUÍDA"].indexOf(rowData.Status) > -1) {
                                btns.push(btnVisualizarDocumento);
                            } 

                            return btns;

                        }
                    }
        ],
        globalFilter: '#globalFilterConsultarDemanda',
        datasource: function (callback) {
            $.ajax({
                url: ROOT_URL + '/Manifesto/ListaManifestos',
                data: { cpfCnpj: cpfCnpj },
                cache: false,
                type: "POST",
                dataType: "json",
                context: this,
                success: function (response) {
                    showModal(false);
                    $('#global-filter-demanda').prop('hidden', false);
                    $('#TotalLinhasManifestos').val(response.length);
                    $('#btnDownload').show();
                    callback.call(this, response);

                    //Label Paginador
                    let pagina = 1;
                    let totalDeLinhas = response.length;
                    let intervaloInicial = (pagina * 10) - 9;
                    let intervaloFinal = pagina * 10 < totalDeLinhas ? pagina * 10 : totalDeLinhas;
                    $('#volumeManifestos').text(intervaloInicial + ' a ' + intervaloFinal + ' de ' + totalDeLinhas);
                },
                error: function (request, status, error) {
                    $('#global-filter-demanda').prop('hidden', true);
                    if (request.status === 400 && request.responseJSON !== undefined) {
                        var vm = request.responseJSON;
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
                }
            });
        },
        paginator: { rows: 10 },
        emptyMessage: 'Nenhum registro encontrado!'
    });

}

// Retorna Cor Status
function CorStatus(status, classNameAdicional) {
    var componente = "";
    var iuClass = "ui horizontal label";
    var classColor;

    switch (status) {
        case "ATIVA":
            classColor = "orange";
            break;

        case "EXPIRADA":
        case "VENCIDA":
            classColor = "gray";
            break;

        case "CONCLUÍDA":
            classColor = "green";
            break;

        default:
            status = "Não definido";
            classColor = "grey";
            break;
    }

    if (classNameAdicional = ! null) {
        componente += "<span class='" + iuClass + " sts-dem-" + classColor + "' style = 'width: 120px !important;'>" +
            status +
                       "</span>";
    }
    else {
        componente += "<span class='ui pull-right label sts-dem-" + classColor + "' style = 'width: 120px !important;'>" +
            status +
                       "</span>";
    }

    return componente;
}

//Filtro de página para listagem baseadas em pudatatable
function filtroDePagina() {
    var input, filter, table, tr, td, i, j, txtValue, tbody;
    input = document.getElementById("globalFilterConsultarDemanda");
    filter = input.value.toUpperCase();
    table = document.getElementsByTagName("table");
    tbody = table[0].getElementsByTagName("tbody");
    tr = tbody[0].getElementsByTagName("tr");

    for (i = 0; i < tr.length; i++) {
        td = tr[i].getElementsByTagName("td");
        if (td) {
            let hasMatch = false;
            for (j = 0; j < td.length; j++) {
                txtValue = td[j].textContent.trim() || td[j].innerText;
                hasMatch = (txtValue.toUpperCase().indexOf(filter) > -1);
                if (hasMatch == true) break;
            }
            tr[i].style.display = hasMatch == true ? "" : "none";
        }
    }
}


function hidePopup(codigo) {

    var idDiv = "D" + codigo;
    var arrayObjetos = [];
    var id;
    $('.header-table').find('*').each(function () {
        id = $(this).attr("id");
        arrayObjetos.push({ id: id });
    });
    for (let value of arrayObjetos) {
        var obj = value.id;
        if (idDiv !== obj) {
            $('#' + obj).popup('destroy');;
        }
    }
}

//Mostra modal de carregamento
function showModal(show) {
    if (show == true) {
        $('#loading.ui.modal').modal({
            closable: 'false'
        }).modal('show');
    } else {
        $('#loading.ui.modal').modal('hide');
    }
}

//Gerar Excel e Fazer Download
function ExportarDados() {

    showModal(true);

    var _parametro = {
        Codigo: 0,
        NumeroAnoDemanda: $('#Codigo').val() != "" ? $('#Codigo').val().substring(0, 2) : 0,
        SequencialAnoDemanda: $('#Codigo').val() != "" ? $('#Codigo').val().substring(2, 6) : 0,
        Status: $('#StatusDemandaAuditoria').val() || [],
        DataCadastroFinal: $('#DataCadastroFinal').val(),
        DataCadastroInicial: $('#DataCadastroInicial').val(),
        DataAuditoriaFinal: $('#DataAuditoriaFinal').val(),
        DataAuditoriaInicial: $('#DataAuditoriaInicial').val(),
        DataAtendimentoFinal: $('#DataAtendimentoFinal').val(),
        DataAtendimentoInicial: $('#DataAtendimentoInicial').val(),
        NumeroDocumentoReferencia: $('#NumeroDocumentoReferencia').val(),
        PalavraChave: $('#PalavraChave').val(),
        NomeResponsavelAuditoria: $('#NomeResponsavelAuditoria').val(),
        DemandanteAuditoriaCodigo: $('#DemandanteAuditoriaCodigo').val() || [],
        TipoDemandaAuditoriaCodigo: $('#TipoDemandaAuditoriaCodigo').val() || [],
        CanalDemandaAuditoriaCodigo: $('#CanalDemandaAuditoriaCodigo').val() || [],
        GrupamentoResponsavelDemandaAuditoriaCodigo: $('#GrupamentoResponsavelDemandaAuditoriaCodigo').val() || []
    }

    $.ajax({
        type: 'POST',
        url: ROOT_URL + '/DemandaAuditoria/ExportarDemanda',
        data: _parametro,
        cache: true,
        success: function (data) {
            //Exportar planilha
            $('.ui.modal').modal('hide');
            window.location = ROOT_URL + '/Manifesto/DownloadExcel?fileGuid=' + data.FileGuid + '&fileName=' + data.FileName;
            console.log(data.FileGuid)
            console.log(data.FileName)
        },
        error: function (req, status, error) {
            $('.ui.modal').modal('hide');;
            $('#respostaAcaoFalha').text(error);
            $('#modalErro').modal('show');
        }
    });
}

function popUpGerarPix(codigo) {
    $('#loadingModal').modal('show');
    $.ajax({
        type: 'GET',
        url: ROOT_URL + '/Manifesto/GerarPix',
        data: { 'codigo': codigo }
    }).done(function (data) {
        $('#loadingModal').modal('hide');
        if (data !== undefined && data !== null) {
            var qrCodeBase64 = data.qrCodeBase64;
            $("#imgPix").prop('src', "data:image/jpg;base64," + qrCodeBase64);
            $("#pixCopiaCola").text(data.pixCopiaCola);

            setTimeout(function () {
                $('#PixDetalhe').modal({ closable: false }).modal('show');
            }, 1000);
        }
    }).fail(function (error) {
        $('#loadingModal').modal('hide');
        var mensagemErro = $('<div />')
            .addClass('ui message visible negative .other-messages')
            .prop('id', 'sucesso-execucao-ajax');

        var textoMensagemErro = $('<p />')
            .text('Ocorreu um erro ao recuperar o PIX Qr Code.');

        mensagemErro.append(textoMensagemErro);
        $('#erros-validacao').html(mensagemErro);
        setTimeout(function () {
            $('#sucesso-execucao-ajax')
                .transition('fade');
        }, 5000);
    });
}