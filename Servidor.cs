using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Encodings;
using System.Text.Json;

namespace Cliente_Servidor;

public class Servidor
{
    private List<TcpClient> clientes;

    public Servidor(int porta=8080)
    {
        clientes = new List<TcpClient>();

        TcpListener servidor = new TcpListener(IPAddress.Any, porta);
        
        servidor.Start();
        Console.WriteLine($"Servidor iniciado. IP: {ObterIpLocal()}, Porta: {porta}");

        while (true)
        {
            TcpClient cliente = servidor.AcceptTcpClient();
            Console.WriteLine("Cliente Conectado");
            
            clientes.Add(cliente);

            Thread thread = new Thread(() => ProcessarCliente(cliente));
            
            thread.Start();
        }
    }

    void ProcessarCliente(TcpClient cliente)
    {
        NetworkStream stream = cliente.GetStream();
        Console.WriteLine("nova thread");
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
                EnviarParaTodos(json, cliente);
            }
        }
        catch (Exception)
        {
            Console.WriteLine("Cliente Desconectado");
        }
        finally
        {
            clientes.Remove(cliente);
            cliente.Close();
        }
    }

    void EnviarParaTodos(string message, TcpClient cliente)
    {
        byte[] dados = Encoding.UTF8.GetBytes(message);
        
        foreach (var client in clientes)
        {
            if (client.Connected && client != cliente)
            {
                try
                {
                    NetworkStream stream = client.GetStream();
                    stream.Write(dados, 0, dados.Length);
                }
                catch (Exception)
                {
                    clientes.Remove(client);
                }
            }
        }
    }
    
    static string ObterIpLocal()
    {
        string ipLocal = "Não encontrado";
        try
        {
            string host = Dns.GetHostName();
            IPAddress[] ips = Dns.GetHostAddresses(host);
            foreach (var ip in ips)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork) // Pega IPv4
                {
                    ipLocal = ip.ToString();
                    break;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao obter IP local: {ex.Message}");
        }

        return ipLocal;
    }
}