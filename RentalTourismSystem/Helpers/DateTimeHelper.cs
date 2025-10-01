namespace RentalTourismSystem.Helpers
{
    public static class DateTimeHelper
    {
        // Timezone do Brasil
        private static readonly TimeZoneInfo TimeZoneBrasil = TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time");

        /// <summary>
        /// Converte DateTime UTC para horário do Brasil
        /// </summary>
        public static DateTime ConverterParaHorarioBrasil(DateTime dateTimeUtc)
        {
            if (dateTimeUtc.Kind == DateTimeKind.Utc)
            {
                return TimeZoneInfo.ConvertTimeFromUtc(dateTimeUtc, TimeZoneBrasil);
            }

            // Se não está em UTC, assumir que já está no horário local
            return dateTimeUtc;
        }

        /// <summary>
        /// Converte DateTime do Brasil para UTC
        /// </summary>
        public static DateTime ConverterParaUtc(DateTime dateTimeBrasil)
        {
            if (dateTimeBrasil.Kind == DateTimeKind.Unspecified)
            {
                return TimeZoneInfo.ConvertTimeToUtc(dateTimeBrasil, TimeZoneBrasil);
            }

            if (dateTimeBrasil.Kind == DateTimeKind.Local)
            {
                return dateTimeBrasil.ToUniversalTime();
            }

            return dateTimeBrasil;
        }

        /// <summary>
        /// Obter data/hora atual no timezone do Brasil
        /// </summary>
        public static DateTime ObterDataHoraAtualBrasil()
        {
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneBrasil);
        }

        /// <summary>
        /// Formatar DateTime para input datetime-local (sem conversão de timezone)
        /// </summary>
        public static string FormatarParaDateTimeLocal(DateTime dateTime)
        {
            // Para campos datetime-local, não fazer conversão de timezone
            // O navegador já interpreta no timezone local
            return dateTime.ToString("yyyy-MM-ddTHH:mm");
        }

        /// <summary>
        /// Formatar DateTime para input datetime-local considerando timezone brasileiro
        /// </summary>
        public static string FormatarParaDateTimeLocalBrasil(DateTime dateTime)
        {
            var dataHoraBrasil = ConverterParaHorarioBrasil(dateTime);
            return dataHoraBrasil.ToString("yyyy-MM-ddTHH:mm");
        }

        /// <summary>
        /// Converter string de datetime-local para DateTime com timezone brasileiro
        /// </summary>
        public static DateTime? ParseDateTimeLocalBrasil(string dateTimeLocalString)
        {
            if (string.IsNullOrWhiteSpace(dateTimeLocalString))
                return null;

            if (DateTime.TryParse(dateTimeLocalString, out var dateTime))
            {
                // Tratar como horário do Brasil
                return DateTime.SpecifyKind(dateTime, DateTimeKind.Unspecified);
            }

            return null;
        }

        /// <summary>
        /// Validar se data é futura (considerando timezone brasileiro)
        /// </summary>
        public static bool EhDataFutura(DateTime data)
        {
            var agoraBrasil = ObterDataHoraAtualBrasil();
            return data.Date >= agoraBrasil.Date;
        }

        /// <summary>
        /// Validar se data/hora é futura (considerando timezone brasileiro)
        /// </summary>
        public static bool EhDataHoraFutura(DateTime dataHora)
        {
            var agoraBrasil = ObterDataHoraAtualBrasil();
            return dataHora >= agoraBrasil;
        }
    }
}