namespace Project_AG.ViewModels
{
    public class RegistroGastoDetailsViewModel
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public string NombreComercio { get; set; }
        public string Observaciones { get; set; }
        public string TipoDocumento { get; set; }
        public string NombreFondo { get; set; }

        public List<DetalleGastoDetailsViewModel> DetallesGasto { get; set; }
    }

    public class DetalleGastoDetailsViewModel
    {
        public string TipoGastoNombre { get; set; }
        public decimal Monto { get; set; }
    }

}
