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

// ---------- Carrito de compra (persistido en localStorage) ----------
// El carrito vive en el navegador del Cliente (no hay tabla CARRITO en la BD);
// se guarda aquí para que el contador del navbar y la vista /Carrito/Index
// se mantengan sincronizados aunque el cliente navegue a otras páginas.
const CARRITO_STORAGE_KEY = 'salyfuego_carrito';

function obtenerCarritoStorage() {
    try {
        const raw = localStorage.getItem(CARRITO_STORAGE_KEY);
        return raw ? JSON.parse(raw) : [];
    } catch (e) {
        return [];
    }
}

function guardarCarritoStorage(carrito) {
    localStorage.setItem(CARRITO_STORAGE_KEY, JSON.stringify(carrito));
    actualizarBadgeCarrito();
}

function actualizarBadgeCarrito() {
    const badge = document.getElementById('badgeCarrito');
    if (!badge) return;

    const cantidad = obtenerCarritoStorage().reduce((acc, item) => acc + item.cantidad, 0);

    if (cantidad > 0) {
        badge.textContent = cantidad;
        badge.style.display = 'inline-block';
    } else {
        badge.style.display = 'none';
    }
}

actualizarBadgeCarrito();
