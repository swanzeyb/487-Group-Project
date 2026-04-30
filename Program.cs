using System;

try 
{
    using var game = new _487_Group_Project.Game1();
    game.Run();
}
catch (Exception ex)
{
    Console.WriteLine("CRASH DETECTED:");
    Console.WriteLine(ex.ToString());
}