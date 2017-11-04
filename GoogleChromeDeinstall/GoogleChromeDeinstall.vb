Imports System.IO

Module GoogleChromeDeinstall

    Dim process As New Process

    Sub Main()

        Console.WriteLine("===")
        Console.WriteLine("=== " & My.Application.Info.ProductName.ToString & " " & My.Application.Info.Version.ToString & " ===")
        Console.WriteLine("===")

        Dim oRegistryHKLMUninstall As Microsoft.Win32.RegistryKey = Nothing

        Try

            ' Öffnen des Uninstall-Keys (64-Bit)
            oRegistryHKLMUninstall = My.Computer.Registry.LocalMachine.OpenSubKey("SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall")

            If IsNothing(oRegistryHKLMUninstall) = False Then

                FindUninstallString(oRegistryHKLMUninstall)

            End If

            oRegistryHKLMUninstall.Close()

            ' Öffnen des Uninstall-Keys (32-Bit)
            oRegistryHKLMUninstall = My.Computer.Registry.LocalMachine.OpenSubKey("SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall")

            If IsNothing(oRegistryHKLMUninstall) = False Then

                FindUninstallString(oRegistryHKLMUninstall)

            End If

            oRegistryHKLMUninstall.Close()

        Catch ex As Exception
            Console.WriteLine("[ERROR] Exception: " & ex.Message)
        End Try

    End Sub

    Private Sub FindUninstallString(oRegistryKey As Microsoft.Win32.RegistryKey)

        ' Alle SubKeys des Uninstall-Zweigs werden durchlaufen
        For Each strSubKey As String In oRegistryKey.GetSubKeyNames

            Try

                Dim oRegistryUninstallKey As Microsoft.Win32.RegistryKey = oRegistryKey.OpenSubKey(strSubKey)

                Console.WriteLine("Key: " & oRegistryUninstallKey.Name)

                If IsNothing(oRegistryKey) = False Then

                    For Each strRegValue As String In oRegistryUninstallKey.GetValueNames

                        Console.WriteLine("Key: " & oRegistryUninstallKey.Name)

                        If strRegValue = "DisplayName" Then

                            Dim strDisplayName As String = oRegistryUninstallKey.GetValue("DisplayName").ToString
                            Console.WriteLine("DisplayName: " & strDisplayName)

                            Dim strUninstallString As String = ""

                            ' Die Anwendung Google Chrome wurde identifiziert...
                            If strDisplayName.Contains(My.Settings.UnistallApplication) = True Then

                                For Each oRegValue As String In oRegistryUninstallKey.GetValueNames

                                    Console.WriteLine("  " & oRegValue & " = " & oRegistryUninstallKey.GetValue(oRegValue))

                                    ' der UninstallString wird gespeichert
                                    If oRegValue = "UninstallString" Then
                                        strUninstallString = oRegistryUninstallKey.GetValue(oRegValue)
                                    End If

                                Next

                                ' Der UninstallString ist bekannt und wird ausgeführt
                                ExecuteCommand(strUninstallString)

                            End If

                        End If

                    Next

                End If

            Catch ex As Exception

                Console.WriteLine("[ERROR] Exception: " & ex.Message.ToString)

            End Try

        Next

    End Sub

    Private Sub ExecuteCommand(strCommandLine As String)

        Dim strSilentParameter As String = ""

        If strCommandLine.ToLower.Contains("msiexec") = True Then
            strSilentParameter = " " & My.Settings.SilentParameterMsiExec
        Else
            strSilentParameter = " " & My.Settings.SilentParameterSetup
        End If

        Console.WriteLine("CommandLine: " & strCommandLine & strSilentParameter)

        process.StartInfo.UseShellExecute = False
        process.StartInfo.RedirectStandardOutput = True
        process.StartInfo.RedirectStandardError = True
        process.StartInfo.CreateNoWindow = True
        process.StartInfo.FileName = "cmd.exe"
        process.StartInfo.Arguments = "/c " & strCommandLine & strSilentParameter
        process.StartInfo.WorkingDirectory = "C:\Windows\System32"
        process.Start()
        process.WaitForExit()

        Console.WriteLine("ExitCode: " & process.ExitCode)

    End Sub

End Module
