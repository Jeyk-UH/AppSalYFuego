// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// Confirmación de acciones (desactivar/activar/eliminar) usando SweetAlert2
// en vez del confirm() nativo del navegador.
// Uso: <form onsubmit="return confirmarAccion(event, '¿Desactivar este producto?')">
function confirmarAccion(event, mensaje) {
    event.preventDefault();
    const form = event.target;

    Swal.fire({
        title: mensaje,
        icon: 'question',
        showCancelButton: true,
        confirmButtonText: 'Sí',
        cancelButtonText: 'No',
        confirmButtonColor: '#f5a623',
        cancelButtonColor: '#6c757d'
    }).then((result) => {
        if (result.isConfirmed) {
            form.submit();
        }
    });

    return false;
}
