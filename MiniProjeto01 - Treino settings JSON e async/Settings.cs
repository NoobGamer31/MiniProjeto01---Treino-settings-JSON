using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MiniProjeto01___Treino_settings_JSON_e_async
{
    internal class Settings
    {
        public double Numero1 { get; set; }
        public double Numero2 { get; set; }
        public int Operacao { get; set; } // int, porque será tratado de 0-3 ( + - * / )

        // Construtor início da aplicação
        [JsonConstructor]
        public Settings(double numero1 = 1, double numero2 = 1, int operacao = 0)
        {
            Numero1 = numero1;
            Numero2 = numero2;
            Operacao = operacao;
        }
    }
}
