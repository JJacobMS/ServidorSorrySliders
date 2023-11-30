using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace ServidorSorrySliders
{
    public static class CorreoSorrySliders
    {
        private static readonly IConfiguration Configuration = new ConfigurationBuilder()
        .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "recursos"))//Carpeta donde está el json
        .AddJsonFile("ConfiguracionesAplicaciones.json")
        .Build();

        private static readonly string FROM_EMAIL = Configuration["EmailSettings:FromEmail"];
        private static readonly string SMTP_HOST = Configuration["EmailSettings:SmtpHost"];
        private static readonly string EMAIL_PASSWORD = Configuration["EmailSettings:EmailPassword"];
        private static readonly int SMTP_PORT = Configuration.GetSection("EmailSettings")["SmtpPort"] != null
            ? int.Parse(Configuration.GetSection("EmailSettings")["SmtpPort"])
            : 0;

        private const string DISPLAY_NAME = "Wits And Wagers";
        private static SmtpClient client = new SmtpClient(SMTP_HOST, SMTP_PORT);
    }
}
