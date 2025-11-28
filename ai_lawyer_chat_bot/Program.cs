using dotenv.net;
DotEnv.Load();
var envVars = DotEnv.Read();
Console.WriteLine(envVars["TG_TOKEN"]);
