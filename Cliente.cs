using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace Cliente_Servidor;

public class Cliente
{
    static string Read(string question)
    {
        Console.Write(question);
        string input = Console.ReadLine();
        Console.WriteLine();

        return input;
    }

    public Cliente()
    {
        string ip = Read("Qual é o IP: ");

        int porta = Int32.Parse(Read("Qual é a porta(Digite um número): "));

        TcpClient cliente = new TcpClient(ip, porta);

        string username = Read("Digite o nome de Usuario: ");

        Thread thread = new Thread(() => ReceberMensagem(cliente));
        thread.Start();

        NetworkStream stream = cliente.GetStream();

        while (true)
        {
            string conteudo = Read("");
            
            if (conteudo.ToLower() == "/sair")  // Comando para desconectar
            {
                cliente.Close();
                Console.WriteLine("Desconectado do servidor.");
                Environment.Exit(0);
            }

            Mensagem m = new Mensagem() { Username = username, Message = conteudo, time = DateTime.Now };

            string mensagem = JsonSerializer.Serialize(m);
            
            byte[] bytes = Encoding.UTF8.GetBytes(mensagem);
            
            stream.Write(bytes, 0, bytes.Length);
        }
    }

    public void ReceberMensagem(TcpClient client)
    {
        NetworkStream stream = client.GetStream();

        byte[] buffer = new byte[1024];
        try
        {
            while (true)
            {
                int byteslidos = stream.Read(buffer, 0, buffer.Length);
                if (byteslidos == 0) break;

                string json = Encoding.UTF8.GetString(buffer, 0, byteslidos);
                Mensagem mensagem = JsonSerializer.Deserialize<Mensagem>(json);
                
                Console.WriteLine($"[{mensagem.time}]{mensagem.Username}: {mensagem.Message}");
                Console.WriteLine();
            }
        }
        catch (Exception)
        {
            Console.WriteLine("Conexão Encerrada");
        }

        Console.WriteLine("Desconectado do Servidor");
        client.Close();
        Environment.Exit(0);
    }
}