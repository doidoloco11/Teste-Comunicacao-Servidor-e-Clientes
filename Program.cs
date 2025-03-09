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
            new Servidor();
        }
        else if (cs == "Cliente")
        {
            new Cliente();
        }
    }
}