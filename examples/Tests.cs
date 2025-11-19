using SimpleFtp;

await FtpClient.SetCredentials("ftp://147.135.139.33","anonymous","");

var dirs = await FtpClient.GetDirectoriesAsync("");
foreach (var dir in dirs)
{ 
    Console.WriteLine(dir);
}

await FtpClient.DownloadFileAsync("examples/testFile.cpp", "@ace/mod.cpp");

long size = await FtpClient.GetFileSizeAsync("@ace/mod.cpp");
Console.WriteLine(size);