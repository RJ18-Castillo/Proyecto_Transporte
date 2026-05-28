using Transporte.Model;

namespace Transporte.BL
{
    public interface IGestorTransporte
    {
        Usuario Login(string usuario, string clave);

        bool CambiarClave(string usuario, string claveActual, string claveNueva);

        List<Chofer> ListarChoferes(string filtro);

        Chofer ObtenerChofer(int id);

        void AgregarChofer(Chofer chofer);

        void EditarChofer(Chofer chofer);

        List<Pasajero> ListarPasajeros(string filtro);
        Pasajero ObtenerPasajero(int id);
        void AgregarPasajero(Pasajero pasajero);
        void EditarPasajero(Pasajero pasajero);

        List<Ruta> ListarRutas(string filtro);
        Ruta ObtenerRuta(int id);
        void AgregarRuta(Ruta ruta);
        void EditarRuta(Ruta ruta);

        List<Unidad> ListarUnidades();
        Unidad ObtenerUnidad(int id);
        bool ExistePlaca(string placa, int idIgnorar = 0);
        bool AgregarUnidad(Unidad unidad);
        bool EditarUnidad(Unidad unidad);

        List<Viaje> ListarViajes(string filtro, DateTime? fecha);
        Viaje? ObtenerViajePorId(int id);
        void AgregarViaje(Viaje viaje);
        void EditarViaje(Viaje viaje);
        void IniciarViaje(int id);
        void CancelarViaje(int id, string motivoCancelacion);
        void CompletarViaje(int id);
            }
}