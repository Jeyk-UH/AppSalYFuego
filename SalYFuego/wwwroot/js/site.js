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

// ---------- Agregar al carrito desde el catálogo público (Producto/Combo/Menú del día) ----------
// Usa el mismo formato de línea que arma Carrito/Index.cshtml, para que al entrar
// al carrito los ítems agregados desde estas páginas ya aparezcan ahí.
// Marcado esperado en la vista:
//   <div data-carrito-tipo="Producto|Combo" data-carrito-id="1" data-carrito-nombre="..." data-carrito-precio="1000.00">
//       <input class="input-cantidad-agregar" type="number" min="1" value="1">
//       <button class="btn-agregar-carrito">Agregar</button>
//   </div>
function agregarAlCarritoPublico(tipo, id, nombre, precio, cantidad) {
    cantidad = parseInt(cantidad, 10);
    if (!cantidad || cantidad < 1) cantidad = 1;

    const carrito = obtenerCarritoStorage();
    const existente = carrito.find(i => i.tipo === tipo && i.id === id);
    if (existente) {
        existente.cantidad += cantidad;
    } else {
        carrito.push({ tipo, id, nombre, precio, cantidad, observaciones: '' });
    }
    guardarCarritoStorage(carrito);

    if (window.Swal) {
        Swal.fire({
            icon: 'success',
            title: 'Agregado al carrito',
            text: `${nombre} (x${cantidad})`,
            toast: true,
            position: 'top-end',
            timer: 1400,
            showConfirmButton: false
        });
    }
}

document.addEventListener('click', function (e) {
    const btn = e.target.closest('.btn-agregar-carrito');
    if (!btn) return;

    const widget = btn.closest('[data-carrito-tipo]');
    if (!widget) return;

    const tipo = widget.dataset.carritoTipo;
    const id = parseInt(widget.dataset.carritoId, 10);
    const nombre = widget.dataset.carritoNombre;
    const precio = parseFloat(widget.dataset.carritoPrecio);
    const input = widget.querySelector('.input-cantidad-agregar');
    const cantidad = input ? (parseInt(input.value, 10) || 1) : 1;

    agregarAlCarritoPublico(tipo, id, nombre, precio, cantidad);

    if (input) input.value = 1;
});
