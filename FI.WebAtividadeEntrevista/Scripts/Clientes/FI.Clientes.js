let listaBeneficiarios = [];

$(document).ready(function () {
    $('#buttonIncluir').on("click", function () {
        let cpf = document.getElementById("CpfBeneficiario").value;
        let nome = document.getElementById("NomeBeneficiario").value;

        let duplicado;
        listaBeneficiarios.forEach(b => {
            if (b.CPF == document.getElementById("CpfBeneficiario").value)
                duplicado = true;
        });

        if (duplicado) {
            alert('O CPF utilizado já está cadastrado');
        } else if (!validarCPF(document.getElementById("CpfBeneficiario").value)) {
            alert('CPF inválido');
        } else {
            listaBeneficiarios.push(
                {
                Id: 0,
                CPF: document.getElementById("CpfBeneficiario").value,
                Nome: document.getElementById("NomeBeneficiario").value,
                IdCliente: 0
                });

            var random = Math.random().toString().replace('.', '');
            let texto =
                '<tr id="'+random+'">' +
                '<td style="width:20%">' + cpf + '</td>' +
                '<td style="width:50%">' + nome + '</td>' +
                '<td style = "width:30%"> ' +
                '<button type = "button" class="btn btn-primary" > Alterar</button> ' +
                '<button id="' + random + 'Button" type = "button" class="btn btn-primary" onclick="document.getElementById(\'' + random + '\').remove(); listaBeneficiarios.forEach(b => { if (b.CPF == \'' + cpf +'\') listaBeneficiarios.splice(listaBeneficiarios.indexOf(b), 1) });"> Excluir</button>' +
                '</td>' +
                '<tr>';

            $('#table').append(texto);

            document.getElementById("CpfBeneficiario").value = '';
            document.getElementById("NomeBeneficiario").value = '';
        }
    });

    $('#formCadastro').submit(function (e) {
        e.preventDefault();
        $.ajax({
            url: urlPost,
            method: "POST",
            data: {
                "NOME": $(this).find("#Nome").val(),
                "CEP": $(this).find("#CEP").val(),
                "Email": $(this).find("#Email").val(),
                "Sobrenome": $(this).find("#Sobrenome").val(),
                "Nacionalidade": $(this).find("#Nacionalidade").val(),
                "Estado": $(this).find("#Estado").val(),
                "Cidade": $(this).find("#Cidade").val(),
                "Logradouro": $(this).find("#Logradouro").val(),
                "Telefone": $(this).find("#Telefone").val(),
                "CPF": $(this).find("#CPF").val(),
                "Beneficiarios": listaBeneficiarios
            },
            error:
            function (r) {
                if (r.status == 400)
                    ModalDialog("Ocorreu um erro", r.responseJSON);
                else if (r.status == 500)
                    ModalDialog("Ocorreu um erro", "Ocorreu um erro interno no servidor.");
            },
            success:
            function (r) {
                ModalDialog("Sucesso!", r)
                $("#formCadastro")[0].reset();
            }
        });
    })
    
})

function ModalDialog(titulo, texto) {
    var random = Math.random().toString().replace('.', '');
    var texto = '<div id="' + random + '" class="modal fade">                                                               ' +
        '        <div class="modal-dialog">                                                                                 ' +
        '            <div class="modal-content">                                                                            ' +
        '                <div class="modal-header">                                                                         ' +
        '                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">×</button>         ' +
        '                    <h4 class="modal-title">' + titulo + '</h4>                                                    ' +
        '                </div>                                                                                             ' +
        '                <div class="modal-body">                                                                           ' +
        '                    <p>' + texto + '</p>                                                                           ' +
        '                </div>                                                                                             ' +
        '                <div class="modal-footer">                                                                         ' +
        '                    <button type="button" class="btn btn-default" data-dismiss="modal">Fechar</button>             ' +
        '                                                                                                                   ' +
        '                </div>                                                                                             ' +
        '            </div><!-- /.modal-content -->                                                                         ' +
        '  </div><!-- /.modal-dialog -->                                                                                    ' +
        '</div> <!-- /.modal -->                                                                                        ';

    $('body').append(texto);
    $('#' + random).modal('show');
}

function validarCPF(cpf) {
    cpf = cpf.replace(/\D/g, '');

    if (cpf.length !== 11) {
        return false;
    }

    if (/^(\d)\1{10}$/.test(cpf)) {
        return false;
    }

    function calculaDigito(digitos) {
        let soma = 0;
        for (let i = 0; i < digitos.length; i++) {
            soma += digitos[i] * (digitos.length + 1 - i);
        }
        let resto = soma % 11;
        return resto < 2 ? 0 : 11 - resto;
    }

    let digitos = cpf.split('').map(Number);
    let digito1 = calculaDigito(digitos.slice(0, 9));
    let digito2 = calculaDigito(digitos.slice(0, 9).concat(digito1));

    return digito1 === digitos[9] && digito2 === digitos[10];
}