using System.Net;
using System.Net.Sockets;
using System.Text;
using Cliente_Servidor;

class Program
{
    static string Read(string question)
    {
        Console.Write(question);
        string input = Console.ReadLine();
        Console.WriteLine();

        return input;
    }
    static void Main()
    {
        string cs = Read("Escreva se é Cliente ou Servidor: ");

        if (cs == "Servidor")
        {
            Int32.TryParse(Read("Digite a Porta do Servidor(Se não escrever nada Sera 8080): "), out int porta);
            new Servidor(porta == 0 ? 8080: porta);
        }
        else if (cs == "Cliente")
        {
            new Cliente();
        }
    }
}