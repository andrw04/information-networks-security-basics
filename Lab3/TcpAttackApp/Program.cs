using TcpAttackApp;

var serverTask = Task.Run(() => Server.Start());
var attackTask = Task.Run(() => TcpAttacker.Start());

await Task.WhenAll(serverTask, attackTask);
Console.ReadLine();