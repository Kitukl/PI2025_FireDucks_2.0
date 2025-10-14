using System;
using ConsoleApp.models;

namespace ConsoleApp;

class Program
{
    public static void Main()
    {
        try
        {
            var categories = new Lesson();
            categories.GenerateLessons();
        }
        catch (Exception e)
        {
            Console.Write(e.Message);
        }
    }
}