using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

internal class MainClass
{
    private static string License = "j9qQymxhiFXwVRYKQCIK6w==";

    private static string EncryptString(string ClearText)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(ClearText);
        SymmetricAlgorithm algorithm = SymmetricAlgorithm.Create();
        MemoryStream stream = new MemoryStream();
        byte[] rgbIV = Encoding.ASCII.GetBytes("ryojvlzmdalyglrj");
        byte[] rgbKey = Encoding.ASCII.GetBytes("hcxilkqbbhczfeultgbskdmaunivmfuo");
        CryptoStream stream2 = new CryptoStream(stream, algorithm.CreateEncryptor(rgbKey, rgbIV), CryptoStreamMode.Write);
        stream2.Write(bytes, 0, bytes.Length);
        stream2.Close();
        return Convert.ToBase64String(stream.ToArray());
    }

    private static int Main(string[] args)
    {
        try
        {
            if (License == EncryptString(Environment.MachineName))
            {
                if (args[0] == "SEL")
                {
                    if (args[1] == "")
                    {
                        Functions.FromSql(args[0], "CAT", Convert.ToDateTime(args[2]).ToUniversalTime(), args[3], 0);
                        Functions.FromSql(args[0], "ROLE", Convert.ToDateTime(args[2]).ToUniversalTime(), args[3], 0);
                        Functions.FromSql(args[0], "CUST", Convert.ToDateTime(args[2]).ToUniversalTime(), args[3], 0);
                        Functions.FromSql(args[0], "ITEM", Convert.ToDateTime(args[2]).ToUniversalTime(), args[3], 0);
                        Functions.FromSql(args[0], "SH", Convert.ToDateTime(args[2]).ToUniversalTime(), args[3], 0);
                        Functions.FromSql(args[0], "SHL", Convert.ToDateTime(args[2]).ToUniversalTime(), args[3], 0);
                    }
                    else
                    {
                        Functions.FromSql(args[0], args[1], Convert.ToDateTime(args[2]).ToUniversalTime(), args[3], 0);
                    }
                }
                else
                {
                    DirectoryInfo info;
                    if (args[0] == "DELPIC")
                    {
                        info = new DirectoryInfo(args[1]);
                        foreach (FileInfo info2 in info.GetFiles())
                        {
                            info2.Delete();
                        }
                    }
                    else if (args[0] == "PIC")
                    {
                        info = new DirectoryInfo(args[1]);
                        bool deleteall = true;
                        if (info.FullName.Contains("ITEM"))
                        {
                            deleteall = false;
                        }
                        foreach (FileInfo info2 in info.GetFiles())
                        {
                            Functions.CreatePic(info2.FullName.Contains("CAT") ? "CAT" : "ITEM", info2.Name.Replace(".BMP", ""), info2.FullName, deleteall);
                            if (deleteall)
                            {
                                deleteall = false;
                            }
                        }
                    }
                    else if (args[0] == "QTY")
                    {
                        Functions.SetQty(args[4]);
                    }
                    else
                    {
                        DateTime time = Convert.ToDateTime(args[2]);
                        List<string> list = new List<string> { "CAT", "ROLE", "CUST", "ITEM", "PRICE", "SADDR", "SH", "SHL", "COMP" };
                        if (File.Exists(args[4] + @"\QTY.txt"))
                        {
                            Functions.SetQty(args[4] + @"\QTY.txt");
                            File.Delete(args[4] + @"\QTY.txt");
                        }
                        else
                        {
                            foreach (string str in list)
                            {
                                if (File.Exists(args[4] + @"\" + str + ".txt"))
                                {
                                    Functions.ToSql(args[0], str, Convert.ToDateTime(args[2]).ToUniversalTime(), "", new List<SqlParameter>(), args[4] + @"\" + str + ".txt");
                                    File.Delete(args[4] + @"\" + str + ".txt");
                                }
                            }
                            list.Remove("PRICE");
                            list.Remove("SADDR");
                            list.Remove("COMP");
                            foreach (string str in list)
                            {
                                Functions.FromSql(args[0], str, Convert.ToDateTime(args[2]).ToUniversalTime(), args[3], 0);
                            }
                        }
                    }
                }
            }
            return 0;
        }
        catch (Exception exception)
        {
            StreamWriter writer = new StreamWriter(@"C:\Install\NCObjs\NCErrorLog.txt", true);
            writer.WriteLine("Main" + exception.ToString());
            writer.Close();
            return 0;
        }
    }
}

