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
        /*
         
         Nota aprendizado:
        Ao usar o camelCase como padrão de nome, tanto o serializador tanto o desserializador, colocam como camelCase dentro do JSON.
        Ao tentar procurar (na desserialização), ele procura por camelCase. No projeto está PascalCase, exception no JSON.

        Para resolver: Tirar camelCase; Colocar [JsonPropertyName] nas propriedades das Settings;
        Fazer o ficheiro seguir as regras de camelCase; CaseInsetive (PropertyNameCaseInsensitive = true)

        */

        // Caminho base das settings da app
        static string path = Path.Combine(Directory.GetCurrentDirectory(), "app_settings.json");
        static async Task Main(string[] args)
        {
            // Ver settings antes de iniciar para carregá-las ou default
            VerSettings();

            // Introdução da app
            Console.WriteLine($"Bem-vindo!\r\n" +
                $"{settings.Numero1} {Calcular(settings.Numero1, settings.Numero2, settings.Operacao).op} {settings.Numero2} = {Calcular(settings.Numero1, settings.Numero2, settings.Operacao).resul}");

            // Começo da calculadora
            while (true)
            {
                Console.WriteLine($"Escolha a sua opção:\r\n[0] - Fechar calculadora\r\n[1] - Calculadora padrão\r\n[2] - Métodos");
                switch (Console.ReadLine())
                {
                    case "0":
                        Environment.Exit(0);
                        break;
                    case "1":
                        // Iniciar as variáveis
                        double num1 = 0, num2 = 0;
                        int operador = -1; // -1 = Inválido
                        Console.Write("Intrduza o número à esquerda: "); 
                        double.TryParse(Console.ReadLine(), out num1);
                        
                        Console.Write("Intrduza o operador: ");
                        string op = Console.ReadLine();
                        if (op == "+") operador = 0;
                        else if (op == "-") operador = 1;
                        else if (op == "*") operador = 2;
                        else if (op == "/") operador = 3;

                        Console.Write("Intrduza o número à direita: ");
                        double.TryParse(Console.ReadLine(), out num2);

                        if (operador == -1)
                        {
                            Console.WriteLine("Ops! Você não colocou um operador váido. Vale relembrar quais são: + - * /\r\nAperte qualquer tecla para voltar a tentar");
                            Console.ReadKey();
                            Console.Clear();
                            continue; // return; fecha o método Main e não continua para a próxima iteração por conta do while
                        }

                        // Tupla para retirar sinal e resultado do cálculo
                        (double resultado, string operadorSimbolo) = Calcular(num1, num2, operador);

                        Console.Write($"{num1} {operadorSimbolo} {num2} = {resultado}\r\n Aperte qualquer tecla para continuar");
                        Console.ReadKey();
                        Console.Clear();
                        break;
                    default:
                        Console.WriteLine("Nenhuma opção válida foi selecionada. Aperte qualquer tecla para voltar a tentar.");
                        Console.ReadKey();
                        Console.Clear();
                        break;
                }
            }
        }





        /*
            ============================================ 
                            MÉTODOS
            ============================================             
        */
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

            // Se acontecer algo erro totalmente inesperado e o operador chegar aqui com -1 ou diferente do intervalo de 0-3
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
                // path = Debug/app_settings.json. Se existir, tenta carregar
                if (File.Exists(path))
                {
                    using FileStream reader = File.OpenRead(path);
                    settings = CarregarSettings(reader);
                }
                else // Cria um novo ficheiro do zero
                {
                    using FileStream writer = File.Create(path);
                    JsonSerializer.Serialize<Settings>(writer, settings);

                    using FileStream reader = File.OpenRead(path);
                    settings = CarregarSettings(reader);
                }
            }
            catch (IOException) { settings = new Settings(); }
            catch (JsonException) { settings = new Settings(); } // Em caso de qualquer erro com IO ou Json, inicializa com um novo
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