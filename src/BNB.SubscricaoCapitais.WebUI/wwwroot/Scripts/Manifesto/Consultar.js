$(document).ready(function () {
    $('#btnDownload').hide();

    $('#btn-consultarManifestos').click(function (e) {
        var cpfCnpj = $('#CPFOuCNPJ').val();
        ConsultarManifestos(cpfCnpj);
    });

    $('#listaManifestos').click(function (e) {
        var clicado = $(e.target);

        var actives = document.getElementsByClassName('ui-state-active');

        for (var i = 0; i < actives.length; i++) {
            var isNumeroPagina = !isNaN(parseFloat(actives[i].textContent))

            if (isNumeroPagina) {
                let pagina = actives[i].textContent;
                let totalDeLinhas = $('#TotalLinhasManifestos').val();
                let intervaloInicial = (pagina * 10) - 9;
                let intervaloFinal = pagina * 10 < totalDeLinhas ? pagina * 10 : totalDeLinhas;
                $('#volumeManifestos').text(intervaloInicial + ' a ' + intervaloFinal + ' de ' + totalDeLinhas);
            }
        }
    });
});

function ConsultarManifestos(cpfCnpj) {
    if (cpfCnpj) {
        $('#btn-consultarManifestos').prop('disabled', true);
        ListarManifestos(cpfCnpj);
    }
}

function ListarManifestos(cpfCnpj) {

    showModal(true);

    $('#listaManifestos').puidatatable({
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
                            var _urlPix = ROOT_URL + "/Manifesto/GerarPix?codigo=" + rowData.Id;
                            var btnPix = '<a class="button default highlight" data-tooltip="Gerar PIX" href="' + _urlPix + '"><i class="money icon"></i></a>';
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
                    $("html, body").animate({ scrollTop: $('html, body').height() }, 1500, "easeInOutExpo");
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
                    showModal(false);
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

function GetDemandaPorCodigo(codigo) {
    $.ajax({
        type: 'GET',
        url: ROOT_URL + '/DemandaAuditoria/GetDemandaPorCodigo',
        data: {
            Codigo: codigo
        }
    }).done(function (data) {
        var detalhe = {
            Texto: data.Texto
        };
        hidePopup(codigo);
        var stringReduzida = detalhe.Texto.length < 400 ? detalhe.Texto : detalhe.Texto.substr(0, 399) + "...";
        $('#D' + codigo).attr("data-title", "Texto: ")
        $('#D' + codigo).attr("data-content", "" + stringReduzida);
        $('#D' + codigo).popup('show');
        $('.ui.popup').attr('style', function (i, s) { return s + 'max-width: 850px; min-width:350px; !important;' });

    }).fail(function () {
        $('#loader').transition('fade');
        showModal(false);
    });
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