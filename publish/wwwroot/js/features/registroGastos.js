/**
 * Permite la gestión dinámica de registros de gastos en el formulario
 */
let index = 0;

function initializeRegistroGastos(initialIndex) {
  index = initialIndex;
}

function addRow() {
  const tbody = document.getElementById("detalles-gasto-body");
  const row = document.createElement("tr");

  row.innerHTML = `
        <td>
            <input type="hidden" name="DetallesGasto[${index}].Id" value="0" />
            <select name="DetallesGasto[${index}].TipoGastoId" class="form-control">
                ${getTipoGastoOptions()}
            </select>
        </td>
        <td>
            <input name="DetallesGasto[${index}].Monto" class="form-control" type="number" step="0.01" />
        </td>
        <td>
            <button type="button" class="btn btn-danger btn-sm" onclick="removeRow(this)">Eliminar</button>
        </td>
    `;
  tbody.appendChild(row);
  index++;

  /**
   * Se reinicializa la validación del formulario al agregar una nueva fila
   */
  var form = $(row).closest('form');
  form.removeData('validator');
  form.removeData('unobtrusiveValidation');
  $.validator.unobtrusive.parse(form);
}

function removeRow(button) {
  const row = button.closest("tr");
  row.remove();

  /**
   * Se actualiza la validación del formulario al eliminar una fila
   */
  var form = $(row).closest('form');
  form.removeData('validator');
  form.removeData('unobtrusiveValidation');
  $.validator.unobtrusive.parse(form);
}

/**
 * Obtiene las opciones de tipos de gasto disponibles
 */
function getTipoGastoOptions() {
  return window.tiposDeGastoOptions || '';
}
