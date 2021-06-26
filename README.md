# Update Launcher
An automatic updater for your applications. This will check for new versions, download, unzip, as well as run your app.

<p align="center">
  
![dogeup](https://user-images.githubusercontent.com/76540311/123501675-3ad63300-d60c-11eb-8641-f6bfadd43f4d.gif)
  
</p>

## Premise

Your application needs an automatic update tool that checks for a newer version online, and if applicable, downloads and installs that version.
This tool will do exactly that, and you can change around all neccesary values to suit your needs.

It runs instead of your initial launcher or program. If an update is needed, the GUI will appear and the update will download and open after installing.
If an update isn't needed, the program simply launches your app right away without ever showing anything.

To see it in action, download and run the example updater:    
https://github.com/ryanhlewis/updatelauncher/blob/exampleapp/Updater.exe?raw=true  
After that, check your Registry under Current User/Software/Doge and File Explorer under AppData/Local/Doge to see what the program does, or rather, read below!

## Setup

This was created in [Visual Studio Community](https://visualstudio.microsoft.com/vs/community/) and uses generic Windows libraries and C# .NET.

First, install Visual Studio, and open up the Updater.sln.

Then, update the values immediately visible in the MainWindow() method:

![replaceMe](https://user-images.githubusercontent.com/76540311/123501716-838dec00-d60c-11eb-812c-fa64425da0fe.png)

Your new values should follow the same format, and most importantly, the download links must be **permanent**.  
This link will not change, but the ZIP file and the version text files behind them will.

Here is a brief description of what these values do:

> **appName** : The name of the folder in which your downloaded application is kept. (Also, the name of the Updater in Task Manager)  
**exeName** : The name of your application. This is used to run it after it checks for an update.  
**versionURL** : The permanent download link to a text file containing a version.    
**updateURL** : The permanent download link to your updated program.  

Make sure your updateURL leads to an archivable ZIP formatted file containing your program at the root.  
Make sure your versionURL leads directly to any text file that displays a version such as 0.0.1 or 1.0.0 using ONLY numbers and periods.

If your version classification differs, and has letters, symbols, or other strange formats- change Line 108 and 109 of MainWindow.xaml.cs to match your needs.

After updating these values, go to MainWindow.xaml and change the picture of the app logo to display whilst installing.  
You can also change the color of the progress bar, as well as the text underneath.

![updateeditor](https://user-images.githubusercontent.com/76540311/123516081-f1afce80-d65f-11eb-8736-c59d3c6bc35f.png)


Finally, at the top of your screen, go to Project -> Updater Properties, and rename as well as put in a logo for your Updater application.  
You have a customized Updater readily available!

## Extra Info

This Updater will install your game files to /AppData/Local/[appName]  
  
If you would rather have them in Program Files, obtain Admin Permission by adding this line to your manifest   
```<requestedExecutionLevel level="requireAdministrator" uiAccess="false" />```  
and switch out Line 53 of MainWindow.xaml.cs   
``` path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData); ```  
with  
``` path = "C:\\Program Files"; ```  

The reason I chose not to do this is because your game will ask for Admin Privileges every time it starts up.   
Even if you code around that, it will ask for every detected update. This is unneccesary and will only serve to detriment your users.  

## How It Works

The basic functionality is something I jotted down in a Notepad before I began programming. Here:

![image](https://user-images.githubusercontent.com/76540311/123516234-acd86780-d660-11eb-8dcc-6acd27b20a00.png)

As for how it actually works, the program makes uses of [HttpClient](https://docs.microsoft.com/en-us/dotnet/api/system.net.http.httpclient?view=net-5.0) and [System.IO](https://docs.microsoft.com/en-us/dotnet/api/system.io.stream?view=net-5.0) for downloading and file management.

It stores the last known version of your program in the [Windows Registry](https://docs.microsoft.com/en-us/dotnet/api/microsoft.win32.registry?view=net-5.0) and proceeds to download the update text file and check against it. It also checks whether the program already exists in the known path (AppData/Local/[appName]).

As for version information, the program is designed to simply replace all periods detected with nothing, and parse the result as an integer. 
This will turn 0.0.1 to 1 and 0.2.0 to 20. After that, it is relatively easy to literally compare them as integers.

Once these checks have completed, either the program immediately runs or the Updater GUI pops up and shows a customizable "Downloading.." message while it downloads and unzips your program to the path directory. After that, it runs it.

Your program has been automatically updated!




 
