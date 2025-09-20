using System.Runtime.InteropServices.Marshalling;
using System.Text.Json;

namespace MiniProjeto01___Treino_settings_JSON_e_async
{
    /*
        Mini Projeto 01
    Uma calculadora simples mas que usa settings iniciais para teste e async para JSON.
    A ideia não é ser útil, mas sim ter uma base futura para criar settings.

    Extras caso muito simples: Para além da calculadora, colocar opções extras, como por exemplo:
    - Métodos assincronos para verificar se número é primo ou não, inclui lista ou array para grandes números.
    CancellationToken para listas. Isso lidaria com um métodos com vários ou poucos parâmetros, coisa que eu nunca fiz
    - Encontrar divisores de um número
    - Calcular potências sem Math
    - etc...

    Em permisa geral parece-me simples ao analisar sem escrever ou pensar, mas me parece um bom treino.
    */
    internal class Program
    {
        static Settings settings = new Settings();
        static JsonSerializerOptions jsOption = new JsonSerializerOptions
        {
            //PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };

        // Caminho base das settings da app
        static string path = Path.Combine(Directory.GetCurrentDirectory(), "app_settings.json");
        static async Task Main(string[] args)
        {
            // Ver settings antes de iniciar
            VerSettings();

            Console.WriteLine($"Bem-vindo!\r\n" +
                $"{settings.Numero1} {Calcular(settings.Numero1, settings.Numero2, settings.Operacao).op} {settings.Numero2} = {Calcular(settings.Numero1, settings.Numero2, settings.Operacao).resul}");


            Console.ReadKey();
        }

        static (double resul, string op) Calcular(double numero1, double numero2, int operador)
        {
            if (operador == 0)
            {
                return (numero1 + numero2, "+");
            }

            if (operador == 1)
            {
                return (numero1 - numero2, "-");
            }

            if (operador == 2)
            {
                return (numero1 * numero2, "*");
            }

            if (operador == 3)
            {
                return (numero1 / numero2, "/");
            }

            if (operador > 3 || operador < 0)
            {
                if (File.Exists(path)) DeletarJSON();
                Console.WriteLine("Algum erro inesperado ocorreu. O seu operador não existe na nossa base de dados.\r\nPara impedir erros futuros, limpámos a sua predefinição.\r\nPresione enter para sair da aplicação");
                Console.ReadKey();
            }
            return (0, "");
        }

        static void DeletarJSON() => File.Delete(path);
        static void VerSettings()
        {
            try
            {
                if (File.Exists(path))
                {
                    using FileStream reader = File.OpenRead(path);
                    settings = CarregarSettings(reader);
                }
                else
                {
                    using FileStream writer = File.Create(path);
                    JsonSerializer.Serialize<Settings>(writer, settings);

                    using FileStream reader = File.OpenRead(path);
                    settings = CarregarSettings(reader);
                }
                // Por testar resultado que sai do await assim que criar settings 
            }
            catch (IOException) { settings = new Settings(); }
            catch (JsonException) { settings = new Settings(); }
            catch (Exception e) { ExceptionHandler(e);  }
        }

        public static void ExceptionHandler(Exception e)
        {
            Console.WriteLine($"ERRO!: {e.InnerException}\r\nMessagem: {e.Message}\r\nPressione ENTER para sair da aplicação");
            Console.ReadLine();
            Environment.Exit(0);
        }

        static Settings CarregarSettings(FileStream fs)
        {
            return JsonSerializer.Deserialize<Settings>(fs, jsOption) ?? new Settings();
        }
    }
}