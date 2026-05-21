using Transporte.Model;

namespace Transporte.BL
{
    public interface IGestorTransporte
    {
        Usuario Login(
            string usuario,
            string clave);

        bool CambiarClave(
            string usuario,
            string claveActual,
            string claveNueva);

        List<Chofer> ListarChoferes(
            string filtro);

        Chofer ObtenerChofer(
            int id);

        void AgregarChofer(
            Chofer chofer);

        void EditarChofer(
            Chofer chofer);
    }
}