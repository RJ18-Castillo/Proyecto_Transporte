using Transporte.Model;

namespace Transporte.BL
{
    public interface IGestorTransporte
    {

        List<Pasajero> ListarPasajeros(string filtro);
        Pasajero ObtenerPasajero(string cedula);
        void AgregarPasajero(Pasajero pasajero);
        void EditarPasajero(Pasajero pasajero);

        List<Ruta> ListarRutas(string filtro);
        Ruta ObtenerRuta(int id);
        void AgregarRuta(Ruta ruta);
        void EditarRuta(Ruta ruta);

        List<Unidad> ListarUnidades();
        Unidad ObtenerUnidad(string placa);
        bool ExistePlaca(string placa);
        bool AgregarUnidad(Unidad unidad);
        bool EditarUnidad(Unidad unidad);

        List<Viaje> ListarViajes(string filtro, DateTime? fecha);
        Viaje? ObtenerViajePorId(int id);
        void AgregarViaje(Viaje viaje);
        void EditarViaje(Viaje viaje);
        void IniciarViaje(int id);
        void CancelarViaje(int id, string motivoCancelacion);
        void CompletarViaje(int id);

        List<Viaje> ListarViajesCancelados();

        List<Reserva> ListarReservasPorPasajero(int pasajeroId);
        Reserva? ObtenerDetalleReserva(int reservaId);
    }
}