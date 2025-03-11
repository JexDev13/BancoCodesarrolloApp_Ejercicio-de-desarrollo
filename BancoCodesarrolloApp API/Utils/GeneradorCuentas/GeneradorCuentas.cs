namespace BancoCodesarrolloApp_API.Utils.GeneradorCuentas
{
    public static class GeneradorCuentas
    {
        public static string GenerarNumeroCuenta(int idCliente)
        {
            Random random = new Random();
            string cuentaBase = $"{0001:D4}{000:D3}{idCliente:D6}{random.Next(0, 9)}";

            int digitoVerificacion = CalcularDigitoVerificacion(cuentaBase);

            return $"{cuentaBase}{digitoVerificacion}";
        }

        private static int CalcularDigitoVerificacion(string cuenta)
        {
            int suma = 0;
            int factor = 2;
            for (int i = cuenta.Length - 1; i >= 0; i--)
            {
                int valor = (cuenta[i] - '0') * factor;
                suma += (valor >= 10) ? (valor - 9) : valor;
                factor = (factor == 2) ? 1 : 2;
            }
            int digitoVerificacion = (10 - (suma % 10)) % 10;
            return digitoVerificacion;
        }
    }
}
