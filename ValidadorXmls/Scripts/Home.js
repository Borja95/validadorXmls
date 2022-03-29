
function mostrarTabla() {
    var totalFiles = document.getElementById('inputSubirArchivo').files.length;
    var esValido = true;

    if (totalFiles > 0) {
        for (var i = 0; i < totalFiles; i++) {
            var file = document.getElementById('inputSubirArchivo').files[i];


            if (file.type != "text/xml") {
                esValido = false;
                alert("Archivos no válidos, todos deben de ser tipo XML");
                document.getElementById('inputSubirArchivo').value = "";
                break;
            }
        }

        if (esValido == true) {
            var divTabla = document.getElementById('divTablaArchivos');
            var codigoHtml = "";
            codigoHtml += `<table class="table table-bordered" id="tablaArchivos">
                            <thead>
                                <tr>
                                    <th>#</th>
                                    <th>Nombre</th>
                                </tr>
                            </thead>
                            <tbody>`;

            for (var i = 0; i < totalFiles; i++) {
                var file = document.getElementById('inputSubirArchivo').files[i];

                codigoHtml += `<tr>
                            <td>${i + 1}</td>
                            <td>${file.name}</td>
                        </tr>`;
            }

            codigoHtml += "</tbody></table>";
            divTabla.innerHTML = codigoHtml;
            document.getElementById('btnComprobar').style.display = "block";
        }

    }
}

function enviarXML() {
    var formData = new FormData();
    var totalFiles = document.getElementById('inputSubirArchivo').files.length;
    var esValido = true;
    if (totalFiles == 0) {
        esValido = false;
    }

    if (esValido == true) {
        var divTablaArchivos = document.getElementById('divTablaArchivos');

        for (var i = 0; i < totalFiles; i++) {
            formData.append("archivos", document.getElementById("inputSubirArchivo").files[i]);
        }

        $.ajax({
            type: "POST",
            url: "/Home/ValidarXMLCargar",
            data: formData,
            contentType: false,
            processData: false,
            cache: false,
            beforeSend: function () {
                $("#tablaArchivos > tbody").html("");
                $("#tablaArchivos").append('<tr><td colspan="2" class="fila-loader">Validando Facturas...</td></tr>');
            },
            success: function (response) {
                var respuesta = response;
                console.log(respuesta);

                var codigoHtml = "";
                codigoHtml +=
                    `<table class="table table-bordered" id="tablaArchivos">
                            <thead>
                                <tr>
                                    <th>#</th>
                                    <th>Nombre</th>
                                    <th>Serie</th>
                                    <th>Folio</th>
                                    <th>Emisor</th>
                                    <th>Receptor</th>
                                    <th>Total</th>
                                    <th>Código Estatus</th>
                                    <th>Estado</th>
                                </tr>
                            </thead>
                            <tbody>`;
                for (var i = 0; i < respuesta.length; i++) {
                    var file = document.getElementById('inputSubirArchivo').files[i];

                    codigoHtml += `<tr>
                            <td>${i + 1}</td>
                            <td>${file.name}</td>
                            <td>${(respuesta[i].SERIE == null || respuesta[i].SERIE == 'null') ? '' : respuesta[i].SERIE}</td>
                            <td>${respuesta[i].FOLIO}</td>
                            <td>${respuesta[i].RFC_EMISOR}</td>
                            <td>${respuesta[i].RFC_RECEPTOR}</td>
                            <td>${respuesta[i].TOTAL}</td>
                            <td>${respuesta[i].CODIGO_ESTATUS}</td>
                            <td id="valido-${i + 1}">${respuesta[i].ESTADO}</td>
                        </tr>`;
                }
                codigoHtml += `</tbody></table>`;
                divTablaArchivos.innerHTML = codigoHtml;

                for (var i = 0; i < respuesta.length; i++) {
                    if (respuesta[i].ESTADO.toString().toLowerCase().trim() == 'vigente') {
                        document.getElementById(`valido-${i + 1}`).classList.add("estatus-vigente");
                    } else if (respuesta[i].ESTADO.toString().toLowerCase().trim() == 'cancelado') {
                        document.getElementById(`valido-${i + 1}`).classList.add("estatus-cancelado");
                    }
                }
                document.getElementById('btnComprobar').style.display = 'none';

            },
            error: function (jqXHR, textStatus, errorThrown) {
                console.log(jqXHR);
                console.log(textStatus);
                console.log(errorThrown);
                alert("Ocurrió un error al verificar los CFDI(s): " + jqXHR);
            }
        });

    }
}